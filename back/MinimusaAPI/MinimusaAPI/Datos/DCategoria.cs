using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data;
using System.Data.SqlClient;

namespace MinimusaAPI.Datos
{
    public class Dcategoria
    {
        ConexionBD cn = new ConexionBD();
        public async Task<List<MCategoria>> MostrarCategoria()
        {
            var lista = new List<MCategoria>();
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ListarCategorias", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var item = await cmd.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync())
                        {
                            var mCategoria = new MCategoria();
                            mCategoria.idCategoria = (int)item["idCategoria"];
                            mCategoria.nombreCategoria = (string)item["nombreCategoria"]; 

                            lista.Add(mCategoria);
                        }
                    }
                    return lista;
                }
            }

        }
        public async Task InsertarCategoria(MCategoria parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_RegistrarCategoria", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombreCategoria", parametros.nombreCategoria); 

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task EditarCategoria(MCategoria parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EditarCategoria", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idCategoria", parametros.idCategoria);
                    cmd.Parameters.AddWithValue("@nombreCategoria", parametros.nombreCategoria); 
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task EliminarCategoria(MCategoria parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EliminarCategoria", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idCategoria", parametros.idCategoria);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
