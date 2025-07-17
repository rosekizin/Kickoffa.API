using Microsoft.AspNetCore.Mvc.Filters;

namespace Kickoffa.API.AspNet.Infrastructure.ExceptionHandling
{
	/// <summary>
	/// Exception filter para capturar lógica de exceção específica de domínio ou controller.
	/// </summary>
	public class ExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			/*
			if (context.Exception is DomainException domainEx)
			{
				context.Result = new BadRequestObjectResult(new
				{
					error = domainEx.Message
				});

				context.ExceptionHandled = true;
			}
			*/
		}
	}
}