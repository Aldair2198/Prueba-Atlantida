using Microsoft.EntityFrameworkCore;
using TarjetaCreditoWebApi.Models;

namespace TarjetaCreditoWebApi.DataBase
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<TarjetaCredito> TarjetaCredito { get; set; }
        public DbSet<Transacciones> Transacciones { get; set; }
    }
}
