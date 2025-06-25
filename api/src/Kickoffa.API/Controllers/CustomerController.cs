using Kickoffa.API.Application.Services;
using Kickoffa.API.Contracts.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Kickoffa.API.Controllers;

/// <summary>
/// Controller para operações relacionadas a Customer
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<CustomerResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
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
}