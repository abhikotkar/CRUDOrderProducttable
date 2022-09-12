using CRUDOrderProducttable.Context;
using CRUDOrderProducttable.Entities;
using CRUDOrderProducttable.Repositories.Interfaces;
using Dapper;
using System.Data;

namespace CRUDOrderProducttable.Repositories
{
    
        public class OrderRepository : IOrderRepository
        {
            private readonly DapperContext _context;

            public OrderRepository(DapperContext context)
            {
                _context = context;
            }

        public async Task<Order> GetOrderById(int orderId)
        {
            Order order = new Order();

            using (var connection = _context.CreateConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@orderId", orderId);
                order = connection.Query<Order>("SP_SelectById_DMartBill", dynamicParameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                order.OrderDetails = connection.Query<OrderDetails>("SP_SelectById_ODetails", dynamicParameters, commandType: CommandType.StoredProcedure).ToList();

            }
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            List<Order> orders = new List<Order>();
            Order order = new Order();
            using (var connection = _context.CreateConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                orders = connection.Query<Order>("SP_SelectAll_DMartBill", commandType: CommandType.StoredProcedure).ToList();
                foreach (var item in orders)
                {
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@orderId", item.orderId);
                    item.OrderDetails = connection.Query<OrderDetails>("SP_SelectById_ODetails", dynamicParameters, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            return orders;
        }

        public async Task<int> PlaceOrder(Order order)
        {
            int result = 0;
            using (var connection = _context.CreateConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@orderCode", order.orderCode);
                dynamicParameters.Add("@custName", order.custName);
                dynamicParameters.Add("@mobileNumber", order.mobileNumber);
                dynamicParameters.Add("@shippingAddress", order.shippingAddress);
                dynamicParameters.Add("@billingAddress", order.billingAddress);
                List<OrderDetails> odlist = new List<OrderDetails>();
                odlist = order.OrderDetails.ToList();
                result = await connection.QuerySingleAsync<int>("SP_Place_DMartBill", dynamicParameters, commandType: CommandType.StoredProcedure);
                double result1 = await AddProduct(odlist, result);
                DynamicParameters dynamicParameters1 = new DynamicParameters();
                dynamicParameters1.Add("@orderId", result);
                dynamicParameters1.Add("@totalAmount", result1);
                var result2 = await connection.ExecuteAsync("SP_UP_DMartBill", dynamicParameters1, commandType: CommandType.StoredProcedure);

            }
            return result;
        }


        public async Task<double> AddProduct(List<OrderDetails> orders, int orderId)
        {
            //List<OrderDetails> orderss = new List<OrderDetails>();
            double grandtotal = 0;
            using (var connection = _context.CreateConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                foreach (OrderDetails order in orders)
                {
                    order.orderId = orderId;
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@productId", order.productId);
                    dynamicParameters.Add("@quentity", order.quentity);
                    dynamicParameters.Add("@orderId", order.orderId);
                    int result = await connection.QuerySingleAsync<int>("SP_Add_DMartProduct", dynamicParameters, commandType: CommandType.StoredProcedure);
                    DynamicParameters dynamicParameters2 = new DynamicParameters();
                    dynamicParameters2.Add("@productId", order.productId);
                    orders = connection.Query<OrderDetails>("SP_Fetch_ProductPrice", dynamicParameters2, commandType: CommandType.StoredProcedure).ToList();
                    order.productPrice = orders[0].productPrice;
                    order.productName=orders[0].productName;
                    
                    order.totalAmount = order.productPrice * order.quentity;
                    grandtotal = grandtotal + order.totalAmount;
                    DynamicParameters dynamicParameters1 = new DynamicParameters();
                    dynamicParameters1.Add("@detailsId", result);
                    dynamicParameters1.Add("@totalAmount", order.totalAmount);
                    result = await connection.ExecuteAsync("SP_UP_ProductDetails", dynamicParameters1, commandType: CommandType.StoredProcedure);

                }
            }
            return grandtotal;
        }

        public async Task<int> UpdateOrder(Order order)
        {
            int result = 0;
            using (var connection = _context.CreateConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@orderId", order.orderId);
                dynamicParameters.Add("@orderCode", order.orderCode);
                dynamicParameters.Add("@custName", order.custName);
                dynamicParameters.Add("@mobileNumber", order.mobileNumber);
                dynamicParameters.Add("@shippingAddress", order.shippingAddress);
                dynamicParameters.Add("@billingAddress", order.billingAddress);
                List<OrderDetails> odlist = new List<OrderDetails>();
                odlist = order.OrderDetails.ToList();
                result = connection.Execute("SP_Update_DMartBill", dynamicParameters, commandType: CommandType.StoredProcedure);
                double result1 = await AddProduct(odlist, order.orderId);
                DynamicParameters dynamicParameters1 = new DynamicParameters();
                dynamicParameters1.Add("@orderId", order.orderId);
                dynamicParameters1.Add("@totalAmount", result1);
                var result2 = await connection.ExecuteAsync("SP_UP_DMartBill", dynamicParameters1, commandType: CommandType.StoredProcedure);
                result = order.orderId;
            }
            return result;
        }

        public async Task<int> Delete(int id)
        {
            int result = 0;
            using (var connection = _context.CreateConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@orderId", id);
                result = connection.Execute("SP_Delete_DMartBill", dynamicParameters, commandType: CommandType.StoredProcedure);
            }
            return result;
        }
    }
}
