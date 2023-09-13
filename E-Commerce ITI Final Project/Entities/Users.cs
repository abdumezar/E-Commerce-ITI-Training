using Microsoft.AspNetCore.Identity;

namespace E_Commerce_ITI_Final_Project.Entities
{
    public class Account : IdentityUser
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string? ProfilePicture { get; set; }
        public string Address { get; set; }
        public bool Gender { get; set; }
    }

    public class Seller : Account
    {
        public ICollection<Product> Products { get; set; }
    }

    public class AppUser : Account
    {
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
