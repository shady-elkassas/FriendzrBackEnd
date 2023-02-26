using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Social.Controllers;
using Social.Entity.DBContext;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.HangFireJobs;
using Social.Helper;
using Social.Sercices.Helpers;
using Social.Services.Attributes;
using Social.Services.DateTimeModelBinderProvider;
using Social.Services.FireBase_Helper;
using Social.Services.Helpers;
using Social.Services.Implementation;
using Social.Services.PushNotification;
using Social.Services.Services;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Social
{
    public class Startup
    {

        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment IHostingEnvironment)
        {
            Configuration = configuration;
            _environment = IHostingEnvironment;

        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var path = Path.Combine(_environment.ContentRootPath, "wwwroot\\FireBase\\friendzr-1631017594822-firebase-adminsdk-3o7sj-26049eb40c.json");
            var path = Path.Combine(_environment.ContentRootPath, "friendzr_FirbaseSettings.json");


            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(path),
            });
            // configure form options limit
            services.AddMvc(options =>
            {
                options.MaxModelBindingCollectionSize = 100000;
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
            services.AddControllers(options =>
            {
                var jsonInputFormatter = options.InputFormatters
                    .OfType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>()
                    .Single();
                jsonInputFormatter.SupportedMediaTypes.Add("application/csp-report");
            });
            // identity config
            services.Configure<IdentityOptions>(options =>
            {
                // Default User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-., _@#$%^*=!+";
                options.User.RequireUniqueEmail = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;
            });
            services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromMinutes(5));
            //            services.Configure(a =>
            //{
            //    a.InvalidModelStateResponseFactory = context =>
            //    {
            //        var problemDetails = new CustomBadRequest(context);
            //        return new BadRequestObjectResult(problemDetails)
            //        {
            //            ContentTypes = { "application/problem+json","application/problem+xml"}
            //        };
            //    };
            //});


            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            
            // swagger config
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Place Info Service API",
                    Version = "v2",
                    Description = "Sample service for Learner",
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });

            });
            // For Identity 
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IGoogleLocationService, LocationService>();
            services.AddScoped<IGlobalMethodsService, globalMethodsService>();

            //services.AddSingleton<LocService>();
            services.AddScoped<IMessageServes, MessageServes>();
            services.AddScoped<IFrindRequest, FrindRequest>();
            services.AddScoped<IEventServ, EventServ>();
            services.AddScoped<IWhatBestDescripsMe, WhatBestDescrips>();
            services.AddScoped<IIpreferto, Ipreferto>();
            services.AddScoped<IAppConfigrationService, AppConfigrationService>();
            services.AddScoped<IChatGroupService, ChatGroupService>();
            services.AddScoped<IUserReportService, UserReportService>();
            services.AddScoped<IChatGroupReportService, ChatGroupReportService>();
            services.AddScoped<IEventReportService, EventReportService>();
            services.AddScoped<IEventCategoryService, EventCategoryService>();
            services.AddScoped<IEventTypeListService, EventTypeListService>();
            services.AddScoped<IPushNotification, PushNotification>();

            services.AddScoped<IReportReasonService, ReportReasonService>();
            services.AddScoped<IFilteringAccordingToAgeHistoryService, FilteringAccordingToAgeHistoryService>();
            services.AddScoped<IDistanceFilterHistoryService, DistanceFilterHistoryService>();
            services.AddScoped<AuthorizeUser>();
            services.AddScoped<AuthorizeAdmin>();
            services.AddScoped<AuthorizeWhiteLable>();
            services.AddScoped<IErrorLogService, ErrorLogService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IInterestsService, InterestsService>();
            services.AddTransient<EmailHelper>();
            services.AddIdentity<User, ApplicationRole>().AddEntityFrameworkStores<AuthDBContext>().AddDefaultTokenProviders();
            services.AddHttpClient();
            services.AddControllersWithViews();
            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });
            services.AddMvc();
            services.AddCors();
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            
            services.AddScoped<IWhiteLableUserService, WhiteLableUserService>();
            services.AddScoped<ExternalEventUtility, ExternalEventUtility>();
            services.AddScoped<IExternalEventJob, ExternalEventJob>();
            services.AddScoped<PublicController, PublicController>();
            services.AddScoped<AccountupdateController, AccountupdateController>();
            services.AddScoped<INotifyUpdateProfileJob, NotifyUpdateProfileJob>();
            services.AddSingleton(Log.Logger);

            services.AddCors(options => options.AddPolicy("CorsPolicy",
               builder =>
               {
                   builder.AllowAnyHeader()
                          .AllowAnyMethod()
                          .SetIsOriginAllowed((host) => true)
                          .AllowCredentials();
               })
            );

            services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            //services.AddDbContext<AuthDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AuthConnStr")));
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

           // services.AddDbContextPool<AuthDBContext>(options => options.UseLazyLoadingProxies().UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug())).UseSqlServer(Configuration.GetConnectionString("AuthConnStr")));
            services.AddDbContextPool<AuthDBContext>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("AuthConnStr")));

            services.AddRazorPages();
            // Hangfire config 
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("AuthConnStr")));
            services.AddHangfireServer();

            //services.AddDbContext<AuthDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AuthConnStr")));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer  
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                //options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
            })
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var assemblyName = new AssemblyName(typeof(Services.SharedResource).GetTypeInfo().Assembly.FullName);
                    return factory.Create("SharedResource", assemblyName.Name);
                };
            });
            services.AddControllersWithViews(options =>
            {
                //var policy = new AuthorizationPolicyBuilder()
                //    .RequireAuthenticatedUser()
                //    .Build();
                //options.Filters.Add(new AuthorizeFilter(policy));
            }).AddRazorRuntimeCompilation();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);//You can set Time   
            });
            services.AddScoped<IFirebaseManager, FirebaseManager>();

            //services.Configure<RequestLocalizationOptions>(options => 
            //{
            //    List<CultureInfo> supportedCultures = new List<CultureInfo>
            //    {
            //        new CultureInfo("en-US"),
            //        new CultureInfo("ar-EG"),
            //        new CultureInfo("fr-FR")
            //    };
            //    options.DefaultRequestCulture = new RequestCulture("en-US");
            //    options.SupportedCultures = supportedCultures;
            //    options.SupportedUICultures = supportedCultures;
            //});
            services.AddControllers(option =>
            {
                // add the custom binder at the top of the collection
                option.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
            })
                ;
            services.AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problems = new CustomBadRequest(context);
                var messa = new ResponseModel<object>(StatusCodes.Status406NotAcceptable, true,
                   problems.Message, null);
                var result = new ObjectResult(messa) { StatusCode = StatusCodes.Status406NotAcceptable };

                return result;
            };
        });

        }
        //Call the Jobs
        private void EnqueueHangFireJobs(IConfiguration configuration)
        {

            RecurringJob.RemoveIfExists(nameof(IExternalEventJob.ExportExternalEvents));
            RecurringJob.AddOrUpdate<IExternalEventJob>(j => j.ExportExternalEvents(), cronExpression: configuration["Jobs:ExternalEvent:CronExpression"], timeZone: TimeZoneInfo.Local);

            RecurringJob.RemoveIfExists(nameof(INotifyUpdateProfileJob.SendUpdateProfileNotification));
            RecurringJob.AddOrUpdate<INotifyUpdateProfileJob>(j => j.SendUpdateProfileNotification(), cronExpression: configuration["Jobs:UpdateProfile:CronExpression"], timeZone: TimeZoneInfo.Local);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFact/*, AuthDBContext context*/)
        {
            app.UseSerilogRequestLogging(); // <-- Add this line

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Hang fire
            app.UseHangfireDashboard("/FriendzrHangFireDashboard");
           
            EnqueueHangFireJobs(Configuration);
            //
            app.ConfigureCustomExceptionMiddleware();
            //context.Database.Migrate();
            var cultureSupported = new[]
            {
                new CultureInfo("ar-EG"),
                new CultureInfo("en-US"),
                new CultureInfo("fr-FR")
            };

            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = cultureSupported,
                SupportedUICultures = cultureSupported
            };
            //requestLocalizationOptions.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async httpContext => { return new ProviderCultureResult("en-US"); }));
            app.UseRequestLocalization(requestLocalizationOptions);

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseDeveloperExceptionPage();
            //var defaultCulture = new CultureInfo("tr-TR");
            //app.UseRequestLocalization(new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new RequestCulture(defaultCulture),
            //    SupportedCultures = new List<CultureInfo> { defaultCulture },
            //    SupportedUICultures = new List<CultureInfo> { defaultCulture }
            //});
            // Auth
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                   name: "WhitelabelArea",
                   areaName: "Whitelabel",
                   pattern: "Whitelabel/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                    name: "UserArea",
                    areaName: "User",
                    pattern: "User/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                    "AdminArea",
                    "Admin",
                    "Admin/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseEndpoints(endpoints =>
            //{

            //    endpoints.MapControllers();
            //});   

            // swagger
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v2/swagger.json", "PlaceInfo Services"));
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors(options => options
                .WithOrigins(
                    new string[]
                    {
                        "http://localhost:3000", "http://localhost:8080", "http://localhost:4200","http://localhost:63793",
                        "http://localhost:5000"
                    })
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
            );
        }
    }
}
