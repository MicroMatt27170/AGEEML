using Ageeml.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Ageeml.Service.Extensions;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddODataApplication(this IMvcBuilder builder)
    {
        builder.AddOData(options =>
        {
            options.AddRouteComponents("api/v1", GetEdmModel())
                .EnableQueryFeatures();
        });

        return builder;
    }

    private static IEdmModel GetEdmModel()
    {
        var modelBuilder = new ODataConventionModelBuilder();
        
        modelBuilder.EnableLowerCamelCase();

        modelBuilder.EntitySet<Estado>("Estados");
        modelBuilder.EntitySet<Municipio>("Municipios");
        modelBuilder.EntitySet<Localidad>("Localidades");

        return modelBuilder.GetEdmModel();
    }
}
