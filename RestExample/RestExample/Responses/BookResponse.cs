using RestExample.Models;
using RestExample.Responses.Base;

namespace RestExample.Responses
{
    public class BookResponse : BaseResponse<Book>
    {
        public BookResponse()
        {
            this.Field(c => c.Name);
            this.Field<AuthorResponse>(c => c.Source.Author);
        }
    }
}
