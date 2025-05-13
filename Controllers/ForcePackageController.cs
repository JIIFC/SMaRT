using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using SMARTV3.Helpers;
using SMARTV3.Models;
using SMARTV3.Security;

using static Constants;
using static SMARTV3.Helpers.ForcePackageHelper;
using static SMARTV3.Helpers.PaginationHelper;
using static SMARTV3.Security.UserRoleProvider;

using EnvironmentName = Microsoft.AspNetCore.Hosting.EnvironmentName;

namespace SMARTV3.Controllers
{
    [CustomAuthorize(Roles = Admin + "," + SuperUser + ","+ ReportingUser + "," + Modeler)]
    public class ForcePackageController : Controller
    {
        private readonly SMARTV3DbContext _context;
        private readonly CultureHelper cultureHelper;
        private readonly ForcePackageHelper forcePackageHelper;
        private JsonSerializerSettings jsonSerializerSettings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        private string lang; //Current UI language
        private readonly KpiEmail kpiEmail;
        private readonly bool isDevelopment;

        public ForcePackageController(SMARTV3DbContext context, IConfiguration configuration)
        {
            _context = context;
            cultureHelper = new();
            forcePackageHelper = new(_context);
            lang = cultureHelper.GetCurrentCulture();
            isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Development;
            kpiEmail = new KpiEmail(configuration, isDevelopment);
        }

        // This method sets view data for stuff that needs to be set in multiple methods
        private void SetViewData(int? packageOwnersOrganization, int? selectedPurpose, int? readinessStatus, string? selectedOrganization, string? selectedConPlan, string? selectedOperation, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages, string indexPageSize, int? indexPageNumber, string? indexSortOrder, ForcePackage? forcePackage)
        {
            ViewData["packageOwnersOrganization"] = packageOwnersOrganization;
            ViewData["selectedPurpose"] = selectedPurpose;
            ViewData["readinessStatus"] = readinessStatus;
            ViewData["selectedOrganization"] = selectedOrganization ?? "";
            ViewData["selectedConPlan"] = selectedConPlan ?? "";
            ViewData["selectedOperation"] = selectedOperation ?? "";

            ViewData["identifiedOperation"] = identifiedOperation;
            ViewData["identifiedConplan"] = identifiedConplan;
            ViewData["onlyShowMyForcePackages"] = onlyShowMyForcePackages;

            ViewData["indexPageSize"] = indexPageSize;
            ViewData["itemsPerPage"] = GetItemsPerPageList(indexPageSize);
            ViewData["indexPageNumber"] = indexPageNumber;
            ViewData["indexSortOrder"] = indexSortOrder;

            if (lang == "en")
            {
                ViewData["operationList"] = new SelectList(_context.Operations.Where(d => d.Archived == false).OrderBy(d => d.Ordered).ThenBy(d => d.OperationName).AsNoTracking(), "Id", "OperationName");
                ViewData["conPlanList"] = new SelectList(_context.Conplans.Where(d => d.Archived == false).OrderBy(d => d.Ordered).ThenBy(d => d.ConplanName).AsNoTracking(), "Id", "ConplanName");
            }
            else
            {
                ViewData["operationList"] = new SelectList(_context.Operations.Where(d => d.Archived == false).OrderBy(d => d.Ordered).ThenBy(d => d.OperationNameFre).AsNoTracking(), "Id", "OperationNameFre");
                ViewData["conPlanList"] = new SelectList(_context.Conplans.Where(d => d.Archived == false).OrderBy(d => d.Ordered).ThenBy(d => d.ConplanNameFre).AsNoTracking(), "Id", "ConplanNameFre");
            }

            ViewData["purposeList"] = new SelectList(_context.ForcePackagePurposes.Where(d => d.Archived == false).OrderBy(d => d.Order).AsNoTracking(), "Id", lang == "en" ? "NameEn" : "NameFr");

            if (forcePackage != null)
            {
                ViewData["purposeId"] = new SelectList(_context.ForcePackagePurposes.Where(d => d.Archived == false).OrderBy(d => d.Order).AsNoTracking(), "Id", lang == "en" ? "NameEn" : "NameFr", forcePackage.ForcePackagePurpose);
            }
        }

        // Makes a list of force elements used in the available column
        private void SetForceElementList(ForcePackage forcePackage, int? readinessStatus, string? selectedOrganization, string? selectedConPlan, string? selectedOperation, List<string>? selectedFelmIdsList)
        {
            IQueryable<ForceElement> forceElementQuery = _context.ForceElements.Include(d => d.DataCards)
                                                                                    .ThenInclude(da => da.SrStatus)
                                                                               .Where(d => d.Archived == false && d.DataCards.Count > 0);
          
            List<int> selectedFelmIdsListInt = new();
            if (selectedFelmIdsList != null)
            {
                selectedFelmIdsListInt = selectedFelmIdsList.Select(int.Parse).ToList();
            }

            // Filter force elements
            if (readinessStatus != null)
            {
                forceElementQuery = forceElementQuery.Where(f => f.DataCards.First().SrStatusId == readinessStatus || selectedFelmIdsListInt.Contains(f.Id));
            }
            if (!string.IsNullOrEmpty(selectedOrganization))
            {
                forceElementQuery = forceElementQuery.Where(f => f.OrganizationId == int.Parse(selectedOrganization) || selectedFelmIdsListInt.Contains(f.Id));
            }
            if (!string.IsNullOrEmpty(selectedConPlan))
            {
                forceElementQuery = forceElementQuery.Where(f => f.DataCards.First().Conplans.Any(c => c.Id == int.Parse(selectedConPlan)) || selectedFelmIdsListInt.Contains(f.Id));
            }
            if (!string.IsNullOrEmpty(selectedOperation))
            {
                forceElementQuery = forceElementQuery.Where(f => f.DataCards.First().Operations.Any(o => o.Id == int.Parse(selectedOperation)) || selectedFelmIdsListInt.Contains(f.Id));
            }

            List<ForceElement> availableForceElements = forceElementQuery.OrderBy(d => d.ElementName).AsNoTracking().ToList();
            List<int?> forcePackgeFelmIds = forcePackage.DummyForceElements.Select(f => f.ForceElementId).ToList();
            availableForceElements.RemoveAll(f => forcePackgeFelmIds.Contains(f.Id));
            ViewData["forceElementList"] = availableForceElements;
        }

        private User? GetCurrentUser()
        {
            if (User.Identity == null || User.Identity.Name == null) return null;
            string username = RemoveDomainFromUsername(User.Identity.Name);
            User? user = null;
            if (String.IsNullOrEmpty(username)) return null;
            user = _context.Users.Include(i => i.Roles).Include(i => i.Organization).FirstOrDefault(m => m.UserName == username);
            return user;
        }

        public List<string> FindUsersNotInPackage(int forcePackageId, string userName)
        {
            ForcePackage? forcePackage = _context.ForcePackages.Include(f => f.Users)
                                                               .Include(f => f.PackageOwner)
                                                               .Where(f => f.Id == forcePackageId).FirstOrDefault();
            if (forcePackage == null) return new List<string>();
            List<string> userList = _context.Users.Where(user => user.Enabled && user.UserName.Contains(userName)
                                                    && !forcePackage.Users.Contains(user) && user != forcePackage.PackageOwner)
                                                  .Select(user => user.UserName).ToList();
            return userList;
        }

