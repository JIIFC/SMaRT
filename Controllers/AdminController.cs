using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.AccountManagement;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SMARTV3.Helpers;
using SMARTV3.Models;
using SMARTV3.Security;

using static Constants;
using static SMARTV3.Helpers.PaginationHelper;
using static SMARTV3.Security.UserRoleProvider;

using EnvironmentName = Microsoft.AspNetCore.Hosting.EnvironmentName;

namespace SMARTV3.Controllers
{
    [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + ReadOnlyUser)]
    public class AdminController : Controller
    {
        private SMARTV3DbContext _context;
        private bool isDevelopment;
        private PrincipalContext AD; // active directory connection

        public AdminController(SMARTV3DbContext context)
        {
            _context = context;
            isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Development;
            //AD = new(ContextType.Domain, isDevelopment ? DevLdapUrl : ProdLdapUrl);
        }

        // Sets viewdata for stuff that gets set a lot
        private void SetViewData(int selectedPageSize, bool showDisabledFelms, bool showAspirationalFelms, string organizationFilter, string elementIdFilter, string elementNameFilter, int pageNumber)
        {
            ViewData["selectedPageSize"] = selectedPageSize;
            ViewData["showDisabledFelms"] = showDisabledFelms;
            ViewData["showAspirationalFelms"] = showAspirationalFelms;
            ViewData["organizationFilter"] = organizationFilter;
            ViewData["ElementIdFilter"] = elementIdFilter;
            ViewData["ElementNameFilter"] = elementNameFilter;
            ViewData["pageNumber"] = pageNumber;
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser)]
        public IActionResult Index() => View();

        #region Archive

        public ActionResult ArchiveModal()
        {
            return PartialView("ArchiveModal");
        }

        public ActionResult ArchiveCommentTable()
        {
            return PartialView("ArchiveCommentTable");
        }

        public string GetArchiveComment(int commentId)
        {
            ArchiveComment? archiveComment = _context.ArchiveComments.Include(c => c.ChangeUserNavigation).Where(c => c.Id == commentId).FirstOrDefault();
            if (archiveComment == null)
            {
                return "";
            }
            var tempComment = new
            {
                archiveComment.Archived,
                archiveComment.Comments,
                ChangeDate = archiveComment.ChangeDate.ToString("M/d/yyyy h:mm tt"),
                ChangeUser = archiveComment.ChangeUserNavigation.UserName
            };
            return JsonConvert.SerializeObject(tempComment);
        }

        private ArchiveComment CreateArchiveComment(string ArchiveReason, bool ArchiveStatus)
        {
            return new ArchiveComment()
            {
                Comments = ArchiveReason,
                Archived = ArchiveStatus,
                ChangeDate = DateTime.Now,
                ChangeUser = GetCurrentUser()!.Id
            };
        }

        #endregion Archive

        #region ReportPOCInformation

        // This manages the report point of contact info in the database. This info is inserted into the generated excel reports

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditReportPOC()
        {
            ReportPocinformation? reportPOC = await _context.ReportPocinformations.FirstOrDefaultAsync();
            if (reportPOC == null) return NotFound();
            return View(reportPOC);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditReportPOC(ReportPocinformation? reportPOC)
        {
            if (reportPOC != null)
            {
                _context.Update(reportPOC);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        #endregion ReportPOCInformation

        #region Users

        // Used on add user page to autocomplete potential usernames
        public List<string> FindUsersInAD(string userName)
        {
            List<string> existingUsernames = _context.Users.Select(u => u.UserName).ToList();
            UserPrincipal userPrincipal = new(AD)
            {
                SamAccountName = "*" + userName + "*"
            };
            PrincipalSearcher search = new(userPrincipal);
            List<string> potentialUsernames = search.FindAll().AsQueryable().Select(user => user.SamAccountName).Take(50).ToList();
            search.Dispose();
            if (!potentialUsernames.Any())
            {
                return potentialUsernames;
            }
            potentialUsernames.RemoveAll(item => existingUsernames.Contains(item));
            return potentialUsernames;
        }

        public string FindUserInAD(string userName)
        {
            string jsonUser;
            UserPrincipal userPrincipal = new(AD)
            {
                SamAccountName = userName
            };
            PrincipalSearcher search = new(userPrincipal);
            UserPrincipal foundUser = (UserPrincipal)search.FindOne();
            search.Dispose();
            if (isDevelopment)
            {
                jsonUser = JsonConvert.SerializeObject(new
                {
                    FirstName = foundUser.GivenName,
                    LastName = foundUser.Surname,
                    Email = foundUser.UserPrincipalName
                });
            }
            else
            {
                jsonUser = JsonConvert.SerializeObject(new
                {
                    FirstName = foundUser.GivenName,
                    LastName = foundUser.Surname,
                    Email = foundUser.EmailAddress
                });
            }
            return jsonUser;
        }

        public List<string> FindExistingUsers(string userName)
        {
            return _context.Users.Where(user => user.UserName.Contains(userName))
                                 .Select(user => user.UserName).ToList();
        }

        private List<SelectListItem> MakeRoleList(string selectedRole)
        {
            List<SelectListItem> items = new();
            List<Role>? roles = _context.Roles.OrderBy(i => i.Order).ToList();
            for (int i = 0; i < roles.Count; i++)
            {
                if (roles[i].RoleName == Admin && User.IsInRole(SuperUser)) continue;
                SelectListItem tempItem = new() { Text = roles[i].RoleName, Value = roles[i].RoleName };
                // Set selected role as selected in dropdown
                if (roles[i].RoleName == selectedRole) tempItem.Selected = true;
                items.Add(tempItem);
            };
            return items;
        }

        private MultiSelectList MakeRoleMultiList(string[] selectedRoles)
        {
            List<Role>? roles = _context.Roles.OrderBy(i => i.Order).ToList();
            return new MultiSelectList(roles, "Id", "RoleName", selectedRoles); 
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser)]
        public async Task<IActionResult> ManageUsers(string sortOrder, string selectedOrganization, bool showDisabledUsers, string? selectedPageSize, int? pageNumber)
        {
            CultureHelper cultureHelper = new();
            string lang = cultureHelper.GetCurrentCulture();

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            ViewData["selectedPageSize"] = int.Parse(selectedPageSize);
            ViewData["itemsPerPage"] = GetItemsPerPageList(selectedPageSize);
            ViewData["pageNumber"] = pageNumber;
            ViewData["sortOrder"] = sortOrder;
            ViewData["showDisabledUsers"] = showDisabledUsers;
            ViewData["selectedOrganization"] = selectedOrganization;

            ViewData["organizationList"] = new SelectList(_context.Organizations.OrderBy(x => x.Ordered).Where(d => d.Archived == false), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");

            IQueryable<User>? users = from user in _context.Users.Include(user => user.Roles)
                                                                 .Include(user => user.Organization)
                                      where user.Enabled == !showDisabledUsers
                                      select user;

            if (!string.IsNullOrEmpty(selectedOrganization))
            {
                users = users.Where(i => i.OrganizationId == int.Parse(selectedOrganization));
            }
            users = sortOrder switch
            {
                "username_asc" => users.OrderBy(i => i.UserName),
                "username_desc" => users.OrderByDescending(i => i.UserName),
                "role_asc" => users.OrderBy(i => i.Roles.First().RoleName),
                "role_desc" => users.OrderByDescending(i => i.Roles.First().RoleName),
                "org_asc" => users.OrderBy(i => i.Organization.OrganizationName),
                "org_desc" => users.OrderByDescending(i => i.Organization.OrganizationName),
                _ => users.OrderBy(i => i.UserName)
            };
            return View("User/ManageUsers",
                await PaginatedList<User>.CreateAsync(users.AsNoTracking(), pageNumber ?? 1, int.Parse(selectedPageSize)));
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser)]
        public async Task<IActionResult> EditUser(int id, string sortOrder, string selectedOrganization, bool showDisabledUsers, string? selectedPageSize, int? pageNumber)
        {
            CultureHelper cultureHelper = new();
            string lang = cultureHelper.GetCurrentCulture();

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            ViewData["selectedPageSize"] = int.Parse(selectedPageSize);
            ViewData["pageNumber"] = pageNumber;
            ViewData["sortOrder"] = sortOrder;
            ViewData["showDisabledUsers"] = showDisabledUsers;
            ViewData["selectedOrganization"] = selectedOrganization;

            User? user = await _context.Users.Include(i => i.Roles).Include(i => i.Organization).FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) return NotFound();
            if (user.Roles.First().RoleName == Admin && !User.IsInRole(Admin)) return Unauthorized();
            ViewBag.Roles = MakeRoleMultiList(user.Roles.Select(x => x.Id.ToString()).ToArray());

            ViewData["OrganizationID"] = new SelectList(_context.Organizations.OrderBy(x => x.Ordered).Where(d => d.Archived == false), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre", user.OrganizationId);

            return View("User/EditUser", user);
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(User? user, string[] SelectedRole, string sortOrder, string selectedOrganization, bool showDisabledUsers, string? selectedPageSize, int? pageNumber, bool POC)
        {
            CultureHelper cultureHelper = new();
            string lang = cultureHelper.GetCurrentCulture();

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            ViewData["selectedPageSize"] = int.Parse(selectedPageSize);
            ViewData["pageNumber"] = pageNumber;
            ViewData["sortOrder"] = sortOrder;
            ViewData["showDisabledUsers"] = showDisabledUsers;
            ViewData["selectedOrganization"] = selectedOrganization;

            ModelState.Remove("sortOrder");
            ModelState.Remove("selectedOrganization");
            ModelState.Remove("showDisabledUsers");
            ModelState.Remove("selectedPageSize");
            ModelState.Remove("pageNumber");

            User? tempUser = _context.Users
                .Include(i => i.Roles)
                .Include(i => i.Organization)
                .FirstOrDefault(m => m.Id == user!.Id);

            if (user!.OrganizationId != 0)
            {
                ModelState.Remove("Organization");
            }
            if (ModelState.IsValid
                && user != null
                && tempUser != null
                && SelectedRole.Count() != 0
                && user.OrganizationId != 0)
            {
                IEnumerable<Role?> editRole = _context.Roles.Where(i => SelectedRole.Contains(i.Id.ToString()) );
                if (editRole != null && editRole.Count() != 0)
                {
                    Role readOnlyRole = _context.Roles.Where(i => i.RoleName == ReadOnlyUser).FirstOrDefault();
                    // Update users role
                    tempUser.Roles.Clear();
                    if (!editRole.Contains(readOnlyRole)) {
                        foreach (Role userRole in editRole) {
                            tempUser.Roles.Add(userRole);
                        }
                    }
                    else
                    {
                        tempUser.Roles.Add(readOnlyRole);
                    }
                }
                // Only update changed fields
                if (user.UserName != tempUser.UserName) tempUser.UserName = user.UserName;
                if (user.FirstName != tempUser.FirstName) tempUser.FirstName = user.FirstName;
                if (user.LastName != tempUser.LastName) tempUser.LastName = user.LastName;
                if (user.Email != tempUser.Email) tempUser.Email = user.Email;
                if (user.Title != tempUser.Title) tempUser.Title = user.Title;
                if (user.Enabled != tempUser.Enabled && tempUser.POC)
                {
                    tempUser.Enabled = user.Enabled;
                    user.POC = false;
                }
                else
                {
                    tempUser.Enabled = user.Enabled;
                }
                if (user.POC != tempUser.POC) tempUser.POC = user.POC;                
                if (user.OrganizationId != tempUser.OrganizationId) tempUser.OrganizationId = user.OrganizationId;
                _context.Update(tempUser);
                _context.SaveChanges();
            }
            else
            {
                ViewBag.Roles = MakeRoleList("");

                ViewData["OrganizationID"] = new SelectList(_context.Organizations.OrderBy(x => x.Ordered).Where(d => d.Archived == false), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre", user!.OrganizationId);

                return View("User/EditUser", user);
            }

            return RedirectToAction("ManageUsers", new { sortOrder, selectedOrganization, showDisabledUsers, selectedPageSize, pageNumber });
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser)]
        public ActionResult AddUser(string sortOrder, string selectedOrganization, bool showDisabledUsers, string? selectedPageSize, int? pageNumber)
        {
            CultureHelper cultureHelper = new();
            string lang = cultureHelper.GetCurrentCulture();

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            ViewData["selectedPageSize"] = int.Parse(selectedPageSize);
            ViewData["pageNumber"] = pageNumber;
            ViewData["sortOrder"] = sortOrder;
            ViewData["showDisabledUsers"] = showDisabledUsers;
            ViewData["selectedOrganization"] = selectedOrganization;
            Role readOnlyRole = _context.Roles.Where(i => i.RoleName == ReadOnlyUser).FirstOrDefault();
            ViewBag.Roles = MakeRoleMultiList(new[] { readOnlyRole.Id.ToString() });

            ViewData["OrganizationID"] = new SelectList(_context.Organizations.OrderBy(x => x.Ordered).Where(d => d.Archived == false), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");

            return View("User/AddUser");
        }

        [CustomAuthorize(Roles = Admin + "," + SuperUser)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(User? user, string[] SelectedRole, string? sortOrder, string? selectedOrganization, bool? showDisabledUsers, string? selectedPageSize, int? pageNumber, bool POC)
        {
            CultureHelper cultureHelper = new();
            string lang = cultureHelper.GetCurrentCulture();
            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            ViewData["selectedPageSize"] = int.Parse(selectedPageSize);
            ViewData["pageNumber"] = pageNumber;
            ViewData["sortOrder"] = sortOrder;
            ViewData["showDisabledUsers"] = showDisabledUsers;
            ViewData["selectedOrganization"] = selectedOrganization;

            if (user!.OrganizationId != 0)
            {
                ModelState.Remove("Organization");
            }
            if (ModelState.IsValid && user != null && SelectedRole.Count() != 0 && user.OrganizationId != 0)
            {
                IEnumerable<Role?> editRole = _context.Roles.Where(i => SelectedRole.Contains(i.Id.ToString()));
                //Role? editRole = _context.Roles.Where(i => i.RoleName == SelectedRole).FirstOrDefault();
                if (editRole == null) return NotFound();
                //user.Roles.Add(editRole);
                Role readOnlyRole = _context.Roles.Where(i => i.RoleName == ReadOnlyUser).FirstOrDefault();
                if (!editRole.Contains(readOnlyRole))
                {
                    foreach (Role userRole in editRole)
                    {
                        user.Roles.Add(userRole);
                    }
                }
                else
                {
                    user.Roles.Add(readOnlyRole);
                }
                if (!ModelState.IsValid)
                {
                    return View("User/AddUser", user);
                }

                _context.Add(user);
                _context.SaveChanges();

                return RedirectToAction("ManageUsers");
            }
            ViewBag.Roles = MakeRoleList(ReadOnlyUser);
            ViewData["OrganizationID"] = new SelectList(_context.Organizations.OrderBy(x => x.Ordered).Where(d => d.Archived == false), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");
            return View("User/AddUser", user);
        }

        [CustomAuthorize(Roles = Admin)]
        public IActionResult DeleteUser(int id, string sortOrder, string selectedOrganization, bool showDisabledUsers, string? selectedPageSize, int? pageNumber)
        {
            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            ViewData["selectedPageSize"] = int.Parse(selectedPageSize);
            ViewData["pageNumber"] = pageNumber;
            ViewData["sortOrder"] = sortOrder;
            ViewData["showDisabledUsers"] = showDisabledUsers;
            ViewData["selectedOrganization"] = selectedOrganization;

            User? user = _context.Users.Where(i => i.Id == id).FirstOrDefault();
            bool error = _context.ChangeLogs.Any(log => log.LastEditUser == id) || _context.DataCardHistories.Any(log => log.LastEditUser == id);
            if (user == null) return NotFound();
            if (error) return RedirectToAction("UserDeleteError");

            return View("User/DeleteUser", user);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(User? user, string sortOrder, string selectedOrganization, bool showDisabledUsers, string? selectedPageSize, int? pageNumber)
        {
            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            ViewData["selectedPageSize"] = int.Parse(selectedPageSize);
            ViewData["pageNumber"] = pageNumber;
            ViewData["sortOrder"] = sortOrder;
            ViewData["showDisabledUsers"] = showDisabledUsers;
            ViewData["selectedOrganization"] = selectedOrganization;

            User? tempUser = null;
            if (user == null) return NotFound();
            tempUser = _context.Users.Include(i => i.Roles).Where(i => i.Id == user.Id).FirstOrDefault();
            if (tempUser == null) return NotFound();

            tempUser.Roles.Clear();
            _context.Update(tempUser);
            _context.SaveChanges();
            _context.Users.Remove(tempUser);
            _context.ChangeTracker.DetectChanges();
            _context.SaveChanges();
            return RedirectToAction("ManageUsers", new { sortOrder, selectedOrganization, showDisabledUsers, selectedPageSize, pageNumber });
        }

        public ActionResult UserDeleteError(string sortOrder, string selectedOrganization, bool showDisabledUsers, string? selectedPageSize, int? pageNumber)
        {
            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            ViewData["selectedPageSize"] = int.Parse(selectedPageSize);
            ViewData["pageNumber"] = pageNumber;
            ViewData["sortOrder"] = sortOrder;
            ViewData["showDisabledUsers"] = showDisabledUsers;
            ViewData["selectedOrganization"] = selectedOrganization;
            return View("User/UserDeleteError");
        }

        private User? GetCurrentUser()
        {
            if (User.Identity == null || User.Identity.Name == null) return null;
            string username = RemoveDomainFromUsername(User.Identity.Name);
            User? user = null;
            if (string.IsNullOrEmpty(username)) return null;
            user = _context.Users.Include(i => i.Roles).Include(i => i.Organization).FirstOrDefault(m => m.UserName == username);
            return user;
        }

        #endregion Users

        #region Conplans

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageConplans()
        {
            List<Conplan> conplans = await _context.Conplans
                                        .Include(m => m.ConplanArchiveComments)
                                        .ThenInclude(m => m.ArchiveComment)
                                        .ThenInclude(m => m.ChangeUserNavigation)
                                        .ToListAsync();
            return View("Conplan/ManageConplans", conplans);
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditConplan(int id)
        {
            Conplan? conplan = await _context.Conplans.Where(m => m.Id == id).Include(m => m.ConplanArchiveComments)
                                                                                .ThenInclude(m => m.ArchiveComment)
                                                                                    .ThenInclude(m => m.ChangeUserNavigation)
                                                                             .FirstOrDefaultAsync();
            return View("Conplan/EditConplan", conplan);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditConplan(Conplan? conplan, string? ArchiveReason)
        {
            if (conplan != null)
            {
                bool currentArchiveStatus = _context.Conplans.AsNoTracking().Where(c => c.Id == conplan.Id).First().Archived;
                if (currentArchiveStatus != conplan.Archived && ArchiveReason != null)
                {
                    ConplanArchiveComment conplanArchiveComment = new()
                    {
                        ConplanId = conplan.Id,
                        ArchiveComment = CreateArchiveComment(ArchiveReason, conplan.Archived)
                    };
                    _context.ConplanArchiveComments.Add(conplanArchiveComment);
                    _context.SaveChanges();
                }
                _context.Update(conplan);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageConplans");
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddConplan()
        {
            return View("Conplan/AddConplan");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddConplan(Conplan? conplan)
        {
            if (conplan != null)
            {
                _context.Add(conplan);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageConplans");
        }

        #endregion Conplans

        #region AfsTrainingPercentage

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageAfsTrainingPercentages()
        {
            List<AfsTrainingPercentage> afsTrainingPercentages = await _context.AfsTrainingPercentages.Include(m => m.AfsTrainingPercentageArchiveComments)
                                                                                                        .ThenInclude(m => m.ArchiveComment)
                                                                                                            .ThenInclude(m => m.ChangeUserNavigation)
                                                                                                      .ToListAsync();
            return View("AfsTrainingPercentage/ManageAfsTrainingPercentages", afsTrainingPercentages);
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditAfsTrainingPercentage(int id)
        {
            AfsTrainingPercentage? afsTrainingPercentage = await _context.AfsTrainingPercentages.Include(m => m.AfsTrainingPercentageArchiveComments)
                                                                                                    .ThenInclude(m => m.ArchiveComment)
                                                                                                        .ThenInclude(m => m.ChangeUserNavigation)
                                                                                                .FirstOrDefaultAsync(m => m.Id == id);
            return View("AfsTrainingPercentage/EditAfsTrainingPercentage", afsTrainingPercentage);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAfsTrainingPercentage(AfsTrainingPercentage? afsTrainingPercentage, string? ArchiveReason)
        {
            if (afsTrainingPercentage != null)
            {
                bool currentArchiveStatus = _context.AfsTrainingPercentages.AsNoTracking().Where(c => c.Id == afsTrainingPercentage.Id).First().Archived;
                if (currentArchiveStatus != afsTrainingPercentage.Archived && ArchiveReason != null)
                {
                    AfsTrainingPercentageArchiveComment afsTrainingPercentageArchiveComment = new()
                    {
                        AfsTrainingPercentageId = afsTrainingPercentage.Id,
                        ArchiveComment = CreateArchiveComment(ArchiveReason, afsTrainingPercentage.Archived)
                    };
                    _context.AfsTrainingPercentageArchiveComments.Add(afsTrainingPercentageArchiveComment);
                    _context.SaveChanges();
                }
                _context.Update(afsTrainingPercentage);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageAfsTrainingPercentages");
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddAfsTrainingPercentage()
        {
            return View("AfsTrainingPercentage/AddAfsTrainingPercentage");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAfsTrainingPercentage(AfsTrainingPercentage? afsTrainingPercentage)
        {
            if (afsTrainingPercentage != null)
            {
                _context.Add(afsTrainingPercentage);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageAfsTrainingPercentages");
        }

        #endregion AfsTrainingPercentage

        #region NTM
        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageNTM()
        {
            List<NoticeToMove> NTMs = await _context.NoticeToMoves.ToListAsync();
            return View("NTM/ManageNTM", NTMs);
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditNTM(int id)
        {
            NoticeToMove? NTM = await _context.NoticeToMoves.FirstOrDefaultAsync(m => m.Id == id);
            return View("NTM/EditNTM", NTM);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditNTM(NoticeToMove? NTM)
        {
            if (NTM != null)
            {   
                _context.Update(NTM);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageNTM");
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddNTM()
        {
            return View("NTM/AddNTM");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNTM(NoticeToMove? NTM)
        {
            if (NTM != null)
            {
                _context.Add(NTM);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageNTM");
        }

        #endregion NTM

        #region Operations

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageOperations()
        {
            List<Operation> operations = await _context.Operations.Include(m => m.OperationArchiveComments)
                                                                    .ThenInclude(m => m.ArchiveComment)
                                                                        .ThenInclude(m => m.ChangeUserNavigation)
                                                                  .ToListAsync();
            return View("Operation/ManageOperations", operations);
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditOperation(int id)
        {
            Operation? operation = await _context.Operations.Include(m => m.OperationArchiveComments)
                                                                .ThenInclude(m => m.ArchiveComment)
                                                                    .ThenInclude(m => m.ChangeUserNavigation)
                                                            .FirstOrDefaultAsync(m => m.Id == id);
            return View("Operation/EditOperation", operation);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOperation(Operation? operation, string? ArchiveReason)
        {
            if (operation != null)
            {
                bool currentArchiveStatus = _context.Operations.AsNoTracking().Where(c => c.Id == operation.Id).First().Archived;
                if (currentArchiveStatus != operation.Archived && ArchiveReason != null)
                {
                    OperationArchiveComment operationArchiveComment = new()
                    {
                        OperationId = operation.Id,
                        ArchiveComment = CreateArchiveComment(ArchiveReason, operation.Archived)
                    };
                    _context.OperationArchiveComments.Add(operationArchiveComment);
                    _context.SaveChanges();
                }
                _context.Update(operation);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageOperations");
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddOperation()
        {
            return View("Operation/AddOperation");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOperation(Operation? operation)
        {
            if (operation != null)
            {
                _context.Add(operation);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageOperations");
        }

        #endregion Operations

        #region Capabilities

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageCapabilities()
        {
            List<Capability> capabilities  = await _context.Capabilities.Include(m => m.CapabilityArchiveComments)
                                                                            .ThenInclude(m => m.ArchiveComment)
                                                                                .ThenInclude(m => m.ChangeUserNavigation)
                                                                       .ToListAsync();
            return View("Capability/ManageCapabilities", capabilities);
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditCapability(int id)
        {
            Capability? capability = await _context.Capabilities.Include(m => m.CapabilityArchiveComments)
                                                                    .ThenInclude(m => m.ArchiveComment)
                                                                        .ThenInclude(m => m.ChangeUserNavigation)
                                                                .FirstOrDefaultAsync(m => m.Id == id);

            ViewData["metlList"] = new SelectList(_context.Metls.Where(m => m.Archived == false), "MetId", "MetName");
            ViewData["CapMetlList"] = JsonConvert.SerializeObject(_context.CapibilityMets.Where(x => x.CapabilityId == id).Join(_context.Metls, CapMetls => CapMetls.MetId, metl => metl.MetId, (CapMetls, metl) => new { CapMetls, metl }).Select(m => new { metlId = m.CapMetls.MetId, metlName = m.metl.MetName, validFrom = m.CapMetls.ValidFrom, validTo = m.CapMetls.ValidTo }));
            return View("Capability/EditCapability", capability);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCapability(Capability? capability, string? ArchiveReason, string? ParentMetlList)
        {
            ICollection<Models.MetlList> MetlListCollection;

            if (capability != null)
            {

                IQueryable<CapibilityMet> currentList = _context.CapibilityMets.Where(x => x.CapabilityId == capability.Id);
                if (currentList.Count() > 0)
                {
                    foreach (CapibilityMet metl in currentList)
                    {
                        _context.Remove(metl);
                    }
                }

                if (ParentMetlList != null)
                {

                    MetlListCollection = JsonConvert.DeserializeObject<ICollection<MetlList>>(ParentMetlList)!;

                    foreach (MetlList ml in MetlListCollection)
                    {
                        if (ml == null) continue;
                        CapibilityMet cm = new CapibilityMet();
                        cm.CapabilityId = capability.Id;
                        cm.MetId = ml.metlId;
                        cm.ValidFrom = ml.validFrom;
                        cm.ValidTo = ml.validto;
                        _context.CapibilityMets.Add(cm);
                    }
                }
                bool currentArchiveStatus = _context.Capabilities.AsNoTracking().Where(c => c.Id == capability.Id).First().Archived;
                if (currentArchiveStatus != capability.Archived && ArchiveReason != null)
                {
                    CapabilityArchiveComment capabilityArchiveComment = new()
                    {
                        CapabilityId = capability.Id,
                        ArchiveComment = CreateArchiveComment(ArchiveReason, capability.Archived)
                    };
                    _context.CapabilityArchiveComments.Add(capabilityArchiveComment);
                    _context.SaveChanges();
                }
                _context.Update(capability);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageCapabilities");
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddCapability()
        {
            ViewData["metlList"] = new SelectList(_context.Metls.Where(m => m.Archived == false), "MetId", "MetName");
            return View("Capability/AddCapability");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCapability(Capability? capability, string? ParentMetlList)
        {
            if (capability != null)
            {
                ICollection<Models.MetlList> MetlListCollection;

                
            
                _context.Add(capability);
                _context.SaveChanges();
                if (ParentMetlList != null)
                {

                    MetlListCollection = JsonConvert.DeserializeObject<ICollection<MetlList>>(ParentMetlList)!;

                    foreach (MetlList ml in MetlListCollection)
                    {
                        if (ml == null) continue;
                        CapibilityMet cm = new CapibilityMet();
                        cm.CapabilityId = capability.Id;
                        cm.MetId = ml.metlId;
                        cm.ValidFrom = ml.validFrom;
                        cm.ValidTo = ml.validto;
                        _context.CapibilityMets.Add(cm);
                    }
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("ManageCapabilities");
        }

        #endregion Capabilities

        #region Categories

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageCategories()
        {
            List<Category> categories = await _context.Categories.Include(m => m.CategoryArchiveComments)
                                                                    .ThenInclude(m => m.ArchiveComment)
                                                                        .ThenInclude(m => m.ChangeUserNavigation)
                                                                 .ToListAsync();
            return View("Category/ManageCategories", categories);
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditCategory(int id)
        {
            Category? category = await _context.Categories.Include(m => m.CategoryArchiveComments)
                                                            .ThenInclude(m => m.ArchiveComment)
                                                                .ThenInclude(m => m.ChangeUserNavigation)
                                                          .FirstOrDefaultAsync(m => m.Id == id);
            return View("Category/EditCategory", category);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(Models.Category? category, string? ArchiveReason)
        {
            if (category != null)
            {
                bool currentArchiveStatus = _context.Categories.AsNoTracking().Where(c => c.Id == category.Id).First().Archived;
                if (currentArchiveStatus != category.Archived && ArchiveReason != null)
                {
                    CategoryArchiveComment categoryArchiveComment = new()
                    {
                        CategoryId = category.Id,
                        ArchiveComment = CreateArchiveComment(ArchiveReason, category.Archived)
                    };
                    _context.CategoryArchiveComments.Add(categoryArchiveComment);
                    _context.SaveChanges();
                }
                _context.Update(category);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageCategories");
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddCategory()
        {
            return View("Category/AddCategory");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(Models.Category? category)
        {
            if (category != null)
            {
                _context.Add(category);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageCategories");
        }

        #endregion Categories

        #region Designations

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageDesignations()
        {
            List<Designation> designations = await _context.Designations.Include(m => m.DesignationArchiveComments)
                                                                            .ThenInclude(m => m.ArchiveComment)
                                                                                .ThenInclude(m => m.ChangeUserNavigation)
                                                                        .ToListAsync();
            return View("Designation/ManageDesignations", designations);
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditDesignation(int id)
        {
            Designation? designation = await _context.Designations.Include(m => m.DesignationArchiveComments)
                                                                    .ThenInclude(m => m.ArchiveComment)
                                                                        .ThenInclude(m => m.ChangeUserNavigation)
                                                                  .FirstOrDefaultAsync(m => m.Id == id);
            return View("Designation/EditDesignation", designation);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDesignation(Designation? designation, string? ArchiveReason)
        {
            if (designation != null)
            {
                bool currentArchiveStatus = _context.Designations.AsNoTracking().Where(c => c.Id == designation.Id).First().Archived;
                if (currentArchiveStatus != designation.Archived && ArchiveReason != null)
                {
                    DesignationArchiveComment designationArchiveComment = new()
                    {
                        DesignationId = designation.Id,
                        ArchiveComment = CreateArchiveComment(ArchiveReason, designation.Archived)
                    };
                    _context.DesignationArchiveComments.Add(designationArchiveComment);
                    _context.SaveChanges();
                }
                _context.Update(designation);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageDesignations");
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddDesignation()
        {
            return View("Designation/AddDesignation");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddDesignation(Designation? designation)
        {
            if (designation != null)
            {
                _context.Add(designation);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageDesignations");
        }

        [CustomAuthorize(Roles = Admin)]

        #endregion Designations

        #region Organizations

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageOrganizations()
        {
            List<Organization> organizations = await _context.Organizations.Include(m => m.OrganizationArchiveComments)
                                                                                .ThenInclude(m => m.ArchiveComment)
                                                                                    .ThenInclude(m => m.ChangeUserNavigation)
                                                                           .ToListAsync();
            return View("Organization/ManageOrganizations", organizations);
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditOrganization(int id)
        {
            Organization? organization = await _context.Organizations.Include(m => m.OrganizationArchiveComments)
                                                                        .ThenInclude(m => m.ArchiveComment)
                                                                            .ThenInclude(m => m.ChangeUserNavigation)
                                                                     .FirstOrDefaultAsync(m => m.Id == id);
            return View("Organization/EditOrganization", organization);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOrganization(Organization? organization, string? ArchiveReason)
        {
            if (organization != null)
            {
                bool currentArchiveStatus = _context.Organizations.AsNoTracking().Where(c => c.Id == organization.Id).First().Archived;
                if (currentArchiveStatus != organization.Archived && ArchiveReason != null)
                {
                    OrganizationArchiveComment organizationArchiveComment = new()
                    {
                        OrganizationId = organization.Id,
                        ArchiveComment = CreateArchiveComment(ArchiveReason, organization.Archived)
                    };
                    _context.OrganizationArchiveComments.Add(organizationArchiveComment);
                    _context.SaveChanges();
                }
                _context.Update(organization);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageOrganizations");
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddOrganization()
        {
            return View("Organization/AddOrganization");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrganization(Organization? organization)
        {
            if (organization != null)
            {
                _context.Add(organization);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageOrganizations");
        }

        #endregion Organizations

        #region Metls

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageMetls()
        {
            List<Metl> metls = await _context.Metls.ToListAsync();
            return View("Metl/ManageMetls", metls);
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditMetl(int id)
        {
            Metl? metl = await _context.Metls.FirstOrDefaultAsync(m => m.MetId == id);
            return View("METL/EditMETL", metl);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMetl(Metl? metl, string? ArchiveReason)
        {
            if (metl != null)
            {                
                _context.Update(metl);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageMETLS");
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddMetl()
        {
            return View("METL/AddMETL");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMetl(Metl? metl)
        {
            if (metl != null)
            {
                _context.Add(metl);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageMETLS");
        }

        #endregion Metls

        #region Services

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageServices()
        {
            List<Service> services = await _context.Services.Include(m => m.ServiceArchiveComments)
                                                                .ThenInclude(m => m.ArchiveComment)
                                                                    .ThenInclude(m => m.ChangeUserNavigation)
                                                            .ToListAsync();
            return View("Service/ManageServices", services);
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditService(int id)
        {
            Service? service = await _context.Services.Include(m => m.ServiceArchiveComments)
                                                        .ThenInclude(m => m.ArchiveComment)
                                                            .ThenInclude(m => m.ChangeUserNavigation)
                                                      .FirstOrDefaultAsync(m => m.Id == id);
            return View("Service/EditService", service);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditService(Service? service, string? ArchiveReason)
        {
            if (service != null)
            {
                bool currentArchiveStatus = _context.Services.AsNoTracking().Where(c => c.Id == service.Id).First().Archived;
                if (currentArchiveStatus != service.Archived && ArchiveReason != null)
                {
                    ServiceArchiveComment serviceArchiveComment = new()
                    {
                        ServiceId = service.Id,
                        ArchiveComment = CreateArchiveComment(ArchiveReason, service.Archived)
                    };
                    _context.ServiceArchiveComments.Add(serviceArchiveComment);
                    _context.SaveChanges();
                }
                _context.Update(service);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageServices");
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddService()
        {
            return View("Service/AddService");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddService(Service? service)
        {
            if (service != null)
            {
                _context.Add(service);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageServices");
        }

        #endregion Services

        #region Force Elements

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageForceElements(bool showDisabledFelms, bool showAspirationalFelms, string organizationFilter, string elementIdFilter, string elementNameFilter, string? selectedPageSize, int? pageNumber, string? sortOrder)
        {
            CultureHelper cultureHelper = new();
            string lang = cultureHelper.GetCurrentCulture();

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(int.Parse(selectedPageSize), showDisabledFelms, showAspirationalFelms, organizationFilter, elementIdFilter, elementNameFilter, pageNumber ?? 1);

            ViewData["itemsPerPage"] = GetItemsPerPageList(selectedPageSize);
            ViewData["sortOrder"] = sortOrder;
            ViewData["OrganizationID"] = new SelectList(_context.Organizations.OrderBy(x => x.Ordered).Where(d => d.Archived == false), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");

            IQueryable<ForceElement> forceElements = from d in _context.ForceElements.Include(d => d.DataCards)
                                                                                         .ThenInclude(f => f.NoticeToMove)
                                                                                     .Include(d => d.Organization)
                                                                                     .Include(d => d.Weighting)
                                                                                     .Include(d => d.ForceElementArchiveComments)
                                                                                         .ThenInclude(f => f.ArchiveComment)
                                                                                             .ThenInclude(a => a.ChangeUserNavigation)
                                                     where d.Archived == showDisabledFelms && d.Aspirational == showAspirationalFelms
                                                     select d;

            if (!string.IsNullOrEmpty(organizationFilter))
            {
                forceElements = forceElements.Where(d => d.OrganizationId == int.Parse(organizationFilter));
            }

            if (!string.IsNullOrEmpty(elementIdFilter))
            {
                forceElements = forceElements.Where(d => EF.Functions.Like(d.ElementId, "%" + elementIdFilter + "%"));
            }

            if (!string.IsNullOrEmpty(elementNameFilter))
            {
                forceElements = forceElements.Where(d => EF.Functions.Like(d.ElementName!, "%" + elementNameFilter + "%"));
            }

            switch (sortOrder)
            {
                case "elemId_asc":
                    forceElements = forceElements.OrderBy(d => d.ElementId);
                    break;

                case "elemId_desc":
                    forceElements = forceElements.OrderByDescending(d => d.ElementId);
                    break;

                case "elemName_asc":
                    forceElements = forceElements.OrderBy(d => d.ElementName);
                    break;

                case "elemName_desc":
                    forceElements = forceElements.OrderByDescending(d => d.ElementName);
                    break;

                case "elemNameFr_asc":
                    forceElements = forceElements.OrderBy(d => d.ElementNameFre);
                    break;

                case "elemNameFr_desc":
                    forceElements = forceElements.OrderByDescending(d => d.ElementNameFre);
                    break;

                case "orgFG_asc":
                    forceElements = forceElements.OrderBy(d => d.Organization.OrganizationName);
                    break;

                case "orgFG_desc":
                    forceElements = forceElements.OrderByDescending(d => d.Organization.OrganizationName);
                    break;

                case "weight_asc":
                    forceElements = forceElements.OrderBy(d => d.Weighting!.WeightValue);
                    break;

                case "weight_desc":
                    forceElements = forceElements.OrderByDescending(d => d.Weighting!.WeightValue);
                    break;

                case "ntm_asc":
                    forceElements = forceElements.OrderBy(d => d.DataCards.First().NoticeToMove!.NoticeToMoveName);
                    break;

                case "ntm_desc":
                    forceElements = forceElements.OrderByDescending(d => d.DataCards.First().NoticeToMove!.NoticeToMoveName);
                    break;

                case "sortOrder_asc":
                    forceElements = forceElements.OrderBy(d => d.Ordered);
                    break;

                case "sortOrder_desc":
                    forceElements = forceElements.OrderByDescending(d => d.Ordered);
                    break;

                case "commDate_asc":
                    forceElements = forceElements.OrderBy(
                        d => d.ForceElementArchiveComments.OrderByDescending(a => a.ArchiveComment.ChangeDate).FirstOrDefault()!.ArchiveComment.ChangeDate
                    );
                    break;

                case "commDate_desc":
                    forceElements = forceElements.OrderByDescending(
                        d => d.ForceElementArchiveComments.OrderByDescending(a => a.ArchiveComment.ChangeDate).FirstOrDefault()!.ArchiveComment.ChangeDate
                    );
                    break;
            }

            return View("ForceElement/ManageForceElements", await PaginatedList<ForceElement>.CreateAsync(forceElements.AsNoTracking(), pageNumber ?? 1, int.Parse(selectedPageSize)));
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditForceElement(int id, bool showDisabledFelms, bool showAspirationalFelms, string organizationFilter, string elementIdFilter, string elementNameFilter, string? selectedPageSize, int? pageNumber, string? sortOrder)
        {
            CultureHelper cultureHelper = new();
            string lang = cultureHelper.GetCurrentCulture();

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            ForceElement? forceElement = await _context.ForceElements.Include(m => m.ForceElementArchiveComments)
                                                                        .ThenInclude(m => m.ArchiveComment)
                                                                            .ThenInclude(m => m.ChangeUserNavigation)
                                                                     .Include(m => m.DataCards)
                                                                     .FirstOrDefaultAsync(m => m.Id == id);
            DataCard? dataCard = forceElement!.DataCards.FirstOrDefault();

            SetViewData(int.Parse(selectedPageSize), showDisabledFelms, showAspirationalFelms, organizationFilter, elementIdFilter, elementNameFilter, pageNumber ?? 1);
            ViewData["sortOrder"] = sortOrder;
            ViewData["metlList"] = new SelectList(_context.Metls.Where(m => m.Archived == false), "MetId", "MetName");
            ViewData["FelmMetlList"] = JsonConvert.SerializeObject(_context.FelmMetls.Where(x => x.FelmId == id).Join(_context.Metls, felmMetls => felmMetls.MetId, metl => metl.MetId, (felmMetls, metl) => new { felmMetls, metl }).Select(m => new { metlId = m.felmMetls.MetId, metlName = m.metl.MetName, validFrom = m.felmMetls.ValidFrom, validTo = m.felmMetls.ValidTo }));
            ViewBag.WeightingId = new SelectList(_context.Weightings.ToList().OrderByDescending(w => w.WeightValue), "Id", "WeightValue");
            ViewBag.OrganizationId = new SelectList(_context.Organizations.Where(d => d.Archived == false), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");
            ViewData["NoticeToMoveId"] = new SelectList(_context.NoticeToMoves.OrderBy(x => x.Ordered).Where(d => d.Archived == false).AsNoTracking(),
                "Id", lang == "en" ? "NoticeToMoveName" : "NoticeToMoveNameFre", dataCard != null ? dataCard!.NoticeToMoveId : null);
            ViewData["NtmOtherID"] = _context.NoticeToMoves.Where(c => c.NoticeToMoveName == "Other").FirstOrDefault()?.Id;
            ViewData["Ntmdetails"] = dataCard != null ? dataCard!.Ntmdetails : "";

            return View("ForceElement/EditForceElement", forceElement);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditForceElement(ForceElement? forceElement, int NoticeToMoveId, string Ntmdetails, string organizationFilter, bool showDisabledFelms, bool showAspirationalFelms, string elementIdFilter, string elementNameFilter, string? selectedPageSize, int? pageNumber, string? ArchiveReason, string? sortOrder, string? ParentMetlList)
        {

            CultureHelper cultureHelper = new();
            string lang = cultureHelper.GetCurrentCulture();
            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(int.Parse(selectedPageSize), showDisabledFelms, showAspirationalFelms, organizationFilter, elementIdFilter, elementNameFilter, pageNumber ?? 1);
            ViewData["sortOrder"] = sortOrder;
            ViewBag.WeightingId = new SelectList(_context.Weightings.ToList().OrderByDescending(w => w.WeightValue), "Id", "WeightValue");
            ViewBag.OrganizationId = new SelectList(_context.Organizations.Where(d => d.Archived == false), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");
            ViewData["NoticeToMoveId"] = new SelectList(_context.NoticeToMoves.OrderBy(x => x.Ordered).Where(d => d.Archived == false).AsNoTracking(),
                "Id", lang == "en" ? "NoticeToMoveName" : "NoticeToMoveNameFre", NoticeToMoveId);
            int? ntmOtherId = _context.NoticeToMoves.Where(c => c.NoticeToMoveName == "Other").FirstOrDefault()?.Id;
            ViewData["NtmOtherID"] = ntmOtherId;
            ViewData["Ntmdetails"] = Ntmdetails;
            ViewBag.parentMetlList = ParentMetlList;

            ICollection<Models.MetlList> MetlListCollection;

            if (forceElement == null)
            {
                return View("ForceElement/EditForceElement", forceElement);
            }

            DataCard? dataCard = _context.DataCards.Where(d => d.ForceElementId == forceElement.Id).FirstOrDefault();
            if (dataCard == null) return NotFound();

            // Organization id is set but foreign object doesn't matter here
            ModelState.Remove("Organization");
            ModelState.Remove("elementIdFilter");
            ModelState.Remove("elementNameFilter");
            ModelState.Remove("organizationFilter");
            ModelState.Remove("showDisabledFelms");
            ModelState.Remove("showAspirationalFelms");
            ModelState.Remove("Ntmdetails");

            IQueryable<FelmMetl> currentList = _context.FelmMetls.Where(x => x.FelmId == forceElement.Id);
            if (currentList.Count() > 0)
            {
                foreach (FelmMetl metl in currentList)
                {
                    _context.Remove(metl);
                }
            }

            if (ParentMetlList != null)
            {

                MetlListCollection = JsonConvert.DeserializeObject<ICollection<MetlList>>(ParentMetlList)!;

                foreach (MetlList ml in MetlListCollection)
                {
                    if (ml == null) continue;
                    FelmMetl fm = new FelmMetl();
                    fm.FelmId = forceElement.Id;
                    fm.MetId = ml.metlId;
                    fm.ValidFrom = ml.validFrom;
                    fm.ValidTo = ml.validto;
                    _context.FelmMetls.Add(fm);
                }
            }

            if (!ModelState.IsValid)
            {
                return View("ForceElement/EditForceElement", forceElement);
            }

            dataCard.NoticeToMoveId = NoticeToMoveId;
            dataCard.Ntmdetails = Ntmdetails;
            _context.Update(dataCard);
            _context.SaveChanges();

            bool currentArchiveStatus = _context.ForceElements.AsNoTracking().Where(c => c.Id == forceElement.Id).First().Archived;
            if (currentArchiveStatus != forceElement.Archived && ArchiveReason != null)
            {
                ForceElementArchiveComment forceElementArchiveComment = new()
                {
                    ForceElementId = forceElement.Id,
                    ArchiveComment = CreateArchiveComment(ArchiveReason, forceElement.Archived)
                };
                _context.ForceElementArchiveComments.Add(forceElementArchiveComment);
                _context.SaveChanges();
            }
            _context.Update(forceElement);
            _context.SaveChanges();
            return RedirectToAction("ManageForceElements", new { organizationFilter, elementIdFilter, elementNameFilter, selectedPageSize, pageNumber, sortOrder });
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddForceElement(string organizationFilter, bool showDisabledFelms, bool showAspirationalFelms, string elementIdFilter, string elementNameFilter, string? selectedPageSize, int? pageNumber, string? sortOrder)
        {
            CultureHelper cultureHelper = new();
            string lang = cultureHelper.GetCurrentCulture();

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(int.Parse(selectedPageSize), showDisabledFelms, showAspirationalFelms, organizationFilter, elementIdFilter, elementNameFilter, pageNumber ?? 1);
            ViewData["sortOrder"] = sortOrder;
            ViewData["metlList"] = new SelectList(_context.Metls.Where(m => m.Archived == false), "MetId", "MetName");
            ViewBag.WeightingId = new SelectList(_context.Weightings.ToList().OrderByDescending(w => w.WeightValue), "Id", "WeightValue");
            ViewBag.OrganizationId = new SelectList(_context.Organizations.Where(d => d.Archived == false), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");
            ViewData["NoticeToMoveId"] = new SelectList(_context.NoticeToMoves.OrderBy(x => x.Ordered).Where(d => d.Archived == false).AsNoTracking(), "Id", lang == "en" ? "NoticeToMoveName" : "NoticeToMoveNameFre");
            ViewData["NtmOtherID"] = _context.NoticeToMoves.Where(c => c.NoticeToMoveName == "Other").FirstOrDefault()?.Id;

            return View("ForceElement/AddForceElement");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddForceElement(ForceElement? forceElement, int NoticeToMoveId, string Ntmdetails, string organizationFilter, bool showDisabledFelms, bool showAspirationalFelms, string elementIdFilter, string elementNameFilter, string? selectedPageSize, int? pageNumber, string? sortOrder, string? ParentMetlList)
        {
            CultureHelper cultureHelper = new();
            string lang = cultureHelper.GetCurrentCulture();
            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(int.Parse(selectedPageSize), showDisabledFelms, showAspirationalFelms, organizationFilter, elementIdFilter, elementNameFilter, pageNumber ?? 1);
            ViewData["sortOrder"] = sortOrder;
            ViewData["metlList"] = new SelectList(_context.Metls.Where(m => m.Archived == false), "MetId", "MetName");
            ViewBag.WeightingId = new SelectList(_context.Weightings.ToList().OrderByDescending(w => w.WeightValue), "Id", "WeightValue");
            ViewBag.OrganizationId = new SelectList(_context.Organizations.Where(d => d.Archived == false), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");
            ViewData["NoticeToMoveId"] = new SelectList(_context.NoticeToMoves.OrderBy(x => x.Ordered).Where(d => d.Archived == false).AsNoTracking(),
                "Id", lang == "en" ? "NoticeToMoveName" : "NoticeToMoveNameFre", NoticeToMoveId);
            ViewData["Ntmdetails"] = Ntmdetails;
            int? ntmOtherId = _context.NoticeToMoves.Where(c => c.NoticeToMoveName == "Other").FirstOrDefault()?.Id;
            ViewData["NtmOtherID"] = ntmOtherId;
            ICollection<Models.MetlList> MetlListCollection;

            // Organization id is set but foreign object doesn't matter here
            ModelState.Remove("Organization");
            ModelState.Remove("elementIdFilter");
            ModelState.Remove("elementNameFilter");
            ModelState.Remove("organizationFilter");
            ModelState.Remove("showDisabledFelms");
            ModelState.Remove("showAspirationalFelms");
            ModelState.Remove("Ntmdetails");

            //if (NoticeToMoveId != ntmOtherId)
            //{
            //    ModelState.Remove("Ntmdetails");
            //}



            if (forceElement == null || !ModelState.IsValid)
            { 
                return View("ForceElement/AddForceElement", forceElement);
            }


            DataCard dataCard = new()
            {
                ForceElement = forceElement,
                NoticeToMoveId = NoticeToMoveId,
                Ntmdetails = Ntmdetails,
                SrStatusId = 4,
                PersonnelStatusId = 4,
                EquipmentStatusId = 4,
                EquipmentCombatVehicleStatusId = 4,
                EquipmentSupportVehicleStatusId = 4,
                EquipmentWeaponsServiceRateStatusId = 4,
                EquipmentCommunicationsEquipmentStatusId = 4,
                EquipmentSpecialEquipmentStatusId = 4,
                TrainingStatusId = 4,
                TrainingCollectiveTrainingStatusId = 4,
                TrainingIndividualTrainingStatusId = 4,
                SustainmentStatusId = 4,
                SustainmentCombatRationsStatusId = 4,
                SustainmentPersonalEquipmentStatusId = 4,
                SustainmentPetrolStatusId = 4,
                SustainmentAmmunitionStatusId = 4,
                SustainmentSparesStatusId = 4,
                SustainmentOtherStatusId = 4,
                LastEditUser = GetCurrentUser()?.Id,
                LastEditDate = DateTime.Today
            };

            _context.Add(dataCard);
            _context.SaveChanges();
            if (ParentMetlList != null)
            {

                MetlListCollection = JsonConvert.DeserializeObject<ICollection<MetlList>>(ParentMetlList)!;

                foreach (MetlList ml in MetlListCollection)
                {
                    if (ml == null) continue;
                    FelmMetl fm = new FelmMetl();
                    fm.FelmId = forceElement.Id;
                    fm.MetId = ml.metlId;
                    fm.ValidFrom = ml.validFrom;
                    fm.ValidTo = ml.validto;
                    _context.FelmMetls.Add(fm);
                }
                _context.SaveChanges();
            }
            
            return RedirectToAction("ManageForceElements", new { organizationFilter, elementIdFilter, elementNameFilter, selectedPageSize, pageNumber, sortOrder });
        }

        #endregion Force Elements

        #region Echelon

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageEchelons()
        {
            List<Echelon> echelons = await _context.Echelons.Include(m => m.EchelonArchiveComments)
                                                                .ThenInclude(m => m.ArchiveComment)
                                                                    .ThenInclude(m => m.ChangeUserNavigation)
                                                            .ToListAsync();

            return View("Echelon/ManageEchelons", echelons);
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> EditEchelon(int id)
        {
            Echelon? echelon = await _context.Echelons.Include(m => m.EchelonArchiveComments)
                                                        .ThenInclude(m => m.ArchiveComment)
                                                            .ThenInclude(m => m.ChangeUserNavigation)
                                                      .FirstOrDefaultAsync(m => m.Id == id);
            return View("Echelon/EditEchelon", echelon);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEchelon(Echelon? echelon, string? ArchiveReason)
        {
            if (echelon != null)
            {
                bool currentArchiveStatus = _context.Echelons.AsNoTracking().Where(c => c.Id == echelon.Id).First().Archived;
                if (currentArchiveStatus != echelon.Archived && ArchiveReason != null)
                {
                    EchelonArchiveComment echelonArchiveComment = new()
                    {
                        EchelonId = echelon.Id,
                        ArchiveComment = CreateArchiveComment(ArchiveReason, echelon.Archived)
                    };
                    _context.EchelonArchiveComments.Add(echelonArchiveComment);
                    _context.SaveChanges();
                }
                _context.Update(echelon);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageEchelons");
        }

        [CustomAuthorize(Roles = Admin)]
        public ActionResult AddEchelon()
        {
            return View("Echelon/AddEchelon");
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEchelon(Echelon? echelon)
        {
            if (echelon != null)
            {
                _context.Add(echelon);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageEchelons");
        }

        #endregion Echelon

        #region Data Card History

        [CustomAuthorize(Roles = Admin)]
        public string CreateHistoryData()
        {
            int currentYear = int.Parse(DateTime.Now.ToString("yyyy"));
            int currentMonth = int.Parse(DateTime.Now.ToString("MM"));

            //Check and see if the process has already been run
            int i = _context.HistoryMigrations.Count(d => d.HistoryYear == currentYear & d.HistoryMonth == currentMonth);

            //If the migration was not run for the current year and month
            if (i == 0)
            {
                string sql = "EXEC MigrateHistoryData @Year, @Month";
                List<SqlParameter> parms = new()
                {
                    // Create parameters
                    new SqlParameter { ParameterName = "@Year", Value = currentYear },
                    new SqlParameter { ParameterName = "@Month", Value = currentMonth }
                };

                int rowsAffected = _context.Database.ExecuteSqlRaw(sql, parms.ToArray());
            }
            return currentMonth.ToString() + " / " + currentYear.ToString();
        }

        [CustomAuthorize(Roles = Admin)]
        public IActionResult AddHistory()
        {
            int currentYear = int.Parse(DateTime.Now.ToString("yyyy"));
            int currentMonth = int.Parse(DateTime.Now.ToString("MM"));
            int i = _context.HistoryMigrations.Count(d => d.HistoryYear == currentYear && d.HistoryMonth == currentMonth);

            //If the migration was not run for the current year and month
            if (i == 0)
            {
                string dateString = CreateHistoryData();
                ViewBag.Date = dateString;
            }
            else
            {
                ViewBag.Date = "";
                ViewBag.Message = "The Add History has already been run for this month and year.";
            }

            return View("History/AddHistory");
        }

        [CustomAuthorize(Roles = Admin)]
        public IActionResult DeleteHistory(string id, string id2)
        {
            ViewBag.Date = id + " / " + id2;
            int year = int.Parse(id);
            int month = int.Parse(id2);
            DeleteHistoryData(year, month);
            return View("History/DeleteHistory");
        }

        [CustomAuthorize(Roles = Admin)]
        public IActionResult DeleteHistoryData(int year, int month)
        {
            string sql = "EXEC DeleteHistoryData @Year, @Month";
            List<SqlParameter> parms = new()
            {
                // Create parameters
                new SqlParameter { ParameterName = "@Year", Value = year },
                new SqlParameter { ParameterName = "@Month", Value = month }
            };

            _context.Database.ExecuteSqlRaw(sql, parms.ToArray());

            return RedirectToAction("Index", "Home");
        }

        [CustomAuthorize(Roles = Admin)]
        public async Task<IActionResult> ManageHistory()
        {
            return View("History/ManageHistory", await _context.HistoryMigrations.OrderByDescending(h => h.HistoryYear).ThenByDescending(h => h.HistoryMonth).ToListAsync());
        }

        #endregion Data Card History
    }
}