using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    public class BingoDbContextFactory : IDesignTimeDbContextFactory<BingoGameDbContext>
    {
        public BingoGameDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BingoGameDbContext>();
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BingoGameDb");
            
            return new BingoGameDbContext(optionsBuilder.Options);
        }
    }
}