# Core application : API - Application domain and Infrastructure

## Getting started

## Database connection
To develop locally, you can use the local PostGresql database defined in docker compose file `infrastructure/dev`
Then add user secrets name `DesignTimeConnectionString` with value expected value wihtin the ` ./sources/core/Ggio.DddCore.Infrastructure` folder. 
```json 
{
  "DesignConnectionString": "Host=localhost;Port=5432;Database=<db_name>;Username=<user_name>;Password=<password>"
}
```

## Design concept

Use of Ardalis library but not complete template. 

### DDD Core : (called SharedKernel by Ardalis)

Define base interfaces and classes for domain entities and value objects.
Add Transactional flag for events

### Infrastructure : 

> **About IRepository Pattern**  
We prefer managing additions via a factory and not re-wrapping a `DbSet<>` which is itself an implementation of this Pattern.

So a wrapper of Ardalis RepositoryBase implemnting our IRepository interface is provided and shoulld be always used. 


 
### Application domain :
We take as presented Ardalis library



