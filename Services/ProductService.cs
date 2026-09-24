using ProvaPub.Extensions;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
	public class ProductService
	{
		private readonly TestDbContext _ctx;
		
		public ProductService(TestDbContext ctx)
		{
			_ctx = ctx;
		}
		
		public PagedList<Product> ListProducts(int page)
		{
			return _ctx.Products
				.OrderBy(p => p.Id)
				.ToPagedList(page);
		}
	}
}