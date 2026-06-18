using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data;
using System.Data.SqlClient;

namespace MinimusaAPI.Datos
{
    public class Dproveedor
    {
        ConexionBD cn = new ConexionBD();
        public async Task<List<MProveedor>> MostrarProveedor()
        {
            var lista = new List<MProveedor>();
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ListarProveedor", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var item = await cmd.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync())
                        {
                            var mProveedor = new MProveedor
                            {
                                IdProveedor = item["IdProveedor"] != DBNull.Value
            ? Convert.ToInt32(item["IdProveedor"])
            : 0,

                                nombreProveedor = item["NombreProveedor"]?.ToString() ?? "",

                                ruc = item["ruc"]?.ToString() ?? "",

                                direccion = item["Direccion"]?.ToString() ?? "",

                                telefono = item["telefono"]?.ToString() ?? "",

                                email = item["Email"] != DBNull.Value
            ? item["Email"].ToString()
            : null
                            };
                            lista.Add(mProveedor);
                        }
                    }
                    return lista;
                }
            }

        }
        public async Task InsertarProveedor(MProveedor parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_RegistrarProveedor", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombreProveedor", parametros.nombreProveedor);
                    cmd.Parameters.AddWithValue("@ruc", parametros.ruc);
                    cmd.Parameters.Add("@direccion", SqlDbType.VarChar, 250)
       .Value = string.IsNullOrWhiteSpace(parametros.direccion)
           ? DBNull.Value
           : parametros.direccion;

                    cmd.Parameters.Add("@telefono", SqlDbType.VarChar, 250)
                        .Value = string.IsNullOrWhiteSpace(parametros.telefono)
                            ? DBNull.Value
                            : parametros.telefono;

                    cmd.Parameters.Add("@email", SqlDbType.VarChar, 250)
                        .Value = string.IsNullOrWhiteSpace(parametros.email)
                            ? DBNull.Value
                            : parametros.email;

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task EditarProveedor(MProveedor parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            using (var cmd = new SqlCommand("sp_EditarProveedor", sql))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@IdProveedor", SqlDbType.Int)
                    .Value = parametros.IdProveedor;

                cmd.Parameters.Add("@NombreProveedor", SqlDbType.VarChar, 150)
                    .Value = parametros.nombreProveedor;

                cmd.Parameters.Add("@Ruc", SqlDbType.VarChar, 20)
                    .Value = parametros.ruc ?? "";

                cmd.Parameters.Add("@Direccion", SqlDbType.VarChar, 250)
     .Value = string.IsNullOrWhiteSpace(parametros.direccion)
         ? DBNull.Value
         : parametros.direccion;


                cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 250)
     .Value = string.IsNullOrWhiteSpace(parametros.telefono)
         ? DBNull.Value
         : parametros.telefono;

                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 250)
                    .Value = string.IsNullOrWhiteSpace(parametros.email)
                        ? DBNull.Value
                        : parametros.email;


                await sql.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarProveedor(MProveedor parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EliminarProveedor", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProveedor", parametros.IdProveedor);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
