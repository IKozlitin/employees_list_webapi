using Microsoft.EntityFrameworkCore;

namespace employees_list
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Employee> Employees { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = Guid.NewGuid().ToString(), Name = "Ivanov Ivan", Department = "Development", Phone = 79208887766 },
                new Employee { Id = Guid.NewGuid().ToString(), Name = "Petrov Petr", Department = "Design", Phone = 79208885544 },
                new Employee { Id = Guid.NewGuid().ToString(), Name = "Magomedov Magomed", Department = "Test", Phone = 79208883322 }
                );
        }
    }
}
