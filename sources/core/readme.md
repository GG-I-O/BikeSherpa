# Core application: API - Application domain and Infrastructure

## Getting started

## Database connection

To develop locally, you can use the local PostGresql database defined in docker compose file `infrastructure/dev`

Add user secrets name `ConnectionString` with expected value within the `./sources/core/Ggio.BikeSherpa.Backend` folder.

```json
{
  "ConnectionString": "Host=localhost;Port=5432;Database=<db_name>;Username=<user_name>;Password=<password>"
}
```

Add user secrets name `DesignTimeConnectionString` with expected value within the `./sources/core/Ggio.BikeSherpa.Backend.Infrastructure` folder.

```json
{
  "DesignConnectionString": "Host=localhost;Port=5432;Database=<db_name>;Username=<user_name>;Password=<password>"
}
```

## Authentication

To enable authentication with Auth0, use user secrets on Backend project.

```json
"Auth0Domain": "<domain>",
"Auth0Identifier": "<identifier>",
"Auth0Issuer": "<issuer>",
"Auth0Metadata": "<issuer>/.well-known/openid-configuration"
```

## Design concept

We use the Ardalis library but not a complete template.

### DDD Core: (called SharedKernel by Ardalis)

Define base interfaces and classes for domain entities and value objects.
Add Transactional and Post Transactional features to manage domain event handlers in order to manage domain events only if transaction in database is commited.

### Infrastructure

> **About IRepository Pattern**
We prefer managing additions via a factory and not re-wrapping a `DbSet<>` which is itself an implementation of this Pattern.

So a wrapper of Ardalis RepositoryBase implementing our IRepository interface is provided and should always be used.

### Application domain

We take as presented Ardalis library