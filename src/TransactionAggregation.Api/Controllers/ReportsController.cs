using MediatR;
using Microsoft.AspNetCore.Mvc;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Application.Queries;

namespace TransactionAggregation.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("categories/summary")]
    [Produces<TransactionSummaryResponse>()]
    public async Task<IActionResult> GetCategorySummary(CancellationToken cancellationToken)
    {
        var summary = await _mediator.Send(new GetCategorySummaryQuery(), cancellationToken);

        return Ok(summary);
    }

    [HttpGet("transactions/summary")]
    [Produces<TransactionSummaryResponse>()]
    public async Task<IActionResult> GetTransactionSummary(CancellationToken cancellationToken)
    {
        var summary = await _mediator.Send(new GetOverallSummaryQuery(), cancellationToken);

        return Ok(summary);
    }
}
