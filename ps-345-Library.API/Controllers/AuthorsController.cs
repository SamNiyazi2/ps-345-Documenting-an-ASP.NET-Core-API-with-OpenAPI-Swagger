using AutoMapper;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Library.API.Constants;

namespace Library.API.Controllers
{

    // 03/15/2022 06:22 pm - SSN - [20220315-1713] - [009] - M06-05 - Demo - Versioning your API
    // [Route("api/authors")]
    [Route("api/v{version:apiVersion}/authors")]
    //[Route("api/v1.0/authors")]

    [ApiController]
    // 03/15/2022 04:32 pm - SSN - [20220315-1627] - [004] - M06-03 - Demo - Working with multiple OpenAPI specifications
    // 03/15/2022 05:20 pm - SSN - [20220315-1713] - [003] - M06-05 - Demo - Versioning your API
    // Removed multiple specification.  Used in Bbranch m06-03 only.
    // [ApiExplorerSettings(GroupName = "LibraryOpenApiSpecificationAuthors")]

    // 03/15/2022 05:38 pm - SSN - [20220315-1713] - [006] - M06-05 - Demo - Versioning your API
    // This wasn't here before, but somehow, we did have a problem before defaulting to JSON.
    // Noticed in video M06-05 that it was the top line.
    [Produces(MediaTypesConstants.APPLICATION_JSON, MediaTypesConstants.APPLICATION_XML)]

    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorsRepository;
        private readonly IMapper _mapper;

        public AuthorsController(
            IAuthorRepository authorsRepository,
            IMapper mapper)
        {
            _authorsRepository = authorsRepository;
            _mapper = mapper;
        }

        [HttpGet]

        // 03/15/2022 05:32 pm - SSN - [20220315-1713] - [005] - M06-05 - Demo - Versioning your API
        // No ProducesResponseType(StatusCodes were defiend. Added 200.
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            var authorsFromRepo = await _authorsRepository.GetAuthorsAsync();
            return Ok(_mapper.Map<IEnumerable<Author>>(authorsFromRepo));
        }

        // 03/11/2022 08:20 pm - SSN - [20220311-1947] - [001] - M03-05 - Incorporating XML comments on actions
        /// <summary>
        /// Get an author by his/her id
        /// </summary>
        /// <param name="authorId">The id of the author to get</param>
        /// <returns>An ActionResult of type Author. NOT visible in Swagger documentation.</returns>

        // 03/12/2022 04:54 am - SSN - [20220312-0304] - [007] - M04-05 - Demo - Using API analyzers to improve the OpenAPI specification 

        [HttpGet("{authorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Author>> GetAuthor(
            Guid authorId)
        {
            var authorFromRepo = await _authorsRepository.GetAuthorAsync(authorId);
            if (authorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<Author>(authorFromRepo));
        }

        [HttpPut("{authorId}")]
        // 03/12/2022 04:27 am - SSN - [20220312-0304] - [003] - M04-05 - Demo - Using API analyzers to improve the OpenAPI specification 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Author>> UpdateAuthor(
            Guid authorId,
            AuthorForUpdate authorForUpdate)
        {
            var authorFromRepo = await _authorsRepository.GetAuthorAsync(authorId);
            if (authorFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(authorForUpdate, authorFromRepo);

            //// update & save
            _authorsRepository.UpdateAuthor(authorFromRepo);
            await _authorsRepository.SaveChangesAsync();

            // return the author
            return Ok(_mapper.Map<Author>(authorFromRepo));
        }


        // 03/11/2022 10:40 pm - SSN - [20220311-2235] - [001] - M03-08 - Improving documentation with examples
        /// <summary>
        /// Partially update an author
        /// </summary>
        /// <param name="authorId">The id of the author you want to update</param>
        /// <param name="patchDocument">The set of operations to apply</param>
        /// <returns>An ActionResult of type Author.  Does not display in API documenation.</returns>
        /// <remarks>Sample request (this request updates the author's **first name**)   
        ///     PATCH /authors/authorId   
        ///     [   
        ///          {   
        ///               "op": "==replace==",   
        ///               "path": "/afirstname",  
        ///               "value": **"new first name"**  
        ///          }  
        ///     ]
        ///   
        /// </remarks>

        // 03/12/2022 04:54 am - SSN - [20220312-0304] - [006] - M04-05 - Demo - Using API analyzers to improve the OpenAPI specification 

        [HttpPatch("{authorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status422UnprocessableEntity)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Author>> UpdateAuthor(
            Guid authorId,
            JsonPatchDocument<AuthorForUpdate> patchDocument)
        {
            var authorFromRepo = await _authorsRepository.GetAuthorAsync(authorId);
            if (authorFromRepo == null)
            {
                return NotFound();
            }

            // map to DTO to apply the patch to
            var author = _mapper.Map<AuthorForUpdate>(authorFromRepo);
            patchDocument.ApplyTo(author, ModelState);

            // if there are errors when applying the patch the patch doc 
            // was badly formed  These aren't caught via the ApiController
            // validation, so we must manually check the modelstate and
            // potentially return these errors.
            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            // map the applied changes on the DTO back into the entity
            _mapper.Map(author, authorFromRepo);

            // update & save
            _authorsRepository.UpdateAuthor(authorFromRepo);
            await _authorsRepository.SaveChangesAsync();

            // return the author
            return Ok(_mapper.Map<Author>(authorFromRepo));
        }
    }
}
