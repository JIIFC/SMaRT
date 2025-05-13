using System.Diagnostics;

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMARTV3.Models;

namespace SMARTV3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly SMARTV3DbContext _context;
        

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, SMARTV3DbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }
       
       

        public async Task<IActionResult> Index()
        {
      
            var contactList = _context.Users.Where(user => user.POC == true)
                .Include(l => l.Organization)
                .OrderBy(l => l.Organization.OrganizationName == "SJS" ? 0 : 1)
                .ThenBy(l => l.Organization.Ordered)
                .ToList();

            ViewData["UserGuideLink"] = _configuration.GetValue<string>("UserGuideLink");
            
            return View(contactList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() 
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CultureManagement(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

            return LocalRedirect(returnUrl);
        }

     


    }
}