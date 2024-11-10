using ETickets.Data.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ETickets.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required, MinLength(2, ErrorMessage = "The Shortest movie name must be from 2 letters")]
        public string Name { get; set; }

        [Required, MinLength(2, ErrorMessage = "The Shortest movie name must be from 2 letters")]
        public string Description { get; set; }

        [Range(1, 1000)]
        public double Price { get; set; }

        [ValidateNever]
        public string ImgUrl { get; set; }
        public string TrailerUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public MovieStatus MovieStatus { get; set; }
        [Range(0, 100)]
        public int NumberOfTickets { get; set; } = 100;
        public int CinemaId { get; set; }
        public int CategoryId { get; set; }

        [ValidateNever]
        public Cinema Cinema { get; set; }

        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public List<Actor> Actors { get; set; }
        
        [ValidateNever]
        public List<ApplicationUser> ApplicationUsers { get; set; }
    }
}
