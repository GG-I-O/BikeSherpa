using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ggio.DddCore.Infrastructure.Persistence;

public class EfCoreReadRepository<TEntity> : RepositoryBase<TEntity>, IReadRepository<TEntity> where TEntity : class, IAggregateRoot
{
     public EfCoreReadRepository(DbContext dbContext) : base(dbContext)
     {
     }

     public EfCoreReadRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
     {
     }

     public new Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) => base.FirstOrDefaultAsync(specification, cancellationToken);

     public new Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default) => base.FirstOrDefaultAsync(specification, cancellationToken);

     public new Task<TEntity?> SingleOrDefaultAsync(ISingleResultSpecification<TEntity> specification, CancellationToken cancellationToken = default) => base.SingleOrDefaultAsync(specification, cancellationToken);

     public new Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default) => base.SingleOrDefaultAsync(specification, cancellationToken);

     public new Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default) => base.ListAsync(cancellationToken);

     public new Task<List<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) => base.ListAsync(specification, cancellationToken);

     public new Task<List<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default) => base.ListAsync(specification, cancellationToken);

     public new Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) => base.CountAsync(specification, cancellationToken);

     public new Task<int> CountAsync(CancellationToken cancellationToken = default) => base.CountAsync(cancellationToken);

     public new Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) => base.AnyAsync(specification, cancellationToken);

     public new Task<bool> AnyAsync(CancellationToken cancellationToken = default) => base.AnyAsync(cancellationToken);

     public new IAsyncEnumerable<TEntity> AsAsyncEnumerable(ISpecification<TEntity> specification) => base.AsAsyncEnumerable(specification);
}
