using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using SFA.DAS.RoATPService.Api.Client;
using SFA.DAS.RoATPService.Api.Client.Interfaces;
using SFA.DAS.RoATPService.Application.Api.Helpers;
using SFA.DAS.RoATPService.Application.Handlers;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Mappers;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Data.Helpers;

namespace SFA.DAS.RoATPService.Application.Api.StartupConfiguration
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation.AspNetCore;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Middleware;
    using Settings;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        private const string ServiceName = "SFA.DAS.RoATPService";
        private const string Version = "1.0";
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Startup> _logger;

        private IConfiguration ApplicationConfiguration { get; }

        public Startup(IHostingEnvironment env, IConfiguration config, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;
            _logger.LogInformation("In startup constructor.  Before GetConfig");
            Configuration = ConfigurationService
                .GetConfig(config["EnvironmentName"], config["ConfigurationStorageConnectionString"], Version, ServiceName).Result;
            ApplicationConfiguration = config;
            _logger.LogInformation("In startup constructor.  After GetConfig");
        }

        public IWebConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddAuthentication(o => { o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                    .AddJwtBearer(o =>
                    {
                        o.Authority = $"https://login.microsoftonline.com/{Configuration.ApiAuthentication.TenantId}";
                        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                            ValidAudiences = new List<string>
                            {
                                Configuration.ApiAuthentication.Audience,
                                Configuration.ApiAuthentication.ClientId
                            }
                        };
                        o.Events = new JwtBearerEvents()
                        {
                            OnTokenValidated = context => { return Task.FromResult(0); }
                        };
                    });

                services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

                services.Configure<RequestLocalizationOptions>(options =>
                {
                    options.DefaultRequestCulture = new RequestCulture("en-GB");
                    options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                    options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                    options.RequestCultureProviders.Clear();
                });

                var auditLogSettings = new RegisterAuditLogSettings();
                ApplicationConfiguration.Bind("RegisterAuditLogSettings", auditLogSettings);
                services.AddSingleton(auditLogSettings);

                services.AddHealthChecks();
                IMvcBuilder mvcBuilder;
                if (_env.IsDevelopment())
                    mvcBuilder = services.AddMvc(opt => { opt.Filters.Add(new AllowAnonymousFilter()); });
                else
                    mvcBuilder = services.AddMvc();

                mvcBuilder
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
                        opts => { opts.ResourcesPath = "Resources"; })
                    .AddDataAnnotationsLocalization()
                    .AddControllersAsServices()
                    .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "SFA.DAS.RoATPService.Application.Api", Version = "v1" });

                    if (_env.IsDevelopment())
                    {
                        var basePath = AppContext.BaseDirectory;
                        var xmlPath = Path.Combine(basePath, "SFA.DAS.RoATPService.Application.Api.xml");
                        c.IncludeXmlComments(xmlPath);
                    }
                });

                ConfigureDependencyInjection(services);

                if (_env.IsDevelopment())
                {
                   // TestDataService.AddTestData(serviceProvider.GetService<AssessorDbContext>());
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during Startup Configure Services");
                throw;
            }
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient(x => Configuration);
            services.AddTransient<IDownloadRegisterRepository, DownloadRegisterRepository>();
            services.AddTransient<ILookupDataRepository, LookupDataRepository>();
            services.AddTransient<IOrganisationRepository, OrganisationRepository>();
            services.AddTransient<IDuplicateCheckRepository, DuplicateCheckRepository>();
            services.AddTransient<ICreateOrganisationRepository, CreateOrganisationRepository>();
            services.AddTransient<IOrganisationSearchRepository, OrganisationSearchRepository>();
            services.AddTransient<IUpdateOrganisationRepository, UpdateOrganisationRepository>();
            services.AddTransient<IDataTableHelper, DataTableHelper>();
            services.AddTransient<ICacheHelper, CacheHelper>();
            services.AddTransient<IProviderTypeValidator, ProviderTypeValidator>();
            services.AddTransient<IOrganisationSearchValidator, OrganisationSearchValidator>();
            services.AddTransient<IOrganisationValidator, OrganisationValidator>();
            services.AddTransient<IOrganisationSearchValidator, OrganisationSearchValidator>();
            services.AddTransient<IMapCreateOrganisationRequestToCommand, MapCreateOrganisationRequestToCommand>();
            services.AddTransient<ITextSanitiser, TextSanitiser>();
            services.AddTransient<IUkrlpApiClient, UkrlpApiClient>();
            services.AddTransient<IAuditLogService, AuditLogService>();
            services.AddTransient<IOrganisationStatusManager, OrganisationStatusManager>();        
            services.AddTransient<HttpClient>();
            services.AddTransient<IUkrlpSoapSerializer, UkrlpSoapSerializer>();
            services.AddTransient<IEventsRepository, EventsRepository>();

            services.AddMediatR(typeof(GetProviderTypesHandler).GetTypeInfo().Assembly);
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {
                MappingStartup.AddMappings();

                if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

                app.UseSwagger()
                    .UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SFA.DAS.RoATPService.Application.Api v1");
                    })
                    .UseAuthentication();

                app.UseMiddleware(typeof(ErrorHandlingMiddleware));

                app.UseRequestLocalization();
                app.UseHealthChecks("/health");

                app.UseMvc();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during Startup Configure");
                throw;
            }

        }
    }
}
