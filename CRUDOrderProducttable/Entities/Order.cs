namespace CRUDOrderProducttable.Entities
{
    public class Order
    {
            public int orderId { get; set; }
            public int orderCode { get; set; }
            public string? custName { get; set; }
            public string? mobileNumber { get; set; }
            public string? shippingAddress { get; set; }
            public string? billingAddress { get; set; }
            public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
            public double totalAmount { get; set; }

     
    }
}
