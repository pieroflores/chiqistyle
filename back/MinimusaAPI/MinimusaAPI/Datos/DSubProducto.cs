using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data;
using System.Data.SqlClient;

namespace MinimusaAPI.Datos
{
    public class DSubProducto
    {
        ConexionBD cn = new ConexionBD();

        // Listar
        public async Task<List<MSubProducto>> MostrarSubProductos()
        {
            var lista = new List<MSubProducto>();
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ListarSubProductos", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var item = await cmd.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync())
                        {
                            var mSubProducto = new MSubProducto
                            {
                                IdSubProducto = (int)item["IdSubProducto"],
                                IdProductoPrincipal = (int)item["IdProductoPrincipal"],
                                IdColor = (int)item["IdColor"],
                                IdTalla = (int)item["IdTalla"],
                                PrecioCompra = (decimal)item["PrecioCompra"],
                                PrecioVenta = (decimal)item["PrecioVenta"],
                                CodigoSubProducto = item["CodigoSubProducto"] != DBNull.Value
                                    ? (string)item["CodigoSubProducto"]
                                    : null
                            };
                            lista.Add(mSubProducto);
                        }
                    }
                }
            }
            return lista;
        }

        // Insertar
        public async Task InsertarSubProducto(MSubProducto parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_InsertarSubProducto", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProductoPrincipal", parametros.IdProductoPrincipal);
                    cmd.Parameters.AddWithValue("@IdColor", parametros.IdColor);
                    cmd.Parameters.AddWithValue("@IdTalla", parametros.IdTalla);
                    cmd.Parameters.AddWithValue("@PrecioCompra", parametros.PrecioCompra);
                    cmd.Parameters.AddWithValue("@PrecioVenta", parametros.PrecioVenta);
                    cmd.Parameters.AddWithValue("@PrecioVentaPorMayor", parametros.PrecioVentaPorMayor);
                    cmd.Parameters.AddWithValue("@PrecioVentaLiquidacion", parametros.PrecioVentaLiquidacion);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // Editar
        public async Task EditarSubProducto(MSubProducto parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EditarSubProducto", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdSubProducto", parametros.IdSubProducto);
                    cmd.Parameters.AddWithValue("@IdProductoPrincipal", parametros.IdProductoPrincipal);
                    cmd.Parameters.AddWithValue("@IdColor", parametros.IdColor);
                    cmd.Parameters.AddWithValue("@IdTalla", parametros.IdTalla);
                    cmd.Parameters.AddWithValue("@PrecioCompra", parametros.PrecioCompra);
                    cmd.Parameters.AddWithValue("@PrecioVenta", parametros.PrecioVenta);
                    cmd.Parameters.AddWithValue("@CodigoSubProducto", (object?)parametros.CodigoSubProducto ?? DBNull.Value);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // Eliminar
        public async Task EliminarSubProducto(MSubProducto parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EliminarSubProducto", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdSubProducto", parametros.IdSubProducto);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<MSubProductoDTO>> MostrarSubProductosPorProducto(int idProductoPrincipal)
        {
            var lista = new List<MSubProductoDTO>();

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ListarSubProductosProd", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idProductoPrincipal", idProductoPrincipal);

                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var subProd = new MSubProductoDTO
                            {
                                IdSubProducto = (int)reader["IdSubProducto"],
                                IdProductoPrincipal = (int)reader["IdProductoPrincipal"],
                                NombreColor = reader["NombreColor"].ToString()!,
                                NombreTalla = reader["NombreTalla"].ToString()!,
                                PrecioCompra = (decimal)reader["PrecioCompra"],
                                PrecioVenta = (decimal)reader["PrecioVenta"],
                                PrecioVentaPorMayor = (decimal)reader["PrecioVentaPorMayor"],
                                PrecioVentaLiquidacion = (decimal)reader["PrecioVentaLiquidacion"],
                            };

                            lista.Add(subProd);
                        }
                    }
                }
            }

            return lista;
        }

    }
}
