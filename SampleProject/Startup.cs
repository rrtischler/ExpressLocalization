using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SampleProject.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LazZiya.TranslationServices;
using LazZiya.TranslationServices.IBMWatsonTranslate;
using LazZiya.TranslationServices.MyMemoryTranslate;
using LazZiya.TranslationServices.SystranTranslate;
using LazZiya.TranslationServices.YandexTranslate;
using LazZiya.TranslationServices.GoogleTranslate;
using System.Globalization;
using LazZiya.ExpressLocalization.Routing;
using SampleProject.LocalizationResources;
using LazZiya.ExpressLocalization.Xml;
using LazZiya.ExpressLocalization.Resx;
using LazZiya.ExpressLocalization.DB;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace SampleProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient, ServiceLifetime.Transient);

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<ITranslationService, IBMWatsonTranslateService>();
            services.AddTransient<ITranslationService, MyMemoryTranslateService>();
            services.AddTransient<ITranslationService, SystranTranslateService>();
            services.AddTransient<ITranslationService, YandexTranslateService>();
            services.AddTransient<ITranslationService, GoogleTranslateService>();
            /*
            services.AddRazorPages()
                .AddExpressLocalization<LocSource>(ops =>
                {
                    ops.ResourcesPath = "LocalizationResources";

                    var cultures = new CultureInfo[] { new CultureInfo("en"), new CultureInfo("tr"), new CultureInfo("ar") };
                    ops.RequestLocalizationOptions = o =>
                    {
                        o.SupportedCultures = cultures;
                        o.SupportedUICultures = cultures;
                        o.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
                    };

                    ops.AutoAddKeys = false;
                    ops.OnlineTranslation = true;
                    ops.TranslationService = typeof(MyMemoryTranslateService);
                });
            */
            /*
            services.AddRazorPages()
                .AddExpressLocalizationDB<ApplicationDbContext>(ops =>
                {
                    ops.AutoAddKeys = true;
                    ops.OnlineTranslation = false;
                    ops.TranslationService = typeof(MyMemoryTranslateService);
                    ops.ServeUnapprovedTranslations = true;
                    ops.ResourcesPath = "LocalizationResources";
                    ops.ResourceType = typeof(LocSource);
                });
            
            */
            
            var cultures = new CultureInfo[]
            {
                new CultureInfo("en"),
                new CultureInfo("tr"),
                new CultureInfo("ar")
            };

            services.Configure<RequestLocalizationOptions>(ops =>
            {
                ops.SupportedCultures = cultures;
                ops.SupportedUICultures = cultures;
                ops.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
                ops.RequestCultureProviders.Insert(0, new RouteSegmentRequestCultureProvider(cultures));
            });

            services.AddRazorPages()
                .AddRazorPagesOptions(ops => { ops.Conventions?.Insert(0, new RouteTemplateModelConventionRazorPages()); })
                .AddExpressLocalizationResx<LocSource>((x) =>
                {
                    x.ResourcesPath = "LocalizationResources";
                    x.AutoTranslate = true;
                    x.AutoAddKeys = true;
                    x.ServeUnApprovedTranslations = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRequestLocalization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
