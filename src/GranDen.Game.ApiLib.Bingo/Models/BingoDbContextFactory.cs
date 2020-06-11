using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    public class BingoDbContextFactory : IDesignTimeDbContextFactory<BingoDbContext>
    {
        public BingoDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BingoDbContext>();
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BingoDemoDb");
            
            return new BingoDbContext(optionsBuilder.Options);
        }
    }
}