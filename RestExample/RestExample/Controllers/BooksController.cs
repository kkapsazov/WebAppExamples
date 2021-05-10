using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhlatQL.Core.Validation;
using RestExample.Models;
using RestExample.Requests;
using RestExample.Responses;

namespace RestExample.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BooksController : ControllerBase
    {
        private readonly CoreDbContext dbContext;
        private readonly BookType type;

        public BooksController(CoreDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.type = new BookType();
        }

        [HttpGet]
        public IActionResult All()
        {
            List<Book> books = this.dbContext.Books.Include(x => x.Author).ToList();

            return this.Ok(this.type.Build(books));
        }

        [HttpPost]
        public IActionResult Create([FromBody] BookCreateInput<Book> patch)
        {
            Book entity = new();
            patch.ApplyTo(entity);

            ErrorResponse errors = patch.Validate(entity);
            if (errors.Any())
            {
                return this.BadRequest(errors);
            }

            this.dbContext.Books.Add(entity);
            this.dbContext.SaveChanges();

            return this.Ok(this.type.Build(entity));
        }

        [HttpPatch]
        public IActionResult Update([FromQuery] int id, [FromBody] BookInput<Book> patch)
        {
            Book entity = this.dbContext.Books.Include(x => x.Author).FirstOrDefault(x => x.Id == id);
            if (entity is null)
            {
                return this.NotFound();
            }

            ErrorResponse errors = patch.Validate(entity);
            if (errors.Any())
            {
                return this.BadRequest(errors);
            }

            patch.ApplyTo(entity);

            this.dbContext.Update(entity);
            this.dbContext.SaveChanges();

            return this.Ok(this.type.Build(entity));
        }
    }
}
