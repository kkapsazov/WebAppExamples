using System.Collections.Generic;
using System.Security.Claims;
using GraphQL.Authorization;

namespace GraphQLExample.GraphQL
{
    public class GraphQLUserContext : Dictionary<string, object>, IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }
    }
}
