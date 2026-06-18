using MinimusaAPI.Modelo;
using System.Data.SqlClient;
using System.Data;
using MinimusaAPI.Conexion;

namespace MinimusaAPI.Datos
{
    public class DReporteDatos
    {
        ConexionBD cn = new ConexionBD();

        public List<MReporteVenta> ReporteVentas(DateTime? fechaInicio, DateTime? fechaFin, string estado, string metodoPago)
        {
            List<MReporteVenta> lista = new List<MReporteVenta>();

            using (var conexion = new SqlConnection(cn.cadenaSQL()))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ReporteVentas", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaInicio", (object)fechaInicio ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaFin", (object)fechaFin ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", string.IsNullOrEmpty(estado) ? (object)DBNull.Value : estado);
                    cmd.Parameters.AddWithValue("@MetodoPago", string.IsNullOrEmpty(metodoPago) ? (object)DBNull.Value : metodoPago);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new MReporteVenta
                            {
                                IdVenta = Convert.ToInt32(dr["IdVenta"]),
                                NombreCliente = dr["NombreCliente"].ToString(),
                                FechaVenta = Convert.ToDateTime(dr["FechaVenta"]),
                                Total = Convert.ToDecimal(dr["Total"]),
                                MontoPagado = Convert.ToDecimal(dr["MontoPagado"]),
                                MontoPendiente = Convert.ToDecimal(dr["MontoPendiente"]),
                                Estado = dr["Estado"].ToString(),
                                MetodoPago = dr["MetodoPago"].ToString(), 
                                Cantidad = Convert.ToDecimal(dr["Cantidad"]),
                                PrecioCompraUnitaria = Convert.ToDecimal(dr["PrecioCompraUnitaria"]),
                                PrecioCompraTotal = Convert.ToDecimal(dr["PrecioCompraTotal"])

                            });
                        }
                    }
                }
            }

            return lista;
        }
        public async Task<List<ReporteCompras>> ReporteCompras(DateTime? fechaInicio, DateTime? fechaFin, int? idProveedor, string estado)
        {
            var lista = new List<ReporteCompras>();
            using var sql = new SqlConnection(cn.cadenaSQL());
            using var cmd = new SqlCommand("sp_ReporteCompras", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FechaInicio", (object)fechaInicio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaFin", (object)fechaFin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdProveedor", (object)idProveedor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", (object)estado ?? DBNull.Value);

            await sql.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new ReporteCompras
                {
                    IdCompra = Convert.ToInt32(reader["IdCompra"]),
                    FechaCompra = Convert.ToDateTime(reader["FechaCompra"]),
                    NombreProveedor = reader["NombreProveedor"].ToString(),
                    Total = Convert.ToDecimal(reader["Total"]),
                    Estado = reader["Estado"].ToString()
                });
            }
            return lista;
        }

        // 🔹 Reporte de Pagos por Cliente
        public async Task<List<ReportePagosCliente>> ListarPagosPorCliente(int idCliente, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var lista = new List<ReportePagosCliente>();
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ReportePagosPorCliente", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", idCliente == 0 ? (object)DBNull.Value : idCliente);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin ?? (object)DBNull.Value);

                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new ReportePagosCliente
                            {
                                IdVenta = (int)reader["IdVenta"],
                                Total = (decimal)reader["Total"],
                                NombreCliente = reader["NombreCliente"].ToString(),
                                FechaVenta = reader["FechaVenta"].ToString(),
                            });
                        }
                    }
                }
            }
            return lista;
        }
        public async Task<MDetalleCompletoVenta> ObtenerDetalleCompletoVenta(int idVenta)
        {
            var detalleCompleto = new MDetalleCompletoVenta();
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ReportePagosPorClienteDetalle", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idventa", idVenta);
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        // 1️⃣ Primer Result Set: Pagos Realizados
                        while (await reader.ReadAsync())
                        {
                            detalleCompleto.Pagos.Add(new MRegistroPagoLista
                            {
                                IdPago = (int)reader["IdPago"],
                                Monto = (decimal)reader["Monto"],
                                FechaPago = (DateTime)reader["FechaPago"],
                                Metodo = reader["Metodo"].ToString(),
                                Comprobante = reader["Comprobante"] == DBNull.Value ? null : reader["Comprobante"].ToString(),
                            });
                        }

                        // 2️⃣ Segundo Result Set: Detalle de Productos
                        if (reader.NextResult())
                        {
                            while (await reader.ReadAsync())
                            {
                                detalleCompleto.Productos.Add(new MDetalleVentaId
                                {
                                    IdVentaDetalle = (int)reader["IdVentaDetalle"],
                                    IdSubProducto = (int)reader["IdSubProducto"],
                                    Cantidad = (int)reader["Cantidad"],
                                    PrecioUnitario = (decimal)reader["PrecioUnitario"],
                                    Subtotal = (decimal)reader["Subtotal"],
                                    NombreProducto = reader["NombreProducto"].ToString(),
                                    CodigoSubProducto = reader["CodigoSubProducto"].ToString(),
                                    NombreColor = reader["NombreColor"].ToString(),
                                    NombreTalla = reader["NombreTalla"].ToString(),
                                    // ✅ Asegúrate de leer Ubicacion si está en el modelo MDetalleVentaId
                                    Ubicacion = reader["Ubicacion"].ToString() // <-- Verifica si esto es necesario
                                });
                            }
                        }
                    }
                }
            }
            return detalleCompleto;
        }

    }
}
