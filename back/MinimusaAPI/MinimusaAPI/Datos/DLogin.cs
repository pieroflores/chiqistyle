using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data;
using System.Data.SqlClient;

namespace MinimusaAPI.Datos
{
    public class DLogin
    {
        ConexionBD cn = new ConexionBD();

        public async Task<MLogin?> LoginUsuarioAsync(string usuario, string clave)
        {
            MLogin? login = null;
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_LoginUsuario", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Clave", clave);

                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (login == null)
                            {
                                login = new MLogin
                                {
                                    idUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                    usuarioLogin = reader["UsuarioLogin"].ToString(),
                                    nombreCompleto = reader["NombreCompleto"].ToString(),
                                    idRol = Convert.ToInt32(reader["IdRol"]),
                                    nombreRol = reader["NombreRol"].ToString(),
                                    modulo = new List<MModulo>()
                                };
                            }

                            // cada fila tiene un módulo diferente
                            login.modulo.Add(new MModulo
                            {
                                NombreModulo = reader["NombreModulo"].ToString()
                            });
                        }
                    }
                }
            }
            return login;
        }
    }
}
