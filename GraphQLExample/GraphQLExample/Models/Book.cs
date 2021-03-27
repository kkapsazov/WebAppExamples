using System.ComponentModel.DataAnnotations;

namespace GraphQLExample.Models
{
    public class Book : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int AuthorId { get; set; }
        
        public virtual Author Author { get; set; }
    }
}
