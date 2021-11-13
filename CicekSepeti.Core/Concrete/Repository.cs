using CicekSepeti.Core.Extensions;
using CicekSepeti.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CicekSepeti.Core.Concrete
{
    public partial class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {

        private readonly DbContext _context;
        private DbSet<TEntity> _entities;
        public Repository(DbContext context)
        {
            _context = context;
        }
        public TEntity Delete(TEntity entity)
        {
            var entry = Entities.Remove(entity);
            return entry.Entity;
        }


        public virtual async Task<IPaginate<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
             int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = func != null ? func(Table) : Table;

            return await query.ToPaginateAsync(pageIndex, pageSize);
        }

        public virtual async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null)
        {
            var query = func != null ? func(Table) : Table;
            return await query.ToListAsync();
        }

        public virtual IQueryable<TEntity> GetQuery(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null)
        {
            var query = func != null ? func(Table) : Table;
            return query;
        }

        public virtual async Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await Entities.FindAsync(id);
        }
        
        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await Entities.AddAsync(entity);
            return result.Entity;
        }

        public virtual void Update(TEntity entity)
        {
            foreach (var property in _context.Entry(entity).Properties)
            {
                if (property.OriginalValue != null && !property.OriginalValue.Equals(property.CurrentValue))
                    property.IsModified = true;
            }

            //Entities.Update(entity);
        }

        public virtual void UpdateRange(List<TEntity> entities)
        {
            Entities.UpdateRange(entities);
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
        }

        public void RemoveRange(List<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        protected virtual DbSet<TEntity> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<TEntity>();
                return _entities;

            }
        }

        public virtual IQueryable<TEntity> Table => Entities;


    }
}
