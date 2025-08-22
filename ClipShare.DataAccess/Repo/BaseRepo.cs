using ClipShare.Core.Entities;
using ClipShare.Core.IRepo;
using ClipShare.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.DataAccess.Repo
{
    public class BaseRepo<T> : IBaseRepo<T> where T : BaseEntity
    {
        private readonly Context context;
        internal DbSet<T> contextSet;

        public BaseRepo(Context context)
        {
            this.context = context;
            contextSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            contextSet.Add(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> criteria)
        {
            IQueryable<T> query = contextSet;
            query = query.Where(criteria);
            return await query.AnyAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria = null)
        {
            IQueryable<T> query = contextSet;

            if (criteria != null)
            {
                query = query.Where(criteria);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> criteria = null, string includePrperties = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderedBy = null)
        {
            IQueryable<T> query = contextSet;

            if (criteria != null)
            {
                query = query.Where(criteria);
            }

            if (!string.IsNullOrEmpty(includePrperties))
            {
                query = GetQueryWithIncludedProperties(query, includePrperties);
            }

            if (orderedBy != null)
            {
                return await orderedBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id, string includePrperties = null)
        {
            IQueryable<T> query = contextSet;
            if (!string.IsNullOrEmpty(includePrperties))
            {
                query = GetQueryWithIncludedProperties(query, includePrperties);
            }

            return await query.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> criteria, string includePrperties = null)
        {
            IQueryable<T> query = contextSet;
            if (!string.IsNullOrEmpty(includePrperties))
            {
                query = GetQueryWithIncludedProperties(query, includePrperties);
            }

            return await query.Where(criteria).FirstOrDefaultAsync();
        }

        public void Remove(T entity)
        {
            contextSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            contextSet.RemoveRange(entities);
        }

        public void Update(T source, T destination)
        {
            contextSet.Entry(source).CurrentValues.SetValues(destination);
        }

        #region Static Methods
        public static IQueryable<T> GetQueryWithIncludedProperties(IQueryable<T> query, string includeProperties)
        {
            var props = includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var prop in props)
            {
                query = query.Include(prop);
            }

            return query;
        }
        #endregion
    }
}
