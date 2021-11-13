using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CicekSepeti.Core.Interfaces
{
    public partial interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        void Update(TEntity entity);

        void UpdateRange(List<TEntity> entities);
        TEntity Delete(TEntity entity);

        Task<IPaginate<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
          int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null);

        IQueryable<TEntity> GetQuery(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null);

        IQueryable<TEntity> Table { get; }

        Task AddRangeAsync(List<TEntity> entities);
        void RemoveRange(List<TEntity> entities);

    }
}
