using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using SMARTV3.Helpers;
using SMARTV3.Models;

using static SMARTV3.Helpers.PaginationHelper;

namespace SMARTV3.Controllers
{
    public class ChangeLogsController : Controller
    {
        private readonly SMARTV3DbContext _context;

        public ChangeLogsController(SMARTV3DbContext context)
        {
            _context = context;
        }

        // GET: ChangeLogs
        public async Task<IActionResult> Index(string elementIdFilter, string elementNameFilter, string startDateFilter, string endDateFilter, string? selectedPageSize, int? pageNumber, string sortOrder, string? changeUser)
        {
            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(Int32.Parse(selectedPageSize), elementIdFilter, elementNameFilter, startDateFilter, endDateFilter, pageNumber ?? 1, sortOrder, changeUser ?? "");
            ViewData["itemsPerPage"] = GetItemsPerPageList(selectedPageSize);
            ViewData["overallStatusList"] = _context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).ToList();
            ViewData["deployedStatusListNonSelect"] = _context.DeployedStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).ToList();

            IQueryable<ChangeLog>? sMARTV3DbContext = from d in _context.ChangeLogs
                .Include(c => c.CommandOverideStatus)
                .Include(c => c.DeployedStatus)
                .Include(c => c.EquipmentStatus)
                .Include(c => c.ForceElement)
                .Include(c => c.PersonnelStatus)
                .Include(c => c.SrStatus)
                .Include(c => c.SustainmentStatus)
                .Include(c => c.TrainingStatus)
                .Include(c => c.LastEditUserNavigation)
                                                      select d;

            if (elementIdFilter != "" & elementIdFilter != null)
            {
                sMARTV3DbContext = sMARTV3DbContext.Where(d => EF.Functions.Like(d.ForceElement!.ElementId, "%" + elementIdFilter + "%"));
            }

            if (elementNameFilter != "" & elementNameFilter != null)
            {
                sMARTV3DbContext = sMARTV3DbContext.Where(d => EF.Functions.Like(d.ForceElement!.ElementName!, "%" + elementNameFilter + "%"));
            }

            if (!String.IsNullOrEmpty(startDateFilter) && !String.IsNullOrEmpty(endDateFilter))
            {
                DateTime startDate = DateTime.Parse(startDateFilter);
                DateTime endDate = DateTime.Parse(endDateFilter);

                sMARTV3DbContext = sMARTV3DbContext.Where(d => d.ChangedDate >= startDate);
                sMARTV3DbContext = sMARTV3DbContext.Where(d => d.ChangedDate < endDate.AddDays(1));
            }

            if (!String.IsNullOrEmpty(changeUser))
            {
                sMARTV3DbContext = sMARTV3DbContext.Where(d => d.LastEditUserNavigation!.UserName == changeUser);
            }

            switch (sortOrder)
            {
                case "date_asc":
                    sMARTV3DbContext = sMARTV3DbContext.OrderBy(a => a.ChangedDate);
                    break;

                case "date_desc":
                    sMARTV3DbContext = sMARTV3DbContext.OrderByDescending(a => a.ChangedDate);
                    break;

                case "changeUser_asc":
                    sMARTV3DbContext = sMARTV3DbContext.OrderBy(a => a.LastEditUserNavigation!.UserName);
                    break;

                case "changeUser_desc":
                    sMARTV3DbContext = sMARTV3DbContext.OrderByDescending(a => a.LastEditUserNavigation!.UserName);
                    break;
            }

