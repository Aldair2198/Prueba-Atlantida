using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TarjetaCreditoWebApp.Models
{
    public class TarjetaCredito
    {
        public int Id { get; set; }
        [DisplayName("Número de Tarjeta")]
        public string? NumeroTarjeta { get; set; }
        public string? NumeroTarjetaMask { get; set; }
        [Required]
        public string? Nombres { get; set; }
        [Required]
        public string? Apellidos { get; set; }
        public string? NombreCompleto { get; set; }
        [Required]
        [DisplayName("Saldo Actual")]
        public double SaldoActual { get; set; }
        public double InteresBonificable { get; set; }
        public double SaldoDisponible { get; set; }
        public double ComprasActual { get; set; }
        public double ComprasPrevio { get; set; }
        public double Limite { get; set; }
        [DisplayName("Porcentaje de Interés")]
        public double PorcInteres { get; set; }
        public string? Estado { get; set; }
        public List<Transacciones>? Transacciones { get; set; }
        public string? NombreMesAct { get; set; }
        public string? NombreMesPrev { get; set; }
    }
}
