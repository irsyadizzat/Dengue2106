using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Dengue.DAL
{
    public class DataGateway<T> : IDataGateway<T> where T : class
    {
        internal DengueContext db = new DengueContext();
        internal DbSet<T> data = null;

        public DataGateway()
        {
            this.data = db.Set<T>();
        }

        public T Delete(int? id)
        {
            T obj = data.Find(id);
            data.Remove(obj);
            
            return obj;
        }

        public void Insert(T obj)
        {
            data.Add(obj);
            db.SaveChanges();
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public IEnumerable<T> SelectAll()
        {
            return data;
        }

        public T SelectById(int? id)
        {
            T obj = data.Find(id);
            return obj;
        }

        public void Update(T obj)
        {

            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}