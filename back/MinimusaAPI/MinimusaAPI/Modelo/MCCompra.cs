namespace MinimusaAPI.Modelo
{
    public class MCCompra
    {
        public class MSubProductoCombo
        {
            public int IdSubProducto { get; set; }
            public string ProductoVariable { get; set; } // Mapea a 'productoVariable'
        }
        public class MProductosCombo
        {
            public int IdProductoPrincipal { get; set; }
            public string NombreProducto { get; set; }
        }

        // Modelo 2: Ubicaciones (para el combo de Ubicación Específica)
        public class MAlmacenCombo
        {
            public int IdAlmacen { get; set; }
            public string Ubicacion { get; set; } // Mapea a 'ubicacion'
        }

        // Modelo 3: Tipo de Documento
        public class MTipoDocumento
        {
            public int IdTipoDocumentoMercaderia { get; set; } // Asumo este nombre por convención
            public string Nombre { get; set; } // O el nombre de la columna que obtienes
        }

        // Modelo 4: Contenedor para los 3 resultados (la respuesta final de la API)
        public class MDatosComboCompra
        {
            public List<MSubProductoCombo> Productos { get; set; } = new List<MSubProductoCombo>();
            public List<MAlmacenCombo> Ubicaciones { get; set; } = new List<MAlmacenCombo>();
            public List<MTipoDocumento> TiposDocumento { get; set; } = new List<MTipoDocumento>();
            public List<MSubProductoCombo> ProductoDisponibles { get; set; } = new List<MSubProductoCombo>();
            public List<MProductosCombo> ProductoPrincipal { get; set; } = new List<MProductosCombo>();
        }
        public class MCompra
        {
            public int idProveedor { get; set; }
            public DateTime fechaCompra { get; set; }
            public int idUsuario { get; set; }
            public string documentoReferencia { get; set; }
            public string observacion { get; set; }
            public List<MCompraDetalle> detalle { get; set; }
        }
        public class SubProductoDTO
        {
            public int IdProductoPrincipal { get; set; }
            public int IdSubProducto { get; set; }
            public string Talla { get; set; }
            public string NombreColor { get; set; }
            public string DescripcionSubproducto { get; set; }
            public decimal PrecioCompra { get; set; }


        }


        public class MCompraDetalle
        {
            public int idSubProducto { get; set; }
            public int cantidad { get; set; }
            public decimal precioUnitario { get; set; }
            public int idAlmacen { get; set; }
        }
    }
}
