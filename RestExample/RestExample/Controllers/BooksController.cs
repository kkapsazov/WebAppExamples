using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestExample.Models;
using RestExample.Responses;

namespace RestExample.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BooksController : ControllerBase
    {
        private readonly CoreDbContext dbContext;
        private readonly BookResponse response;

        public BooksController(CoreDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.response = new BookResponse();
        }

        [HttpGet]
        public IActionResult All()
        {
            List<Book> books = this.dbContext.Books.Include(x => x.Author).ToList();

            return this.Ok(this.response.Build(books));
        }
    }
}
