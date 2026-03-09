using TransactionAggregation.Api.Domain.External;

namespace TransactionAggregation.Api.Infrastructure.Services;

public interface ITransactionSourceClient
{
    Task<List<SourceTransaction>> GetTransactionsAsync(CancellationToken cancellationToken = default);
}
