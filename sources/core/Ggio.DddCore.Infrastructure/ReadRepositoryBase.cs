using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ggio.DddCore.Infrastructure;

public abstract class ReadRepositoryBase<TEntity> : RepositoryBase<TEntity>, IReadRepository<TEntity> where TEntity : class, IAggregateRoot
{
     protected ReadRepositoryBase(DbContext dbContext) : base(dbContext)
     {
     }

     protected ReadRepositoryBase(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
     {
     }

     public new Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
     {
          return base.FirstOrDefaultAsync(specification, cancellationToken);
     }

     public new Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
     {
          return base.FirstOrDefaultAsync(specification, cancellationToken);
     }

     public new Task<TEntity?> SingleOrDefaultAsync(ISingleResultSpecification<TEntity> specification, CancellationToken cancellationToken = default)
     {
          return base.SingleOrDefaultAsync(specification, cancellationToken);
     }

     public new Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
     {
          return base.SingleOrDefaultAsync(specification, cancellationToken);
     }

     public new Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
     {
          return base.ListAsync(cancellationToken);
     }

     public new Task<List<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
     {
          return base.ListAsync(specification, cancellationToken);
     }

     public new Task<List<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
     {
          return base.ListAsync(specification, cancellationToken);
     }

     public new Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
     {
          return base.CountAsync(specification, cancellationToken);
     }

     public new Task<int> CountAsync(CancellationToken cancellationToken = default)
     {
          return base.CountAsync(cancellationToken);
     }

     public new Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
     {
          return base.AnyAsync(specification, cancellationToken);
     }

     public new Task<bool> AnyAsync(CancellationToken cancellationToken = default)
     {
          return base.AnyAsync(cancellationToken);
     }

     public new IAsyncEnumerable<TEntity> AsAsyncEnumerable(ISpecification<TEntity> specification)
     {
          return base.AsAsyncEnumerable(specification);
     }
}
