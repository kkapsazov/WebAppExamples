using GraphQL.Types;
using GraphQLExample.Models;

namespace GraphQLExample.GraphQLTypes
{
    public class AuthorType : ObjectGraphType<Author>
    {
        public AuthorType()
        {
            this.Field(c => c.Id);
            this.Field(c => c.Name);
        }
    }
}
