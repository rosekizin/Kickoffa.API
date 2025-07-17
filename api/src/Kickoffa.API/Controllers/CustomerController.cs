using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
using Kickoffa.API.Contracts.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kickoffa.API.Controllers;

/// <summary>
/// Controller para operações relacionadas a Customer
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class CustomerController : ControllerBase
{
	private readonly ICustomerService _customerService;
	private readonly IActionResultErrorHandler _actionResultErrorHandler;

	public CustomerController(ICustomerService customerService, IActionResultErrorHandler actionResultErrorHandler)
	{
		_customerService = customerService;
		_actionResultErrorHandler = actionResultErrorHandler;
	}

	/// <summary>
	/// Busca todos os customers
	/// </summary>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>Lista de customers</returns>
	[HttpGet]
	public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAllAsync(CancellationToken cancellationToken)
	{
		var customers = await _customerService.GetAllAsync(cancellationToken);
		return Ok(customers);
	}

	/// <summary>
	/// Busca um customer por ID
	/// </summary>
	/// <param name="id">ID do customer</param>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>Customer encontrado</returns>
	[HttpGet("{id:long}")]
	public async Task<ActionResult<CustomerResponse>> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
	{
		var customer = await _customerService.GetByIdAsync(id, cancellationToken);
		return customer is null ? NotFound() : Ok(customer);
	}

	/// <summary>
	/// Cria um novo customer
	/// </summary>
	/// <param name="request">Dados do customer a ser criado</param>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>Customer criado</returns>
	[HttpPost]
	public async Task<ActionResult<CustomerResponse>> CreateAsync([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
	{
		var response = await _customerService.CreateAsync(request, cancellationToken);
		return Created($"/api/customer/{response.Id}", response);
	}

	/// <summary>
	/// Atualiza um customer existente
	/// </summary>
	/// <param name="id">ID do customer</param>
	/// <param name="request">Dados atualizados do customer</param>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>Customer atualizado</returns>
	[HttpPut("{id:long}")]
	public async Task<ActionResult<CustomerResponse>> UpdateAsync([FromRoute] long id, [FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
	{
		var result = await _customerService.UpdateAsync(id, request, cancellationToken);

		if (result.IsFailure)
		{
			return (ActionResult)_actionResultErrorHandler.GetActionResultFromError(result.ErrorObject!);
		}

		return Ok(result.Value);
	}

	/// <summary>
	/// Remove um customer
	/// </summary>
	/// <param name="id">ID do customer</param>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>Confirmação de remoção</returns>
	[HttpDelete("{id:long}")]
	public async Task<ActionResult> DeleteAsync([FromRoute] long id, CancellationToken cancellationToken)
	{
		var deleted = await _customerService.DeleteAsync(id, cancellationToken);
		return deleted ? NoContent() : NotFound();
	}
}