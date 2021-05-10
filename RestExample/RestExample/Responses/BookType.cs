using PhlatQL.Core.Types;
using RestExample.Models;

namespace RestExample.Responses
{
    public class BookType : ObjectPhlatType<Book>
    {
        public BookType()
        {
            this.Field(c => c.Name);
            this.Field<AuthorType>(c => c.Source.Author);
        }
    }
}
