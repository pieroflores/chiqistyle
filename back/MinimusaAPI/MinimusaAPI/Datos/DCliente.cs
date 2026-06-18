using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data;
using System.Data.SqlClient;

namespace MinimusaAPI.Datos
{
    public class Dcliente
    {
        ConexionBD cn = new ConexionBD();
        public async Task<List<MCliente>> MostrarCliente()
        {
            var lista = new List<MCliente>();
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ListarClientes", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var item = await cmd.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync())
                        {
                            var mCliente = new MCliente();
                            mCliente.IdCliente = (int)item["IdCliente"];
                            mCliente.NombreCliente = (string)item["NombreCliente"];
                            mCliente.DNI = (string)item["DNI"];
                            mCliente.Direccion = (string)item["Direccion"];
                            if (item["telefono"] != DBNull.Value)
                            {
                                mCliente.Telefono = Convert.ToInt32(item["telefono"]);
                            }
                            else
                            {
                                // Assign a default value, like 0 or a specific error code
                                mCliente.Telefono = 0;
                            }
                            lista.Add(mCliente);
                        }
                    }
                    return lista;
                }
            }

        }
        public async Task InsertarCliente(MCliente parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_RegistrarCliente", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreCliente", parametros.NombreCliente);
                    cmd.Parameters.AddWithValue("@Dni", parametros.DNI);
                    cmd.Parameters.AddWithValue("@Direccion", parametros.Direccion);
                    cmd.Parameters.AddWithValue("@Telefono", parametros.Telefono); 

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task EditarCliente(MCliente parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EditarCliente", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", parametros.IdCliente);
                    cmd.Parameters.AddWithValue("@NombreCliente", parametros.NombreCliente);
                    cmd.Parameters.AddWithValue("@Dni", parametros.DNI);
                    cmd.Parameters.AddWithValue("@Direccion", parametros.Direccion);
                    cmd.Parameters.AddWithValue("@Telefono", parametros.Telefono);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task EliminarCliente(MCliente parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EliminarCliente", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", parametros.IdCliente);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
