using MediatR;
using Microsoft.AspNetCore.Mvc;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Application.Queries;

namespace TransactionAggregation.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{customerId}/transactions")]
    [Produces<IReadOnlyCollection<AggregatedTransactionResponse>>()]
    public async Task<IActionResult> GetTransactions(string customerId, CancellationToken cancellationToken)
    {
        var transactions = await _mediator.Send(new GetCustomerTransactionsQuery(customerId), cancellationToken);

        return Ok(transactions);
    }

    [HttpGet("{customerId}/summary")]
    [Produces<CustomerSummaryResponse>()]
    public async Task<IActionResult> GetSummary(string customerId, CancellationToken cancellationToken)
    {
        var summary = await _mediator.Send(new GetCustomerSummaryQuery(customerId), cancellationToken);

        return Ok(summary);
    }
}
