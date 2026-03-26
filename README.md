## Architecture

A lightweight Event Sourcing implementation. The system processes a real-time stream of betting data via WebSockets, 
transforming raw data into Domain Events (FixtureEvent, BetPlacedEvent). The CustomerAggregate maintains state by applying these events in sequence.

## Components

1. Ingestion: `Services.WagerService` (BackgroundService) manages the WebSocket lifecycle and streams raw JSON.
2. Transformation: `Services.EventProducer` deserializes payloads into specific Domain Events in `Models.Events` (FixtureEvent, BetsPlacedEvent).
3. Messaging: Uses `Wolverine` as an in-memory bus to dispatch events to their respective handlers.
4. Persistence & Aggregate: EventHandlers persist events to the `Repositories.EventStore` and update aggregate from the `Repositories.CustomerRepository`.
5. API Layer: `The /customers/{id}/stats` endpoint utilizes `CustomerService` to orchestrate data between the local aggregate state and supplemental data from a remote `CustomerClient (External API)`.

## Domain

`Domain.CustomerAggregate` is the domain model, it can be built by applying events.

## What missing from the solution
  - Robust error handling and logging
  - Error handling and event retry can be implemented using persistent queues
  - Unit tests for the domain model and event handlers

