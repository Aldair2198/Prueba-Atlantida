using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TarjetaCreditoWebApi.DataBase;
using TarjetaCreditoWebApi.Models;

namespace TarjetaCreditoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionesController(DataContext dataContext) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;

        //[HttpPost]
        //[Route("GetTransacciones")]
        //public async Task<IActionResult> GetTransacciones()
        //{
        //    try
        //    {
        //        var parametros = new SqlParameter[]
        //        {
        //            new("@TipoFiltro", "TODOS"),
        //            new("@Filtro", "")
        //        };

        //        var transacciones = await _dataContext.Transacciones.FromSqlRaw("EXEC dbo.Transacciones_Sel @TipoFiltro, @Filtro", parametros).ToListAsync();
        //        return Ok(transacciones);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("GetTransaccion")]
        //public async Task<IActionResult> GetTransaccion(int NumeroTarjeta)
        //{
        //    try
        //    {
        //        var parametros = new SqlParameter[]
        //        {
        //            new("@TipoFiltro", "NumeroTarjeta"),
        //            new("@Filtro", NumeroTarjeta.ToString() ?? (object)DBNull.Value)
        //        };

        //        var transaccion = await _dataContext.Transacciones.FromSqlRaw("EXEC dbo.Transacciones_Sel @TipoFiltro, @Filtro", parametros).ToListAsync();

        //        return Ok(transaccion);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPut]
        [Route("InsertTransaccion")]
        public async Task<IActionResult> InsertTransaccion(TarjetaCredito model)
        {
            try
            {
                Transacciones tx1 = model.Transacciones[0];

                var parametros = new SqlParameter[]
                {
                    new("@NumeroTarjeta", model.NumeroTarjeta),
                    new("@Fecha", tx1.Fecha),
                    new("@Descripcion", tx1.Descripcion),
                    new("@AbonoCargo", tx1.AbonoCargo),
                    new("@Monto", tx1.Monto),
                    new("@Estado", "A")
                };

                var transaccion = await _dataContext.Database.ExecuteSqlRawAsync("EXEC dbo.Transacciones_Ins " +
                    "@NumeroTarjeta, @Fecha, @Descripcion, @AbonoCargo, @Monto, @Estado", parametros);

                return Ok(transaccion);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
