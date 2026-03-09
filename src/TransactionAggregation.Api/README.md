# Transaction Aggregation API

A simple layered .NET 8 Web API that:

- fetches customer transactions from multiple mock sources
- normalizes and categorizes them
- stores aggregated data in PostgreSQL
- runs periodic aggregation with Hangfire
- exposes a manual endpoint to trigger aggregation immediately

## Architecture

The project intentionally avoids CQRS and uses a simpler architecture:

- Controllers
- Services
- Repositories
- External source clients
- EF Core + PostgreSQL
- Hangfire background jobs

## Main endpoints

- `POST /api/aggregation/run` - queue the Hangfire aggregation job manually
- `GET /api/aggregation/runs` - list recent job runs
- `GET /api/aggregation/runs/{id}` - get a specific run
- `GET /api/transactions` - search transactions with filters
- `GET /api/transactions/{id}` - get transaction by id
- `GET /api/customers/{customerId}/transactions` - get customer transactions
- `GET /api/customers/{customerId}/summary` - get customer summary
- `GET /api/reports/categories/summary` - get category summary
- `GET /api/reports/transactions/summary` - get overall summary

## Quick start

### 1. Start PostgreSQL

Example Docker command:

```bash
docker run --name transaction-aggregation-postgres   -e POSTGRES_PASSWORD=postgres   -e POSTGRES_DB=transaction_aggregation   -p 5432:5432   -d postgres:16
```

### 2. Restore and run

```bash
dotnet restore
dotnet run --project src/TransactionAggregation.Api
```

### 3. Open Swagger

- `https://localhost:xxxx/swagger`
- Hangfire dashboard: `/hangfire`

## Notes

- The recurring Hangfire job runs every 15 minutes.
- The system deduplicates by `(Source, ExternalTransactionId)`.
- Categorization is rule-based for simplicity.
- `Database.Migrate()` is called on startup, so add an EF migration before first run.

## Add initial migration

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/TransactionAggregation.Api --startup-project src/TransactionAggregation.Api
dotnet ef database update --project src/TransactionAggregation.Api --startup-project src/TransactionAggregation.Api
```
