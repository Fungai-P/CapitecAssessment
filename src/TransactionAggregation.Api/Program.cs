using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TransactionAggregation.Api.Data;
using TransactionAggregation.Api.Infrastructure.Repositories;
using TransactionAggregation.Api.Infrastructure.Scheduled;
using TransactionAggregation.Api.Infrastructure.Scheduled.Jobs;
using TransactionAggregation.Api.Infrastructure.Services;
using TransactionAggregation.Api.Infrastructure.Services.ExternalSources.BankA;
using TransactionAggregation.Api.Infrastructure.Services.ExternalSources.BankB;
using TransactionAggregation.Api.Infrastructure.Services.ExternalSources.Card;
using TransactionAggregation.Api.Infrastructure.Services.ExternalSources.Wallet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=transaction_aggregation;Username=postgres;Password=postgres";

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

builder.Services.AddHangfireServer();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IAggregationRunRepository, AggregationRunRepository>();
builder.Services.AddScoped<ITransactionSourceClient, MockBankATransactionSourceClient>();
builder.Services.AddScoped<ITransactionSourceClient, MockBankBTransactionSourceClient>();
builder.Services.AddScoped<ITransactionSourceClient, MockWalletTransactionSourceClient>();
builder.Services.AddScoped<ITransactionSourceClient, MockCardTransactionSourceClient>();
builder.Services.AddScoped<ITransactionCategorizationService, TransactionCategorizationService>();
builder.Services.AddScoped<IAggregationService, AggregationService>();
builder.Services.AddScoped<ITransactionIdempotencyService, TransactionIdempotencyService>();
builder.Services.AddScoped<AggregateTransactionsJob>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

Scheduler.Run();

app.MapControllers();

app.Run();

public partial class Program;
