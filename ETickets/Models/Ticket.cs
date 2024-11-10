using ETickets.ViewModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ETickets.Models
{
    public class Ticket
    {
        public int MovieId { get; set; }
        public string ApplicationUserId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Tickets must be between 1 : 100 tickets as maximum")]
        public int BookedTickets { get; set; }

        [ValidateNever]
        public Movie Movie { get; set; }

        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
