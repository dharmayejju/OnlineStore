using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OnlineStore
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddData(o =>
                {
                    o.ConnectionString = Configuration.GetConnectionString("Default");
                });

            var authBuilder = services
               .AddAuthentication(a =>
               {
                   a.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
               })
               .AddCookie(o =>
               {
                   o.Cookie.Name = ".OnlineStore.SharedCookie";
                   o.Cookie.HttpOnly = true;
                   o.SlidingExpiration = true;
                   o.LoginPath = "/account/signin";
                   o.LogoutPath = "/account/signout";
                   var defaultRedirectHanler = o.Events.OnRedirectToLogin;
                   o.Events.OnRedirectToLogin = ctx =>
                   {
                       if (ctx.Request.Path.StartsWithSegments("/api"))
                       {
                           ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                           ctx.Response.Headers["Location"] = ctx.RedirectUri;
                       }
                       return defaultRedirectHanler(ctx);
                   };
               });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Movies}/{action=Index}/{id?}");
            });
        }
    }
}
