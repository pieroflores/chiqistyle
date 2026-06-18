using MinimusaAPI.Conexion;
using System.Data.SqlClient;
using System.Data;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Datos
{
    public class DStock
    {
        ConexionBD cn = new ConexionBD();
        public async Task<List<MStock>> ListarStockGeneralAsync()
        {
            var lista = new List<MStock>();

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ListarStockGeneral", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure; 
                    await sql.OpenAsync();

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lista.Add(new MStock
                            {
                                IdSubProducto = Convert.ToInt32(dr["IdSubProducto"]),
                                NombreProducto = dr["NombreProducto"].ToString(),
                                CodigoSubProducto = dr["CodigoSubProducto"].ToString(),
                                Color = dr["Color"].ToString(),
                                Talla = dr["Talla"].ToString(),
                                PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                                Stock = Convert.ToInt32(dr["Stock"]),
                                Ubicacion = dr["Ubicacion"].ToString(),
                                FotoProducto = dr["FotoProducto"]?.ToString(),
                                FechaCompra = Convert.ToDateTime(dr["FechaCompra"]),
                                PrecioVentaLiquidacion = Convert.ToDecimal(dr["PrecioVentaLiquidacion"]),
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}
