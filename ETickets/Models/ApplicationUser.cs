using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ETickets.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City{ get; set; }

        [ValidateNever]
        public List<Movie> Movies { get; set; }
    }
}
