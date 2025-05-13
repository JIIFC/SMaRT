using System.Globalization;

using JT.AspNetBaseRoleProvider;

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using SMARTV3.Models;
using SMARTV3.Security;

namespace SMARTV3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //Retrive all of the cultures and add it to the pipeline
            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<SMARTV3DbContext>(options => options.UseSqlServer("name=ConnectionStrings:SMARTConnectionString",
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            services.AddRazorPages();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddControllersWithViews().AddViewLocalization();
            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });
            services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();
            services.Configure<RequestLocalizationOptions>(
                opt =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en"),
                        new CultureInfo("fr")
                    };
                    opt.DefaultRequestCulture = new RequestCulture("en");
                    opt.SupportedCultures = supportedCultures;
                    opt.SupportedUICultures = supportedCultures;
                });
            services.AddControllers();
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddJTRoleAuthorization<UserRoleProvider>();
        }
    }
}