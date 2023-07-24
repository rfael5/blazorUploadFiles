using BlazorFileUpload.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorFileUpload.Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<ResultadoUpload> Uploads => Set<ResultadoUpload>();
    }
}
