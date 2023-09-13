using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_ITI_Final_Project.Entities
{
    public class OrderProduct
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.OnCart;

        [NotMapped]
        public decimal SubTotal
        {
            get { return Quantity * Product.Price; }
        }
    }
}
