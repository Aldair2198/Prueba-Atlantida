using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TarjetaCreditoWebApp.Models
{
    public class Transacciones
    {
        public int Id { get; set; }
        [DisplayName("Número de Autorización")]
        public string? NumeroAutorizacion { get; set; }
        public string? NumeroTarjeta { get; set; }
        public DateTime Fecha { get; set; }
        public string? FechaText { get; set; }
        public string? Descripcion { get; set; }
        public double Monto { get; set; }
        public string? MontoAbono { get; set; }
        public string? MontoCargo { get; set; }
        public string? MontoText { get; set; }
        public string? Estado { get; set; }
        public string? AbonoCargo { get; set; }
    }
}
