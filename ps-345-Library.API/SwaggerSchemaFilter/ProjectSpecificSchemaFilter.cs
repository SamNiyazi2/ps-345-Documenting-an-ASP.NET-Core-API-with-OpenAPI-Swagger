using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/15/2022 04:42 pm - SSN - [20220315-1627] - [006] - M06-03 - Demo - Working with multiple OpenAPI specifications

namespace Library.API.SwaggerSchemaFilter
{
    public class ProjectSpecificSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            Type type = context.Type;
            string name = context.Type.Assembly.GetName().Name;
            string fullName = context.Type.Assembly.GetName().FullName;

        }
    }
}
