using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarjetaCreditoWebApi.Models
{
    public class TarjetaCredito
    {
        public int Id { get; set; }
        public string? NumeroTarjeta { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public double SaldoActual { get; set; }
        public double Limite { get; set; }
        public double PorcInteres { get; set; }
        public string? Estado { get; set; }
        [NotMapped]
        public List<Transacciones>? Transacciones { get; set; }
    }
}
