using GraphQL.Types;

namespace GraphQLExample.GraphQL
{
    public class Mutation : ObjectGraphType<object>
    {
        public Mutation(CoreDbContext dbContext)
        {

        }
    }
}
