using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BlogCore.AccesoDatos.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _Context;
        internal DbSet<T> _Set;

        public Repository(DbContext context)
        {
            _Context = context;
            this._Set = context.Set<T>();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null)
        {
            IQueryable<T> query = _Set;
            if (filter != null) query = query.Where(filter);

            // Incluimos las propiedades de navegacion si son incluidas.
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' } , StringSplitOptions.RemoveEmptyEntries ))
                {
                    query = query.Include(includeProperty);
                }
            }

            return (orderBy != null)? orderBy(query).ToList() : query.ToList();
        }

        public void Add (T entity)
        {
            _Set.Add(entity);
        }

        public T? Get(int id)
        {
            return _Set.Find(id);
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = _Set;
            if (filter != null) query = query.Where(filter);

            // Incluimos las propiedades de navegacion si son incluidas.
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault();

        }

        public void Remove(int id)
        {
            T entityToRemove = _Set.Find(id);
            if (entityToRemove != null) Remove(entityToRemove);
        }

        public void Remove(T entity)
        {
            _Set.Remove(entity);
        }
    }
}
