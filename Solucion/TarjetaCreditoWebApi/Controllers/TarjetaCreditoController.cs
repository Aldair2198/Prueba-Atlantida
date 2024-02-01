using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TarjetaCreditoWebApi.DataBase;

namespace TarjetaCreditoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarjetaCreditoController(DataContext dataContext) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;

        [HttpGet]
        [Route("GetTarjetasCredito")]
        public async Task<IActionResult> GetTarjetasCredito()
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                    new("@TipoFiltro", "TODOS"),
                    new("@Filtro", "")
                };

                var tarjetas = await _dataContext.TarjetaCredito.FromSqlRaw("EXEC dbo.TarjetaCredito_Sel @TipoFiltro, @Filtro", parametros).ToListAsync();
                return Ok(tarjetas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTarjetaCredito")]
        public async Task<IActionResult> GetTarjetaCredito(int id)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                    new("@TipoFiltro", "Id"),
                    new("@Filtro", id.ToString() ?? (object)DBNull.Value)
                };
                var tarjeta = await _dataContext.TarjetaCredito.FromSqlRaw("EXEC dbo.TarjetaCredito_Sel @TipoFiltro, @Filtro", parametros).ToListAsync();

                var parameters = new SqlParameter[]
                {
                    new("@TipoFiltro", "NumeroTarjeta"),
                    new("@Filtro", tarjeta[0].NumeroTarjeta ?? (object)DBNull.Value)
                };
                tarjeta[0].Transacciones = await _dataContext.Transacciones.FromSqlRaw("EXEC dbo.Transacciones_Sel @TipoFiltro, @Filtro", parameters).ToListAsync();

                return Ok(tarjeta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
