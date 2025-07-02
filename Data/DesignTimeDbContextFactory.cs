using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using eCommerceLite.Model;


namespace eCommerceLite.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderContext>
    {
        public OrderContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderContext>();
            optionsBuilder.UseSqlServer("Initial Catalog=eCommDB;Data Source=(localdb)\\MSSQLLocalDB;");

            return new OrderContext(optionsBuilder.Options);
        }
    }
}
