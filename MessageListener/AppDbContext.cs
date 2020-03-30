using System.Data.Entity;

namespace MessageListener
{
    class AppDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public AppDbContext(string connectionStrihg) : base(connectionStrihg)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<AppDbContext>());
            Database.Initialize(false);
        }
    }
}
