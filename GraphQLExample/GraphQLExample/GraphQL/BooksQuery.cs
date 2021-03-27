using System.Linq;
using GraphQL;
using GraphQL.Types;
using GraphQLExample.GraphQLTypes;
using GraphQLExample.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphQLExample.GraphQL
{
    public static class BooksQuery
    {
        public static void AddBooks(Query query, CoreDbContext dbContext)
        {
            query.Connection<BookType>()
                .Name("booksPaged")
                .PageSize(10)
                .Bidirectional()
                .Resolve(context =>
                {
                    return context.GetPaged(dbContext.Books.Include(x => x.Author));
                });

            query.Field<ListGraphType<BookType>>(
                "books",
                resolve: context =>
                {
                    return dbContext.Books.Include(x => x.Author);
                });

            query.Field<BookType>(
                "book",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "id",
                    Description = "id of the book"
                }),
                resolve: context =>
                {
                    int id = context.GetArgument<int>("id");
                    Book? book = dbContext.Books.FirstOrDefault(x => x.Id == id);
                    return book;
                }
            );
        }
    }
}
