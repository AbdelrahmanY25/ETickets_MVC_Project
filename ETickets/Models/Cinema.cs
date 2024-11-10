using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ETickets.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [ValidateNever]
        public string CinemaLogo { get; set; }
        public string Address  { get; set; }       

        [ValidateNever]
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
