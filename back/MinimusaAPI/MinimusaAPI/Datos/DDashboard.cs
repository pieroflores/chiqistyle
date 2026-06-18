using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data.SqlClient;
using System.Data;

namespace MinimusaAPI.Datos
{
    public class DDashboard
    {
        private readonly ConexionBD cn = new ConexionBD();

        public async Task<MResumenDashboard> ObtenerResumenAsync()
        {
            var resumen = new MResumenDashboard();

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            using (var cmd = new SqlCommand("sp_ObtenerResumenDashboard", sql))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await sql.OpenAsync();

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    // 1️⃣ TotalProductos
                    if (await dr.ReadAsync())
                        resumen.TotalProductos = Convert.ToInt32(dr["TotalProductos"]);

                    // 2️⃣ TotalVentas
                    await dr.NextResultAsync();
                    if (await dr.ReadAsync())
                        resumen.TotalVentas = Convert.ToInt32(dr["TotalVentas"]);

                    // 3️⃣ TotalCompras
                    await dr.NextResultAsync();
                    if (await dr.ReadAsync())
                        resumen.TotalCompras = Convert.ToInt32(dr["TotalCompras"]);

                    // 4️⃣ TotalPagos
                    await dr.NextResultAsync();
                    if (await dr.ReadAsync())
                        resumen.TotalPagos = Convert.ToDecimal(dr["TotalPagos"]);

                    // 5️⃣ VentasPorMes
                    await dr.NextResultAsync();
                    while (await dr.ReadAsync())
                    {
                        resumen.VentasPorMes.Add(new VentaMes
                        {
                            Mes = dr["Mes"].ToString() ?? "",
                            Total = Convert.ToDecimal(dr["TotalMes"])
                        });
                    }

                    // 6️⃣ InventarioPorCategoria
                    await dr.NextResultAsync();
                    while (await dr.ReadAsync())
                    {
                        resumen.InventarioPorCategoria.Add(new InventarioCategoria
                        {
                            Categoria = dr["NombreCategoria"].ToString() ?? "",
                            Cantidad = Convert.ToInt32(dr["TotalStock"])
                        });
                    }

                    // 7️⃣ Últimas ventas
                    await dr.NextResultAsync();
                    while (await dr.ReadAsync())
                    {
                        resumen.UltimasVentas.Add(new UltimaVenta
                        {
                            IdVenta = Convert.ToInt32(dr["IdVenta"]),
                            NombreCliente = dr["NombreCliente"].ToString() ?? "",
                            FechaVenta = Convert.ToDateTime(dr["FechaVenta"]),
                            Total = Convert.ToDecimal(dr["Total"]),
                            Estado = dr["Estado"].ToString() ?? "",
                            MetodoPago = dr["MetodoPago"].ToString() ?? ""
                        });
                    }
                }
            }

            return resumen;
        }
    }
}
