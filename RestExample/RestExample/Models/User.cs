using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestExample.Models
{
    public class User : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

        public virtual List<Book> Books { get; set; }
    }
}
