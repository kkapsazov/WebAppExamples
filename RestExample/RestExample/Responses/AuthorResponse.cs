using RestExample.Models;
using RestExample.Responses.Base;

namespace RestExample.Responses
{
    public class AuthorResponse : BaseResponse<Author>
    {
        public AuthorResponse()
        {
            this.Field(c => c.Name);
        }
    }
}
