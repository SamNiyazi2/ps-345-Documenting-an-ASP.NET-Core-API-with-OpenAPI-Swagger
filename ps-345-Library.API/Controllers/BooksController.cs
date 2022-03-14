using AutoMapper;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.API.Controllers
{

    // 03/12/2022 10:25 pm - SSN - [20220312-2220] - [001] - M04-10 - Demo - Specifying the response body type with the Produces attribute
    [Produces("application/json", "application/xml")]
    [Route("api/authors/{authorId}/books")]
    [ApiController]
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
        [HttpGet("{bookId}")]
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


        [HttpPost()]
        // 03/12/2022 04:23 am - SSN - [20220312-0304] - [002] - M04-05 - Demo - Using API analyzers to improve the OpenAPI specification 
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]

        // 03/12/2022 11:26 pm - SSN - [20220312-2256] - [001] - M04-11 - Demo - Specifying the request body type with the Consumes attribute
        // Defaults before adding were:
        // application/json-patch+json
        // application/json
        // text/json
        // application/*+json
        [Consumes("application/json")]


        public async Task<ActionResult<Book>> CreateBook(
            Guid authorId,
            [FromBody] BookForCreation bookForCreation)
        {
            if (!await _authorRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookToAdd = _mapper.Map<Entities.Book>(bookForCreation);
            _bookRepository.AddBook(bookToAdd);
            await _bookRepository.SaveChangesAsync();

            return CreatedAtRoute(
                "GetBook",
                new { authorId, bookId = bookToAdd.Id },
                _mapper.Map<Book>(bookToAdd));
        }
    }
}
