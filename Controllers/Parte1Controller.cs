using Microsoft.AspNetCore.Mvc;
using ProvaPub.Services;

namespace ProvaPub.Controllers
{
	/// <summary>
	/// Ao rodar o código abaixo o serviço deveria sempre retornar um número diferente, mas ele fica retornando sempre o mesmo número.
	/// 1 - Faça as alterações para que o retorno seja sempre diferente
	/// 2 - Tome cuidado 
	/// </summary>
	[ApiController]
	[Route("[controller]")]
	public class Parte1Controller :  ControllerBase
	{
		private readonly RandomService _randomService;
		int maxValue = 100;

		public Parte1Controller(RandomService randomService)
		{
			_randomService = randomService;
		}
		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var number = await _randomService.GetRandom(maxValue);
			
			if(number is null)
				return BadRequest($"Não foi possível gerar um número único. Todos os números possíveis de 0 a {maxValue - 1} já foram gerados.");
			
			return Ok(number.Value);
		}
	}
}
