using GraphQL.Types;
using GraphQLExample.Models;

namespace GraphQLExample.GraphQLTypes
{
    public class BookType : ObjectGraphType<Book>
    {
        public BookType()
        {
            this.Field(c => c.Id);
            this.Field(c => c.Name);
            this.Field<AuthorType>("author", resolve: c => c.Source.Author);
        }
    }
}
