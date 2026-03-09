using TransactionAggregation.Api.Domain.External;

namespace TransactionAggregation.Api.Infrastructure.Services;

public interface ITransactionIdempotencyService
{
    string Build(SourceTransaction transaction);
}
