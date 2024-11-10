using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ETickets.Models
{
    public class UsersOrders
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = null!;
        public string ApplicationName { get; set; }
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public double MoviePrice { get; set; }
        public int NumberOfTickets { get; set; }
        public double OrderPrice { get; set; }

        [ValidateNever]
        public Movie Movie { get; set; }

        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
