using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
	public class RandomService
	{
		private readonly TestDbContext _ctx;
        
		public RandomService(TestDbContext ctx)
		{
			_ctx = ctx;
		}
		
        public async Task<int?> GetRandom(int maxValue)
        {
	        int number;
	        
	        var usedNumbers = await _ctx.Numbers.Select(x => x.Number).ToListAsync();
	        
	        if (usedNumbers.Count() >= maxValue)
		        return null;
	        
	        do
	        {
		        number = GenerateRandomNumber(maxValue);
		        
	        } while (usedNumbers.Any(x => x == number));
	        
	        _ctx.Numbers.Add(new RandomNumber() { Number = number });
            await _ctx.SaveChangesAsync();
            
			return number;
		}

        private int GenerateRandomNumber(int maxValue)
        {
	        int seed = Guid.NewGuid().GetHashCode();
	        return new Random(seed).Next(maxValue);
        }
	}
}
