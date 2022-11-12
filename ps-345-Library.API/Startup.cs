using AutoMapper;
using Library.API.Contexts;
using Library.API.OperationFilters;
using Library.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

// 03/12/2022 04:06 pm - SSN - [20220312-1544] - [002] - M04-06 - Demo - Working with API conventions
[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace Library.API
{
    public class Startup
    {
        const string LibraryOpenApiSpecification = "LibraryOpenApiSpecification";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(setupAction =>
            {
                // 03/12/2022 04:38 am - SSN - [20220312-0304] - [005] - M04-05 - Demo - Using API analyzers to improve the OpenAPI specification 
                // 03/13/2022 11:38 pm - SSN - [20220313-2050] - [004] - M05-06 - Demo - Supporting schema variation by media type (Output IOperationFilter))
                // Reactivate and add ProducesDefaultResponseTypeAttribute 
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                setupAction.Filters.Add(new ProducesDefaultResponseTypeAttribute());

                // 03/14/2022 01:49 am - SSN - [20220314-0111] - [006] - M05-08 - Demo - Supporting schema variation by media type (Input)
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status415UnsupportedMediaType));


                // 03/16/2022 12:14 am - SSN - [20220316-0005] - [002] - M06-09 - Demo - Adding authentication support to the OpenAPI specification
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));


                // 03/15/2022 11:31 pm - SSN - [20220315-2303] - [004] - M06-08 - Demo - Protecting your API
                setupAction.Filters.Add(new AuthorizeFilter());


                setupAction.ReturnHttpNotAcceptable = true;


                // 03/12/2022 10:34 pm - SSN - [20220312-2220] - [002] - M04-10 - Demo - Specifying the response body type with the Produces attribute
                setupAction.OutputFormatters.Add(new XmlSerializerOutputFormatter());


                var jsonOutputFormatter = setupAction.OutputFormatters
                    .OfType<JsonOutputFormatter>().FirstOrDefault();

                if (jsonOutputFormatter != null)
                {
                    // remove text/json as it isn't the approved media type
                    // for working with JSON at API level
                    if (jsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
                    {
                        jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
                    }
                }
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // register the DbContext on the container, getting the connection string from
            // appSettings (note: use this during development; in a production environment,
            // it's better to store the connection string in an environment variable)
            var connectionString = Configuration["ConnectionStrings:LibraryDBConnectionString"];
            services.AddDbContext<LibraryContext>(o => o.UseSqlServer(connectionString));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext =
                        actionContext as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    // if there are modelstate errors & all keys were correctly
                    // found/parsed we're dealing with validation errors
                    if (actionContext.ModelState.ErrorCount > 0
                        && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    // if one of the keys wasn't correctly found / couldn't be parsed
                    // we're dealing with null/unparsable input
                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();

            services.AddAutoMapper();

            // 03/11/2022 06:21 pm - SSN - [20220310-1628] - [001] - M03-03 - Demo - Installing Swashbuckle


            // 03/15/2022 06:42 pm - SSN - [20220315-1836] - [001] - M06-06 - Demo - Matching OpenAPI specifications to API versions
            services.AddVersionedApiExplorer(setupAction =>
            {
                // 'v'(V=Major,V=Minor)
                setupAction.GroupNameFormat = "'v'VV";
            });






            // 03/15/2022 11:28 pm - SSN - [20220315-2303] - [003] - M06-08 - Demo - Protecting your API
            // Order is critical.
            services.AddAuthentication("Basic").AddScheme<AuthenticationSchemeOptions, Authentication.BasicAuthenticationHandler>("Basic", null);






            // 03/15/2022 05:26 pm - SSN - [20220315-1713] - [004] - M06-05 - Demo - Versioning your API
            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;

                // 03/15/2022 06:18 pm - SSN - [20220315-1713] - [008] - M06-05 - Demo - Versioning your API
                // Options for picking up API version
                //setupAction.ApiVersionReader = new HeaderApiVersionReader("api-version");
                //setupAction.ApiVersionReader = new MediaTypeApiVersionReader();

            });


            // 03/15/2022 06:49 pm - SSN - [20220315-1836] - [002] - M06-06 - Demo - Matching OpenAPI specifications to API versions
            // Must be added after AddApiVersioning
            // To pickup descriptions for multiple versions.
            // We can't use DI here because we are in the ConfigureServices method, but we are using it in the configure method
            var apiVersionDescriptionProvider = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();




            services.AddSwaggerGen(setupAction =>
           {

               // 03/15/2022 06:53 pm - SSN - [20220315-1836] - [003] - M06-06 - Demo - Matching OpenAPI specifications to API versions
               foreach (var apiVerDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
               {

                   setupAction.SwaggerDoc(

                       // 03/15/2022 04:27 pm - SSN - [20220315-1627] - [001] - M06-03 - Demo - Working with multiple OpenAPI specifications
                       // 03/15/2022 05:17 pm - SSN - [20220315-1713] - [001] - M06-05 - Demo - Versioning your API
                       // Removed multiple specification. Demoed only in branch M06-03
                       $"{LibraryOpenApiSpecification }{apiVerDescription.GroupName}",
                       //"LibraryOpenApiSpecificationAuthors",
                       new Microsoft.OpenApi.Models.OpenApiInfo()
                       {
                           Title = "ps-345-WebAPI - Swagger - Library API",
                           //Version = "1"
                           Version = apiVerDescription.ApiVersion.ToString()
                           ,

                           // 03/12/2022 01:42 am - SSN - [20220312-0140] - [001] - M03-10 - Demo - Adding API information and description
                           Description = "API for accessing authors and their books."
                           ,
                           Contact = getAPIContact()
                           ,
                           License = new Microsoft.OpenApi.Models.OpenApiLicense
                           {
                               Name = "MIT",
                               Url = new Uri("https://opensource.org/licenses/MIT")
                           }
                           ,
                           TermsOfService = getTermOfService()

                       });


               }



               // 03/16/2022 12:07 am - SSN - [20220316-0005] - [001] - M06-09 - Demo - Adding authentication support to the OpenAPI specification

               setupAction.AddSecurityDefinition("basicAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
               {
                   Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                   Scheme = "basic", // case-sensitive
                   Description = "Input your username and password to access this API"
               });


               setupAction.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
               {
                   {
                        new OpenApiSecurityScheme

                            {
                                Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "basicAuth"
                                    }
                            }
                            , new List<string>()
                   }


               });



               // 03/15/2022 07:11 pm - SSN - [20220315-1836] - [006] - M06-06 - Demo - Matching OpenAPI specifications to API versions
               setupAction.DocInclusionPredicate((documentName, apiDescription) =>
                  {
                      var actionApiVersionModel = apiDescription.ActionDescriptor
                                                    .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                      if (actionApiVersionModel == null)
                      {
                          return true;
                      }

                      if (actionApiVersionModel.DeclaredApiVersions.Any())
                      {
                          return actionApiVersionModel.DeclaredApiVersions.Any(v => $"{LibraryOpenApiSpecification }v{v.ToString()}" == documentName);
                      }

                      return actionApiVersionModel.ImplementedApiVersions.Any(v => $"{LibraryOpenApiSpecification }v{v.ToString()}" == documentName);

                  });









               // 03/13/2022 10:58 pm - SSN - [20220313-2050] - [003] - M05-06 - Demo - Supporting schema variation by media type (Output IOperationFilter))
               // Fix for missing OperationId in OperationFilter.
               // https://stackoverflow.com/questions/52262826/asp-net-core-swashbuckle-set-operationid
               setupAction.CustomOperationIds(o =>
                                                 // $"{o.ActionDescriptor.RouteValues["controller"]}_{o.HttpMethod}"
                                                 $"{o.ActionDescriptor.RouteValues["controller"]}_{o.ActionDescriptor.RouteValues["action"]}"
                                       );


               // 03/13/2022 08:34 pm - SSN - [20220313-2034] - [001] - M05-05 - Demo - Supporting schema variation by media type (Output ResolveConflictingActions)

               // 03/14/2022 12:44 am - SSN - [20220313-2050] - [006] - M05-06 - Demo - Supporting schema variation by media type (Output IOperationFilter))
               // By adding  [ApiExplorerSettings(IgnoreApi = true)] to the duplicate action method, we no longer have a conflict. ResolveConflictingActions
               //  is no longer needed.
               //setupAction.ResolveConflictingActions(apiDescriptions =>
               //{

               //    // Not a valid solution.
               //    return apiDescriptions.First();

               //    //// Errors out on adding duplicate key 200.
               //    //var description1 = apiDescriptions.ElementAt(0);
               //    //var description2 = apiDescriptions.ElementAt(1);

               //    //description1.SupportedResponseTypes.AddRange(
               //    //     description2.SupportedResponseTypes.Where(a => a.StatusCode == 200)
               //    //     );

               //    //return description1;
               //});

               // 03/13/2022 08:55 pm - SSN - [20220313-2050] - [002] - M05-06 - Demo - Supporting schema variation by media type (Output IOperationFilter))

               setupAction.OperationFilter<GetBookOperationFilter>();


               // 03/14/2022 01:37 am - SSN - [20220314-0111] - [005] - M05-08 - Demo - Supporting schema variation by media type (Input)
               setupAction.OperationFilter<CreateBookOperationFilter>();






               // 03/11/2022 08:28 pm - SSN - [20220311-1947] - [002] - M03-05 - Incorporating XML comments on actions
               // We XML Documentation file: Project properties > build > output > XML Documentation file.
               var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
               var xmlcommentFileWithFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

               setupAction.IncludeXmlComments(xmlcommentFileWithFullPath);


               // 03/15/2022 04:43 pm - SSN - [20220315-1627] - [007] - M06-03 - Demo - Working with multiple OpenAPI specifications
               setupAction.SchemaFilter<SwaggerSchemaFilter.ProjectSpecificSchemaFilter>();



           });



        }

        // 11/12/2022 1:43 am - SSN - Remove hard-coded values
        #region Objects from Environment variables
        private static Uri getTermOfService()
        {
            string apiContactName = Environment.GetEnvironmentVariable("ps-345-apiTermOfService"); // "https://ps345api.niyazi.com/termsofservice"
            return string.IsNullOrEmpty(apiContactName) ? null : new Microsoft.OpenApi.Models.OpenApiInfo().TermsOfService = new Uri(apiContactName);
        }

        private static OpenApiContact getAPIContact()
        {
            string apiContactName = Environment.GetEnvironmentVariable("ps-345-apiContactname");
            string apiContactEmail = Environment.GetEnvironmentVariable("ps-345-apiContactEmail");
            string apiContactUrlString = Environment.GetEnvironmentVariable("ps-345-apiContactUrl");

            var apiContractUrl = string.IsNullOrWhiteSpace(apiContactUrlString) ? null : new Uri(apiContactUrlString);

            return new Microsoft.OpenApi.Models.OpenApiContact
            {
                Email = apiContactEmail, //"sam-ps-345-API@nonbs.com",
                Name = apiContactName,
                Url = apiContractUrl  //new Uri("https://ps345api.niyazi.com/contact")
            };
        }

        #endregion Objects from Environment variables


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // 03/15/2022 07:02 pm - SSN - [20220315-1836] - [004] - M06-06 - Demo - Matching OpenAPI specifications to API versions
        // Inject IApiVersionDescriptionProvider
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. 
                // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();




            // 03/11/2022 06:25 pm - SSN - [20220310-1628] - [002] - M03-03 - Demo - Installing Swashbuckle
            // After UseHttpsRedirection
            app.UseSwagger();


            // 03/11/2022 07:17 pm - SSN - [20220311-1915] - [001] - M03-04 - Demo - Adding Swagger UI
            app.UseSwaggerUI(setupAction =>
            {

                // 03/16/2022 02:36 am - SSN - [20220316-0234] - [001] - M07-08 - Demo - Branding the UI by injecting custom CSS

                setupAction.InjectStylesheet("/assets/custom-ui.css");

                // 03/16/2022 09:36 am - SSN - [20220316-0241] - [001] - M07-09 - Demo - Branding the UI by injecting a custom index page
                setupAction.IndexStream = () => GetType().Assembly.GetManifestResourceStream("Library.API.EmbeddedAssets.index_v2.html");


                // 03/15/2022 04:31 pm - SSN - [20220315-1627] - [003] - M06-03 - Demo - Working with multiple OpenAPI specifications
                // 03/15/2022 05:18 pm - SSN - [20220315-1713] - [002] - M06-05 - Demo - Versioning your API
                // Removed muliple specifications

                // 03/15/2022 07:03 pm - SSN - [20220315-1836] - [005] - M06-06 - Demo - Matching OpenAPI specifications to API versions
                foreach (var apiVerDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {

                    setupAction.SwaggerEndpoint(
                        //"/swagger/LibraryOpenApiSpecification/swagger.json",
                        $"/swagger/{LibraryOpenApiSpecification }{apiVerDescription.GroupName}/swagger.json",
                        $"ps-345-WebAPI - Swagger UI - Library API - ({apiVerDescription.GroupName})");

                }


                setupAction.RoutePrefix = ""; // Makes the docs the default URL


                // 03/16/2022 01:51 am - SSN - [20220316-0108] - [001] - M07-04 - Demo - Basic UI customization with the configuration API
                // Format Swagger document.
                setupAction.DefaultModelExpandDepth(2);
                setupAction.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example); //default
                setupAction.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                setupAction.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                setupAction.DisplayRequestDuration();
                setupAction.EnableFilter();


                // 03/16/2022 02:10 am - SSN - [20220316-0209] - [001] - M07-06 - Demo - Supporting deep linking
                setupAction.EnableDeepLinking();
                setupAction.DisplayOperationId();
                // Allows for:
                // https://localhost:44305/index.html#/Books/Books_GetBook
                // https://localhost:44305/index.html#/Authors/Authors_GetAuthor
                // https://localhost:44305/index.html#/Authors/Authors_GetAuthors
                // https://localhost:44305/index.html#/Authors/Authors_UpdateAuthor




            });


            app.UseStaticFiles();


            // 03/15/2022 11:26 pm - SSN - [20220315-2303] - [002] - M06-08 - Demo - Protecting your API
            // Must be called before UseMVC
            app.UseAuthentication();


            app.UseMvc();
        }
    }
}
