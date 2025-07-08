using DocumentFormat.OpenXml.InkML;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using SMARTV3.Helpers;
using SMARTV3.Models;
using SMARTV3.Security;

using static Constants;
using static SMARTV3.Helpers.PaginationHelper;
using static SMARTV3.Security.UserRoleProvider;

using EnvironmentName = Microsoft.AspNetCore.Hosting.EnvironmentName;

namespace SMARTV3.Controllers
{
    [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + ReportingUser + "," + FGPlanner + "," + ReadOnlyUser)]
    public class DataCardsController : Controller
    {
        private readonly SMARTV3DbContext _context;
        private readonly CultureHelper cultureHelper;
        private string lang; //Current UI language
        private readonly KpiEmail kpiEmail;
        private readonly bool isDevelopment;

        public DataCardsController(SMARTV3DbContext context, IConfiguration configuration)
        {
            _context = context;
            cultureHelper = new();
            lang = cultureHelper.GetCurrentCulture();
            isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Development;
            kpiEmail = new KpiEmail(configuration, isDevelopment);
        }

        private List<SelectListItem> MakeStatusList(string selectedStatus)
        {
            List<SelectListItem> tempStatusList = new() { };
            if (lang == "en")
            {
                foreach (KeyValuePair<string, string> option in StatusOptions)
                {
                    tempStatusList.Add(new SelectListItem() { Selected = selectedStatus == option.Value, Text = option.Key, Value = option.Value });
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> option in StatusOptionsFre)
                {
                    tempStatusList.Add(new SelectListItem() { Selected = selectedStatus == option.Value, Text = option.Key, Value = option.Value });
                }
            }
            return tempStatusList;
        }

        private List<SelectListItem> MakeDeployedStatusList(string selectedDeployedStatus)
        {
            string tempString;
            string statusDisplayValue = lang == "en" ? "StatusDisplayValue" : "StatusDisplayValueFre";
            List<DeployedStatus> deployedStatuses = _context.DeployedStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking().ToList();
            List<SelectListItem> tempDeployedStatusList = new SelectList(deployedStatuses, statusDisplayValue, statusDisplayValue).ToList();

            if (lang == "en")
            {
                tempString = deployedStatuses.First().StatusDisplayValue + "/" + deployedStatuses.Last().StatusDisplayValue;
            }
            else
            {
                tempString = deployedStatuses.First().StatusDisplayValueFre + "/" + deployedStatuses.Last().StatusDisplayValueFre;
            }

            tempDeployedStatusList.Add(new SelectListItem()
            {
                Text = tempString,
                Value = tempString
            });

            foreach (SelectListItem? item in tempDeployedStatusList)
            {
                if (item.Value == selectedDeployedStatus)
                {
                    item.Selected = true;
                }
            }

            return tempDeployedStatusList;
        }

        public string GetPetsOverallColourById(int id)
        {
            if (id == 0)
            {
                return "";
            }
            PetsoverallStatus? petsoverallStatus = _context.PetsoverallStatuses.Where(d => d.Archived == false && d.Id == id).FirstOrDefault();
            if (petsoverallStatus == null || petsoverallStatus.StatusDisplayColour == null)
            {
                return "";
            }
            return petsoverallStatus.StatusDisplayColour;
        }

        private bool CanUserEditDatacard(int OrganizationId)
        {
            User? user = GetCurrentUser();
            if (user == null || User.IsInRole(ReadOnlyUser)) return false;
            if (!User.IsInRole(Admin) && OrganizationId != user.OrganizationId) return false;
            return true;
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

        private void SetViewData(int selectedPageSize, int pageNumber, string? selectedOrganization, string? selectedConPlan, string? selectedOperation, string? selectedNato, string? selectedStatus, string? selectedDeployedStatus, string? sortOrder, int selectedOverallStatus, DataCard? dataCard, PETS? PETS)
        {
            ViewData["selectedPageSize"] = selectedPageSize;
            ViewData["pageNumber"] = pageNumber;
            ViewData["selectedOrganization"] = selectedOrganization ?? "";
            ViewData["selectedConPlan"] = selectedConPlan ?? "";
            ViewData["selectedOperation"] = selectedOperation ?? "";
            ViewData["selectedNato"] = selectedNato ?? "";
            ViewData["selectedStatus"] = selectedStatus ?? "";
            ViewData["selectedDeployedStatus"] = selectedDeployedStatus ?? "";
            ViewData["statusList"] = MakeStatusList(selectedStatus ?? "");
            ViewData["deployedStatusList"] = MakeDeployedStatusList(selectedDeployedStatus ?? "");
            ViewData["sortOrder"] = sortOrder ?? "";
            ViewData["selectedOverallStatus"] = selectedOverallStatus;

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

            ViewData["NatoStratLiftCapacityId"] = new SelectList(_context.NatoStratLiftCapacities.OrderBy(x => x.Ordered).Where(d => d.Archived == false).AsNoTracking(), "Id", lang == "en" ? "StratLiftCapacityName" : "StratLiftCapacityNameFre");
            ViewData["NatoNationalDeployId"] = new SelectList(_context.NatoNationalDeploys.OrderBy(x => x.Ordered).Where(d => d.Archived == false).AsNoTracking(), "Id", lang == "en" ? "NationalDeployName" : "NationalDeployNameFre");

            if (dataCard != null)
            {
                ViewData["CommandOverideStatusId"] = new SelectList(_context.CommandOverideStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", dataCard.CommandOverrideStatusId);
                ViewData["DeployedStatusId"] = new SelectList(_context.DeployedStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", dataCard.DeployedStatusId);
                ViewData["EquipmentCombatVehicleStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.EquipmentCombatVehicleStatusId);
                ViewData["EquipmentCommunicationsEquipmentStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.EquipmentCommunicationsEquipmentStatusId);
                ViewData["EquipmentSpecialEquipmentStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.EquipmentSpecialEquipmentStatusId);
                ViewData["EquipmentStatusId"] = new SelectList(_context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.EquipmentStatusId);
                ViewData["EquipmentSupportVehicleStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.EquipmentSupportVehicleStatusId);
                ViewData["EquipmentWeaponsServiceRateStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.EquipmentWeaponsServiceRateStatusId);
                ViewData["ForceElementId"] = new SelectList(_context.ForceElements.Where(d => d.Archived == false).AsNoTracking(), "Id", "Id", dataCard.ForceElementId);
                ViewData["LastEditUser"] = new SelectList(_context.Users.AsNoTracking(), "Id", "Id", dataCard.LastEditUser);
                ViewData["PersonnelStatusId"] = new SelectList(_context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.PersonnelStatusId);
                ViewData["SrStatusId"] = new SelectList(_context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", dataCard.SrStatusId);
                ViewData["SustainmentAmmunitionStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.SustainmentAmmunitionStatusId);
                ViewData["SustainmentCombatRationsStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.SustainmentCombatRationsStatusId);
                ViewData["SustainmentPersonalEquipmentStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.SustainmentPersonalEquipmentStatusId);
                ViewData["SustainmentPetrolStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.SustainmentPetrolStatusId);
                ViewData["SustainmentSparesStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.SustainmentSparesStatusId);
                ViewData["SustainmentOtherStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.SustainmentOtherStatusId);
                ViewData["SustainmentStatusId"] = new SelectList(_context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.SustainmentStatusId);
                ViewData["TrainingCollectiveTrainingStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.TrainingCollectiveTrainingStatusId);
                ViewData["TrainingIndividualTrainingStatusId"] = new SelectList(_context.PetspercentStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.TrainingIndividualTrainingStatusId);
                ViewData["TrainingStatusId"] = new SelectList(_context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "Id", PETS.TrainingStatusId);
                ViewData["petsStatus"] = _context.PetsoverallStatuses.Where(d => d.Archived == false).AsNoTracking();
                ViewData["percentStatus"] = _context.PetspercentStatuses.Where(d => d.Archived == false).AsNoTracking();
                ViewData["deployStatus"] = _context.DeployedStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking();
                ViewData["commandOverideStatus"] = _context.CommandOverideStatuses.Where(d => d.Archived == false).AsNoTracking();
                ViewData["AfsPercentageId"] = new SelectList(_context.AfsTrainingPercentages.Where(d => d.Archived == false).OrderBy(d => d.Ordering).AsNoTracking(), "Id", "StatusDisplayValue", dataCard.NatoAFSTrainingPercentage);
                ViewData["NatoFPHYesNoBlankId"] = new SelectList(_context.YesNoBlanks.OrderBy(x => x.Order).AsNoTracking(), "Id", lang == "en" ? "Value" : "ValueFre", dataCard.NatoFPHYesNoBlank);
                ViewData["Nato12SdosId"] = new SelectList(_context.YesNoNaBlanks.OrderBy(x => x.Order).AsNoTracking(), "Id", lang == "en" ? "Value" : "ValueFre", dataCard.Nato12Sdos);
                ViewData["Nato18SdosId"] = new SelectList(_context.YesNoNaBlanks.OrderBy(x => x.Order).AsNoTracking(), "Id", lang == "en" ? "Value" : "ValueFre", dataCard.Nato18Sdos);
                ViewData["NatoNatSupplyPlanId"] = new SelectList(_context.YesNoNaBlanks.OrderBy(x => x.Order).AsNoTracking(), "Id", lang == "en" ? "Value" : "ValueFre", dataCard.NatoNatSupplyPlan);
                ViewData["NatoNatSupportElemId"] = new SelectList(_context.YesNoNaBlanks.OrderBy(x => x.Order).AsNoTracking(), "Id", lang == "en" ? "Value" : "ValueFre", dataCard.NatoNatSupportElem);
                ViewData["NatoEvalCompletedId"] = new SelectList(_context.YesNoNaBlanks.OrderBy(x => x.Order).AsNoTracking(), "Id", lang == "en" ? "Value" : "ValueFre", dataCard.NatoEvalCompleted);
                ViewData["NatoCertProgramCoordId"] = new SelectList(_context.YesNoNaBlanks.OrderBy(x => x.Order).AsNoTracking(), "Id", lang == "en" ? "Value" : "ValueFre", dataCard.NatoCertProgramCoord);
                ViewData["NatoCertCompletedId"] = new SelectList(_context.YesNoNaBlanks.OrderBy(x => x.Order).AsNoTracking(), "Id", lang == "en" ? "Value" : "ValueFre", dataCard.NatoCertCompleted);

                ViewData["CREVALId"] = new SelectList(_context.Crevals.AsNoTracking(), "Id", lang == "en" ? "CrevalName" : "CrevalNameFre", PETS.TrainingCREVAL);
                ViewData["CapabilityId"] = new SelectList(_context.Capabilities.Where(d => d.Archived == false).Where(d => d.Archived == false).AsNoTracking(), "Id", "CapabilityName", dataCard.CapabilityId);
                ViewData["TrainingSpecialtySkillsId"] = new SelectList(_context.SpecialtySkills.AsNoTracking(), "Id", lang == "en" ? "SpecialtySkillName" : "SpecialtySkillNameFre", PETS.TrainingSpecialtySkillsId);
                ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(x => x.Ordered).Where(d => d.Archived == false).AsNoTracking(), "Id", lang == "en" ? "CategoryName" : "CategoryNameFre", dataCard.CategoryId);
                ViewData["DesignationId"] = new SelectList(_context.Designations.OrderBy(x => x.Ordered).Where(d => d.Archived == false).AsNoTracking(), "Id", lang == "en" ? "DesignationName" : "DesignationNameFre", dataCard.DesignationId);
                ViewData["EchelonId"] = new SelectList(_context.Echelons.OrderBy(x => x.Ordered).Where(d => d.Archived == false).AsNoTracking(), "Id", lang == "en" ? "EchelonName" : "EchelonNameFre", dataCard.EchelonId);
                ViewData["NoticeToMoveId"] = new SelectList(_context.NoticeToMoves.OrderBy(x => x.Ordered).Where(d => d.Archived == false).AsNoTracking(), "Id", lang == "en" ? "NoticeToMoveName" : "NoticeToMoveNameFre", dataCard.NoticeToMoveId);
                ViewData["ServiceId"] = new SelectList(_context.Services.OrderBy(x => x.Ordered).Where(d => d.Archived == false).AsNoTracking(), "Id", lang == "en" ? "ServiceName" : "ServiceNameFre", dataCard.ServiceId);
            }
        }

        private IQueryable<DataCard> GetDataCardQueryWIncludes()
        {
            return _context.DataCards.Include(d => d.Capability)
                                     .Include(d => d.Category)
                                     .Include(d => d.CommandOverideStatus)
                                     .Include(d => d.Conplans)
                                     .Include(d => d.DeployedStatus)
                                     .Include(d => d.Designation)
                                     .Include(d => d.Echelon)
                                     /*.Include(d => d.EquipmentCombatVehicleStatus)
                                     .Include(d => d.EquipmentCommunicationsEquipmentStatus)
                                     .Include(d => d.EquipmentSpecialEquipmentStatus)
                                     .Include(d => d.EquipmentStatus)
                                     .Include(d => d.EquipmentSupportVehicleStatus)
                                     .Include(d => d.EquipmentWeaponsServiceRateStatus)*/
                                     .Include(d => d.ForceElement)
                                            .ThenInclude(e => e!.Weighting)
                                     .Include(d => d.ForceElement)
                                            .ThenInclude(e => e!.Organization)
                                     .Include(d => d.LastEditUserNavigation)
                                     .Include(d => d.Nato12SdosNavigation)
                                     .Include(d => d.Nato18SdosNavigation)
                                     .Include(d => d.NatoAfstrainingPercentageNavigation)
                                     .Include(d => d.NatoEvalCompletedNavigation)
                                     .Include(d => d.NatoCertProgramCoordNavigation)
                                     .Include(d => d.NatoCertCompletedNavigation)
                                     .Include(d => d.NatoFphyesNoBlankNavigation)
                                     .Include(d => d.NatoStratLiftCapacityNavigation)
                                     .Include(d => d.NatoNationalDeployNavigation)
                                     .Include(d => d.NatoNatSupplyPlanNavigation)
                                     .Include(d => d.NatoNatSupportElemNavigation)
                                     .Include(d => d.NoticeToMove)
                                     .Include(d => d.Operations)
                                     //.Include(d => d.PersonnelStatus)
                                     .Include(d => d.Service)
                                     .Include(d => d.SrStatus)
                                     /*.Include(d => d.SustainmentAmmunitionStatus)
                                     .Include(d => d.SustainmentCombatRationsStatus)
                                     .Include(d => d.SustainmentPersonalEquipmentStatus)
                                     .Include(d => d.SustainmentPetrolStatus)
                                     .Include(d => d.SustainmentSparesStatus)
                                     .Include(d => d.SustainmentOtherStatus)
                                     .Include(d => d.SustainmentStatus)
                                     .Include(d => d.TrainingCollectiveTrainingStatus)
                                     .Include(d => d.TrainingIndividualTrainingStatus)
                                     .Include(d => d.TrainingStatus)
                                     .Include(d => d.TrainingSpecialtySkills)
                                     .Include(d => d.TrainingCrevalNavigation)*/;
        }

        public async Task<IActionResult> Index(string? selectedOrganization, string? selectedConPlan, string? selectedOperation, string? selectedNato, string? selectedStatus, string? selectedDeployedStatus, string? selectedPageSize, int? pageNumber, string? sortOrder, int selectedOverallStatus)
        {
            lang = cultureHelper.GetCurrentCulture();

            IQueryable<DataCard> dataCards = GetDataCardQueryWIncludes().Where(a => a.ForceElement!.Archived != true);

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(Int32.Parse(selectedPageSize), pageNumber ?? 1, selectedOrganization, selectedConPlan, selectedOperation, selectedNato, selectedStatus, selectedDeployedStatus, sortOrder, selectedOverallStatus, null);
            ViewData["itemsPerPage"] = GetItemsPerPageList(selectedPageSize);

            ViewData["organizationList"] = new SelectList(_context.Organizations.Where(d => d.Archived == false).OrderBy(d => d.Ordered), "Id", lang == "en" ? "OrganizationName" : "OrganizationNameFre");
            ViewData["natoList"] = new SelectList(_context.Designations.Where(d => d.Archived == false).OrderBy(d => d.Ordered), "Id", lang == "en" ? "DesignationName" : "DesignationNameFre");
            ViewData["overallStatusList"] = _context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).ToList();

            ViewData["statusList"] = MakeStatusList(selectedStatus ?? "");
            ViewData["deployedStatusList"] = MakeDeployedStatusList(selectedDeployedStatus ?? "");
            ViewData["deployedStatusListNonSelect"] = _context.DeployedStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).ToList();
            ViewData["givenUser"] = GetCurrentUser();

            // Apply filters if they exist
            if (!string.IsNullOrEmpty(selectedOrganization))
            {
                dataCards = dataCards.Where(f => f.ForceElement!.OrganizationId == Int32.Parse(selectedOrganization));
            }
            if (!string.IsNullOrEmpty(selectedConPlan))
            {
                dataCards = dataCards.Where(a => a.Conplans.Any(sc => sc.Id == Int32.Parse(selectedConPlan)));
            }
            if (!string.IsNullOrEmpty(selectedOperation))
            {
                dataCards = dataCards.Where(o => o.Operations.Any(so => so.Id == Int32.Parse(selectedOperation)));
            }
            if (!string.IsNullOrEmpty(selectedNato))
            {
                dataCards = dataCards.Where(f => f.DesignationId == Int32.Parse(selectedNato));
            }
            if (selectedOverallStatus != 0)
            {
               // dataCards = dataCards.Where(f => (f.CommandOverideStatusId != null && f.CommandOverideStatusId == selectedOverallStatus) || (f.CommandOverideStatusId == null && f.SrStatusId == selectedOverallStatus));
            }
            if (selectedStatus == "False" || selectedStatus == "True")
            {
                dataCards = dataCards.Where(f => f.DataCardComplete == bool.Parse(selectedStatus));
            }
            if (!string.IsNullOrEmpty(selectedDeployedStatus))
            {
                dataCards = dataCards.Where(f => f.DeployedStatusId == Int32.Parse(selectedDeployedStatus));
            }

            DatacardReadinessTableCalculator DRTCalculator = new();
            ViewData["dataCardDisplayModel"] = DRTCalculator.CalculateDisplay(dataCards.ToList());

            switch (sortOrder)
            {
                case "status_asc":
                    dataCards = dataCards.OrderBy(d => d.DataCardComplete);
                    break;

                case "status_desc":
                    dataCards = dataCards.OrderByDescending(d => d.DataCardComplete);
                    break;

                case "felmId_asc":
                    dataCards = dataCards.OrderBy(d => d.ForceElement!.ElementId);
                    break;

                case "felmId_desc":
                    dataCards = dataCards.OrderByDescending(d => d.ForceElement!.ElementId);
                    break;

                case "felmName_asc":
                    dataCards = dataCards.OrderBy(d => d.ForceElement!.ElementName);
                    break;

                case "felmName_desc":
                    dataCards = dataCards.OrderByDescending(d => d.ForceElement!.ElementName);
                    break;

                case "readinessStatus_asc":
                    dataCards = dataCards.OrderBy(d => d.SrStatusId);
                    break;

                case "readinessStatus_desc":
                    dataCards = dataCards.OrderByDescending(d => d.SrStatusId);
                    break;

                case "commandOverrideReadinessStatus_asc":
                    //dataCards = dataCards.OrderBy(d => d.CommandOverideStatusId);
                    break;

                case "commandOverrideReadinessStatus_desc":
                    //dataCards = dataCards.OrderByDescending(d => d.CommandOverideStatusId);
                    break;

                case "deployed_asc":
                    dataCards = dataCards.OrderBy(d => d.DeployedStatusId);
                    break;

                case "deployed_desc":
                    dataCards = dataCards.OrderByDescending(d => d.DeployedStatusId);
                    break;

                case "validityDate_asc":
                    //dataCards = dataCards.OrderBy(d => d.Validitydate);
                    break;

                case "validityDate_desc":
                    //dataCards = dataCards.OrderByDescending(d => d.Validitydate);
                    break;
            }

            return View(await PaginatedList<DataCard>.CreateAsync(dataCards.AsNoTracking(), pageNumber ?? 1, Int32.Parse(selectedPageSize)));
        }

        // Return partial view for datacard modal
        [HttpPost]
        public ActionResult DatacardKpiModal(int datacardId, string type)
        {
            int currentUserId = GetCurrentUser()!.Id;

            ViewData["overallStatusList"] = _context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).ToList();
            ViewData["CurrentUserID"] = currentUserId;
            ViewData["datacardId"] = datacardId;
            DatacardKpi? datacardKpi = _context.DatacardKpis.Where(d => d.DatacardId == datacardId && d.UserId == currentUserId)
                                                            .FirstOrDefault();
            datacardKpi ??= new()
            {
                UserId = currentUserId,
                DatacardId = datacardId
            };
            return PartialView(type + "KpiModal", datacardKpi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + FGPlanner + "," + ReportingUser)]
        public void CreateDatacardKpi(int UserId, int DatacardId, int OverallStatusBelowId, int OverallStatusAboveId, string AlertAnyChanges, string AlertOnSubmit, string AlertWhenIncomplete)
        {
            DatacardKpi datacardKpi = new()
            {
                UserId = UserId,
                DatacardId = DatacardId,
                AlertAnyChanges = bool.Parse(AlertAnyChanges),
                AlertOnSubmit = bool.Parse(AlertOnSubmit),
                AlertWhenIncomplete = bool.Parse(AlertWhenIncomplete),
                OverallStatusBelowId = OverallStatusBelowId == 0 ? null : OverallStatusBelowId,
                OverallStatusAboveId = OverallStatusAboveId == 0 ? null : OverallStatusAboveId
            };
            if (datacardKpi != null)
            {
                _context.Add(datacardKpi);
                _context.SaveChanges();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + FGPlanner + "," + ReportingUser)]
        public void EditDatacardKpi(int DatacardKpiId, int OverallStatusBelowId, int OverallStatusAboveId, string AlertAnyChanges, string AlertOnSubmit, string AlertWhenIncomplete)
        {
            DatacardKpi? datacardKpi = _context.DatacardKpis.Where(d => d.Id == DatacardKpiId).FirstOrDefault();
            if (datacardKpi != null)
            {
                datacardKpi.OverallStatusBelowId = OverallStatusBelowId == 0 ? null : OverallStatusBelowId;
                datacardKpi.OverallStatusAboveId = OverallStatusAboveId == 0 ? null : OverallStatusAboveId;
                datacardKpi.AlertAnyChanges = bool.Parse(AlertAnyChanges);
                datacardKpi.AlertOnSubmit = bool.Parse(AlertOnSubmit);
                datacardKpi.AlertWhenIncomplete = bool.Parse(AlertWhenIncomplete);
                _context.Update(datacardKpi);
                _context.SaveChanges();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + FGPlanner + "," + ReportingUser)]
        public void DeleteDatacardKpi(int datacardKpiId)
        {
            DatacardKpi? datacardKpi = _context.DatacardKpis.Where(d => d.Id == datacardKpiId).FirstOrDefault();
            if (datacardKpi != null)
            {
                _context.Remove(datacardKpi);
                _context.SaveChanges();
            }
        }

        // GET: DataCards/Details/5
        public async Task<IActionResult> Details(int? id, string? selectedOrganization, string? selectedConPlan, string? selectedOperation, string? selectedNato, string? selectedStatus, string? selectedDeployedStatus, string? selectedPageSize, int? pageNumber, string? sortOrder, int selectedOverallStatus)
        {
            if (id == null || _context.DataCards == null)
            {
                return NotFound();
            }

            DataCard? dataCard = await GetDataCardQueryWIncludes().FirstOrDefaultAsync(m => m.Id == id);

            if (dataCard == null || dataCard.ForceElement == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(Int32.Parse(selectedPageSize), pageNumber ?? 1, selectedOrganization, selectedConPlan, selectedOperation, selectedNato, selectedStatus, selectedDeployedStatus, sortOrder, selectedOverallStatus, null);
            ViewData["UserCanEdit"] = CanUserEditDatacard(dataCard.ForceElement.OrganizationId);
            ViewData["NtmOtherID"] = _context.NoticeToMoves.Where(c => c.NoticeToMoveName == "Other").FirstOrDefault()?.Id;

            return View(dataCard);
        }

        // GET: DataCards/Edit/5
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + FGPlanner + "," + ReportingUser)]
        public async Task<IActionResult> Edit(int? id, string? selectedOrganization, string? selectedConPlan, string? selectedOperation, string? selectedNato, string? selectedStatus, string? selectedDeployedStatus, string? selectedPageSize, int? pageNumber, string? sortOrder, int selectedOverallStatus)
        {
            lang = cultureHelper.GetCurrentCulture();

            if (id == null || _context.DataCards == null)
            {
                return NotFound();
            }

            ViewData["ConplanNaID"] = _context.Conplans.Where(c => c.ConplanName == "N/A").FirstOrDefault()?.Id;
            ViewData["OperationNaID"] = _context.Operations.Where(c => c.OperationName == "N/A").FirstOrDefault()?.Id;
            ViewData["YNNANoID"] = _context.YesNoNaBlanks.Where(c => c.Value == "No").FirstOrDefault()?.Id;
            ViewData["NtmOtherID"] = _context.NoticeToMoves.Where(c => c.NoticeToMoveName == "Other").FirstOrDefault()?.Id;
            ViewData["CurrentUserID"] = GetCurrentUser()!.Id;

            DataCard? dataCard = await GetDataCardQueryWIncludes().FirstOrDefaultAsync(m => m.Id == id);

            if (dataCard == null)
            {
                return NotFound();
            }

            if (dataCard.ForceElement != null && !CanUserEditDatacard(dataCard.ForceElement.OrganizationId))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(Int32.Parse(selectedPageSize), pageNumber ?? 1, selectedOrganization, selectedConPlan, selectedOperation, selectedNato, selectedStatus, selectedDeployedStatus, sortOrder, selectedOverallStatus, dataCard);

            return View(dataCard);
        }

        // POST: DataCards/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = Admin + "," + SuperUser + "," + FGPlanner + "," + ReportingUser)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataCardComplete,ForceElementId,Division,Brigade,Unit,Subunit,ForceElementDesc,SrStatusId," +
            "CommandOverideStatusId,CommandOverrideAuthority,CommandOverrideComments,ReadinessFromDate,ReadinessToDate,Validitydate,DeployedStatusId,DesignationId,NatoAssetsDeclared,ServiceId,EchelonId," +
            "CapabilityId,CategoryId,NoticeToMoveId,Ntmdetails,NatoNoticeToEffect,NatoActive,FwdNato,NFMActive,Rds,NatoLocation,NatoCoordinates,NatoMajorEquipmentComments,NatoCavets,NatoGeneralComments,NatoRequirementName,NatoNationalName," +
            "NatoStratLiftCapacityId,NatoStratLiftCapacity,NatoStratLiftCapacityComments,NatoNationalDeployId,NatoNationalDeploy,NatoNationalDeployComments,NatoFphyesNoBlank,NatoAfstrainingPercentage," +
            "NatoEvalCompleted,NatoPlannedEvalDate,NatoCertProgramCoord,NatoCertCompleted,NatoNationalTrainingRemarks,NatoNationalAssesmentComments," +
            "Nato12Sdos,Nato18Sdos,NatoCurrentSdos,NatoNatSupplyPlan,NatoNatSupportElem,PersonnelStatusId,PersonnelDesignatedStrength,PersonnelActualStrength,PersonnelLob,PersonnelMedical,PersonnelDental,PersonnelAps,PersonnelLightDuties," +
            "PersonnelTradeInsufficienciesReadinessFactor,PersonnelFitness,PersonnelIt,PersonnelComments,EquipmentStatusId,EquipmentCombatVehicleStatusId,EquipmentSupportVehicleStatusId,EquipmentWeaponsServiceRateStatusId," +
            "EquipmentCommunicationsEquipmentStatusId,EquipmentSpecialEquipmentStatusId,EquipmentVehicalsReadinessFactor,EquipmentWeaponsReadinessFactor," +
            "EquipmentCommunicationsReadinessFactor,EquipmentSensorsReadinessFactor,EquipmentSpecialEquimentReadinessFactor,EquipmentTechniciansReadinessFactor,EquipmentLifeEntensionDelays,EquipmentProtectionSuites,EquipmentOther,EquipmentComments,TrainingStatusId," +
            "TrainingProjectedCrevaldate,TrainingCreval,TrainingSpecialtySkillsId,TrainingCrevaldate,TrainingAmmoReadinessFactor,TrainingFuelReadinessFactor,TrainingRationsReadinessFactor," +
            "TrainingWeaponsReadinessFactor,TrainingRadiosReadinessFactor,TrainingEquipmentReadinessFactor,TrainingSparePartssReadinessFactor,TrainingItreadinessFactor," +
            "TrainingRangeAvailabilityReadinessFactor,TrainingCtreadinessFactor,TrainingSpecialQuals,TrainingTrgFleets,TrainingComments,TrainingCollectiveTrainingStatusId,TrainingIndividualTrainingStatusId,SustainmentStatusId," +
            "SustainmentCombatRationsStatusId,SustainmentPersonalEquipmentStatusId,SustainmentPetrolStatusId,SustainmentAmmunitionStatusId,SustainmentSparesStatusId, SustainmentOtherStatusId," +
            "SustainmentRationsReadinessFactor,SustainmentUniformsReadinessFactor,SustainmentPpereadinessFactor,SustainmentFuelReadinessFactor,SustainmentAmmunitionReadinessFactor," +
            "SustainmentSparePartsReadinessFactor,SustainmentOtherReadinessFactor,SustainmentComments,LastEditUser,LastEditDate,Concurrency,ConcurrencyCommnets")] DataCard dataCard, IFormCollection formCollection,
             string? selectedOrganization, string? selectedConPlan, string? selectedOperation, string? selectedNato, string? selectedStatus, string? selectedDeployedStatus, string? selectedPageSize, int? pageNumber, string? sortOrder, int selectedOverallStatus)
        {
            if (id != dataCard.Id)
            {
                return NotFound();
            }

            lang = cultureHelper.GetCurrentCulture();

            if (ModelState.IsValid)
            {
                try
                {
                    if (User.Identity == null || User.Identity.Name == null) return Unauthorized();
                    string username = RemoveDomainFromUsername(User.Identity.Name);
                    User? user = null;
                    if (!String.IsNullOrEmpty(username))
                    {
                        user = await _context.Users.Include(i => i.Roles).Include(i => i.Organization).FirstOrDefaultAsync(m => m.UserName == username);
                    }
                    if (dataCard.ForceElement != null && !CanUserEditDatacard(dataCard.ForceElement.OrganizationId))
                    {
                        return Unauthorized();
                    }

                    //Add an audit entry
                    ChangeLog changeLog = new()
                    {
                        ForceElementId = dataCard.ForceElementId,
                        SrStatusId = dataCard.SrStatusId,
                       // CommandOverideStatusId = dataCard.CommandOverideStatusId,
                        CommandOverrideComments = dataCard.CommandOverrideComments,
                        NatoGeneralComments = dataCard.NatoGeneralComments,
                        NatoMajorEquipmentComments = dataCard.NatoMajorEquipmentComments,
                        NatoCavets = dataCard.NatoCavets,
                        NatoStratLiftCapacityComments = dataCard.NatoStratLiftCapacityComments,
                        NatoNationalDeployComments = dataCard.NatoNationalDeployComments,
                        NatoNationalAssesmentComments = dataCard.NatoNationalAssesmentComments,
                       /* PersonnelStatusId = dataCard.PersonnelStatusId,
                        PersonnelComments = dataCard.PersonnelComments,
                        TrainingStatusId = dataCard.TrainingStatusId,
                        TrainingComments = dataCard.TrainingComments,
                        EquipmentStatusId = dataCard.EquipmentStatusId,
                        EquipmentComments = dataCard.EquipmentComments,
                        SustainmentStatusId = dataCard.SustainmentStatusId,
                        SustainmentComments = dataCard.SustainmentComments,*/
                        DeployedStatusId = dataCard.DeployedStatusId,
                        ChangedDate = DateTime.Now
                    };

                    if (user != null)
                    {
                        changeLog.LastEditUser = user.Id;
                    }
                    else
                    {
                        changeLog.LastEditUser = null;
                    }

                    _context.ChangeLogs.Add(changeLog);
                    _context.SaveChanges();

                    //dataCard.Validitydate = DateTime.Now;
                    dataCard.LastEditDate = DateTime.Now;

                    if (user != null)
                    {
                        dataCard.LastEditUser = user.Id;
                    }
                    else
                    {
                        dataCard.LastEditUser = null;
                    }

                    //If NATO & NFM Active is off remove all the data
                    if (dataCard.NatoActive == false && dataCard.NFMActive == false)
                    {
                        dataCard.CategoryId = null;
                        dataCard.CapabilityId = null;
                        dataCard.DesignationId = null;
                        dataCard.EchelonId = null;
                        dataCard.ServiceId = null;
                        dataCard.NatoCavets = null;
                        //dataCard.NatoFphyesNoBlank = null;
                        dataCard.Nato12Sdos = null;
                        dataCard.Nato18Sdos = null;
                        dataCard.NatoCurrentSdos = null;
                        dataCard.NatoNatSupplyPlan = null;
                        dataCard.NatoRequirementName = null;
                        dataCard.NatoNationalName = null;
                        dataCard.NatoNatSupportElem = null;
                        //dataCard.NatoAfstrainingPercentage = null;

                        dataCard.NatoEvalCompleted = null;
                        dataCard.NatoPlannedEvalDate = null;
                        dataCard.NatoCertProgramCoord = null;
                        dataCard.NatoCertCompleted = null;
                        dataCard.NatoNationalTrainingRemarks = null;

                        dataCard.NatoGeneralComments = null;
                        dataCard.NatoMajorEquipmentComments = null;
                        dataCard.NatoNationalAssesmentComments = null;
                        dataCard.NatoNationalDeploy = false;
                        dataCard.NatoNationalDeployComments = null;
                        dataCard.NatoStratLiftCapacity = false;
                        dataCard.NatoStratLiftCapacityComments = null;
                        dataCard.NatoAssetsDeclared = null;
                        dataCard.NatoNationalDeployId = null;
                        dataCard.NatoStratLiftCapacityId = null;
                    }

                   /* if (dataCard.CommandOverideStatusId == null)
                    {
                        dataCard.CommandOverrideComments = null;
                        dataCard.CommandOverrideAuthority = null;
                    }
                   */
                    int ops = formCollection["Operation"].Count;
                    int cons = formCollection["Conplan"].Count;

                    if (ops >= 1 & cons >= 1)
                    {
                        dataCard.Concurrency = true;
                    }
                    else
                    {
                        dataCard.Concurrency = false;
                        dataCard.ConcurrencyCommnets = null;
                    }

                    _context.Update(dataCard);
                    await _context.SaveChangesAsync();

                    // Get the datcard to update
                    DataCard? updateDataCard = await GetDataCardQueryWIncludes().FirstOrDefaultAsync(m => m.Id == id);

                    if (updateDataCard != null)
                    {
                        // Remove all the conplans for the data casd
                        foreach (Conplan? item in updateDataCard.Conplans)
                        {
                            updateDataCard.Conplans.Remove(item);
                            _context.SaveChanges();
                        }
                        // Get the selected conplans from the form
                        Array? plans = formCollection["Conplan"];

                        if (plans != null)
                        {
                            // Loop through the list and add each add the conplan
                            foreach (var item in plans)
                            {
                                string? StringItem = item.ToString();
                                if (StringItem != null)
                                {
                                    Conplan conplan = _context.Conplans.Where(c => c.Id == int.Parse(StringItem)).First();
                                    updateDataCard.Conplans.Add(conplan);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        // Remove all the operations for the data card
                        foreach (Operation? item in updateDataCard.Operations)
                        {
                            updateDataCard.Operations.Remove(item);
                            _context.SaveChanges();
                        }

                        Array? operations = formCollection["Operation"];

                        if (operations != null)
                        {
                            foreach (var item in operations)
                            {
                                string? StringItem = item.ToString();
                                if (StringItem != null)
                                {
                                    Operation op = _context.Operations.Where(o => o.Id == int.Parse(StringItem)).First();
                                    updateDataCard.Operations.Add(op);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        // Send KPI Alerts
                        List<DatacardKpi> datacardKpis = _context.DatacardKpis.Where(d => d.DatacardId == id)
                            .Include(d => d.User)
                            .Include(d => d.OverallStatusAbove)
                            .Include(d => d.OverallStatusBelow)
                            .ToList();

                        string defaultEmailBody = "This is an alert that force element: " + dataCard.ForceElement!.ElementName
                                        + " has been modified. The overall status is now ";
                        defaultEmailBody += dataCard.SrStatusId == 1 ? "Green" : dataCard.SrStatus!.StatusDisplayColour;
                        foreach (DatacardKpi datacardKpi in datacardKpis)
                        {
                            if (datacardKpi.AlertAnyChanges)
                            {
                                kpiEmail.SendEmail(datacardKpi.User.Email, defaultEmailBody);
                            }
                            else if (datacardKpi.AlertOnSubmit && dataCard.DataCardComplete != null && (bool)dataCard.DataCardComplete)
                            {
                                string emailBody = "This is an alert that force element: " + dataCard.ForceElement!.ElementName
                                        + " has been submited. The overall status is now ";
                                emailBody += dataCard.SrStatusId == 1 ? "Green" : dataCard.SrStatus!.StatusDisplayColour;
                                kpiEmail.SendEmail(datacardKpi.User.Email, emailBody);
                            }
                            else if (datacardKpi.AlertWhenIncomplete && dataCard.DataCardComplete != null && (bool)dataCard.DataCardComplete == false)
                            {
                                string emailBody = "This is an alert that force element: " + dataCard.ForceElement!.ElementName
                                        + " has been modified. The datacard is now incomplete.";
                                kpiEmail.SendEmail(datacardKpi.User.Email, emailBody);
                            }
                            else if (datacardKpi.OverallStatusBelowId != null && datacardKpi.OverallStatusBelow!.StatusValue < dataCard.SrStatus!.StatusValue)
                            {
                                kpiEmail.SendEmail(datacardKpi.User.Email, defaultEmailBody);
                            }
                            else if (datacardKpi.OverallStatusAboveId != null && datacardKpi.OverallStatusAbove!.StatusValue > dataCard.SrStatus!.StatusValue)
                            {
                                kpiEmail.SendEmail(datacardKpi.User.Email, defaultEmailBody);
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DataCardExists(dataCard.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index", new { selectedOrganization, selectedConPlan, selectedOperation, selectedNato, selectedStatus, selectedPageSize, pageNumber, sortOrder });
            }

            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(Int32.Parse(selectedPageSize), pageNumber ?? 1, selectedOrganization, selectedConPlan, selectedOperation, selectedNato, selectedStatus, selectedDeployedStatus, sortOrder, selectedOverallStatus, dataCard);

            ViewData["ConplanNaID"] = _context.Conplans.Where(c => c.ConplanName == "N/A").FirstOrDefault()?.Id;
            ViewData["OperationNaID"] = _context.Operations.Where(c => c.OperationName == "N/A").FirstOrDefault()?.Id;
            ViewData["YNNANoID"] = _context.YesNoNaBlanks.Where(c => c.Value == "No").FirstOrDefault()?.Id;
            ViewData["CurrentUserID"] = GetCurrentUser()!.Id;

            return View(dataCard);
        }

        private bool DataCardExists(int id)
        {
            return (_context.DataCards?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        public JsonResult CalculateOverallStatus(IFormCollection formcollection)
        {
            JsonResponseViewModel model = new();
            PETSStatus petsStatus = new();
            StatusHelper statusHelper = new(_context);

            var personnelStatus = formcollection["PersonnelStatus"];
            var equipmentStatus = formcollection["EquipmentStatus"];
            var trainingStatus = formcollection["TrainingStatus"];
            var sustainmentStatus = formcollection["SustainmentStatus"];

            int count = 4;

            int g = 0;
            int y = 0;
            int o = 0;
            int r = 0;

            switch (personnelStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (equipmentStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (trainingStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (sustainmentStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }

            string overallStatus = g.ToString() + y.ToString() + o.ToString() + r.ToString();

            CardStatus cardStatusOverall = statusHelper.CalculateStatus(overallStatus, count);

            petsStatus.srStatusColour = cardStatusOverall.statusColour;
            petsStatus.srStatusID = cardStatusOverall.statusID;

            model.ResponseCode = 0;
            model.ResponseMessage = JsonConvert.SerializeObject(petsStatus);

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetSustainmentStatus(IFormCollection formcollection)
        {
            JsonResponseViewModel model = new();
            PETSStatus petsStatus = new();
            StatusHelper statusHelper = new(_context);

            var combatRationsStatus = formcollection["CombatRationsStatus"];
            var personalEquipmentStatus = formcollection["PersonalEquipmentStatus"];
            var petrolStatus = formcollection["PetrolStatus"];
            var ammunitionStatus = formcollection["AmmunitionStatus"];
            var sparesStatus = formcollection["SparesStatus"];
            var otherStatus = formcollection["OtherStatus"];
            var personnelStatus = formcollection["PersonnelStatus"];
            var equipmentStatus = formcollection["EquipmentStatus"];
            var trainingStatus = formcollection["TrainingStatus"];

            // Variables to hold colour counts for sustainment values
            int sus_g = 0;
            int sus_y = 0;
            int sus_o = 0;
            int sus_r = 0;
            // Holds the count of selections
            int sus_count = 0;

            // Get the count of colour / selections
            switch (combatRationsStatus)
            {
                case "1":
                    sus_g++;
                    sus_count++;
                    break;

                case "2":
                    sus_y++;
                    sus_count++;
                    break;

                case "3":
                    sus_o++;
                    sus_count++;
                    break;

                case "4":
                    sus_r++;
                    sus_count++;
                    break;
            }
            switch (personalEquipmentStatus)
            {
                case "1":
                    sus_g++;
                    sus_count++;
                    break;

                case "2":
                    sus_y++;
                    sus_count++;
                    break;

                case "3":
                    sus_o++;
                    sus_count++;
                    break;

                case "4":
                    sus_r++;
                    sus_count++;
                    break;
            }
            switch (petrolStatus)
            {
                case "1":
                    sus_g++;
                    sus_count++;
                    break;

                case "2":
                    sus_y++;
                    sus_count++;
                    break;

                case "3":
                    sus_o++;
                    sus_count++;
                    break;

                case "4":
                    sus_r++;
                    sus_count++;
                    break;
            }
            switch (ammunitionStatus)
            {
                case "1":
                    sus_g++;
                    sus_count++;
                    break;

                case "2":
                    sus_y++;
                    sus_count++;
                    break;

                case "3":
                    sus_o++;
                    sus_count++;
                    break;

                case "4":
                    sus_r++;
                    sus_count++;
                    break;
            }
            switch (sparesStatus)
            {
                case "1":
                    sus_g++;
                    sus_count++;
                    break;

                case "2":
                    sus_y++;
                    sus_count++;
                    break;

                case "3":
                    sus_o++;
                    sus_count++;
                    break;

                case "4":
                    sus_r++;
                    sus_count++;
                    break;
            }
            switch (otherStatus)
            {
                case "1":
                    sus_g++;
                    sus_count++;
                    break;

                case "2":
                    sus_y++;
                    sus_count++;
                    break;

                case "3":
                    sus_o++;
                    sus_count++;
                    break;

                case "4":
                    sus_r++;
                    sus_count++;
                    break;
            }

            // Make call to calculate the over Sustainment status
            string sectionStatuses = sus_g.ToString() + sus_y.ToString() + sus_o.ToString() + sus_r.ToString();

            CardStatus cardStatus = statusHelper.CalculateStatus(sectionStatuses, sus_count);

            int count = 4;

            int g = 0;
            int y = 0;
            int o = 0;
            int r = 0;

            switch (personnelStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (equipmentStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (trainingStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (cardStatus.statusID.ToString())
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }

            string overallStatus = g.ToString() + y.ToString() + o.ToString() + r.ToString();

            CardStatus cardStatusOverall = statusHelper.CalculateStatus(overallStatus, count);

            petsStatus.petsStatusColour = cardStatus.statusColour;
            petsStatus.petsStatusID = cardStatus.statusID;
            petsStatus.srStatusColour = cardStatusOverall.statusColour;
            petsStatus.srStatusID = cardStatusOverall.statusID;

            model.ResponseCode = 0;
            model.ResponseMessage = JsonConvert.SerializeObject(petsStatus);

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetEquipmentStatus(IFormCollection formcollection)
        {
            JsonResponseViewModel model = new();
            PETSStatus petsStatus = new();
            StatusHelper statusHelper = new(_context);

            var combatVehicleStatus = formcollection["CombatVehicleStatus"];
            var supportVehicleStatus = formcollection["SupportVehicleStatus"];
            var weaponsServiceRateStatus = formcollection["WeaponsServiceRateStatus"];
            var communicationsEquipmentStatus = formcollection["CommunicationsEquipmentStatus"];
            var specialEquipmentStatus = formcollection["SpecialEquipmentStatus"];

            var personnelStatus = formcollection["PersonnelStatus"];
            var trainingStatus = formcollection["TrainingStatus"];
            var sustainimentStatus = formcollection["SustainmentStatus"];

            // Variables to hold colour counts for sustainment values
            int equ_g = 0;
            int equ_y = 0;
            int equ_o = 0;
            int equ_r = 0;
            // Holds the count of selections
            int equ_count = 0;

            switch (combatVehicleStatus)
            {
                case "1":
                    equ_g++;
                    equ_count++;
                    break;

                case "2":
                    equ_y++;
                    equ_count++;
                    break;

                case "3":
                    equ_o++;
                    equ_count++;
                    break;

                case "4":
                    equ_r++;
                    equ_count++;
                    break;
            }
            switch (supportVehicleStatus)
            {
                case "1":
                    equ_g++;
                    equ_count++;
                    break;

                case "2":
                    equ_y++;
                    equ_count++;
                    break;

                case "3":
                    equ_o++;
                    equ_count++;
                    break;

                case "4":
                    equ_r++;
                    equ_count++;
                    break;
            }
            switch (weaponsServiceRateStatus)
            {
                case "1":
                    equ_g++;
                    equ_count++;
                    break;

                case "2":
                    equ_y++;
                    equ_count++;
                    break;

                case "3":
                    equ_o++;
                    equ_count++;
                    break;

                case "4":
                    equ_r++;
                    equ_count++;
                    break;
            }
            switch (communicationsEquipmentStatus)
            {
                case "1":
                    equ_g++;
                    equ_count++;
                    break;

                case "2":
                    equ_y++;
                    equ_count++;
                    break;

                case "3":
                    equ_o++;
                    equ_count++;
                    break;

                case "4":
                    equ_r++;
                    equ_count++;
                    break;
            }
            switch (specialEquipmentStatus)
            {
                case "1":
                    equ_g++;
                    equ_count++;
                    break;

                case "2":
                    equ_y++;
                    equ_count++;
                    break;

                case "3":
                    equ_o++;
                    equ_count++;
                    break;

                case "4":
                    equ_r++;
                    equ_count++;
                    break;
            }

            // Make call to calculate the over Sustainment status
            string sectionStatuses = equ_g.ToString() + equ_y.ToString() + equ_o.ToString() + equ_r.ToString();

            CardStatus cardStatus = statusHelper.CalculateStatus(sectionStatuses, equ_count);
            int count = 4;

            int g = 0;
            int y = 0;
            int o = 0;
            int r = 0;

            switch (personnelStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            // Get the return from the section status call to get the new status for equipment
            switch (cardStatus.statusID.ToString())
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (trainingStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (sustainimentStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }

            string overallStatus = g.ToString() + y.ToString() + o.ToString() + r.ToString();

            CardStatus cardStatusOverall = statusHelper.CalculateStatus(overallStatus, count);

            petsStatus.petsStatusColour = cardStatus.statusColour;
            petsStatus.petsStatusID = cardStatus.statusID;
            petsStatus.srStatusColour = cardStatusOverall.statusColour;
            petsStatus.srStatusID = cardStatusOverall.statusID;

            model.ResponseCode = 0;
            model.ResponseMessage = JsonConvert.SerializeObject(petsStatus);

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetTrainingStatus(IFormCollection formcollection)
        {
            JsonResponseViewModel model = new();
            PETSStatus petsStatus = new();
            StatusHelper statusHelper = new(_context);

            var collectiveTrainingStatus = formcollection["CollectiveTrainingStatus"];
            var individualTrainingStatus = formcollection["IndividualTrainingStatus"];

            var personnelStatus = formcollection["PersonnelStatus"];
            var equipmentStatus = formcollection["EquipmentStatus"];
            var sustainimentStatus = formcollection["SustainimentStatus"];

            // Variables to hold colour counts for sustainment values
            int trn_g = 0;
            int trn_y = 0;
            int trn_o = 0;
            int trn_r = 0;
            // Holds the count of selections
            int trn_count = 0;

            // Get the count of colour / selections
            switch (collectiveTrainingStatus)
            {
                case "1":
                    trn_g++;
                    trn_count++;
                    break;

                case "2":
                    trn_y++;
                    trn_count++;
                    break;

                case "3":
                    trn_o++;
                    trn_count++;
                    break;

                case "4":
                    trn_r++;
                    trn_count++;
                    break;
            }
            switch (individualTrainingStatus)
            {
                case "1":
                    trn_g++;
                    trn_count++;
                    break;

                case "2":
                    trn_y++;
                    trn_count++;
                    break;

                case "3":
                    trn_o++;
                    trn_count++;
                    break;

                case "4":
                    trn_r++;
                    trn_count++;
                    break;
            }

            // Make call to calculate the over Sustainment status
            string sectionStatuses = trn_g.ToString() + trn_y.ToString() + trn_o.ToString() + trn_r.ToString();

            CardStatus cardStatus = statusHelper.CalculateStatus(sectionStatuses, trn_count);
            int count = 4;

            int g = 0;
            int y = 0;
            int o = 0;
            int r = 0;

            switch (personnelStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (equipmentStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (cardStatus.statusID.ToString())
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (sustainimentStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }

            string overallStatus = g.ToString() + y.ToString() + o.ToString() + r.ToString();

            CardStatus cardStatusOverall = statusHelper.CalculateStatus(overallStatus, count);

            petsStatus.petsStatusColour = cardStatus.statusColour;
            petsStatus.petsStatusID = cardStatus.statusID;
            petsStatus.srStatusColour = cardStatusOverall.statusColour;
            petsStatus.srStatusID = cardStatusOverall.statusID;

            model.ResponseCode = 0;
            model.ResponseMessage = JsonConvert.SerializeObject(petsStatus);

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetpersonalStatus(IFormCollection formcollection)
        {
            JsonResponseViewModel model = new();
            PETSStatus petsStatus = new();
            StatusHelper statusHelper = new(_context);

            var personnelStatusStatus = formcollection["PersonnelStatusStatus"];

            var equipmentStatus = formcollection["EquipmentStatus"];
            var trainingStatus = formcollection["TrainingStatus"];
            var sustainmentStatus = formcollection["SustainmentStatus"];

            // Variables to hold colour counts for sustainment values
            int per_g = 0;
            int per_y = 0;
            int per_o = 0;
            int per_r = 0;
            // Holds the count of selections
            int per_count = 0;

            // Get the count of colour / selections
            switch (personnelStatusStatus)
            {
                case "1":
                    per_g++;
                    per_count++;
                    break;

                case "2":
                    per_y++;
                    per_count++;
                    break;

                case "3":
                    per_o++;
                    per_count++;
                    break;

                case "4":
                    per_r++;
                    per_count++;
                    break;
            }

            //Make call to calculate the over Sustainment status
            string sectionStatuses = per_g.ToString() + per_y.ToString() + per_o.ToString() + per_r.ToString();

            CardStatus cardStatus = statusHelper.CalculateStatus(sectionStatuses, per_count);

            int count = 4;

            int g = 0;
            int y = 0;
            int o = 0;
            int r = 0;

            switch (cardStatus.statusID.ToString())
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (equipmentStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (trainingStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }
            switch (sustainmentStatus)
            {
                case "1":
                    g++;
                    break;

                case "2":
                    y++;
                    break;

                case "3":
                    o++;
                    break;

                case "4":
                    r++;
                    break;

                case "5":
                    count--;
                    break;
            }

            string overallStatus = g.ToString() + y.ToString() + o.ToString() + r.ToString();

            CardStatus cardStatusOverall = statusHelper.CalculateStatus(overallStatus, count);

            petsStatus.petsStatusColour = cardStatus.statusColour;
            petsStatus.petsStatusID = cardStatus.statusID;
            petsStatus.srStatusColour = cardStatusOverall.statusColour;
            petsStatus.srStatusID = cardStatusOverall.statusID;

            model.ResponseCode = 0;
            model.ResponseMessage = JsonConvert.SerializeObject(petsStatus);

            return Json(model);
        }
    }
}
