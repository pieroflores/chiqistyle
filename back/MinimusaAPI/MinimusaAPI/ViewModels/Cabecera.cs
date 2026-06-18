using System;
using System.Collections.Generic;

namespace MinimusaAPI.ViewModels
{
    public partial class Cabecera
    {
        public Cabecera()
        {
            Detalles = new HashSet<Detalles>();
        }
        public int Idcabecera { get; set; }
        public string Idmoneda { get; set; }
        public string Observaciones { get; set; }
        public string Idtipocomp { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public bool? Incligv { get; set; }
        public decimal? Dscto { get; set; }
        public decimal? Igv { get; set; }
        public decimal? Total { get; set; }
        public DateTime? Fechaemision { get; set; }
        public string Numdoc { get; set; }
        public string Usuario { get; set; }
        public DateTime? Fechavencimiento { get; set; }
        public bool? Chser { get; set; }
        public decimal? TCompra { get; set; }
        public bool? Activo { get; set; }
        public decimal? Porcigv { get; set; }
        public int? Idcliente { get; set; }
        public string Guiaremision { get; set; }
        public string TotSubtotal { get; set; }
        public decimal? TotDsctos { get; set; }
        public decimal? TotTotal { get; set; }
        public decimal? TotIgv { get; set; }
        public decimal? TotIcbper { get; set; }
        public decimal? TotISC { get; set; }
        public decimal? TotOtros { get; set; }
        public decimal? TotTributos { get; set; }
        public decimal? TotNeto { get; set; }

        public string EmpresaDepartamento { get; set; }
        public string ID_EmpresaDepartamento { get; set; }
        public string EmpresaProvincia { get; set; }
        public string ID_EmpresaProvincia { get; set; }
        public string EmpresaDistrito { get; set; }
        public string ID_EmpresaDistrito { get; set; }
        public string EmpresaRazonSocial { get; set; }
        public string EmpresaRUC { get; set; }
        public string EmpresaDireccion { get; set; }

        public string Cab_Clte_ID_RazonSocial { get; set; }
        public string Cab_Clte_IdDistrito { get; set; }
        public string Cab_Clte_Distrito { get; set; }
        public string Cab_Clte_IdProvincia { get; set; }
        public string Cab_Clte_Provincia { get; set; }
        public string Cab_Clte_IdDepartamento { get; set; }
        public string Cab_Clte_Departamento { get; set; }

        public string Cab_Ref_Serie { get; set; }
        public string Cab_Ref_Numero { get; set; }
        public string Cab_Ref_TipoDeDocumento { get; set; }
        public string Cab_Ref_Motivo { get; set; }
        public string Cab_Ref_TipoNotaCredito { get; set; }
        public string Cab_Ref_TipoNotaDebito { get; set; }
        public DateTime Cab_Ref_FechaEmision { get; set; }

        public string ClienteRazonSocial { get; set; }
        public string ClienteDireccion { get; set; }
        public string ClienteUbigeo { get; set; }
        public string ClienteTipodocumento { get; set; }
        public string ClienteNumeroDocumento { get; set; }

        public string FormaPago { get; set; }
        public int NumeroCuotas { get; set; }
        public decimal MontoCuota { get; set; }

        public virtual ICollection<Detalles> Detalles { get; set; }
    }
}
