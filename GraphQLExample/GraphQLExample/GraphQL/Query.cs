using GraphQL.Types;

namespace GraphQLExample.GraphQL
{
    public class Query : ObjectGraphType<object>
    {
        public Query(CoreDbContext dbContext)
        {
            BooksQuery.AddBooks(this, dbContext);
        }
    }
}
