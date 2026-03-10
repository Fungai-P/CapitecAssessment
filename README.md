# Transaction Aggregation API

A demonstration system that aggregates financial transactions from
multiple sources, normalizes them into a common model, deduplicates
overlapping records, categorizes transactions, and stores them in
PostgreSQL.

The system is intentionally designed to be **simple but well
structured**, demonstrating architectural concepts often used in
production systems such as layered design, simplified CQRS, idempotent
processing, and background job scheduling.

# Overview

This application periodically collects transaction data from multiple
mock providers and aggregates the data into a single normalized dataset.

Key capabilities:

-   Aggregate transactions from heterogeneous sources
-   Normalize different formats into a common model
-   Detect and remove duplicate transactions
-   Guarantee idempotency across retries
-   Categorize transactions automatically
-   Provide query endpoints for retrieving aggregated data

A background job powered by **Hangfire** periodically runs the
aggregation process. The aggregation can also be triggered manually
through the API.

# How to Run the Application

## Prerequisites

-   .NET 8 SDK
-   Docker
-   Docker Compose

# Running PostgreSQL with Docker

The project includes a **docker-compose.yml** file that runs PostgreSQL
locally.

Start the database:

    docker compose up -d

Example database configuration:

    Host: localhost
    Port: 5432
    Database: transaction_aggregation
    Username: postgres
    Password: postgres

# Running EF Core Migrations

The application uses **Entity Framework Core migrations** to manage the
database schema.

Run migrations manually:

    dotnet ef database update

This ensures the schema is always up to date when the application
launches.

# Running the API

Start the application:

    dotnet run

The API automatically redirects the root URL to Swagger:

    http://localhost:60746/

Swagger UI allows exploration and testing of all endpoints.

Call the endpoint below to pull transaction data from the multiple sources and aggregate it into the database:
    
    api/aggregation/run

# Background Job Execution

Aggregation runs as a **Hangfire job**.

It executes periodically according to the scheduler configuration and
can also be triggered manually through the API.

Hangfire dashboard:

    http://localhost:60746/hangfire

The dashboard provides visibility into:

-   scheduled jobs
-   running jobs
-   failed jobs
-   retries

In production, access to this dashboard MUST be restricted using ASP.NET Core authentication/authorization. For the purposes of this demo, it has only bee restricted to the **development environment**.

# System Architecture

The architecture follows a **simplified CQRS-style design** used for
demonstration purposes.

The system separates:

-   **Commands (write operations)**
-   **Queries (read operations)**

However, it avoids the complexity of a full CQRS architecture such as
message buses or distributed services.

This keeps the implementation easy to understand while demonstrating the
architectural concepts.

# Project Structure

    API
      Requests
      Responses

    Controllers

    Data

    Application
      Commands
      Queries

    Handlers
      Mappers

    Domain
      Models
      Entities

    Infrastructure
      Services
      Persistence

# Layer Explanation

## Controllers

Controllers expose HTTP API endpoints.

Responsibilities:

-   Receive HTTP requests
-   Validate input
-   Dispatch commands or queries
-   Return responses

Controllers intentionally contain **no business logic**.

## API (Requests / Responses)

These represent:

-   request contracts from clients
-   response models returned by the API

They are kept separate from domain models to avoid coupling the API
contract to internal entities.

## Application Layer

The **Application layer** contains system use cases.

It is divided into:

    Commands
    Queries

### Commands

Commands represent operations that **change system state**.

Examples:

-   Run aggregation
-   Persist transactions

Commands typically result in database writes.

### Queries

Queries represent operations that **retrieve data**.

Examples:

-   Get transactions
-   Get customer summaries
-   Get category summaries

Queries never modify system state.

# Handlers

Handlers execute application logic.

Each command or query has a corresponding handler.

Examples:

    GetTransactionByIdHandler
    GetCustomerTransactionsHandler
    SearchTransactionHandler

Handlers coordinate:

-   repositories
-   domain models
-   services
-   mapping logic

# Mappers

Mappers convert data between layers:

    External Source Format
            ↓
    Normalized Source Model
            ↓
    Domain Entities
            ↓
    API Responses

This allows integration with multiple external data formats without
polluting domain models.

# Domain Layer

The domain layer contains the **core business models**.

It includes:

    Models
    Entities

### Entities

Entities represent persistent data stored in the database.

Examples:

-   AggregatedTransaction
-   AggregationRun

### Models

Models represent domain concepts used during processing.

Examples:

-   SourceTransaction
-   TransactionCategory

# Infrastructure Layer

Infrastructure contains integrations with external systems.

It includes:

    Services
    Persistence

## Services

Services implement domain functionality such as:

-   transaction categorization
-   idempotency logic
-   external transaction providers

Examples:

    TransactionCategorizationService
    TransactionIdempotencyService

## Persistence

Persistence is implemented through repositories.

Examples:

    TransactionRepository
    AggregationRunRepository

Repositories isolate database logic from the application layer.

# Mock Transaction Sources

The system simulates multiple external financial providers.

Each provider exposes transactions in a **different format**, which must
be normalized before aggregation.

Example sources:

-   Bank A
-   Bank B
-   Wallet provider
-   Card provider

Each source implements:

    ITransactionSourceClient

This abstraction allows additional providers to be added easily.

# Transaction Aggregation Pipeline

The aggregation pipeline performs the following steps:

    Fetch transactions from sources
           ↓
    Normalize data formats
           ↓
    Generate idempotency key
           ↓
    Deduplicate transactions
           ↓
    Categorize transactions
           ↓
    Persist aggregated results

The process runs inside **AggregationService**.

# Deduplication

Multiple providers may report the same transaction.

Example:

    BankB     Uber    -60
    Card      Uber    -60

These represent the same real-world payment.

Deduplication occurs by generating a **transaction fingerprint** based
on:

    CustomerId
    Amount
    Merchant
    Description
    Timestamp (minute precision)

Transactions with identical fingerprints are treated as the same logical
transaction.

# Idempotency

Idempotency ensures that running the aggregation job multiple times will
not create duplicate records.

Each aggregated transaction receives a deterministic **IdempotencyKey**.

The database enforces uniqueness using a **unique index** on this key.

Benefits:

-   Safe job retries
-   Safe concurrent executions
-   Safe manual re-runs
-   Safe recovery after failures

Even if the job runs multiple times, duplicates are prevented at the
database level.

# Testing

The project includes automated tests validating key system behavior.

Test coverage focuses on:

-   aggregation logic
-   deduplication behavior
-   categorization rules
-   query handlers

Tests verify that:

-   duplicate transactions are handled correctly
-   multiple providers reporting the same transaction do not create
    duplicates
-   aggregation runs are idempotent

The test suite ensures the core aggregation logic behaves reliably.

# Summary

This project demonstrates several important architectural patterns in a
simplified environment:

-   Layered architecture
-   Simplified CQRS pattern
-   Background job scheduling with Hangfire
-   Idempotent aggregation processing
-   Transaction deduplication across multiple providers
-   External source normalization
-   PostgreSQL persistence using EF Core

While simplified for demonstration purposes, the architecture closely
mirrors patterns used in real-world financial transaction aggregation
systems.
