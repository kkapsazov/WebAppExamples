using System.ComponentModel.DataAnnotations;

namespace RestExample.Models
{
    public class Author : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
