using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ETickets.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio {  get; set; }

        [ValidateNever]
        public string ProfilePicture { get; set; }
        public string News { get; set; }

        [ValidateNever]
        public List<Movie> Movies { get; set; }

    }
}
