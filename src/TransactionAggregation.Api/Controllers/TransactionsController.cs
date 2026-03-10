using MediatR;
using Microsoft.AspNetCore.Mvc;
using TransactionAggregation.Api.API.Requests;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Application.Handlers.Mappers;
using TransactionAggregation.Api.Application.Queries;

namespace TransactionAggregation.Api.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Produces<PagedResult<AggregatedTransactionResponse>>()]
    public async Task<IActionResult> Search([FromQuery] TransactionSearchRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request.Map(), cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Produces<AggregatedTransactionResponse>()]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var transaction = await _mediator.Send( new GetTransactionByIdQuery(id), cancellationToken);

        return transaction is null ? NotFound() : Ok(transaction);
    }
}
