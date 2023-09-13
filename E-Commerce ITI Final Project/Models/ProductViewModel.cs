using System.ComponentModel.DataAnnotations;

namespace E_Commerce_ITI_Final_Project.Models
{
    public class ProductViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Price is Required")]
        public decimal Price { get; set; }
        public IFormFile? Picture { get; set; }
        [Required(ErrorMessage = "Quantity is Required")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; }
        public string? SellerId { get; set; }
        public int CategoryId { get; set; }
        public DateTime? AddedOn { get; set; }
    }
}
