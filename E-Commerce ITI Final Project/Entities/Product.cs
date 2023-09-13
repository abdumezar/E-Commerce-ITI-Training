namespace E_Commerce_ITI_Final_Project.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Picture { get; set; }
        public int Quantity { get; set; } = 0;
        public string Description { get; set; } = "No Description";
        public string SellerId { get; set; }
        public virtual Seller Seller { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public DateTime? AddedOn { get; set; } = DateTime.Now;
        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
