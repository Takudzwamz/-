using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Faculty.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Faculty.Models;
using System.Reflection;
using Faculty.Controllers;
using Rotativa.AspNetCore;

namespace Faculty
{
    public class Startup
    {
        //Razor file compilation in ASP.NET Core
        //https://docs.microsoft.com/en-us/aspnet/core/mvc/views/view-compilation?view=aspnetcore-3.0&tabs=visual-studio#runtime-compilation
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //options.UseLazyLoadingProxies().UseSqlServer...  включаем ленивую загрузку с пакетом Microsoft.EntityFrameworkCore.Proxies
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseLazyLoadingProxies().UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //set the options.SignIn.RequireConfirmedAccount to false (это по умолчанию) (было в оригинале true)
            //This allow to log in via a user I add at startup (see next section). This demo user won’t have its email verified so I need to turn off that constraint.
            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false; // default
                options.Password.RequireDigit = false; // default true
                options.Password.RequiredLength = 3;// default 6
                options.Password.RequireLowercase = false;// default true
                options.Password.RequireUppercase = false;// default true
                options.Password.RequiredUniqueChars = 1; // default 1
                options.Password.RequireNonAlphanumeric = false; // default true
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole",
                     policy => policy.RequireRole(BuiltinRoles.Administrator));
                options.AddPolicy("RequireEmployeeRole",
                     policy => policy.RequireRole(BuiltinRoles.Employee));
                options.AddPolicy("RequireStudentRole",
                     policy => policy.RequireRole(BuiltinRoles.Student));

            });


            //services.AddRazorPages();
            IMvcBuilder builder = services.AddRazorPages();
#if DEBUG
            if (Env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
#endif

            services.AddMvc().ConfigureApplicationPartManager(p =>
        p.FeatureProviders.Add(new GenericControllerFeatureProvider()));

            //services.AddMvc().AddMvcOptions(opts =>
            //{
            //    opts.ModelMetadataDetailsProviders.Add(new CustomMetadataProvider());
            //});
            //services.AddDbContext<FacultyContext>(options =>
            //        options.UseSqlServer(Configuration.GetConnectionString("FacultyContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            CreateRoles(serviceProvider);
            //RegisterAllIModelMetadataAwareTypes();
            RotativaConfiguration.Setup((Microsoft.AspNetCore.Hosting.IHostingEnvironment)env);
        }

        //private void RegisterAllIModelMetadataAwareTypes()
        //{
        //    // Get all the model metadata aware attributes in the class
        //    // and register them so they can add information to the metadata object.
        //    var InterfaceType = typeof(IModelMetadataAware);
        //    var CurrentAssembly = Assembly.GetAssembly(typeof(IModelMetadataAware));
        //    var Types = CurrentAssembly
        //        .GetTypes()
        //        .Where(t =>
        //            InterfaceType.IsAssignableFrom(t)
        //        );
        //    foreach (var Type in Types)
        //    {
        //        MyModelMetadataProvider.RegisterMetadataAwareAttribute(Type);
        //    }
        //}

        private void CreateRoles(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<IdentityUser> userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            CreateRole(BuiltinRoles.Administrator, roleManager);
            CreateRole(BuiltinRoles.Employee, roleManager);
            CreateRole(BuiltinRoles.Student, roleManager);
            CreateUser("admin", BuiltinRoles.Administrator, userManager);
            CreateUser("employee", BuiltinRoles.Employee, userManager);
            CreateUser("student", BuiltinRoles.Student, userManager);

        }
        private void CreateUser(string partName,string roleName, UserManager<IdentityUser> userManager)
        {
            //Check if the admin user exists and create it if not
            string email = $"{partName}@gmail.com";
            Task<IdentityUser> testUser = userManager.FindByEmailAsync(email);
            testUser.Wait();
            if (testUser.Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    Email = email,
                    UserName = email
                };

                Task<IdentityResult> newUser = userManager.CreateAsync(user, partName); 
                newUser.Wait();

                if (newUser.Result.Succeeded)
                {
                    //Add to the Administrator role
                    Task<IdentityResult> newUserRole = userManager.AddToRoleAsync(user, roleName);
                    newUserRole.Wait();
                }
            }
        }

        private void CreateRole(string roleName, RoleManager<IdentityRole> roleManager)
        {
            //Check if role exists and create if not
            Task<bool> hasAdminRole = roleManager.RoleExistsAsync(roleName);
            hasAdminRole.Wait();

            if (!hasAdminRole.Result)
            {
                Task<IdentityResult> roleResult = roleManager.CreateAsync(new IdentityRole(roleName) { NormalizedName = roleName.ToUpper() });
                roleResult.Wait();
            }
        }
    }
}
