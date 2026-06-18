using System;
using System.Collections.Generic;

namespace MinimusaAPI.ViewModels
{
    public partial class Detalles
    {
        public int Idcab { get; set; }
        public string Codcom { get; set; }
        public string DescripcionProducto { get; set; }
        public string UnidadMedida{ get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
        public decimal mtoValorVentaItem { get; set; }
        public decimal porIgvItem { get; set; }        
        public bool? Activo { get; set; }
        public string Usuario { get; set; }
        public decimal? Igv { get; set; }
        public decimal? Descuento { get; set; }
      
    }
}