        [HttpPost]
        public ActionResult ForcePackageKpiModal(int forcePackageId)
        {
            int currentUserId = GetCurrentUser()!.Id;
            ViewData["overallStatusList"] = _context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).ToList();
            ViewData["CurrentUserID"] = currentUserId;
            ViewData["forcePackageId"] = forcePackageId;
            ForcePackageKpi? forcePackageKpi = _context.ForcePackageKpis.Where(d => d.ForcePackageId == forcePackageId && d.UserId == currentUserId)
                                                                        .FirstOrDefault();
            forcePackageKpi ??= new()
            {
                UserId = currentUserId,
                ForcePackageId = forcePackageId
            };
            return PartialView("ForcePackageKpiModal", forcePackageKpi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void CreateForcePackageKpi(string serializedKpi)
        {
            ForcePackageKpi? forcePackageKpi = JsonConvert.DeserializeObject<ForcePackageKpi>(serializedKpi);
            if (forcePackageKpi != null)
            {
                _context.Add(forcePackageKpi);
                _context.SaveChanges();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void DeleteForcePackageKpi(int forcePackageKpiId)
        {
            ForcePackageKpi? forcePackageKpi = _context.ForcePackageKpis.Where(d => d.Id == forcePackageKpiId).FirstOrDefault();
            if (forcePackageKpi != null)
            {
                _context.Remove(forcePackageKpi);
                _context.SaveChanges();
            }
        }

        // GET: ForcePackageController
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public async Task<IActionResult> Index(int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages, string? indexPageSize, int? indexPageNumber, string indexSortOrder)
        {
            lang = cultureHelper.GetCurrentCulture();
            IQueryable<ForcePackage> forcePackages = forcePackageHelper.GetForcePackageQueryWIncludes();
            User? currentUser = GetCurrentUser();

            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }

            // Filter force elements
            if (packageOwnersOrganization != null)
            {
                forcePackages = forcePackages.Where(fp => fp.PackageOwner.OrganizationId == packageOwnersOrganization);
            }
            if (selectedPurpose != null)
            {
                forcePackages = forcePackages.Where(fp => fp.ForcePackagePurpose == selectedPurpose);
            }
            if (identifiedOperation != null)
            {
                forcePackages = forcePackages.Where(fp => fp.Operations.Where(op => op.Id == identifiedOperation).Any());
            }
            if (identifiedConplan != null)
            {
                forcePackages = forcePackages.Where(fp => fp.Conplans.Where(con => con.Id == identifiedConplan).Any());
            }
            if (onlyShowMyForcePackages == true && currentUser != null)
            {
                forcePackages = forcePackages.Where(fp => fp.PackageOwnerId == currentUser.Id);
            }

            // Sort force elements
            switch (indexSortOrder)
            {
                case "packageName_asc":
                    forcePackages = forcePackages.OrderBy(d => d.ForcePackageName);
                    break;

                case "packageName_desc":
                    forcePackages = forcePackages.OrderByDescending(d => d.ForcePackageName);
                    break;

                case "packageOwner_asc":
                    forcePackages = forcePackages.OrderBy(d => d.PackageOwner.UserName);
                    break;

                case "packageOwner_desc":
                    forcePackages = forcePackages.OrderByDescending(d => d.PackageOwner.UserName);
                    break;
            }

            SetViewData(packageOwnersOrganization, selectedPurpose, null, "", "", "", identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);

            ViewData["organizationList"] = new SelectList(_context.Organizations.Where(d => d.Archived == false).OrderBy(d => d.Ordered).AsNoTracking(), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");
            ViewData["overallStatusList"] = _context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking().ToList();
            ViewData["givenUser"] = currentUser;

            return View(await PaginatedList<ForcePackage>.CreateAsync(forcePackages.AsNoTracking(), indexPageNumber ?? 1, int.Parse(indexPageSize)));
        }

        // GET: ForcePackageController/Create
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public ActionResult Create(int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages, string? indexPageSize, int? indexPageNumber, string indexSortOrder)
        {
            lang = cultureHelper.GetCurrentCulture();
            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }
            SetViewData(packageOwnersOrganization, selectedPurpose, null, "", "", "", identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);
            ViewData["operationListForCheckboxes"] = _context.Operations.Where(d => d.Archived == false).OrderBy(d => d.Ordered).ToList();
            ViewData["conplanListForCheckboxes"] = _context.Conplans.Where(d => d.Archived == false).OrderBy(d => d.Ordered).ToList();
            return View();
        }

        // This method is called by AJAX in the Create View
        // POST: ForcePackageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public async Task<IActionResult> Create(string ForcePackageName, string ForcePackagePurpose, string? ForcePackageDescription, string[] CheckedOperations, string[] CheckedConplans)
        {
            lang = cultureHelper.GetCurrentCulture();

            if (ForcePackageName == null || ForcePackagePurpose == null)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return View();
            }

            ForcePackage forcePackage = new()
            {
                ForcePackageName = ForcePackageName,
                ForcePackagePurpose = int.Parse(ForcePackagePurpose),
                ForcePackageDescription = ForcePackageDescription,
            };

            if (User.Identity == null || User.Identity.Name == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Unauthorized();
            }

            string username = RemoveDomainFromUsername(User.Identity.Name);
            User? user = null;
            if (!String.IsNullOrEmpty(username))
            {
                user = await _context.Users.Include(i => i.Roles).Include(i => i.Organization).FirstOrDefaultAsync(m => m.UserName == username);
                if (user == null)
                {
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Unauthorized();
                }
                else
                {
                    forcePackage.PackageOwnerId = user.Id;
                    forcePackage.LastEditUser = user.Id;
                    forcePackage.LastEditDate = DateTime.Now;
                }
            }

            ModelState.Remove("PackageOwner");
            ModelState.Remove("CheckedOperations");
            ModelState.Remove("CheckedConplans");

            if (!ModelState.IsValid)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return View(forcePackage);
            }
            if (CheckedOperations.Length > 0)
            {
                foreach (string operationId in CheckedOperations)
                {
                    forcePackage.Operations.Add(_context.Operations.Where(op => op.Id == int.Parse(operationId)).First());
                }
            }
            if (CheckedConplans.Length > 0)
            {
                foreach (string conplanId in CheckedConplans)
                {
                    forcePackage.Conplans.Add(_context.Conplans.Where(conplan => conplan.Id == int.Parse(conplanId)).First());
                }
            }

            _context.Add(forcePackage);
            _context.SaveChanges();
            Response.StatusCode = StatusCodes.Status200OK;
            return RedirectToAction("Index");
        }

        // GET: ForcePackageController/Details/5
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public async Task<IActionResult> Details(int id, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages,
            string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? detailsPageSize, int? detailsPageNumber, string detailsSortOrder)
        {
            lang = cultureHelper.GetCurrentCulture();
            ForcePackage? forcePackage = await forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == id).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }
            if (string.IsNullOrEmpty(detailsPageSize))
            {
                detailsPageSize = "10";
            }

            if (forcePackage == null)
            {
                return NotFound();
            }

