using Microsoft.EntityFrameworkCore;
using Shop.Web.Data.Entities;

namespace Shop.Web.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

    }
}
