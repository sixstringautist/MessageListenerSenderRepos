using System;
using System.Collections.Generic;

namespace MessageListener
{
    public interface IRepository<T> : IDisposable
        where T : class
    {
        void Add(T item);
        void Delete(int id);
        RType Get<RType>(Func<RType, bool> predicate)
            where RType : class;
        IEnumerable<RType> GetAll<RType>()
            where RType : class;
        IEnumerable<RType> Filter<RType>(Func<RType, bool> predicate)
            where RType : class;

    }


    class MessageRepository : IRepository<Message>
    {
        AppDbContext context;

        public MessageRepository(string connectionString)
        {
            context = new AppDbContext(connectionString);
        }
        private void CheckNull<T>(params T[] p)
        {
            foreach (var el in p)
                if (p == null)
                    throw new ArgumentNullException($"{nameof(el)} is null");
        }

        public void Add(Message item)
        {
            CheckNull(item);
            context.Messages.Add(item);
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<RType> IRepository<Message>.Filter<RType>(Func<RType, bool> predicate)
        {
            throw new NotImplementedException();
        }

        RType IRepository<Message>.Get<RType>(Func<RType, bool> predicate)
        {
            throw new NotImplementedException();
        }

        IEnumerable<RType> IRepository<Message>.GetAll<RType>()
        {
            return context.Set<RType>();
        }
        public IEnumerable<RType> GetAll<RType>()
            where RType : Message
        {
            return context.Set<RType>();
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
    }
}
