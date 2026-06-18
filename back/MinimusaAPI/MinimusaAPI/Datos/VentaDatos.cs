using System.Data;
using System.Data.SqlClient;
using MinimusaAPI.Modelo;
using MinimusaAPI.Conexion;

namespace Minimusa.Datos
{
    public class VentaDatos
    { 
        ConexionBD _conexion = new ConexionBD();

        // ... tus otros métodos aquí (ListarPendientes, RegistrarVenta, etc.) ...

        public List<Pago> ListarPagosPorVenta(int idVenta)
        {
            List<Pago> lista = new List<Pago>();

            using (var conexion = new SqlConnection(_conexion.cadenaSQL()))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ListarPagosPorVenta", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Pago
                            {
                                IdPago = Convert.ToInt32(dr["IdPago"]),
                                IdVenta = Convert.ToInt32(dr["IdVenta"]),
                                Monto = Convert.ToDecimal(dr["Monto"]),
                                Metodo = dr["Metodo"].ToString(),
                                Comprobante = dr["Comprobante"] == DBNull.Value ? "" : dr["Comprobante"].ToString(),
                                FechaPago = Convert.ToDateTime(dr["FechaPago"])
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}
