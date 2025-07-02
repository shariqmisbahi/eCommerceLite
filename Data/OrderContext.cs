using eCommerceLite.Data;
using eCommerceLite.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Design;

namespace eCommerceLite.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }

        public DbSet<OrderRequest> Orders { get; set; }

        public DbSet<ProductEmbeddings> ProductEmbeddings { get; set; }
    }
}


