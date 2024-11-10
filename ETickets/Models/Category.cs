using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ETickets.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ValidateNever]
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
