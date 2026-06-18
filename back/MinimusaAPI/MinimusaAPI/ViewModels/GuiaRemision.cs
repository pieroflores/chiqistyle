using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimusaAPI.ViewModels
{
  public  class GuiaRemision : GUIA_REMISION
    {
        public string EmpresaDepartamento { get; set; }
        public string ID_EmpresaDepartamento { get; set; }
        public string EmpresaProvincia { get; set; }
        public string ID_EmpresaProvincia { get; set; }
        public string EmpresaDistrito { get; set; }
        public string ID_EmpresaDistrito { get; set; }

        public string EmpresaRazonSocial { get; set; }
        public string EmpresaRUC { get; set; }
        public string EmpresaDireccion { get; set; }
    }
}
