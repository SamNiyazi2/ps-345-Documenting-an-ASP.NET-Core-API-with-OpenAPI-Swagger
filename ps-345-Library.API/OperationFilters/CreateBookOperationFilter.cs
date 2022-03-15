using Library.API.Constants;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/14/2022 01:32 am - SSN - [20220314-0111] - [004] - M05-08 - Demo - Supporting schema variation by media type (Input)

namespace Library.API.OperationFilters
{
    public class CreateBookOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            if (operation.OperationId != "Books_CreateBook") return;

            OpenApiSchema newSchema = context.SchemaGenerator.GenerateSchema(typeof(Models.BookForCreationWithAmountOfPages), context.SchemaRepository);

            operation.RequestBody.Content.Add(MediaTypesConstants.APPLICATION_VND_MARVIN_BookForCreationWithAmountOfPages_JSON,
                new OpenApiMediaType()
                {
                    Schema = context.SchemaRepository.GetOrAdd(
                        typeof(Models.BookForCreationWithAmountOfPages),
                        nameof(Models.BookForCreationWithAmountOfPages),
                        () => newSchema)
                });
        }
    }
}
