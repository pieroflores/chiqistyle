using System;
using System.Linq;
//using System.Web.UI.WebControls;
using System.Data;
//Para generar imagen QR
using System.Drawing;
using System.Drawing.Imaging;
using ZXing.QrCode;
using ZXing;
using ZXing.SkiaSharp;
using SkiaSharp;


//Para enviar correos
using System.Net.Mail;
using System.Net;
using System.Xml;
using System.IO.Compression; 
//using WebVentas.Models;
//using ZXing.Core;


namespace MinimusaAPI.Util
{
    public static class Helper
    {
        public static string EnviarCorreo(string para, string asunto, string mensaje, 
                                                              string adjunto ,string emailcuenta ,string nombreCuenta , string password,
                                                              int puerto, string smtp)
        {
            //Crear un mensaje de correo
            MailMessage msje = new MailMessage();

            try
            {
                //De -- Quien envia el correo
                msje.From = new MailAddress(emailcuenta,  nombreCuenta);
                //Para 
                msje.To.Add(para);
                
                //Asunto
                msje.Subject = asunto;
                //Mensaje
                msje.Body = mensaje; 
                //formato del mensaje : Texto - HTML
                msje.IsBodyHtml = false;
                //Adjuntar el archivo al correo
                msje.Attachments.Add(new Attachment(adjunto));                
                ///definir las credenciales de la cuenta utlizada para enviar los correos
                NetworkCredential credenciales = new NetworkCredential();
                credenciales.UserName = emailcuenta;
                credenciales.Password = password; 
                //Definir el servidor de correo de salida
                SmtpClient servidor = new SmtpClient();
                servidor.Host = smtp;
                servidor.Port =puerto; 
                servidor.Credentials = credenciales;
                //Habilitamos el modo de envio seguro que es obligatorio  para Hotmail
                servidor.EnableSsl = true;
                //Enviar el mensaje al cliente
                servidor.Send(msje);
                return "Enviado";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
     
        public static void GenerarQR(string RUC,
                                                   string Tipocomprobante,
                                                   string Numeración,
                                                   string SumatoriaIGV,
                                                   string ImporteTotalVenta,
                                                   string FechaEmisión,
                                                   string TipoDocumentoAdquirente,
                                                   string NúmeroDocumentoAdquirente,
                                                   string CódigoHash , string Path ,  int id)
        {            
            if (String.IsNullOrWhiteSpace(RUC) || String.IsNullOrEmpty(Tipocomprobante) || String.IsNullOrEmpty(Numeración) ||
                 String.IsNullOrWhiteSpace(SumatoriaIGV) || String.IsNullOrEmpty(ImporteTotalVenta) || String.IsNullOrEmpty(FechaEmisión) ||
                 String.IsNullOrWhiteSpace(ImporteTotalVenta) || String.IsNullOrWhiteSpace(FechaEmisión) || String.IsNullOrWhiteSpace(TipoDocumentoAdquirente) ||
                  String.IsNullOrWhiteSpace(NúmeroDocumentoAdquirente) || String.IsNullOrWhiteSpace(CódigoHash))
            {
                throw new Exception("Debe proporcionar todos los parametros para la generación del QR");
            }
           else
                {
                  string codigo = RUC + "|" + Tipocomprobante + "|" + Numeración + "|" + SumatoriaIGV + "|" + ImporteTotalVenta + "|" + FechaEmisión + "|" +
                                                         TipoDocumentoAdquirente + "|" + NúmeroDocumentoAdquirente + "|" + CódigoHash;

                  QrCodeEncodingOptions options = new QrCodeEncodingOptions();                
                  options = new QrCodeEncodingOptions
                  {
                  DisableECI = true,
                  CharacterSet = "UTF-8",
                  Width = 250,
                  Height = 250,
                   };

                //var qr = new ZXing.ZKWeb.BarcodeWriter();
                var qr = new ZXing.BarcodeWriter<Bitmap>
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = options
                };
                qr.Options = options;
                qr.Format = ZXing.BarcodeFormat.QR_CODE;                
                var result = qr.Write(codigo);
                string ruta = Path + id + ".jpg";
                result.Save(ruta);                
            }
        }
                         
        public static string Right(string original, int numberCharacters)
        {
            return original.Substring(original.Length - numberCharacters);
        }

        public static string ObtenerRespuestaSunat(string ruta)
        {
            System.IO.FileInfo arch = new System.IO.FileInfo(ruta);
            if (arch.Extension == ".xml")

            {
                XmlDocument oXmlSunat = new XmlDocument();
                oXmlSunat.Load(ruta);
                string oResult = "";
                XmlNamespaceManager manager = new XmlNamespaceManager(oXmlSunat.NameTable);
                manager.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                manager.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                string valor = oXmlSunat.SelectSingleNode("//cac:DocumentResponse/cac:Response/cbc:ResponseCode", manager).InnerText;
                if (valor == "0")
                    oResult = "Aceptado por SUNAT";
                else
                    oResult = "Rechazado por SUNAT";

                return oResult;
            }
            else
            {
                return "";
            }
        }

        //public static string ObtenerValorParam(string categoria, string nombre)
        //{
        //    using (BD_VENTASContext _db = new BD_VENTASContext())
        //    {
        //        var valor = _db.Parametros.Where(p => p.Categoria == categoria && p.Nombre == nombre).FirstOrDefault().Valor;
        //        return valor;
        //    }
        //}
        public static string[] LeerRepuestaCDR(string ruta, string nomfile,string ruccliente)
        {
            string r = "";
            string file = "";
            string[] datos = new string[3];

            string ruc = ruccliente;

            try
            {
                using (ZipArchive zip = ZipFile.Open(ruta, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry zentry = null;
                    file = zip.Entries[1].ToString();
                    zentry = zip.GetEntry(file);
                    XmlDocument xd = new XmlDocument();
                    xd.Load(zentry.Open());
                    XmlNodeList xnl = xd.GetElementsByTagName("cbc:Description");
                    foreach (XmlElement item in xnl)
                    {
                        r = item.InnerText;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            datos[0] = r;
            datos[1] = file;
            datos[2] = nomfile;
            return datos;
        }

        public static string[] ObtenerRespuestaZIPSunat(string ruta , string ruccliente)
        {
            System.IO.FileInfo arch = new System.IO.FileInfo(ruta);

            if (arch.Extension == ".zip")
            {
                return LeerRepuestaCDR(ruta, System.IO.Path.GetFileName(ruta), ruccliente);
            }
            else
            {
                return null;
            }
        }

        public static string ObtenerDigestValueSunat(string archxml)
        {
            XmlDocument oXmlSunat = new XmlDocument();
            oXmlSunat.Load(archxml);
            
            XmlNamespaceManager manager = new XmlNamespaceManager(oXmlSunat.NameTable);
            manager.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            manager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            string valor = oXmlSunat.SelectSingleNode("//ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent/ds:Signature/ds:SignedInfo/ds:Reference/ds:DigestValue", manager).InnerText;
            return valor;
        }
        public static string ObtenerSignatureValueSunat(XmlDocument oXmlSunat)
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(oXmlSunat.NameTable);
            manager.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            manager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            string valor = oXmlSunat.SelectSingleNode("//ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent/ds:Signature/ds:SignatureValue", manager).InnerText;
            return valor;
        }
        public static string ObtenerNombreArchivoSummary(XmlDocument oXmlSunat)
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(oXmlSunat.NameTable);
            manager.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            string valor = oXmlSunat.SelectSingleNode("//cbc:ID", manager).InnerText;
            return valor;
        }

        public static string NumeroDocumentoSunatVoid(string RucEmpresa, string sCorrelativo)
        {
            string[] sValues =
                {   RucEmpresa,
                    "RA-" + DateTime.Now.Date.ToString("yyyyMMdd"),
                    sCorrelativo
                };

            string cNroDocumentoVoid = string.Format("{0}-{1}-{2}", sValues);
            return cNroDocumentoVoid;

        }
        public static string NumeroDocumentoSunatSummary(string RucEmpresa, string sCorrelativo)
        {
            string[] sValues =
                {   RucEmpresa,
                    "RC-" + DateTime.Now.Date.ToString("yyyyMMdd"),
                    sCorrelativo
                };

            string cNroDocumentoSummary = string.Format("{0}-{1}-{2}", sValues);
            return cNroDocumentoSummary;

        }

        public static string NumeroDocumentoSunatReversion(string RucEmpresa, string sCorrelativo)
        {
            string[] sValues =
                {   RucEmpresa,
                    "RR-" + DateTime.Now.Date.ToString("yyyyMMdd"),
                    sCorrelativo
                };

            string cNroDocumentoVoid = string.Format("{0}-{1}-{2}", sValues);
            return cNroDocumentoVoid;

        }


    }
}




