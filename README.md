########
# ExpressYourself API

Technical assignment API for retrieving, caching, and maintaining IP address information, with reporting capabilities.

---

## Overview

This project provides a RESTful API that:

- Retrieves country information for a given IP address
- Caches responses for performance optimization
- Persists IP data in a relational database
- Automatically refreshes stored data using a background job
- Generates reports using raw SQL queries

The system was designed following clean architecture principles, emphasizing separation of concerns, testability, and maintainability.

---

## Architecture

The solution is organized into multiple projects:

- **ExpressYourself.Api**
  - Entry point
  - Controllers
  - Swagger configuration

- **ExpressYourself.Application**
  - Use cases (business orchestration)
  - Interfaces (contracts)
  - DTOs

- **ExpressYourself.Domain**
  - Entities
  - Value Objects
  - Enums

- **ExpressYourself.Infrastructure**
  - Database access (EF Core)
  - Raw SQL reporting
  - External provider integration (IP2C)
  - Caching (MemoryCache)
  - Background job

- **ExpressYourself.Tests**
  - Unit and integration tests

---

## Features

### 1. IP Information Retrieval

- Endpoint: `GET /api/IpInformation?ip={ip}`
- Flow:
  1. Check cache
  2. Check database
  3. Query external provider (IP2C)
  4. Persist result
  5. Cache response

---

### 2. Caching

- In-memory caching using `MemoryCache`
- Sliding + absolute expiration
- Cache invalidation when data changes

---

### 3. Background Job (Automation)

- Periodically refreshes stored IP data
- Runs as a hosted service
- Processes data in batches of 100
- Updates country information if changed
- Invalidates affected cache entries

Configuration via `appsettings.json`:

```json
"BackgroundJobs": {
  "RefreshIpInformation": {
    "Interval": "01:00:00"
  }
}
########