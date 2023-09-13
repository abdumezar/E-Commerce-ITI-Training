using System.ComponentModel.DataAnnotations;

namespace E_Commerce_ITI_Final_Project.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LName { get; set; }
        [Required(ErrorMessage = "Address is Required")]
        [MinLength(3, ErrorMessage = "Min Length is 3 Chars")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [MinLength(6, ErrorMessage = "Min Length is 6 chars")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is Required")]
        [Compare("Password", ErrorMessage = "Confirm Password doesn't match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        [Phone]
        public string PhoneNumber { get; set; }
        public bool isSeller { get; set; } = false;

        [Required]
        public bool Gender { get; set; } = true;

        public IFormFile? ProfilePic { get; set; }
    }
}
