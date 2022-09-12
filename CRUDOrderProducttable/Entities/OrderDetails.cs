namespace CRUDOrderProducttable.Entities
{
    public class OrderDetails
    {
        public int detailsId { get; set; }
       
        public new int? productId { get; set; }
        public string? productName { get; set; }
        public double productPrice { get; set; }
        public int quentity { get; set; }
        public double totalAmount { get; set; }

        public int orderId { get; set; }
    }
}