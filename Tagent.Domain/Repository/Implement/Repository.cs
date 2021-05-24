using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using Tagent.Domain.Database;
using Tagent.Domain.Repository.Define;

namespace Tagent.Domain.Repository.Implement
{
    public class Repository<T> : IRepository<T> where T :class
    {
        private readonly TagentDBContext _dbcontext;
        private readonly DbSet<T> _dbSet;

        public Repository(TagentDBContext dbcontext)
        {
            _dbcontext = dbcontext;
            _dbSet = _dbcontext.Set<T>();
        }

        public void Create(T _entity)
        {
            var entity = _dbcontext.Add(_entity);
        }
        public void DeleteById(int _id)
        {
            var entity = _dbSet.Find(_id);
            _dbcontext.Remove(entity);
        }

        public IQueryable<T> GetEntities(Expression<Func<T, bool>> expression = null, int numpage = 1, int perpage = 0, params string[] include)
        {
            if(numpage  ==0 && perpage ==0)
            {
                if (expression == null)
                {
                    if (include == null)
                    {
                        return _dbSet;
                    }
                    else
                    {
                        IQueryable<T> query = this._dbSet;
                        query = include.Aggregate(query, (currnet, inc) => currnet.Include(inc));
                        return query;
                    }
                }
                else
                {
                    if (include == null)
                    {
                        return _dbSet.Where(expression);
                    }
                    else
                    {
                        IQueryable<T> query = this._dbSet;
                        query = include.Aggregate(query, (currnet, inc) => currnet.Include(inc));
                        return query.Where(expression);
                    }
                }
            }
            else
            {

                if (expression == null)
                {
                    if (include == null)
                    {
                        return _dbSet.Skip((numpage - 1) * perpage).Take(perpage);
                    }
                    else
                    {
                        IQueryable<T> query = this._dbSet;
                        query = include.Aggregate(query, (currnet, inc) => currnet.Include(inc));
                        return query.Skip((numpage - 1) * perpage).Take(perpage);
                    }
                }
                else
                { 
                    if (include == null)
                    {
                        return _dbSet.Where(expression).Skip((numpage - 1) * perpage).Take(perpage);
                    }
                    else
                    {
                        IQueryable<T> query = this._dbSet;
                        query = include.Aggregate(query, (currnet, inc) => currnet.Include(inc));
                        return query.Where(expression).Skip((numpage - 1) * perpage).Take(perpage);
                    }
                }
            }
        }

        public T Get(Expression<Func<T, bool>> expression, string[] include = null)
        {
            if(include == null)
            {
                return _dbSet.Where(expression).FirstOrDefault();
            }
            else
            {
                IQueryable<T> query = this._dbSet;
                query = include.Aggregate(query, (currnet, inc) => currnet.Include(inc));
                return query.Where(expression).FirstOrDefault();
            }
        }

        public void Update(T _entity)
        {
            if (_dbcontext.Entry<T>(_entity).State == EntityState.Detached)
            {
                _dbSet.Attach(_entity);
            }
            _dbcontext.Entry<T>(_entity).State = EntityState.Modified;
        }
    }
}
