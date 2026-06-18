using MinimusaAPI.Modelo;
using System.Data.SqlClient;
using System.Data;
using MinimusaAPI.Conexion;
using MinimusaAPI.ViewModels;

namespace MinimusaAPI.Datos
{
    public class DVenta
    {
        ConexionBD cn = new ConexionBD();
        public async Task<List<MConsultaProductoVenta>> ConsultarProductoVenta(int idSubProducto)
        {
            var lista = new List<MConsultaProductoVenta>();

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_consultaProductoVenta", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdSubProducto", idSubProducto);
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new MConsultaProductoVenta
                            {
                                IdAlmacen = (int)reader["IdAlmacen"],
                                PrecioVenta = (decimal)reader["PrecioVenta"],
                                PrecioVentaPorMayor = (decimal)reader["PrecioVentaPorMayor"],
                                PrecioVentaLiquidacion = (decimal)reader["PrecioVentaLiquidacion"],
                                UbicacionTexto = reader["UbicacionTexto"].ToString(),
                                CantidadDisponible = (int)reader["CantidadDisponible"]
                            };
                            lista.Add(item);
                        }
                    }
                }
            }

            return lista;
        }
        public async Task<List<MVentaPendiente>> ListarVentasPendientes()
        {
            var lista = new List<MVentaPendiente>();

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ListarVentasPendientes", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new MVentaPendiente
                            {
                                IdVenta = (int)reader["IdVenta"],
                                IdCliente = (int)reader["IdCliente"],
                                NombreCliente = reader["NombreCliente"].ToString(),
                                DNI = reader["DNI"].ToString(),
                                FechaVenta = (DateTime)reader["FechaVenta"],
                                Total = (decimal)reader["Total"],
                                MontoPagado = (decimal)reader["MontoPagado"],
                                MontoPendiente = (decimal)reader["MontoPendiente"],
                                Estado = reader["Estado"].ToString(),
                                MetodoPago = reader["MetodoPago"] == DBNull.Value ? null : reader["MetodoPago"].ToString(),
                                Comprobante = reader["Comprobante"] == DBNull.Value ? null : reader["Comprobante"].ToString(),
                                TipoTransaccion = reader["TipoTransaccion"] == DBNull.Value ? null : reader["TipoTransaccion"].ToString()
                            };

                            lista.Add(item);
                        }
                    }
                }
            }

            return lista;
        }
        public async Task<List<MDetalleVentaId>> ObtenerDetalleVenta(int idVenta)
        {
            var lista = new List<MDetalleVentaId>();

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ObtenerDetalleVenta", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var detalle = new MDetalleVentaId
                            {
                                IdVentaDetalle = (int)reader["IdVentaDetalle"],
                                IdSubProducto = (int)reader["IdSubProducto"],
                                NombreProducto = reader["NombreProducto"].ToString(),
                                CodigoSubProducto = reader["CodigoSubProducto"].ToString(),
                                Cantidad = (int)reader["Cantidad"],
                                PrecioUnitario = (decimal)reader["PrecioUnitario"],
                                Subtotal = (decimal)reader["Subtotal"],
                                Ubicacion = reader["Ubicacion"].ToString()
                            };

                            lista.Add(detalle);
                        }
                    }
                }
            }

            return lista;
        }

        // 🔹 REGISTRAR VENTA CON VARIOS PRODUCTOS
        public async Task<int> RegistrarVenta(MVenta venta)
        {
            int idVentaGenerado = 0;

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                await sql.OpenAsync();
                using (var tran = sql.BeginTransaction())
                {
                    try
                    {
                        // 1️⃣ Registrar cabecera y primer producto
                        using (var cmd = new SqlCommand("sp_RegistrarVenta", sql, tran))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IdCliente", venta.IdCliente);
                            cmd.Parameters.AddWithValue("@FechaVenta", venta.FechaVenta);
                            cmd.Parameters.AddWithValue("@IdSubProducto", venta.Detalle[0].IdSubProducto);
                            cmd.Parameters.AddWithValue("@Cantidad", venta.Detalle[0].Cantidad);
                            cmd.Parameters.AddWithValue("@PrecioUnitario", venta.Detalle[0].PrecioUnitario);
                            cmd.Parameters.AddWithValue("@UsaPrecioPorMayor", venta.Detalle[0].UsaPrecioPorMayor); // 👈 NUEVO
                            cmd.Parameters.AddWithValue("@IdAlmacen", venta.Detalle[0].IdAlmacen);
                            cmd.Parameters.AddWithValue("@IdUsuario", venta.IdUsuario);
                            cmd.Parameters.AddWithValue("@MontoPagado", venta.MontoPagado);
                            cmd.Parameters.AddWithValue("@MetodoPago", venta.MetodoPago ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Comprobante", venta.Comprobante ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@TipoTransaccion", venta.TipoTransaccion ?? (object)DBNull.Value);

                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                    idVentaGenerado = Convert.ToInt32(reader["IdVenta"]); // 👈 CORREGIDO
                            }
                        }

                        // 2️⃣ Insertar los productos adicionales si hay más
                        for (int i = 1; i < venta.Detalle.Count; i++)
                        {
                            var d = venta.Detalle[i];
                            using (var cmdDet = new SqlCommand("sp_RegistrarVentaDetalle", sql, tran))
                            {
                                cmdDet.CommandType = CommandType.StoredProcedure;
                                cmdDet.Parameters.AddWithValue("@IdVenta", idVentaGenerado);
                                cmdDet.Parameters.AddWithValue("@IdSubProducto", d.IdSubProducto);
                                cmdDet.Parameters.AddWithValue("@Cantidad", d.Cantidad);
                                cmdDet.Parameters.AddWithValue("@PrecioUnitario", d.PrecioUnitario);
                                cmdDet.Parameters.AddWithValue("@UsaPrecioPorMayor", d.UsaPrecioPorMayor); // 👈 NUEVO
                                cmdDet.Parameters.AddWithValue("@IdAlmacen", d.IdAlmacen);
                                cmdDet.Parameters.AddWithValue("@IdUsuario", venta.IdUsuario);
                                await cmdDet.ExecuteNonQueryAsync();
                            }
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return idVentaGenerado;
        }
        public async Task RegistrarPagoAdicional(MPagoAdicional pago)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_RegistrarPagoAdicional", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdVenta", pago.IdVenta);
                    cmd.Parameters.AddWithValue("@Monto", pago.Monto);
                    cmd.Parameters.AddWithValue("@MetodoPago", pago.MetodoPago);
                    cmd.Parameters.AddWithValue("@Comprobante", (object)pago.Comprobante ?? DBNull.Value);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<Cabecera> ObtenerCabeceraPorVenta(int idVenta)
        {
            Cabecera cabecera = null;

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ObtenerCabeceraVenta", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            cabecera = new Cabecera
                            {
                                Idcabecera = (int)reader["IdCabecera"],
                                Idtipocomp = reader["IdTipoComp"].ToString(),
                                Serie = reader["Serie"].ToString(),
                                Numero = reader["Numero"].ToString(),
                                Fechaemision = (DateTime)reader["FechaEmision"],

                                TotSubtotal = reader["Subtotal"].ToString(),
                                TotIgv = (decimal)reader["IGV"],
                                TotTotal = (decimal)reader["Total"],

                                EmpresaRUC = reader["EmpresaRUC"].ToString(),
                                EmpresaRazonSocial = reader["EmpresaRazonSocial"].ToString(),
                                EmpresaDireccion = reader["EmpresaDireccion"].ToString(),

                                ClienteRazonSocial = reader["ClienteRazonSocial"].ToString(),
                                ClienteNumeroDocumento = reader["ClienteDocumento"].ToString(),
                                ClienteTipodocumento = reader["ClienteTipoDoc"].ToString(),
                                ClienteDireccion = reader["ClienteDireccion"].ToString()
                            };
                        }
                    }
                }
            }

            // 🔹 CARGAR DETALLE (MAPEADO A SUNAT)
            if (cabecera != null)
            {
                var detalleBD = await ObtenerDetalleCabecera(idVenta);

                foreach (var d in detalleBD)
                {
                    cabecera.Detalles.Add(new Detalles
                    {
                        Idcab = cabecera.Idcabecera,
                        Codcom = cabecera.Idtipocomp,

                        DescripcionProducto = d.NombreProducto,
                        UnidadMedida = "NIU", // estándar SUNAT

                        Cantidad = d.Cantidad,
                        Precio = d.PrecioUnitario,
                        Total = d.Subtotal,

                        mtoValorVentaItem = d.Subtotal,
                        porIgvItem = 18, // IGV Perú

                        Igv = d.Subtotal * 0.18m,
                        Activo = true,
                        Usuario = "SYSTEM"
                    });
                }
            }

            return cabecera;
        }
        private async Task<ICollection<MDetalleVentaId>> ObtenerDetalleCabecera(int idVenta)
        {
            var detalles = new List<MDetalleVentaId>();

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ObtenerDetalleVenta2", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            detalles.Add(new MDetalleVentaId
                            {
                                Cantidad = (int)reader["Cantidad"],
                                PrecioUnitario = (decimal)reader["PrecioUnitario"],
                                NombreProducto = reader["NombreProducto"].ToString(),
                                Subtotal = (decimal)reader["Subtotal"]
                            });
                        }
                    }
                }
            }

            return detalles;
        }
        public async Task<List<MClienteDeuda>> ListarClientesConDeuda()
        {
            var lista = new List<MClienteDeuda>();

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ListarClientesConDeuda", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new MClienteDeuda
                            {
                                IdCliente = (int)reader["IdCliente"],
                                NombreCliente = reader["NombreCliente"].ToString(),
                                DNI = reader["DNI"].ToString(),
                                TotalVentas = (decimal)reader["TotalVentas"],
                                TotalPagado = (decimal)reader["TotalPagado"],
                                TotalPendiente = (decimal)reader["TotalPendiente"]
                            });
                        }
                    }
                }
            }

            return lista;
        }

    }
}
