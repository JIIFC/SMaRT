using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SMARTV3.Helpers;
using SMARTV3.Models;
using SMARTV3.Security;
using System.Data.Common;
using System.Globalization;
using System.Text.Json;
using static Constants;
using static SMARTV3.Helpers.PaginationHelper;
using static SMARTV3.Security.UserRoleProvider;

namespace SMARTV3.Controllers
{

    [CustomAuthorize(Roles = Admin)]
    public class MropController : Controller
    {
        private readonly SMARTV3DbContext _context;

        public MropController(SMARTV3DbContext context, IConfiguration configuration)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //ViewBag.gantData = JsonConvert.SerializeObject(_context.OutputTasks.OrderBy(x => x.OutputName)
            //    .Join(_context.OutputForceElements, OT => OT.Id, OFE => OFE.OutputTaskId, (OT, OFE) => new { OT, OFE })
            //    .Join(_context.ForceElements, GD => GD.OFE.FelmId, FElm => FElm.Id, (GD, FElm) => new { GD, FElm })
            //    .Select(o => new { category = o.GD.OT.OutputName, start = o.GD.OFE.AssignmentStart, end = o.GD.OFE.AssignmentEnd, felm = o.FElm.ElementName, id = o.GD.OFE.Id, otId = o.GD.OT.Id, color = o.FElm.DataCards.FirstOrDefault().SrStatus.StatusDisplayColour.ToString().ToLower() }));

            var gantData = _context.OutputTasks
            .OrderBy(x => x.OutputName)
            .Join(_context.OutputForceElements, OT => OT.Id, OFE => OFE.OutputTaskId, (OT, OFE) => new { OT, OFE })
            .Join(_context.ForceElements, GD => GD.OFE.FelmId, FElm => FElm.Id, (GD, FElm) => new { GD, FElm })
            .Select(o => new
            {
                category = o.GD.OT.OutputName,
                start = o.GD.OFE.AssignmentStart != null ? o.GD.OFE.AssignmentStart.ToString("yyyy-MM-dd") : null,
                end = o.GD.OFE.AssignmentEnd != null ? o.GD.OFE.AssignmentEnd.ToString("yyyy-MM-dd") : null,
                felm = o.FElm.ElementName,
                id = o.GD.OFE.Id,
                otId = o.GD.OT.Id,
                color = o.FElm.DataCards.Any() && o.FElm.DataCards.FirstOrDefault().SrStatus != null
                    ? o.FElm.DataCards.FirstOrDefault().SrStatus.StatusDisplayColour.ToLower()
                    : "#cccccc"
            }).ToList();

            ViewBag.gantData = JsonConvert.SerializeObject(gantData);

            ViewBag.gantCategories = JsonConvert.SerializeObject(
                _context.OutputTasks.Select(t => new {
                    category = t.OutputName,
                    otId = t.Id,
                    priority = t.Priority
                }).ToList()
            );

            return View();
        }

        [CustomAuthorize(Roles = Admin)]
        public IActionResult CreateOutputTask()
        {
            ViewBag.capabilityId = new SelectList(_context.Capabilities.Where(d => d.Archived == false), "Id", "CapabilityName");
            ViewBag.ntmId = new SelectList(_context.NoticeToMoves.Where(d => d.Archived == false), "Id", "NoticeToMoveName");
            return View();
        }
        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOutputTask(OutputTask? outputTask)
        {

            if (ModelState.IsValid && outputTask != null)
            {
                _context.Add(outputTask);
                _context.SaveChanges();
            }

            ViewBag.capabilityId = new SelectList(_context.Capabilities.Where(d => d.Archived == false), "Id", "CapabilityName");
            ViewBag.ntmId = new SelectList(_context.NoticeToMoves.Where(d => d.Archived == false), "Id", "NoticeToMoveName");

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = Admin)]
        public IActionResult AssignFelm()
        {
            ViewBag.felmId = new SelectList(_context.ForceElements.Where(d => d.Archived == false), "Id", "ElementName");
            ViewBag.outputTaskId = new SelectList(_context.OutputTasks, "Id", "OutputName");
            return View();
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignFelm(OutputForceElement? outputForceElement)
        {

            ModelState.Remove("OutputTask");
            if (ModelState.IsValid && outputForceElement != null)
            {
                _context.Add(outputForceElement);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = Admin)]
        public IActionResult EditAssignedFelm(int id)
        {
            OutputForceElement outputForceElement = _context.OutputForceElements.FirstOrDefault(d => d.Id == id);
            if (outputForceElement == null) return NotFound();
            ViewBag.felmId = new SelectList(_context.ForceElements.Where(d => d.Archived == false), "Id", "ElementName");
            ViewBag.outputTaskId = new SelectList(_context.OutputTasks, "Id", "OutputName");
            return View(outputForceElement);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAssignedFelm(OutputForceElement? outputForceElement)
        {
            ModelState.Remove("OutputTask");
            if (ModelState.IsValid && outputForceElement != null)
            {
                _context.Update(outputForceElement);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult DeleteAssignedForceElement(int id)
        {
            OutputForceElement outputForceElement = _context.OutputForceElements.FirstOrDefault(d => d.Id == id);
            if (outputForceElement == null) return NotFound();
            _context.Remove(outputForceElement);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = Admin)]
        public IActionResult EditOutputTask(int id)
        {
            OutputTask outputTask = _context.OutputTasks.FirstOrDefault(d => d.Id == id);
            if (outputTask == null) return NotFound();

            ViewBag.capabilityId = new SelectList(_context.Capabilities.Where(d => d.Archived == false), "Id", "CapabilityName");
            ViewBag.ntmId = new SelectList(_context.NoticeToMoves.Where(d => d.Archived == false), "Id", "NoticeToMoveName");
            return View(outputTask);
        }

        [CustomAuthorize(Roles = Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOutputTask(OutputTask? outputTask)
        {
            if (ModelState.IsValid && outputTask != null)
            {
                _context.Update(outputTask);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateFelm([FromBody] FelmUpdate update)
        {
            if (update == null)
                return BadRequest("Invalid data.");

            DateTime startDate;
            DateTime endDate;

            bool isStartValid = DateTime.TryParseExact(update.Start, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
            bool isEndValid = DateTime.TryParseExact(update.End, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);

            if (!isStartValid || !isEndValid)
                return BadRequest("Invalid date format.");

            var felmEntity = _context.OutputForceElements.Find(update.Id);
            if (felmEntity == null)
                return NotFound();

            felmEntity.AssignmentStart = startDate;
            felmEntity.AssignmentEnd = endDate;

            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}
