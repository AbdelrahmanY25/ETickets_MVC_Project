using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModel
{
    public class ApplicationUserlVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "There is no matchis with Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string City { get; set; }
    }
}
