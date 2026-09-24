using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services.Payments;

namespace ProvaPub.Services
{
	public class OrderService
	{
        private readonly TestDbContext _ctx;
        private readonly Dictionary<string, IPaymentMethod> _paymentMethods;
        
        public OrderService(TestDbContext ctx, IEnumerable<IPaymentMethod> paymentMethods)
        {
	        _ctx = ctx;
	        _paymentMethods = paymentMethods.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
		{
			if (!_paymentMethods.TryGetValue(paymentMethod, out var payment))
				throw new ArgumentException($"Forma de pagamento '{paymentMethod}' não suportada.");

			await payment.PayAsync(paymentValue, customerId);

			return await InsertOrder(new Order() //Retorna o pedido para o controller
            {
                Value = paymentValue,
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow
            });
		}

		private async Task<Order> InsertOrder(Order order)
        {
			await _ctx.Orders.AddAsync(order);
			await _ctx.SaveChangesAsync();
			return order;
        }
	}
}