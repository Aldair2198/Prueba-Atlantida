using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarjetaCreditoWebApi.Models
{
    public class Transacciones
    {
        public int Id { get; set; }
        public string? NumeroTarjeta { get; set; }
        public DateTime Fecha { get; set; }
        public string? Descripcion { get; set; }
        public decimal Monto { get; set; }
        public string? Estado { get; set; }
        public char? AbonoCargo { get; set; }
    }
}
