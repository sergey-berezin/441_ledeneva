using Microsoft.EntityFrameworkCore;

namespace DataBase
{
    class LibContext : DbContext
    {
        public DbSet<DBItem> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<DBItem>().HasKey(x => x.DBItemId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder b) => b.UseSqlite(@"Data Source=C:\Users\kuris\Documents\GitHub\441_ledeneva\DataBase\library.db");
    }
}