            SetViewData(packageOwnersOrganization, selectedPurpose, null, "", "", "", identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, forcePackage);
            ViewData["overallStatusList"] = _context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking().ToList();
            ViewData["deployedStatusListNonSelect"] = _context.DeployedStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).ToList();
            ViewData["givenUser"] = GetCurrentUser();
            ViewData["detailsPageSize"] = detailsPageSize;
            ViewData["detailsPageNumber"] = detailsPageNumber;
            ViewData["detailsSortOrder"] = detailsSortOrder;

            IQueryable<DummyForceElement> dummyForceElements = forcePackageHelper.GetDummyForceElementsQueryWIncludes()
                                    .Where(d => d.ForcePackageId == id && d.IsActiveInForcePackage).AsNoTracking();

            // Sort force elements
            switch (detailsSortOrder)
            {
                case "felmId_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.ElementId);
                    break;

                case "felmId_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.ElementId);
                    break;

                case "felmName_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.ElementName);
                    break;

                case "felmName_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.ElementName);
                    break;

                case "realFelm_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.IsTiedToRealFelm);
                    break;

                case "realFelm_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.IsTiedToRealFelm);
                    break;

                case "overallStatus_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().SrStatusId);
                    break;

                case "overallStatus_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().SrStatusId);
                    break;

                case "commandOverride_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().CommandOverideStatusId);
                    break;

                case "commandOverride_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().CommandOverideStatusId);
                    break;

                case "personnel_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().PersonnelStatusId);
                    break;

                case "personnel_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().PersonnelStatusId);
                    break;

                case "equipment_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().EquipmentStatusId);
                    break;

                case "equipment_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().EquipmentStatusId);
                    break;

                case "training_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().TrainingStatusId);
                    break;

                case "training_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().TrainingStatusId);
                    break;

                case "sustainment_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().SustainmentStatusId);
                    break;

                case "sustainment_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().SustainmentStatusId);
                    break;

                case "forceEmployment_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().DeployedStatusId);
                    break;

                case "forceEmployment_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().DeployedStatusId);
                    break;
            }

            ViewData["forceElementList"] = await PaginatedList<DummyForceElement>.CreateAsync(dummyForceElements, detailsPageNumber ?? 1, int.Parse(detailsPageSize));

            List<DummyDataCard> dummyDataCards = forcePackage.DummyForceElements.Select(f => f.DummyDataCards.First()).ToList();

            if (dummyDataCards.Count > 0)
            {
                DatacardReadinessTableCalculator DRTCalculator = new();
                ViewData["dataCardDisplayModel"] = DRTCalculator.CalculateDisplay(dummyDataCards);
            }
            return View(forcePackage);
        }

        private void SendKpiAlerts(int forcePackageId, string emailBody)
        {
            //List<ForcePackageKpi> forcePackageKpis = _context.ForcePackageKpis.Where(d => d.ForcePackageId == forcePackageId)
            //                                                      .Include(d => d.User).ToList();
            //try
            //{
            //    foreach (ForcePackageKpi forcePackageKpi in forcePackageKpis)
            //    {
            //        kpiEmail.SendEmail(forcePackageKpi.User.Email, emailBody);
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
        }

        // Update all force elements in a force packages data from the latest real data
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public ActionResult UpdateFelmData(int id, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages,
            string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? detailsPageSize, int? detailsPageNumber, string detailsSortOrder)
        {
            ForcePackage? forcePackage = forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == id).FirstOrDefault();
            if (forcePackage != null && forcePackage.DummyForceElements.Any())
            {
                foreach (DummyForceElement dummyForceElement in forcePackage.DummyForceElements.ToList())
                {
                    if (!dummyForceElement.IsTiedToRealFelm) continue;

                    // get real felm and datacard
                    ForceElement? selectedforceElement = _context.ForceElements.Include(d => d.DataCards)
                                                                               .Include(d => d.Organization)
                                                                               .Include(d => d.Weighting)
                                                                               .Include(d => d.ForceElementArchiveComments)
                                                                                   .ThenInclude(f => f.ArchiveComment)
                                                                                       .ThenInclude(a => a.ChangeUserNavigation)
                                                                               .Where(f => f.Id == dummyForceElement.ForceElementId)
                                                                               .AsNoTracking()
                                                                               .FirstOrDefault();

                    if (selectedforceElement == null) continue;
                    DataCard? dataCard = selectedforceElement.DataCards.First();
                    if (dataCard == null) continue;

                    // Remove existing felm and recreate with new data
                    foreach (DummyDataCard dummyDataCard in dummyForceElement.DummyDataCards)
                    {
                        _context.DummyDataCards.Remove(dummyDataCard);
                        _context.SaveChanges();
                    }
                    forcePackage.DummyForceElements.Remove(dummyForceElement);
                    _context.DummyForceElements.Remove(dummyForceElement);
                    _context.SaveChanges();

                    // Copy data
                    DummyDataCard dummyDatacard = DatacardToDummy(forcePackage.Id, dataCard);
                    if (dummyDatacard == null) continue;

                    _context.DummyDataCards.Add(dummyDatacard);
                    _context.SaveChanges();

                    forcePackage.DummyForceElements.Add(dummyDatacard.DummyForceElement);
                }
                forcePackage.DateLastFetchedLiveData = DateTime.Now;
                forcePackage.LastEditDate = DateTime.Now;
                forcePackage.LastEditUser = GetCurrentUser()?.Id;

                _context.Update(forcePackage);
                _context.SaveChanges();

                string emailBody = "This is an alert that force package: " + forcePackage.ForcePackageName + " has been modified. Its data was updated to live data.";
                SendKpiAlerts(forcePackage.Id, emailBody);

                return RedirectToAction("Details", new { id, packageOwnersOrganization, selectedPurpose, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, detailsPageSize, detailsPageNumber, detailsSortOrder });
            }
            return RedirectToAction("Index");
        }

        // GET: ForcePackageController/Edit/5
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public async Task<IActionResult> Edit(int id, int? packageOwnersOrganization, int? readinessStatus, int? selectedPurpose, string? selectedOrganization, string? selectedConPlan,
            string? selectedOperation, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages, string? indexPageSize, int? indexPageNumber, string indexSortOrder,
            string? forcePackageName, string? forcePackagePurpose, string? forcePackageDescription, string? checkedOperations, string? checkedConplans, string? selectedForceElementIds,
            string? selectedDummyForceElementIds, string? showSavedToast)
        {
            lang = cultureHelper.GetCurrentCulture();
            List<string>? selectedFelmIdsList = null;
            ForcePackage? forcePackage = await forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == id).AsNoTracking().FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }

            SetViewData(packageOwnersOrganization, selectedPurpose, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, forcePackage);
            ViewData["operationListForCheckboxes"] = _context.Operations.Where(d => d.Archived == false).OrderBy(d => d.Ordered).AsNoTracking().ToList();
            ViewData["conplanListForCheckboxes"] = _context.Conplans.Where(d => d.Archived == false).OrderBy(d => d.Ordered).AsNoTracking().ToList();
            ViewData["organizationList"] = new SelectList(_context.Organizations.Where(d => d.Archived == false).OrderBy(d => d.Ordered).AsNoTracking(), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");
            ViewData["overallStatusList"] = _context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking().ToList();
            ViewData["givenUser"] = GetCurrentUser();
            ViewData["showSavedToast"] = showSavedToast;

            if (forcePackage == null)
            {
                return NotFound();
            }

            if (forcePackageName != null)
            {
                forcePackage.ForcePackageName = forcePackageName;
            }
            if (forcePackagePurpose != null)
            {
                forcePackage.ForcePackagePurpose = int.Parse(forcePackagePurpose);
            }
            if (forcePackageDescription != null)
            {
                forcePackage.ForcePackageDescription = forcePackageDescription;
            }
            if (checkedOperations != null)
            {
                List<string>? checkedOperationsList = JsonConvert.DeserializeObject<List<string>>(checkedOperations);
                // Clear operations then add selected
                forcePackage.Operations.Clear();
                if (checkedOperationsList != null && checkedOperationsList.Any())
                {
                    foreach (string operationId in checkedOperationsList)
                    {
                        forcePackage.Operations.Add(_context.Operations.Where(op => op.Id == int.Parse(operationId)).First());
                    }
                }
            }
            if (checkedConplans != null)
            {
                List<string>? checkedConplansList = JsonConvert.DeserializeObject<List<string>>(checkedConplans);
                // Clear conplans then add selected
                forcePackage.Conplans.Clear();
                if (checkedConplansList != null && checkedConplansList.Any())
                {
                    foreach (string conplanId in checkedConplansList)
                    {
                        forcePackage.Conplans.Add(_context.Conplans.Where(conplan => conplan.Id == int.Parse(conplanId)).First());
                    }
                }
            }
            if (selectedForceElementIds != null)
            {
                ViewData["selectedForceElementIds"] = selectedForceElementIds;
                selectedFelmIdsList = JsonConvert.DeserializeObject<List<string>>(selectedForceElementIds);
            }
            if (selectedDummyForceElementIds != null)
            {
                ViewData["selectedDummyForceElementIds"] = selectedDummyForceElementIds;
            }
            else
            {
                ViewData["selectedDummyForceElementIds"] = JsonConvert.SerializeObject(forcePackage.DummyForceElements.Where(d => d.IsActiveInForcePackage).Select(d => d.Id.ToString()).ToList());
            }

            SetForceElementList(forcePackage, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, selectedFelmIdsList);
            return View(forcePackage);
        }

        // This method is called by AJAX in the Edit View
        // POST: ForcePackageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public async Task<IActionResult> Edit(int Id, string ForcePackageName, string ForcePackagePurpose, string? ForcePackageDescription, string[] CheckedOperations, string[] CheckedConplans, string[] SelectedForceElements, string[] SelectedDummyForceElements)
        {
            lang = cultureHelper.GetCurrentCulture();

            if (Id == 0 || ForcePackageName == null || ForcePackagePurpose == null)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return View();
            }
            int ForcePackagePurposeInt = int.Parse(ForcePackagePurpose);

            ForcePackage? forcePackage = forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == Id).First();
            if (forcePackage == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return View();
            }

            SetForceElementList(forcePackage, null, "", "", "", null);

            if (User.Identity == null || User.Identity.Name == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Unauthorized();
            }

            // Force package user
            string username = RemoveDomainFromUsername(User.Identity.Name);
            User? user = null;
            if (!String.IsNullOrEmpty(username))
            {
                user = await _context.Users.Include(i => i.Roles).Include(i => i.Organization).FirstOrDefaultAsync(m => m.UserName == username);
                if (user == null)
                {
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Unauthorized();
                }
                else
                {
                    forcePackage.LastEditUser = user.Id;
                    forcePackage.LastEditDate = DateTime.Now;
                }
            }

            // Update changed value
            if (ForcePackageName != forcePackage.ForcePackageName)
            {
                forcePackage.ForcePackageName = ForcePackageName;
            }
            if (ForcePackagePurposeInt != forcePackage.ForcePackagePurpose)
            {
                forcePackage.ForcePackagePurpose = ForcePackagePurposeInt;
            }
            if (ForcePackageDescription != forcePackage.ForcePackageDescription)
            {
                forcePackage.ForcePackageDescription = ForcePackageDescription;
            }

            ModelState.Remove("CheckedOperations");
            ModelState.Remove("CheckedConplans");
            ModelState.Remove("SelectedForceElements");

            // Clear operations then add selected
            forcePackage.Operations.Clear();
            if (CheckedOperations.Any())
            {
                foreach (string operationId in CheckedOperations)
                {
                    forcePackage.Operations.Add(_context.Operations.Where(op => op.Id == int.Parse(operationId)).First());
                }
            }

            // Clear conplans then add selected
            forcePackage.Conplans.Clear();
            if (CheckedConplans.Any())
            {
                foreach (string conplanId in CheckedConplans)
                {
                    forcePackage.Conplans.Add(_context.Conplans.Where(conplan => conplan.Id == int.Parse(conplanId)).First());
                }
            }

            // Add dummy felms to package
            if (SelectedDummyForceElements.Any())
            {
                foreach (string selectedDummyForceElementId in SelectedDummyForceElements)
                {
                    DummyForceElement? dummyForceElement = _context.DummyForceElements.Where(d => d.Id == int.Parse(selectedDummyForceElementId)).First();
                    dummyForceElement.IsActiveInForcePackage = true;
                    _context.SaveChanges();
                    if (dummyForceElement != null && !forcePackage.DummyForceElements.Contains(dummyForceElement))
                    {
                        forcePackage.DummyForceElements.Add(dummyForceElement);
                    }
                }
            }

            // Delete removed dummy force elements
            foreach (DummyForceElement dummyForceElement in forcePackage.DummyForceElements)
            {
                if (!SelectedDummyForceElements.Contains(dummyForceElement.Id.ToString()))
                {
                    if (dummyForceElement.IsTiedToRealFelm)
                    {
                        foreach (DummyDataCard dummyDataCard in dummyForceElement.DummyDataCards)
                        {
                            _context.DummyDataCards.Remove(dummyDataCard);
                            _context.SaveChanges();
                        }
                        forcePackage.DummyForceElements.Remove(dummyForceElement);
                        _context.DummyForceElements.Remove(dummyForceElement);
                        _context.SaveChanges();
                    }
                    else
                    {
                        dummyForceElement.IsActiveInForcePackage = false;
                        _context.SaveChanges();
                    }
                }
            }

            // Create dummy force elements from selected
            if (SelectedForceElements.Any())
            {
                foreach (string selectedForceElementId in SelectedForceElements)
                {
                    int selectedForceElementIdInt = int.Parse(selectedForceElementId);
                    // Skip if it already exists
                    if (forcePackage.DummyForceElements.Any(f => f.ForceElementId == selectedForceElementIdInt)) continue;
                    DataCard? selectedDataCard = _context.ForceElements.Include(d => d.DataCards)
                                                                       .Include(d => d.Organization)
                                                                       .Include(d => d.Weighting)
                                                                       .Include(d => d.ForceElementArchiveComments)
                                                                           .ThenInclude(f => f.ArchiveComment)
                                                                               .ThenInclude(a => a.ChangeUserNavigation)
                                                                       .Where(f => f.Id == selectedForceElementIdInt)
                                                                       .AsNoTracking()
                                                                       .First().DataCards.First();

                    if (selectedDataCard == null) continue;
                    DummyDataCard dummyDatacard = DatacardToDummy(forcePackage.Id, selectedDataCard);
                    if (dummyDatacard == null) continue;

                    _context.DummyDataCards.Add(dummyDatacard);
                    _context.SaveChanges();

                    forcePackage.DummyForceElements.Add(dummyDatacard.DummyForceElement);
                }
            }

            if (!ModelState.IsValid)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return View(forcePackage);
            }

            _context.Update(forcePackage);
            _context.SaveChanges();

            string emailBody = "This is an alert that force package: " + forcePackage.ForcePackageName + " has been modified.";
            SendKpiAlerts(forcePackage.Id, emailBody);

            Response.StatusCode = StatusCodes.Status200OK;
            return RedirectToAction("Index");
        }

        // GET: ForcePackageController/Delete/5
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler) ]
        public async Task<IActionResult> Delete(int id, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages,
            string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? deletePageSize, int? deletePageNumber, string deleteSortOrder)
        {
            lang = cultureHelper.GetCurrentCulture();
            ForcePackage? forcePackage = await forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == id).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }
            if (string.IsNullOrEmpty(deletePageSize))
            {
                deletePageSize = "10";
            }

            SetViewData(packageOwnersOrganization, selectedPurpose, null, "", "", "", identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, forcePackage);
            ViewData["deletePageSize"] = deletePageSize;
            ViewData["deletePageNumber"] = deletePageNumber;
            ViewData["deleteSortOrder"] = deleteSortOrder;

            IQueryable<DummyForceElement> dummyForceElements = forcePackageHelper.GetDummyForceElementsQueryWIncludes().Where(d => d.ForcePackageId == id).AsNoTracking();

            // Sort force elements
            switch (deleteSortOrder)
            {
                case "felmId_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.ElementId);
                    break;

                case "felmId_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.ElementId);
                    break;

                case "felmName_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.ElementName);
                    break;

                case "felmName_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.ElementName);
                    break;

                case "overallStatus_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().SrStatusId);
                    break;

                case "overallStatus_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().SrStatusId);
                    break;

                case "personnel_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().PersonnelStatusId);
                    break;

                case "personnel_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().PersonnelStatusId);
                    break;

                case "equipment_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().EquipmentStatusId);
                    break;

                case "equipment_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().EquipmentStatusId);
                    break;

                case "training_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().TrainingStatusId);
                    break;

                case "training_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().TrainingStatusId);
                    break;

                case "sustainment_asc":
                    dummyForceElements = dummyForceElements.OrderBy(d => d.DummyDataCards.First().SustainmentStatusId);
                    break;

                case "sustainment_desc":
                    dummyForceElements = dummyForceElements.OrderByDescending(d => d.DummyDataCards.First().SustainmentStatusId);
                    break;
            }

            if (forcePackage == null)
            {
                return NotFound();
            }

            ViewData["forceElementList"] = await PaginatedList<DummyForceElement>.CreateAsync(dummyForceElements, deletePageNumber ?? 1, int.Parse(deletePageSize));

            List<DummyDataCard> dummyDataCards = forcePackage.DummyForceElements.Select(f => f.DummyDataCards.First()).ToList();
            if (dummyDataCards.Count > 0)
            {
                DatacardReadinessTableCalculator DRTCalculator = new();
                ViewData["dataCardDisplayModel"] = DRTCalculator.CalculateDisplay(dummyDataCards);
            }

            return View(forcePackage);
        }

        // POST: ForcePackageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public ActionResult Delete(ForcePackage? forcePackage, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages,
            string? indexPageSize, int? indexPageNumber, string indexSortOrder)
        {
            lang = cultureHelper.GetCurrentCulture();
            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }
            SetViewData(packageOwnersOrganization, selectedPurpose, null, "", "", "", identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);

            ForcePackage? tempForcePackage = null;
            if (forcePackage == null)
            {
                return NotFound();
            }

            tempForcePackage = forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == forcePackage.Id).FirstOrDefault();

            if (tempForcePackage == null)
            {
                return NotFound();
            }

            // Remove foreign entities
            foreach (DummyForceElement dummyForceElement in tempForcePackage.DummyForceElements)
            {
                foreach (DummyDataCard dummyDataCard in dummyForceElement.DummyDataCards)
                {
                    _context.DummyDataCards.Remove(dummyDataCard);
                }
                _context.DummyForceElements.Remove(dummyForceElement);
            }
            foreach (ForcePackageKpi forcePackageKpi in tempForcePackage.ForcePackageKpis)
            {
                _context.ForcePackageKpis.Remove(forcePackageKpi);
            }

            // Remove all models with deleted force package
            string fPIdStr = forcePackage.Id.ToString();
            List<FpcompareModel> fpcompareModels = _context.FpcompareModels.Where(f => f.SerializedForcePackageIds.Contains(fPIdStr)).ToList();
            foreach (FpcompareModel fpCompareModel in fpcompareModels)
            {
                List<string>? forcePackageIds = JsonConvert.DeserializeObject<List<string>>(fpCompareModel.SerializedForcePackageIds)?.ToList();
                if (forcePackageIds?.Count == 2 || forcePackageIds?.First() == fPIdStr)
                {
                    _context.FpcompareModels.Remove(fpCompareModel);
                }
                else
                {
                    forcePackageIds?.Remove(fPIdStr);
                    fpCompareModel.SerializedForcePackageIds = JsonConvert.SerializeObject(forcePackageIds, jsonSerializerSettings);
                    _context.Update(fpCompareModel);
                }
            }
            _context.SaveChanges();

            string emailBody = "This is an alert that force package: " + forcePackage.ForcePackageName + " has been deleted.";
            SendKpiAlerts(forcePackage.Id, emailBody);

            // Remove foreign keys
            tempForcePackage.Conplans.Clear();
            tempForcePackage.Operations.Clear();
            tempForcePackage.Users.Clear();
            tempForcePackage.DummyForceElements.Clear();
            tempForcePackage.ForcePackageKpis.Clear();

            _context.Update(tempForcePackage);
            _context.SaveChanges();
            _context.ForcePackages.Remove(tempForcePackage);
            _context.ChangeTracker.DetectChanges();
            _context.SaveChanges();

            return RedirectToAction("Index", new { packageOwnersOrganization, selectedPurpose, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder });
        }

        // GET: ForcePackageController/DuplicateForcePackage
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public ActionResult DuplicateForcePackage(int forcePackageToDuplicateId, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages,
            string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? selectedOrganization, string? selectedConPlan, string? selectedOperation, int? readinessStatus, string? forcePackageName,
            string? forcePackagePurpose, string? forcePackageDescription, string? checkedOperations, string? checkedConplans, string? selectedForceElementIds, string? selectedDummyForceElementIds)
        {
            lang = cultureHelper.GetCurrentCulture();

            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }

            ViewData["forcePackageToDuplicateId"] = forcePackageToDuplicateId;

            ForcePackage? forcePackagetoDuplicate = forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == forcePackageToDuplicateId).AsNoTracking().FirstOrDefault();
            if (forcePackagetoDuplicate == null)
            {
                return View("error");
            }

            ForcePackage forcePackage = new()
            {
                PackageOwnerId = forcePackagetoDuplicate.PackageOwnerId,
                ForcePackageName = forcePackagetoDuplicate.ForcePackageName,
                ForcePackageDescription = forcePackagetoDuplicate.ForcePackageDescription,
                ForcePackagePurpose = forcePackagetoDuplicate.ForcePackagePurpose,
                DateLastFetchedLiveData = forcePackagetoDuplicate.DateLastFetchedLiveData,
                LastEditUser = forcePackagetoDuplicate.LastEditUser,
                LastEditDate = forcePackagetoDuplicate.LastEditDate,
                Conplans = forcePackagetoDuplicate.Conplans,
                Operations = forcePackagetoDuplicate.Operations,
                Users = forcePackagetoDuplicate.Users,
                DummyForceElements = new HashSet<DummyForceElement>()
            };

            SetViewData(packageOwnersOrganization, selectedPurpose, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);
            ViewData["operationListForCheckboxes"] = _context.Operations.Where(d => d.Archived == false).OrderBy(d => d.Ordered).AsNoTracking().ToList();
            ViewData["conplanListForCheckboxes"] = _context.Conplans.Where(d => d.Archived == false).OrderBy(d => d.Ordered).AsNoTracking().ToList();
            ViewData["forcePackageName"] = forcePackageName;
            ViewData["forcePackagePurpose"] = forcePackagePurpose;
            ViewData["forcePackageDescription"] = forcePackageDescription;
            ViewData["checkedOperations"] = checkedOperations;
            ViewData["checkedConplans"] = checkedConplans;
            ViewData["selectedForceElementIds"] = selectedForceElementIds;
            ViewData["selectedDummyForceElementIds"] = selectedDummyForceElementIds;
            return View(forcePackage);
        }

        // POST: ForcePackageController/DuplicateForcePackage
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public async Task<IActionResult> DuplicateForcePackage(int forcePackageToDuplicateId, string ForcePackageName, string ForcePackagePurpose, string? ForcePackageDescription,
            string[] CheckedOperations, string[] CheckedConplans, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages,
            string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? selectedOrganization, string? selectedConPlan, string? selectedOperation, int? readinessStatus)
        {
            lang = cultureHelper.GetCurrentCulture();
            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }

            if (User.Identity == null || User.Identity.Name == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Unauthorized();
            }

            ForcePackage? forcePackagetoDuplicate = forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == forcePackageToDuplicateId).AsNoTracking().FirstOrDefault();
            if (forcePackagetoDuplicate == null)
            {
                return View("error");
            }

            ForcePackage forcePackage = new()
            {
                ForcePackageName = ForcePackageName,
                ForcePackageDescription = forcePackagetoDuplicate.ForcePackageDescription,
                ForcePackagePurpose = int.Parse(ForcePackagePurpose),
                DateLastFetchedLiveData = forcePackagetoDuplicate.DateLastFetchedLiveData,
                DummyForceElements = new HashSet<DummyForceElement>()
            };

            // Transfer data
            if (ForcePackageDescription != null)
            {
                forcePackage.ForcePackageDescription = ForcePackageDescription;
            }
            if (CheckedOperations.Any())
            {
                foreach (string operationId in CheckedOperations)
                {
                    forcePackage.Operations.Add(_context.Operations.Where(op => op.Id == int.Parse(operationId)).First());
                }
            }
            if (CheckedConplans.Any())
            {
                foreach (string conplanId in CheckedConplans)
                {
                    forcePackage.Conplans.Add(_context.Conplans.Where(conplan => conplan.Id == int.Parse(conplanId)).First());
                }
            }

            // Set user info
            string username = RemoveDomainFromUsername(User.Identity.Name);
            User? user = null;
            if (!String.IsNullOrEmpty(username))
            {
                user = await _context.Users.Include(i => i.Roles).Include(i => i.Organization).FirstOrDefaultAsync(m => m.UserName == username);
                if (user == null)
                {
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Unauthorized();
                }
                else
                {
                    forcePackage.PackageOwnerId = user.Id;
                    forcePackage.LastEditUser = user.Id;
                    forcePackage.LastEditDate = DateTime.Now;
                }
            }

            _context.Add(forcePackage);
            _context.SaveChanges();

            // Transfer force elements
            List<DummyForceElement> DummyForceElements = forcePackagetoDuplicate.DummyForceElements.ToList();
            foreach (DummyForceElement dummyForceElement in DummyForceElements)
            {
                DummyDataCard? dummyDataCard = forcePackageHelper.CopyForceElementAndDatacard(forcePackage.Id, dummyForceElement);
                if (dummyDataCard != null)
                {
                    // Create and save
                    _context.DummyDataCards.Add(dummyDataCard);
                    _context.SaveChanges();
                    forcePackage.DummyForceElements.Add(dummyDataCard.DummyForceElement);
                }
            }
            _context.SaveChanges();

            SetViewData(packageOwnersOrganization, selectedPurpose, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, forcePackage);
            ViewData["operationListForCheckboxes"] = _context.Operations.Where(d => d.Archived == false).OrderBy(d => d.Ordered).AsNoTracking().ToList();
            ViewData["conplanListForCheckboxes"] = _context.Conplans.Where(d => d.Archived == false).OrderBy(d => d.Ordered).AsNoTracking().ToList();

            if (forcePackage == null)
            {
                return NotFound();
            }
            SetForceElementList(forcePackage, null, "", "", "", null);
            return View(forcePackage);
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public ActionResult EditDummyForceElement(int DummyForceElementId, int ForcePackageId, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan,
            bool onlyShowMyForcePackages, string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? selectedOrganization, string? selectedConPlan, string? selectedOperation,
            int? readinessStatus, string? forcePackageName, string? forcePackagePurpose, string? forcePackageDescription, string? checkedOperations, string? checkedConplans, string? selectedForceElementIds, string? selectedDummyForceElementIds)
        {
            lang = cultureHelper.GetCurrentCulture();

            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }

            SetViewData(packageOwnersOrganization, selectedPurpose, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);
            ViewData["ForcePackageId"] = ForcePackageId;
            ViewData["WeightingId"] = new SelectList(_context.Weightings.AsNoTracking().ToList().OrderByDescending(w => w.WeightValue), "Id", "WeightValue");
            ViewData["OrganizationId"] = new SelectList(_context.Organizations.Where(d => d.Archived == false).AsNoTracking(), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");
            ViewData["PetsOverallStatus"] = _context.PetsoverallStatuses.Where(p => p.Archived == false).AsNoTracking();
            ViewData["deployStatus"] = _context.DeployedStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking();
            ViewData["forcePackageName"] = forcePackageName;
            ViewData["forcePackagePurpose"] = forcePackagePurpose;
            ViewData["forcePackageDescription"] = forcePackageDescription;
            ViewData["checkedOperations"] = checkedOperations;
            ViewData["checkedConplans"] = checkedConplans;
            ViewData["selectedForceElementIds"] = selectedForceElementIds;
            ViewData["selectedDummyForceElementIds"] = selectedDummyForceElementIds;

            DummyDataCard dummyDataCard = forcePackageHelper.GetDummyForceElementsQueryWIncludes().Where(d => d.Id == DummyForceElementId).First().DummyDataCards.First();
            return View(dummyDataCard);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public IActionResult EditDummyForceElement(DummyDataCard? dummyDataCard, int ForcePackageId, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan,
            bool onlyShowMyForcePackages, string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? selectedOrganization, string? selectedConPlan, string? selectedOperation,
            int? readinessStatus, string? forcePackageName, string? forcePackagePurpose, string? forcePackageDescription, string? checkedOperations, string? checkedConplans, string? selectedForceElementIds, string? selectedDummyForceElementIds)
        {
            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }

            SetViewData(packageOwnersOrganization, selectedPurpose, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);

            if (dummyDataCard == null)
            {
                return NotFound();
            }

            string emailBody = "This is an alert that force package: " + forcePackageName//dummyDataCard.DummyForceElement.ForcePackage.ForcePackageName
                + " has changed. The dummy force element: " + dummyDataCard.DummyForceElement.ElementName + " was modified.";
            SendKpiAlerts(ForcePackageId, emailBody);

            // Fixes error saying conflict force package users
            dummyDataCard.DummyForceElement.ForcePackage = null!;
            dummyDataCard.DummyForceElement.ForcePackageId = ForcePackageId;

            _context.Update(dummyDataCard);
            _context.SaveChanges();

            return RedirectToAction("Edit", new
            {
                id = ForcePackageId,
                packageOwnersOrganization,
                selectedPurpose,
                readinessStatus,
                selectedOrganization,
                selectedConPlan,
                selectedOperation,
                identifiedOperation,
                identifiedConplan,
                onlyShowMyForcePackages,
                indexPageSize,
                indexPageNumber,
                indexSortOrder,
                forcePackageName,
                forcePackagePurpose,
                forcePackageDescription,
                checkedOperations,
                checkedConplans,
                selectedForceElementIds,
                selectedDummyForceElementIds
            });
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public ActionResult DeleteDummyForceElement(int DummyForceElementId, int ForcePackageId, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages,
             string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? selectedOrganization, string? selectedConPlan, string? selectedOperation, int? readinessStatus,
             string? forcePackageName, string? forcePackagePurpose, string? forcePackageDescription, string? checkedOperations, string? checkedConplans, string? selectedForceElementIds, string? selectedDummyForceElementIds)
        {
            lang = cultureHelper.GetCurrentCulture();

            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }

            SetViewData(packageOwnersOrganization, selectedPurpose, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);
            ViewData["ForcePackageId"] = ForcePackageId;
            ViewData["WeightingId"] = new SelectList(_context.Weightings.AsNoTracking().ToList().OrderByDescending(w => w.WeightValue), "Id", "WeightValue");
            ViewData["OrganizationId"] = new SelectList(_context.Organizations.Where(d => d.Archived == false).AsNoTracking(), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");
            ViewData["PetsOverallStatus"] = _context.PetsoverallStatuses.Where(p => p.Archived == false).AsNoTracking();
            ViewData["deployStatus"] = _context.DeployedStatuses.Where(d => d.Archived == false).AsNoTracking();
            ViewData["forcePackageName"] = forcePackageName;
            ViewData["forcePackagePurpose"] = forcePackagePurpose;
            ViewData["forcePackageDescription"] = forcePackageDescription;
            ViewData["checkedOperations"] = checkedOperations;
            ViewData["checkedConplans"] = checkedConplans;
            ViewData["selectedForceElementIds"] = selectedForceElementIds;
            ViewData["selectedDummyForceElementIds"] = selectedDummyForceElementIds;

            DummyForceElement? dummyForceElement = forcePackageHelper.GetDummyForceElementsQueryWIncludes().Where(d => d.Id == DummyForceElementId).FirstOrDefault();
            if (dummyForceElement == null || dummyForceElement.DummyDataCards.First() == null)
            {
                return NotFound();
            }
            return View(dummyForceElement.DummyDataCards.First());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public IActionResult DeleteDummyForceElement(DummyDataCard dummyDataCard, int ForcePackageId, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages,
             string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? selectedOrganization, string? selectedConPlan, string? selectedOperation, int? readinessStatus,
             string? forcePackageName, string? forcePackagePurpose, string? forcePackageDescription, string? checkedOperations, string? checkedConplans, string? selectedForceElementIds, string? selectedDummyForceElementIds)
        {
            if (dummyDataCard == null)
            {
                return NotFound();
            }

            DummyForceElement? tempDummyForceElement = forcePackageHelper.GetDummyForceElementsQueryWIncludes().Where(fp => fp.Id == dummyDataCard.DummyForceElementId).FirstOrDefault();
            ForcePackage? tempForcePackage = forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == ForcePackageId).FirstOrDefault();

            if (tempDummyForceElement == null || tempForcePackage == null)
            {
                return NotFound();
            }

            DummyDataCard? tempDummyDataCard = tempDummyForceElement.DummyDataCards.FirstOrDefault();

            if (tempDummyDataCard == null)
            {
                return NotFound();
            }

            string emailBody = "This is an alert that force package: " + forcePackageName//dummyDataCard.DummyForceElement.ForcePackage.ForcePackageName
                + " has changed. The dummy force element: " + dummyDataCard.DummyForceElement.ElementName + " was deleted.";
            SendKpiAlerts(ForcePackageId, emailBody);

            _context.DummyDataCards.Remove(tempDummyDataCard);
            _context.SaveChanges();
            _context.DummyForceElements.Remove(tempDummyForceElement);
            _context.SaveChanges();
            tempForcePackage.DummyForceElements.Remove(tempDummyForceElement);
            _context.Update(tempForcePackage);
            _context.ChangeTracker.DetectChanges();
            _context.SaveChanges();

            return RedirectToAction("Edit", new
            {
                id = ForcePackageId,
                packageOwnersOrganization,
                selectedPurpose,
                readinessStatus,
                selectedOrganization,
                selectedConPlan,
                selectedOperation,
                identifiedOperation,
                identifiedConplan,
                onlyShowMyForcePackages,
                indexPageSize,
                indexPageNumber,
                indexSortOrder,
                forcePackageName,
                forcePackagePurpose,
                forcePackageDescription,
                checkedOperations,
                checkedConplans,
                selectedForceElementIds,
                selectedDummyForceElementIds
            });
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public ActionResult CreateDummyForceElement(int ForcePackageId, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan, bool onlyShowMyForcePackages,
             string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? selectedOrganization, string? selectedConPlan, string? selectedOperation, int? readinessStatus,
             string? forcePackageName, string? forcePackagePurpose, string? forcePackageDescription, string? checkedOperations, string? checkedConplans, string? selectedForceElementIds, string? selectedDummyForceElementIds)
        {
            lang = cultureHelper.GetCurrentCulture();

            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }

            SetViewData(packageOwnersOrganization, selectedPurpose, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);
            ViewData["ForcePackageId"] = ForcePackageId;
            ViewData["WeightingId"] = new SelectList(_context.Weightings.AsNoTracking().ToList().OrderByDescending(w => w.WeightValue), "Id", "WeightValue");
            ViewData["OrganizationId"] = new SelectList(_context.Organizations.Where(d => d.Archived == false).AsNoTracking(), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");
            ViewData["PetsOverallStatus"] = _context.PetsoverallStatuses.Where(p => p.Archived == false).AsNoTracking();
            ViewData["deployStatus"] = _context.DeployedStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking();

            ViewData["forcePackageName"] = forcePackageName;
            ViewData["forcePackagePurpose"] = forcePackagePurpose;
            ViewData["forcePackageDescription"] = forcePackageDescription;
            ViewData["checkedOperations"] = checkedOperations;
            ViewData["checkedConplans"] = checkedConplans;
            ViewData["selectedForceElementIds"] = selectedForceElementIds;
            ViewData["selectedDummyForceElementIds"] = selectedDummyForceElementIds;

            DummyDataCard dummyDataCard = new()
            {
                DummyForceElement = new()
                {
                    ForcePackage = forcePackageHelper.GetForcePackageQueryWIncludes().Where(f => f.Id == ForcePackageId).AsNoTracking().First(),
                    IsTiedToRealFelm = false,
                    IsActiveInForcePackage = false
                }
            };

            return View(dummyDataCard);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public IActionResult CreateDummyForceElement(DummyDataCard? dummyDataCard, int ForcePackageId, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan,
            bool onlyShowMyForcePackages, string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? selectedOrganization, string? selectedConPlan, string? selectedOperation,
            int? readinessStatus, string? forcePackageName, string? forcePackagePurpose, string? forcePackageDescription, string? checkedOperations, string? checkedConplans, string? selectedForceElementIds, string? selectedDummyForceElementIds)
        {
            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }
            SetViewData(packageOwnersOrganization, selectedPurpose, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);
            ForcePackage? forcePackage = forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == ForcePackageId).FirstOrDefault();

            if (dummyDataCard == null || forcePackage == null)
            {
                return NotFound();
            }

            string emailBody = "This is an alert that force package: " + forcePackageName//dummyDataCard.DummyForceElement.ForcePackage.ForcePackageName
                            + " has changed. The dummy force element: " + dummyDataCard.DummyForceElement.ElementName + " was created.";
            SendKpiAlerts(ForcePackageId, emailBody);

            // Fixes error saying conflict force package users
            dummyDataCard.DummyForceElement.ForcePackage = null!;
            dummyDataCard.DummyForceElement.ForcePackageId = ForcePackageId;

            _context.DummyDataCards.Add(dummyDataCard);
            _context.SaveChanges();
            forcePackage.DummyForceElements.Add(dummyDataCard.DummyForceElement);
            _context.Update(forcePackage);
            _context.SaveChanges();

            return RedirectToAction("Edit", new
            {
                id = ForcePackageId,
                packageOwnersOrganization,
                selectedPurpose,
                readinessStatus,
                selectedOrganization,
                selectedConPlan,
                selectedOperation,
                identifiedOperation,
                identifiedConplan,
                onlyShowMyForcePackages,
                indexPageSize,
                indexPageNumber,
                indexSortOrder,
                forcePackageName,
                forcePackagePurpose,
                forcePackageDescription,
                checkedOperations,
                checkedConplans,
                selectedForceElementIds,
                selectedDummyForceElementIds
            });
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public async Task<IActionResult> ManageUsers(int forcePackageId, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan,
            bool onlyShowMyForcePackages, string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? selectedOrganization, string? selectedConPlan,
            string? selectedOperation, int? readinessStatus, string? userPageSize, int? userPageNumber, string userSortOrder, string? forcePackageName, string? forcePackagePurpose,
            string? forcePackageDescription, string? checkedOperations, string? checkedConplans, string? selectedForceElementIds, string? selectedDummyForceElementIds)
        {
            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }
            if (string.IsNullOrEmpty(userPageSize))
            {
                userPageSize = "10";
            }
            SetViewData(packageOwnersOrganization, selectedPurpose, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);
            ForcePackage? forcePackage = await forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == forcePackageId).FirstOrDefaultAsync();
            if (forcePackage == null)
            {
                return NotFound();
            }

            ViewData["userPageSize"] = userPageSize;
            ViewData["userPageNumber"] = userPageNumber;
            ViewData["userSortOrder"] = userSortOrder;
            ViewData["givenUser"] = GetCurrentUser();
            ViewData["forcePackageName"] = forcePackageName;
            ViewData["forcePackagePurpose"] = forcePackagePurpose;
            ViewData["forcePackageDescription"] = forcePackageDescription;
            ViewData["checkedOperations"] = checkedOperations;
            ViewData["checkedConplans"] = checkedConplans;
            ViewData["selectedForceElementIds"] = selectedForceElementIds;
            ViewData["selectedDummyForceElementIds"] = selectedDummyForceElementIds;

            IQueryable<User> users = _context.Users.Where(user => forcePackage.Users.Contains(user)).Include(user => user.Organization).AsNoTracking();

            // Sort users
            switch (userSortOrder)
            {
                case "username_asc":
                    users = users.OrderBy(u => u.UserName);
                    break;

                case "username_desc":
                    users = users.OrderByDescending(u => u.UserName);
                    break;

                case "organization_asc":
                    users = users.OrderBy(u => u.Organization.OrganizationName);
                    break;

                case "organization_desc":
                    users = users.OrderByDescending(u => u.Organization.OrganizationName);
                    break;
            }

            ViewData["userList"] = await PaginatedList<User>.CreateAsync(users, userPageNumber ?? 1, int.Parse(userPageSize));
            return View(forcePackage);
        }

        // Add user to force package
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public async Task<IActionResult> ManageUsers(ForcePackage? forcePackage, string UserName, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation,
            int? identifiedConplan, bool onlyShowMyForcePackages, string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? selectedOrganization,
            string? selectedConPlan, string? selectedOperation, int? readinessStatus, string? userPageSize, int? userPageNumber, string userSortOrder, string? forcePackageName,
            string? forcePackagePurpose, string? forcePackageDescription, string? checkedOperations, string? checkedConplans, string? selectedForceElementIds, string? selectedDummyForceElementIds)
        {
            if (forcePackage == null)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }
            if (string.IsNullOrEmpty(userPageSize))
            {
                userPageSize = "10";
            }

            SetViewData(packageOwnersOrganization, selectedPurpose, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);
            User? user = await _context.Users.Where(user => user.UserName == UserName).FirstOrDefaultAsync();
            ForcePackage? forcePackageWIncludes = await forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == forcePackage.Id).AsNoTracking().FirstOrDefaultAsync();

            if (forcePackageWIncludes == null)
            {
                return NotFound();
            }

            ViewData["userPageSize"] = userPageSize;
            ViewData["userPageNumber"] = userPageNumber;
            ViewData["userSortOrder"] = userSortOrder;

            IQueryable<User> users = _context.Users.Where(user => forcePackageWIncludes.Users.Contains(user)).AsNoTracking();
            ViewData["userList"] = await PaginatedList<User>.CreateAsync(users, userPageNumber ?? 1, int.Parse(userPageSize));

            if (forcePackage == null || user == null)
            {
                return View(forcePackage);
            }

            forcePackage.Users.Add(user);
            _context.Update(forcePackage);
            _context.SaveChanges();

            return RedirectToAction("ManageUsers", new
            {
                forcePackageId = forcePackage.Id,
                packageOwnersOrganization,
                selectedPurpose,
                identifiedOperation,
                identifiedConplan,
                onlyShowMyForcePackages,
                indexPageSize,
                indexPageNumber,
                indexSortOrder,
                selectedOrganization,
                selectedConPlan,
                selectedOperation,
                readinessStatus,
                userPageSize,
                userPageNumber,
                userSortOrder,
                forcePackageName,
                forcePackagePurpose,
                forcePackageDescription,
                checkedOperations,
                checkedConplans,
                selectedForceElementIds,
                selectedDummyForceElementIds
            });
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public async Task<IActionResult> RemoveUser(int forcePackageId, int userId, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation,
            int? identifiedConplan, bool onlyShowMyForcePackages, string? indexPageSize, int? indexPageNumber, string indexSortOrder, string? selectedOrganization,
            string? selectedConPlan, string? selectedOperation, int? readinessStatus, string? userPageSize, int? userPageNumber, string userSortOrder,
            string? forcePackageName, string? forcePackagePurpose, string? forcePackageDescription, string? checkedOperations, string? checkedConplans,
            string? selectedForceElementIds, string? selectedDummyForceElementIds)
        {
            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }
            if (string.IsNullOrEmpty(userPageSize))
            {
                userPageSize = "10";
            }

            ForcePackage? forcePackage = await forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == forcePackageId).FirstOrDefaultAsync();
            if (forcePackage == null)
            {
                return NotFound();
            }
            User? user = _context.Users.Where(user => user.Id == userId).FirstOrDefault();

            SetViewData(packageOwnersOrganization, selectedPurpose, readinessStatus, selectedOrganization, selectedConPlan, selectedOperation, identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);
            ViewData["userPageSize"] = userPageSize;
            ViewData["userPageNumber"] = userPageNumber;
            ViewData["userSortOrder"] = userSortOrder;

            IQueryable<User> users = _context.Users.Where(user => forcePackage.Users.Contains(user)).AsNoTracking();
            ViewData["userList"] = await PaginatedList<User>.CreateAsync(users, userPageNumber ?? 1, int.Parse(userPageSize));

            if (forcePackage == null || user == null || !forcePackage.Users.Contains(user))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return NotFound();
            }

            forcePackage.Users.Remove(user);
            _context.Update(forcePackage);
            _context.SaveChanges();

            return RedirectToAction("ManageUsers", new
            {
                forcePackageId = forcePackage.Id,
                packageOwnersOrganization,
                selectedPurpose,
                identifiedOperation,
                identifiedConplan,
                onlyShowMyForcePackages,
                indexPageSize,
                indexPageNumber,
                indexSortOrder,
                selectedOrganization,
                selectedConPlan,
                selectedOperation,
                readinessStatus,
                userPageSize,
                userPageNumber,
                userSortOrder,
                forcePackageName,
                forcePackagePurpose,
                forcePackageDescription,
                checkedOperations,
                checkedConplans,
                selectedForceElementIds,
                selectedDummyForceElementIds
            });
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + Modeler)]
        public IActionResult CompareIndex(bool compare, int numberOfPackages, string? serializedForcePackageIds, int? compareModelId, int? packageOwnersOrganization, int? selectedPurpose, int? identifiedOperation, int? identifiedConplan,
            bool onlyShowMyForcePackages, string? indexPageSize, int? indexPageNumber, string indexSortOrder)
        {
            lang = cultureHelper.GetCurrentCulture();
            if (string.IsNullOrEmpty(indexPageSize))
            {
                indexPageSize = "10";
            }
            SetViewData(packageOwnersOrganization, selectedPurpose, null, "", "", "", identifiedOperation, identifiedConplan, onlyShowMyForcePackages, indexPageSize, indexPageNumber, indexSortOrder, null);
            ViewData["compare"] = compare;
            ViewData["numberOfPackages"] = numberOfPackages;
            ViewData["serializedForcePackageIds"] = serializedForcePackageIds;
            ViewData["deployedStatusListNonSelect"] = _context.DeployedStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).ToList();
            ViewData["overallStatusList"] = _context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking().ToList();
            ViewData["cmdOverrideStatusList"] = _context.CommandOverideStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).ToList();

            PetsoverallStatus? notReadyStatus = ((List<PetsoverallStatus>)ViewData["overallStatusList"]).Find(s => s.Id == 4);
            CommandOverideStatus? notReadyStatusCmd = ((List<CommandOverideStatus>)ViewData["cmdOverrideStatusList"]).Find(s => s.Id == 4);
            if (compareModelId != null)
            {
                ViewData["compareModelId"] = compareModelId;
            }

            int currentUserId = GetCurrentUser()!.Id;
            List<FPcompareViewModel> fPcompareViewModels = new();
            List<FpcompareModel> fpcompareModels = _context.FpcompareModels.Where(f => f.UserId == currentUserId).ToList();
            foreach (var item in fpcompareModels)
            {
                // Transfer compare model to view model with force package names
                FPcompareViewModel fPcompareViewModel = new()
                {
                    Id = item.Id,
                    SerializedForcePackageIds = item.SerializedForcePackageIds
                };
                List<int>? forcePackageIds = JsonConvert.DeserializeObject<List<string>>(item.SerializedForcePackageIds)?.Select(int.Parse).ToList();
                if (forcePackageIds != null && forcePackageIds.Any())
                {
                    string mainForcePackage = _context.ForcePackages.Where(f => f.Id == forcePackageIds.First()).Select(f => f.ForcePackageName).First();
                    fPcompareViewModel.MainForcePackage = mainForcePackage;
                    forcePackageIds.RemoveAt(0);
                    fPcompareViewModel.OtherForcePackageNames = _context.ForcePackages.Where(f => forcePackageIds.Contains(f.Id)).Select(f => f.ForcePackageName).ToList();
                    fPcompareViewModels.Add(fPcompareViewModel);
                }
            }
            // Saved compare model list
            ViewData["fPcompareViewModels"] = fPcompareViewModels;

            // Make list of force packages actively being compared
            List<FPSharedViewModel> FPSharedViewModels = new();
            try
            {
                List<int>? deserializedFPIds = JsonConvert.DeserializeObject<List<string>>(serializedForcePackageIds)?.Select(int.Parse).ToList();
                if (deserializedFPIds != null)
                {
                    foreach (int forcePackageId in deserializedFPIds)
                    {
                        ForcePackage? tempForcePackage = forcePackageHelper.GetForcePackageQueryWIncludes().Where(fp => fp.Id == forcePackageId).FirstOrDefault();
                        if (tempForcePackage != null)
                        {
                            FPSharedViewModel FPSharedViewModel = new(tempForcePackage);
                            FPSharedViewModels.Add(FPSharedViewModel);
                        }
                    }
                }
            }
            catch (Exception) { }

            // Set priority of force packages from left to right, marking further recoccurences of felms as not ready (red)
            if (FPSharedViewModels.Any())
            {
                for (int i = 0; i < FPSharedViewModels.Count - 1; i++)
                {
                    FPSharedViewModel fPSharedViewModel = FPSharedViewModels[i];
                    List<DummyForceElement> otherDummyFelms = fPSharedViewModel.OtherDummyForceElements.Where(d => d.IsActiveInForcePackage && d.IsTiedToRealFelm).ToList();
                    foreach (DummyForceElement dummyForceElement in otherDummyFelms)
                    {
                        for (int j = i + 1; j < FPSharedViewModels.Count; j++)
                        {
                            FPSharedViewModel tempFpSharedViewModel = FPSharedViewModels[j];
                            DummyForceElement? tempDummyFelm = tempFpSharedViewModel.OtherDummyForceElements
                                .Where(d => d.IsTiedToRealFelm && d.IsActiveInForcePackage && d.ForceElementId == dummyForceElement.ForceElementId).FirstOrDefault();
                            if (tempDummyFelm != null)
                            {
                                tempFpSharedViewModel.OtherDummyForceElements.Remove(tempDummyFelm);
                                tempDummyFelm.DummyDataCards.First().SrStatus = notReadyStatus; 
                                if (tempDummyFelm.DummyDataCards.First().CommandOverideStatus != null) { 
                                    tempDummyFelm.DummyDataCards.First().CommandOverideStatus = notReadyStatusCmd;
                                }
                                KeyValuePair<string, DummyForceElement> kvp = new(fPSharedViewModel.ForcePackageName, tempDummyFelm);
                                tempFpSharedViewModel.SharedDummyForceElements.Add(kvp);
                            }
                        }
                    }
                }
                // Generate readiness table for given force package
                foreach (var item in FPSharedViewModels)
                {
                    List<DummyDataCard> dummyDataCards = item.OtherDummyForceElements.Where(f => f.IsActiveInForcePackage).Select(f => f.DummyDataCards.First()).ToList();
                    dummyDataCards.AddRange(item.SharedDummyForceElements.Where(f => f.Value.IsActiveInForcePackage).Select(f => f.Value.DummyDataCards.First()).ToList());
                    item.DRTModel = new DatacardReadinessTableCalculator().CalculateDisplay(dummyDataCards);
                }
            }

            return View(FPSharedViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public int SaveCompareModel(string serializedForcePackageIds)
        {
            if (!string.IsNullOrEmpty(serializedForcePackageIds) && serializedForcePackageIds != "[]")
            {
                FpcompareModel fpCompareModel = new()
                {
                    UserId = GetCurrentUser()!.Id,
                    SerializedForcePackageIds = serializedForcePackageIds
                };
                _context.Add(fpCompareModel);
                _context.SaveChanges();
                return fpCompareModel.Id;
            }
            else
            {
                return 0;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void DeleteCompareModel(int id)
        {
            FpcompareModel? fpcompareModel = _context.FpcompareModels.Where(f => f.Id == id).FirstOrDefault();
            if (fpcompareModel != null)
            {
                _context.FpcompareModels.Remove(fpcompareModel);
                _context.SaveChanges();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public List<string> FindForcePackagesByName(string name, string[] packageIdsToExclude)
        {
            List<int> intPackageIdsToExclude = new();
            if (packageIdsToExclude.Length > 0)
            {
                intPackageIdsToExclude = packageIdsToExclude.Select(int.Parse).ToList();
            }
            List<ForcePackage> forcePackages = _context.ForcePackages.Where(fp => fp.ForcePackageName.ToLower().Contains(name.ToLower())
                                            && !intPackageIdsToExclude.Contains(fp.Id)).ToList();
            List<string> forcePackagesList = new();
            foreach (ForcePackage forcePackage in forcePackages)
            {
                var temp = new { label = forcePackage.ForcePackageName, value = forcePackage.Id };
                forcePackagesList.Add(JsonConvert.SerializeObject(temp));
            }
            return forcePackagesList;
        }

        public string FindSingleForcePackageByName(string name)
        {
            name = name.ToLower();
            ForcePackage? forcePackage = forcePackageHelper.GetForcePackageQueryWIncludes()
                                            .Where(fp => fp.ForcePackageName.ToLower().Contains(name))
                                            .AsNoTracking().FirstOrDefault();
            if (forcePackage == null) return "";
            return JsonConvert.SerializeObject(forcePackage, jsonSerializerSettings);
        }

        public string FindSingleForcePackageById(int id)
        {
            ForcePackage? forcePackage = forcePackageHelper.GetForcePackageQueryWIncludes()
                                            .Where(fp => fp.Id == id)
                                            .AsNoTracking().FirstOrDefault();
            if (forcePackage == null) return "";
            return JsonConvert.SerializeObject(forcePackage, jsonSerializerSettings);
        }
    }
}