            return View(await PaginatedList<ChangeLog>.CreateAsync(sMARTV3DbContext.AsNoTracking(), pageNumber ?? 1, Int32.Parse(selectedPageSize)));
        }

        private void SetViewData(int selectedPageSize, string elementIdFilter, string elementNameFilter, string startDateFilter, string endDateFilter, int pageNumber, string sortOrder, string changeUser)
        {
            ViewData["selectedPageSize"] = selectedPageSize;
            ViewData["ElementIdFilter"] = elementIdFilter;
            ViewData["ElementNameFilter"] = elementNameFilter;
            ViewData["StartDateFilter"] = startDateFilter;
            ViewData["EndDateFilter"] = endDateFilter;
            ViewData["pageNumber"] = pageNumber;
            ViewData["sortOrder"] = sortOrder;
            ViewData["changeUser"] = changeUser;
        }

        // GET: ChangeLogs/Details/5
        public async Task<IActionResult> Details(int? id, string elementIdFilter, string elementNameFilter, string startDateFilter, string endDateFilter, string? selectedPageSize, int? pageNumber, string sortOrder, string? changeUser)
        {
            if (id == null || _context.ChangeLogs == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(Int32.Parse(selectedPageSize), elementIdFilter, elementNameFilter, startDateFilter, endDateFilter, pageNumber ?? 1, sortOrder, changeUser ?? "");
            ViewData["overallStatusList"] = _context.PetsoverallStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).ToList();
            ViewData["deployedStatusListNonSelect"] = _context.DeployedStatuses.Where(d => d.Archived == false).OrderBy(d => d.Ordering).ToList();

            ChangeLog? changeLog = await _context.ChangeLogs
                .Include(c => c.CommandOverideStatus)
                .Include(c => c.DeployedStatus)
                .Include(c => c.EquipmentStatus)
                .Include(c => c.ForceElement)
                .Include(c => c.PersonnelStatus)
                .Include(c => c.SrStatus)
                .Include(c => c.SustainmentStatus)
                .Include(c => c.TrainingStatus)
                .Include(c => c.LastEditUserNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeLog == null)
            {
                return NotFound();
            }

            return View(changeLog);
        }

        // GET: ChangeLogs/Create
        public IActionResult Create(string elementIdFilter, string elementNameFilter, string startDateFilter, string endDateFilter, string? selectedPageSize, int? pageNumber, string sortOrder, string? changeUser)
        {
            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(Int32.Parse(selectedPageSize), elementIdFilter, elementNameFilter, startDateFilter, endDateFilter, pageNumber ?? 1, sortOrder, changeUser ?? "");

            ViewData["CommandOverideStatusId"] = new SelectList(_context.CommandOverideStatuses, "Id", "Id");
            ViewData["DeployedStatusId"] = new SelectList(_context.DeployedStatuses, "Id", "Id");
            ViewData["EquipmentStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id");
            ViewData["ForceElementId"] = new SelectList(_context.ForceElements, "Id", "Id");
            ViewData["PersonnelStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id");
            ViewData["SrStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id");
            ViewData["SustainmentStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id");
            ViewData["TrainingStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id");
            return View();
        }

        // POST: ChangeLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ForceElementId,SrStatusId,CommandOverideStatusId,CommandOverrideComments,DeployedStatusId,NatoGeneralComments,NatoMajorEquipmentComments,NatoCavets," +
            "NatoStratLiftCapacityComments,NatoNationalDeployComments,NatoNationalAssesmentComments,PersonnelStatusId,PersonnelComments,TrainingStatusId,TrainingComments,EquipmentStatusId,EquipmentComments,SustainmentStatusId,SustainmentComments,ChangedDate,ChangedBy")]
            ChangeLog changeLog, string elementIdFilter, string elementNameFilter, string startDateFilter, string endDateFilter, string? selectedPageSize, int? pageNumber, string sortOrder, string? changeUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(changeLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(Int32.Parse(selectedPageSize), elementIdFilter, elementNameFilter, startDateFilter, endDateFilter, pageNumber ?? 1, sortOrder, changeUser ?? "");

            ViewData["CommandOverideStatusId"] = new SelectList(_context.CommandOverideStatuses, "Id", "Id", changeLog.CommandOverideStatusId);
            ViewData["DeployedStatusId"] = new SelectList(_context.DeployedStatuses, "Id", "Id", changeLog.DeployedStatusId);
            ViewData["EquipmentStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.EquipmentStatusId);
            ViewData["ForceElementId"] = new SelectList(_context.ForceElements, "Id", "Id", changeLog.ForceElementId);
            ViewData["PersonnelStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.PersonnelStatusId);
            ViewData["SrStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.SrStatusId);
            ViewData["SustainmentStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.SustainmentStatusId);
            ViewData["TrainingStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.TrainingStatusId);
            return View(changeLog);
        }

        // GET: ChangeLogs/Edit/5
        public async Task<IActionResult> Edit(int? id, string elementIdFilter, string elementNameFilter, string startDateFilter, string endDateFilter, string? selectedPageSize, int? pageNumber, string? sortOrder, string? changeUser)
        {
            if (id == null || _context.ChangeLogs == null)
            {
                return NotFound();
            }

            ChangeLog? changeLog = await _context.ChangeLogs.FindAsync(id);
            if (changeLog == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(Int32.Parse(selectedPageSize), elementIdFilter, elementNameFilter, startDateFilter, endDateFilter, pageNumber ?? 1, sortOrder ?? "", changeUser ?? "");

            ViewData["CommandOverideStatusId"] = new SelectList(_context.CommandOverideStatuses, "Id", "Id", changeLog.CommandOverideStatusId);
            ViewData["DeployedStatusId"] = new SelectList(_context.DeployedStatuses, "Id", "Id", changeLog.DeployedStatusId);
            ViewData["EquipmentStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.EquipmentStatusId);
            ViewData["ForceElementId"] = new SelectList(_context.ForceElements, "Id", "Id", changeLog.ForceElementId);
            ViewData["PersonnelStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.PersonnelStatusId);
            ViewData["SrStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.SrStatusId);
            ViewData["SustainmentStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.SustainmentStatusId);
            ViewData["TrainingStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.TrainingStatusId);
            return View(changeLog);
        }

        // POST: ChangeLogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ForceElementId,SrStatusId,CommandOverideStatusId,CommandOverrideComments,DeployedStatusId,NatoGeneralComments,NatoMajorEquipmentComments,NatoCavets,NatoStratLiftCapacityComments,NatoNationalDeployComments," +
            "NatoNationalAssesmentComments,PersonnelStatusId,PersonnelComments,TrainingStatusId,TrainingComments,EquipmentStatusId,EquipmentComments,SustainmentStatusId,SustainmentComments,ChangedDate,ChangedBy")]
            ChangeLog changeLog, string elementIdFilter, string elementNameFilter, string startDateFilter, string endDateFilter, string? selectedPageSize, int? pageNumber, string? sortOrder, string? changeUser)
        {
            if (id != changeLog.Id)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(selectedPageSize))
            {
                selectedPageSize = "10";
            }

            SetViewData(Int32.Parse(selectedPageSize), elementIdFilter, elementNameFilter, startDateFilter, endDateFilter, pageNumber ?? 1, sortOrder ?? "", changeUser ?? "" ?? "");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(changeLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChangeLogExists(changeLog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CommandOverideStatusId"] = new SelectList(_context.CommandOverideStatuses, "Id", "Id", changeLog.CommandOverideStatusId);
            ViewData["DeployedStatusId"] = new SelectList(_context.DeployedStatuses, "Id", "Id", changeLog.DeployedStatusId);
            ViewData["EquipmentStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.EquipmentStatusId);
            ViewData["ForceElementId"] = new SelectList(_context.ForceElements, "Id", "Id", changeLog.ForceElementId);
            ViewData["PersonnelStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.PersonnelStatusId);
            ViewData["SrStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.SrStatusId);
            ViewData["SustainmentStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.SustainmentStatusId);
            ViewData["TrainingStatusId"] = new SelectList(_context.PetsoverallStatuses, "Id", "Id", changeLog.TrainingStatusId);
            return View(changeLog);
        }

        private bool ChangeLogExists(int id)
        {
            return _context.ChangeLogs.Any(e => e.Id == id);
        }
    }
}