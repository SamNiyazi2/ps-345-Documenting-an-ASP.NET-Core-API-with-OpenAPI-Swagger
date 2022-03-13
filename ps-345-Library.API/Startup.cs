using AutoMapper;
using Library.API.Contexts;
using Library.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

// 03/12/2022 04:06 pm - SSN - [20220312-1544] - [002] - M04-06 - Demo - Working with API conventions
[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace Library.API
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
            services.AddMvc(setupAction =>
            {
                // 03/12/2022 04:38 am - SSN - [20220312-0304] - [005] - M04-05 - Demo - Using API analyzers to improve the OpenAPI specification 
                //setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                //setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                //setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));



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

            services.AddSwaggerGen(setupAction =>
           {
               setupAction.SwaggerDoc(
                   "LibraryOpenApiSpecification",
                   new Microsoft.OpenApi.Models.OpenApiInfo()
                   {
                       Title = "ps-345-WebAPI - Swagger - Library API",
                       Version = "1"
                       ,

                       // 03/12/2022 01:42 am - SSN - [20220312-0140] - [001] - M03-10 - Demo - Adding API information and description
                       Description = "Through this API you can access authors and their books."
                       ,
                       Contact = new Microsoft.OpenApi.Models.OpenApiContact
                       {
                           Email = "sam-ps-345-API@nonbs.com",
                           Name = "Sam Niyazi",
                           Url = new Uri("https://ps345api.niyazi.com/contact")
                       }
                       ,
                       License = new Microsoft.OpenApi.Models.OpenApiLicense
                       {
                           Name = "MIT",
                           Url = new Uri("https://opensource.org/licenses/MIT")
                       }
                       ,
                       TermsOfService = new Microsoft.OpenApi.Models.OpenApiInfo().TermsOfService = new Uri("https://ps345api.niyazi.com/termsofservice")

                   });

               // 03/11/2022 08:28 pm - SSN - [20220311-1947] - [002] - M03-05 - Incorporating XML comments on actions
               // We XML Documentation file: Project properties > build > output > XML Documentation file.
               var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
               var xmlcommentFileWithFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

               setupAction.IncludeXmlComments(xmlcommentFileWithFullPath);


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
                setupAction.SwaggerEndpoint(
                    "/swagger/LibraryOpenApiSpecification/swagger.json",
                    "Name-102: ps-345-WebAPI - Swagger UI - Library API");

                setupAction.RoutePrefix = ""; // Makes the docs the default URL

            });


            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
