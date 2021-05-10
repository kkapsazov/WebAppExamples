using PhlatQL.Core.Types;
using RestExample.Models;

namespace RestExample.Responses
{
    public class AuthorType : ObjectPhlatType<Author>
    {
        public AuthorType()
        {
            this.Field(c => c.Name);
        }
    }
}
