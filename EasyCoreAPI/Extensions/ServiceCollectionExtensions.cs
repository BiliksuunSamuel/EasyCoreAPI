using System.Reflection;
using System.Text.Json;
using EasyCoreAPI.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

// ReSharper disable All

namespace EasyCoreAPI.Extensions;

public static class ServiceCollectionExtensions
{

    /// <summary>
    /// Add Swagger Gen
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="schemeName"></param>
    /// <returns></returns>
    public static IServiceCollection AddAPISwaggerGen(
        this IServiceCollection services,
        IConfiguration configuration,
        string schemeName)
    {
        services.Configure<ApiDocsConfig>(c => configuration.GetSection(nameof(ApiDocsConfig)).Bind(c));

        services.ConfigureOptions<ConfigureSwaggerOptions>();
        var projName = Assembly.GetEntryAssembly()?.GetName().Name;

        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();

            c.AddSecurityDefinition(schemeName, new()
            {
                Description = $@"Enter '[schemeName]' [space] and then your token in the text input below.<br/>
                      Example: '{schemeName} 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = schemeName
            });

            c.AddSecurityRequirement(new()
            {
                {
                    new()
                    {
                        Reference = new()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = schemeName
                        },
                        Scheme = "oauth2",
                        Name = schemeName,
                        In = ParameterLocation.Header,
                    },
                    Array.Empty<string>()
                }
            });

            c.DocumentFilter<AdditionalParametersDocumentFilter>();

            c.ResolveConflictingActions(descriptions => { return descriptions.First(); });

            var xmlFile = $"{projName}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }
    
    
    /// <summary>
    /// Use Swagger
    /// </summary>
    /// <param name="app"></param>
    public static void UseAPISwaggerUI(this WebApplication app)
    {
        var apiDocsConfig = app.Services.GetRequiredService<IOptions<ApiDocsConfig>>().Value;

        var apiVersionDescription = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        if (apiDocsConfig.ShowSwaggerUi)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var projName = Assembly.GetExecutingAssembly().GetName().Name;
                foreach (var description in apiVersionDescription.ApiVersionDescriptions.Reverse())
                {
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"{projName} - {description.GroupName}");
                }

                if (apiDocsConfig.EnableSwaggerTryIt)
                {
                    var submitMethods = new SubmitMethod[]
                    {
                        SubmitMethod.Post,
                        SubmitMethod.Get,
                        SubmitMethod.Put,
                        SubmitMethod.Patch,
                        SubmitMethod.Delete,
                    };

                    c.SupportedSubmitMethods(submitMethods);
                }
            });
        }

        if (apiDocsConfig.ShowRedocUi)
        {
            foreach (var description in apiVersionDescription.ApiVersionDescriptions.Reverse())
            {
                app.UseReDoc(options =>
                {
                    options.DocumentTitle = Assembly.GetExecutingAssembly().GetName().Name;
                    options.RoutePrefix = $"api-docs-{description.GroupName}";
                    options.SpecUrl = $"/swagger/{description.GroupName}/swagger.json";
                });
            }
        }
    }


    /// <summary>
    /// Add Api Versioning
    /// </summary>
    /// <param name="services"></param>
    /// <param name="version"></param>
    /// <returns></returns>

    public static IServiceCollection AddAPIVersioning(this IServiceCollection services, int version)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(version, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new MediaTypeApiVersionReader("x-api-version"));
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        return services;
    }


    /// <summary>
    /// Add Api Controllers
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAPIControllers(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        services.AddControllers(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = InvalidModelStateHandler;
            });

        static IActionResult InvalidModelStateHandler(ActionContext context)
        {
            return new BadRequestObjectResult(new ApiResponse<object>(
                message: "Validation Errors",
                code: 400,
                errors: context.ModelState
                    .Where(modelError => modelError.Value?.Errors?.Count > 0)
                    .Select(modelError => new ErrorResponse(
                        field: modelError.Key,
                        errorMessage: modelError.Value?.Errors?.FirstOrDefault()?.ErrorMessage ?? "Invalid Request"))));
        }

        return services;
    }

}