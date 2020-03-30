using System.Data.Entity;

namespace MessageSender
{
    class AppDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public AppDbContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<AppDbContext>());
            Database.Initialize(false);
        }
    }
}
