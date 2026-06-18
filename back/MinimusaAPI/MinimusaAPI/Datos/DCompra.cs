using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data;
using System.Data.SqlClient;
using static MinimusaAPI.Modelo.MCCompra;

namespace MinimusaAPI.Datos
{
    public class DCompra
    {
        ConexionBD cn = new ConexionBD();

        public async Task<MDatosComboCompra> CargarCombos()
        {
            var datosCombos = new MDatosComboCompra();

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("cargaComboCompra", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        // 1. PRIMER CONJUNTO: Productos/Variantes
                        while (await reader.ReadAsync())
                        {
                            datosCombos.Productos.Add(new MSubProductoCombo
                            {
                                IdSubProducto = (int)reader["IdSubProducto"],
                                ProductoVariable = (string)reader["productoVariable"]
                            });
                        }

                        // 2. SEGUNDO CONJUNTO: Ubicaciones (Avanzar al siguiente resultado)
                        await reader.NextResultAsync();
                        while (await reader.ReadAsync())
                        {
                            datosCombos.Ubicaciones.Add(new MAlmacenCombo
                            {
                                IdAlmacen = (int)reader["IdAlmacen"],
                                Ubicacion = (string)reader["ubicacion"]
                            });
                        }

                        // 3. TERCER CONJUNTO: Tipo de Documento (Avanzar al siguiente resultado)
                        await reader.NextResultAsync();
                        while (await reader.ReadAsync())
                        {
                            datosCombos.TiposDocumento.Add(new MTipoDocumento
                            {
                                // Asegúrate que los nombres de las columnas coincidan con el SP
                                IdTipoDocumentoMercaderia = (int)reader["IdTipoDocumentoMercaderia"],
                                Nombre = (string)reader["Nombre"] // Usa el nombre real de tu columna
                            });
                        }
                        await reader.NextResultAsync();
                        while (await reader.ReadAsync())
                        {
                            datosCombos.ProductoDisponibles.Add(new MSubProductoCombo
                            {
                                IdSubProducto = (int)reader["IdSubProducto"],
                                ProductoVariable = (string)reader["productoVariable"]
                            });
                        } //MProductosCombo
                        await reader.NextResultAsync();
                        while (await reader.ReadAsync())
                        {
                            datosCombos.ProductoPrincipal.Add(new MProductosCombo
                            {
                                IdProductoPrincipal = (int)reader["IdProductoPrincipal"],
                                NombreProducto = (string)reader["NombreProducto"]
                            });
                        }
                    }
                    return datosCombos;
                }
            }
        }
        public async Task InsertarCompra(MCompra parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                await sql.OpenAsync();

                foreach (var det in parametros.detalle)
                {
                    using (var cmd = new SqlCommand("sp_RegistrarCompra", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdProveedor", parametros.idProveedor);
                        cmd.Parameters.AddWithValue("@FechaCompra", parametros.fechaCompra);
                        cmd.Parameters.AddWithValue("@IdSubProducto", det.idSubProducto);
                        cmd.Parameters.AddWithValue("@Cantidad", det.cantidad);
                        cmd.Parameters.AddWithValue("@PrecioUnitario", det.precioUnitario);
                        cmd.Parameters.AddWithValue("@IdAlmacen", det.idAlmacen);
                        cmd.Parameters.AddWithValue("@IdUsuario", parametros.idUsuario);
                        cmd.Parameters.AddWithValue("@DocumentoReferencia", parametros.documentoReferencia ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Observacion", parametros.observacion ?? (object)DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }
        public List<SubProductoDTO> ObtenerSubProductos(int idProducto)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                SqlCommand cmd = new SqlCommand("SP_OBTENER_SUBPRODUCTOS", sql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                sql.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                List<SubProductoDTO> lista = new();

                while (dr.Read())
                {
                    lista.Add(new SubProductoDTO
                    {
                        IdProductoPrincipal = Convert.ToInt32(dr["IdProductoPrincipal"]),
                        IdSubProducto = Convert.ToInt32(dr["IdSubProducto"]),
                        Talla = dr["Talla"].ToString(),
                        NombreColor = dr["NombreColor"].ToString(),
                        DescripcionSubproducto = dr["DescripcionSubproducto"].ToString(),
                        PrecioCompra = Convert.ToDecimal(dr["PrecioCompra"])
                    });
                }

                return lista;
            }
        }


    }
}