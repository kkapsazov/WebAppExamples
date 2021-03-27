using GraphQL.Types;

namespace GraphQLExample.GraphQL
{
    public class CoreSchema : Schema
    {
        public CoreSchema(CoreDbContext dbContext)
        {
            this.Query = new Query(dbContext);
            //this.Mutation = new Mutation(dbContext);
        }
    }
}
