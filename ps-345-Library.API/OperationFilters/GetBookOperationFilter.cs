using Library.API.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/13/2022 08:53 pm - SSN - [20220313-2050] - [001] - M05-06 - Demo - Supporting schema variation by media type (Output IOperationFilter))

namespace Library.API.OperationFilters
{
    public class GetBookOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.OperationId != "Books_GetBook") return;

            OpenApiSchema newSchema = context.SchemaGenerator.GenerateSchema(typeof(Models.BookWithConcatenatedAuthorName), context.SchemaRepository);

            operation.Responses[StatusCodes.Status200OK.ToString()].Content.Add(
                MediaTypesConstants.APPLICATION_VND_MARVIN_BookWithConcatenatedAuthorName_JSON,
                new OpenApiMediaType()
                {
                    // No context.SchemaRegistry.  Only Repository and Generator
                    // Schema = context.SchemaRegistry.GetOrRegister(typeof (Models.BookWithConcatenatedAuthorName))

                    Schema = context.SchemaRepository.GetOrAdd(typeof(Models.BookWithConcatenatedAuthorName), nameof(Models.BookWithConcatenatedAuthorName), () => newSchema)

                });

        }
    }
}
