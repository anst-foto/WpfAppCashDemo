using Microsoft.EntityFrameworkCore;

namespace WpfApp7;

public class AppContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public AppContext(DbContextOptions<AppContext> options) : base(options) => 
        Database.EnsureCreated();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // инициализация БД начальными данными
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Tom", Age = 23 },
            new User { Id = 2, Name = "Alice", Age = 26 },
            new User { Id = 3, Name = "Sam", Age = 28 }
        );
    }
}