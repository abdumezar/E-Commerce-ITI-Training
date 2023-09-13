namespace E_Commerce_ITI_Final_Project.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

        public OrderStatus GetStatus()
        {
            if (OrderProducts.ToList().Any(op => op.Status == OrderStatus.OnCart))
                return OrderStatus.OnCart;
            else if (OrderProducts.ToList().All(op => op.Status == OrderStatus.Shipped))
                return OrderStatus.Shipped;
            else if (OrderProducts.ToList().Any(op => op.Status == OrderStatus.Purchased))
                return OrderStatus.Purchased;
            else
                return OrderStatus.OnCart;
        }

        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
        public decimal Total
        {
            get
            {
                return OrderProducts.Sum(op => op.SubTotal);
            }
        }
    }

    public enum OrderStatus
    {
        OnCart,
        Purchased,
        Shipped,
        Pending
    }
}
