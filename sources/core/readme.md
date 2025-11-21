# Core application : API - Application domain and Infrastructure

## Design concept

Use of Ardalis library but not complete template. 

### DDD Core : (called SharedKernel by Ardalis)

Define base interfaces and classes for domain entities and value objects.
Add Transactional flag for events

### Infrastructure : 

> **About IRepository Pattern**  
We prefer managing additions via a factory and not re-wrapping a `DbSet<>` which is itself an implementation of this Pattern.

So a wrapper of Ardalis RepositoryBase implemnting our IRepository interface is provided and shoulld be always used. 

We introduce a Application transactional scope patter. This pattern will wrap SaveChanges() method. And manage transactional and non transcactionnales actions on domain for example to manage messaging actions.

 //TODO : gérer dans l'application transcation le saveChangees la récupération des entités ayant des évènements et les envent handler non transactionnels
### Application domain :
We take as presented Ardalis library



