using AutoMapper;
using Library.API.Attributes;
using Library.API.Constants;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.API.Controllers
{

    // 03/12/2022 10:25 pm - SSN - [20220312-2220] - [001] - M04-10 - Demo - Specifying the response body type with the Produces attribute
    [Produces(MediaTypesConstants.APPLICATION_JSON, MediaTypesConstants.APPLICATION_XML)]
    [Route("api/authors/{authorId}/books")]
    [ApiController]

    // 03/15/2022 04:33 pm - SSN - [20220315-1627] - [005] - M06-03 - Demo - Working with multiple OpenAPI specifications
    [ApiExplorerSettings(GroupName = "LibraryOpenApiSpecificationBooks")]

    // 03/12/2022 04:35 am - SSN - [20220312-0304] - [004] - M04-05 - Demo - Using API analyzers to improve the OpenAPI specification 
    // Removed.  Applied globally [20220312-0304] - [005] 
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public BooksController(
            IBookRepository bookRepository,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        [HttpGet()]
        // 03/12/2022 04:17 am - SSN - [20220312-0304] - [001] - M04-05 - Demo - Using API analyzers to improve the OpenAPI specification 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks(
        Guid authorId)
        {
            if (!await _authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var booksFromRepo = await _bookRepository.GetBooksAsync(authorId);
            return Ok(_mapper.Map<IEnumerable<Book>>(booksFromRepo));
        }

        // 03/12/2022 02:34 am - SSN - [20220312-0232] - [001] - M04-04 - Demo - Describing response types (status codes) with ProducesResponseType

        /// <summary>
        /// Get a book by author Id and book ID 
        /// </summary>
        /// <param name="authorId">The id of the author</param>
        /// <param name="bookId">THe id of the book</param>
        /// <returns>An ActionResuot of type Book</returns>
        /// <response code="200">Returns the request book</response>
        [HttpGet("{bookId}", Name = "GetBook")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        // 03/12/2022 02:41 am - SSN - [20220312-0232] - [002] - M04-04 - Demo - Describing response types (status codes) with ProducesResponseType
        // We can pass the type when using IActionResult instead of ActionResult(Book) for 200 result to show model example.

        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Book))]
        // public async Task<IActionResult> GetBook(

        // Best practice to use ActionResult<Book>
        // Best practice to use ActionResult<Book>
        // Best practice to use ActionResult<Book>






        // 03/13/2022 07:12 pm - SSN - [20220313-1902] - [002] - M05-03 - Demo - Supporting vendor-specific media types


        // 03/14/2022 09:52 pm - SSN - [20220314-2152] - [001] - Reviewing M05 - Solution for 406 on Add new book

        //[Produces(
        //    MediaTypesConstants.APPLICATION_JSON, 


        //    MediaTypesConstants.APPLICATION_XML,


        //            MediaTypesConstants.APPLICATION_VND_MARVIN_BOOK_JSON)]
        [Produces(

             MediaTypesConstants.APPLICATION_XML,             // 406 when missing XML
            MediaTypesConstants.APPLICATION_VND_MARVIN_BOOK_JSON)]







        // 03/14/2022 04:14 am - SSN - [20220314-0111] - [007] - M05-08 - Demo - Supporting schema variation by media type (Input)
        // Todo: Did we mess up? Accept vs Content-Type?
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            MediaTypesConstants.APPLICATION_JSON,

            MediaTypesConstants.APPLICATION_XML,

            MediaTypesConstants.APPLICATION_VND_MARVIN_BOOK_JSON)]

        //[RequestHeaderMatchesMediaType(HeaderNames.ContentType, MediaTypesConstants.APPLICATION_JSON, MediaTypesConstants.APPLICATION_VND_MARVIN_BOOK_JSON)]
        public async Task<ActionResult<Book>> GetBook(
            Guid authorId,
            Guid bookId)
        {
            // End [20220312-0232] - [002] 



            if (!await _authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = await _bookRepository.GetBookAsync(authorId, bookId);
            if (bookFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<Book>(bookFromRepo));
        }




        // 03/13/2022 07:08 pm - SSN - [20220313-1902] - [001] - M05-03 - Demo - Supporting vendor-specific media types


        /// <summary>
        /// Get a book by author Id and book ID 
        /// </summary>
        /// <param name="authorId">The id of the author</param>
        /// <param name="bookId">THe id of the book</param>
        /// <returns>An ActionResuot of type BookWithConcatenatedAuthorName</returns>
        /// <response code="200">Returns the request book</response>
        [HttpGet("{bookId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]



        // 03/13/2022 07:15 pm - SSN - [20220313-1902] - [003] - M05-03 - Demo - Supporting vendor-specific media types

        // 03/14/2022 10:01 pm - SSN - [20220314-2152] - [002] - Reviewing M05 - Solution for 406 on Add new book


        //[Produces(MediaTypesConstants.APPLICATION_VND_MARVIN_BookWithConcatenatedAuthorName_JSON)]
        [Produces(MediaTypesConstants.APPLICATION_VND_MARVIN_BookWithConcatenatedAuthorName_JSON)]




        [RequestHeaderMatchesMediaType(HeaderNames.Accept, MediaTypesConstants.APPLICATION_VND_MARVIN_BookWithConcatenatedAuthorName_JSON)]

        // 03/14/2022 12:42 am - SSN - [20220313-2050] - [005] - M05-06 - Demo - Supporting schema variation by media type (Output IOperationFilter))
        // Prevents Conflict.  Need for ResolveConflictingActions.

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<BookWithConcatenatedAuthorName>> GetBookWithConcatenatedAuthorName(
            Guid authorId,
            Guid bookId)
        {

            if (!await _authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = await _bookRepository.GetBookAsync(authorId, bookId);
            if (bookFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BookWithConcatenatedAuthorName>(bookFromRepo));
        }





        [HttpPost()]
        // 03/12/2022 04:23 am - SSN - [20220312-0304] - [002] - M04-05 - Demo - Using API analyzers to improve the OpenAPI specification 
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesDefaultResponseType]

        // 03/12/2022 11:26 pm - SSN - [20220312-2256] - [001] - M04-11 - Demo - Specifying the request body type with the Consumes attribute
        // Defaults before adding were:
        // application/json-patch+json
        // application/json
        // text/json
        // application/*+json



        [Consumes(MediaTypesConstants.APPLICATION_JSON,
        // 03/14/2022 01:19 am - SSN - [20220314-0111] - [002] - M05-08 - Demo - Supporting schema variation by media type (Input)
                        MediaTypesConstants.APPLICATION_VND_MARVIN_BookForCreation_JSON)]

        // 03/14/2022 11:10 pm - SSN - [20220314-2152] - [003] - Reviewing M05 - Solution for 406 on Add new book
        // First RequestHeaderType was set to Accept instead of ContentType
        [RequestHeaderMatchesMediaType(HeaderNames.ContentType, MediaTypesConstants.APPLICATION_JSON, MediaTypesConstants.APPLICATION_VND_MARVIN_BookForCreation_JSON)]


        public async Task<ActionResult<Book>> CreateBook(
            Guid authorId,
            [FromBody] BookForCreation bookForCreation)
        {
            if (!await _authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookToAdd = _mapper.Map<Entities.Book>(bookForCreation);

            bookToAdd.AuthorId = authorId;

            _bookRepository.AddBook(bookToAdd);
            try
            {
                await _bookRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;

            }

            return CreatedAtRoute(
                "GetBook",
                new { authorId, bookId = bookToAdd.Id },
                _mapper.Map<Book>(bookToAdd));
        }






        // 03/14/2022 01:13 am - SSN - [20220314-0111] - [001] - M05-08 - Demo - Supporting schema variation by media type (Input)

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary))]
        [ProducesDefaultResponseType]


        [Consumes(

            // 03/14/2022 11:48 pm - SSN - [20220314-2152] - [004] - Reviewing M05 - Solution for 406 on Add new book
            // *** This is needed. otherwise 404.
            MediaTypesConstants.APPLICATION_JSON,

            MediaTypesConstants.APPLICATION_VND_MARVIN_BookForCreationWithAmountOfPages_JSON)]

        // 03/14/2022 11:10 pm - SSN - [20220314-2152] - [003] - Reviewing M05 - Solution for 406 on Add new book
        // First RequestHeaderType was set to Accept instead of ContentType
        [RequestHeaderMatchesMediaType(HeaderNames.ContentType, MediaTypesConstants.APPLICATION_VND_MARVIN_BookForCreationWithAmountOfPages_JSON)]


        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<Book>> CreateBookWithAmountOfPages(
            Guid authorId,
            [FromBody] BookForCreationWithAmountOfPages bookForCreation)
        {
            if (!await _authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookToAdd = _mapper.Map<Entities.Book>(bookForCreation);


            bookToAdd.AuthorId = authorId;


            _bookRepository.AddBook(bookToAdd);
            await _bookRepository.SaveChangesAsync();

            return CreatedAtRoute(
                "GetBook",
                new { authorId, bookId = bookToAdd.Id },
                _mapper.Map<Book>(bookToAdd));
        }

    }
}
