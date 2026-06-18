using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Runtime.InteropServices;
using System.Net;
//using MinimusaAPI.ModelosSunat;
using MinimusaAPI.ModelosSunat;
//using MinimusaAPI.Resumenes;
using MinimusaAPI.Resumenes;
using MinimusaAPI.ViewModels;
using ServiceGUIA;
using System.Threading.Tasks;
using System.Drawing;
using System.ServiceModel.Description;
using System.Net.Http.Headers;
using System.Drawing.Imaging;
using ZXing.QrCode.Internal;
using MinimusaAPI.Guias;
using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.InteropServices.ComTypes;


namespace MinimusaAPI.Servicios
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("firmadoCE.firmado")]
    public  class SUNAT_UTIL
    {

        public string RUCEmpresa { get; set; }
        public  string RazonSocialEmpresa { get; set; }
        public  string Ruta_XML { get; set; }
        public  string Ruta_Certificado { get; set; }
        public  string Password_Certificado { get; set; }
        public  string Ruta_ENVIOS { get; set; }
        public  string Ruta_CDRS { get; set; }
        public string Ruta_QR { get; set; }

        public int GenerarComprobanteFB_XML(Cabecera Comprobante)
        {
            //Carga la clase invoice
            MinimusaAPI.ModelosSunat.InvoiceType Factura = new MinimusaAPI.ModelosSunat.InvoiceType();

            try
            {
                //------Namespace necesarios para el UBL
                Factura.Cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                Factura.Cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                Factura.Ccts = "urn:un:unece:uncefact:documentation:2";
                Factura.Ds = "http://www.w3.org/2000/09/xmldsig#";
                Factura.Ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";
                Factura.Qdt = "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2";
                Factura.Udt = "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2";
                //------                

                MinimusaAPI.ModelosSunat.UBLExtensionType[] ublExtensiones = new MinimusaAPI.ModelosSunat.UBLExtensionType[5];
                MinimusaAPI.ModelosSunat.UBLExtensionType ublExtension = new MinimusaAPI.ModelosSunat.UBLExtensionType();
                ublExtensiones[0] = ublExtension;
                Factura.UBLExtensions = ublExtensiones;

                Factura.UBLVersionID = new MinimusaAPI.ModelosSunat.UBLVersionIDType();
                Factura.UBLVersionID.Value = "2.1";

                Factura.CustomizationID = new MinimusaAPI.ModelosSunat.CustomizationIDType();
                //Factura.CustomizationID.schemeAgencyName = "PE:SUNAT";
                Factura.CustomizationID.Value = "2.0";
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Código de tipo de operación                
                Factura.ID = new MinimusaAPI.ModelosSunat.IDType();
                Factura.ID.Value = Comprobante.Serie + "-" + Comprobante.Numero;
                //Fecha de emisión y hora de emision
                Factura.IssueDate = new MinimusaAPI.ModelosSunat.IssueDateType();
                string fechaemision = Convert.ToDateTime(Comprobante.Fechaemision).ToString("dd/MM/yyyy");
                Factura.IssueDate.Value = Convert.ToDateTime(fechaemision);
                Factura.IssueTime = new MinimusaAPI.ModelosSunat.IssueTimeType();
                //   string hora = Convert.ToDateTime(Comprobante.Cab_Fac_Fecha).ToString("HH:mm:ss");
                Factura.IssueTime.Value = DateTime.Now.ToString("HH:mm:ss");
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Fecha de Vencimiento 
                Factura.DueDate = new MinimusaAPI.ModelosSunat.DueDateType();
                Factura.DueDate.Value = Convert.ToDateTime(Comprobante.Fechavencimiento);
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Tipo de documento(Factura)
                MinimusaAPI.ModelosSunat.InvoiceTypeCodeType TipoDoc = new MinimusaAPI.ModelosSunat.InvoiceTypeCodeType();

                TipoDoc.listSchemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo51";
                TipoDoc.name = "Tipo de Operacion";
                TipoDoc.listURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo01";
                TipoDoc.listName = "Tipo de Documento";
                TipoDoc.listAgencyName = "PE:SUNAT";
                TipoDoc.Value = Comprobante.Idtipocomp.ToString();
                TipoDoc.listID = "0101";

                Factura.InvoiceTypeCode = TipoDoc;

                //Leyenda del comprobante
                MinimusaAPI.ModelosSunat.NoteType Leyenda = new MinimusaAPI.ModelosSunat.NoteType();
                Leyenda.languageLocaleID = "1000";
                Leyenda.Value = "MONTO EN SOLES";
                List<MinimusaAPI.ModelosSunat.NoteType> notasLeyenda = new List<ModelosSunat.NoteType>();
                notasLeyenda.Add(Leyenda);
                Factura.Note = notasLeyenda.ToArray();

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Tipo de Moneda
                MinimusaAPI.ModelosSunat.DocumentCurrencyCodeType moneda = new MinimusaAPI.ModelosSunat.DocumentCurrencyCodeType()
                {
                    //listSchemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo51",
                    listID = "ISO 4217 Alpha",
                    listName = "Currency",
                    listAgencyName = "United Nations Economic Commission for Europe",
                    Value = Comprobante.Idmoneda
                };

                Factura.DocumentCurrencyCode = moneda;
                ModelosSunat.LineCountNumericType numitems = new ModelosSunat.LineCountNumericType();
                numitems.Value = Comprobante.Detalles.Count;
                //Factura.LineCountNumeric = numitems;

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                //Nombre Comercial del emisor Apellidos y nombres, denominación o razón social del emisor Tipo y Número de RUC del emisor Código del domicilio fiscal 
                //o de local anexo del emisor 
                MinimusaAPI.ModelosSunat.SignatureType Firma = new MinimusaAPI.ModelosSunat.SignatureType();
                MinimusaAPI.ModelosSunat.SignatureType[] Firmas = new MinimusaAPI.ModelosSunat.SignatureType[2];

                MinimusaAPI.ModelosSunat.PartyType partySign = new MinimusaAPI.ModelosSunat.PartyType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType partyIdentificacion = new MinimusaAPI.ModelosSunat.PartyIdentificationType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType[] partyIdentificacions = new MinimusaAPI.ModelosSunat.PartyIdentificationType[2];
                MinimusaAPI.ModelosSunat.IDType idFirma = new MinimusaAPI.ModelosSunat.IDType();
                idFirma.Value = Comprobante.EmpresaRUC;
                Firma.ID = idFirma;

                partyIdentificacion.ID = idFirma;
                partyIdentificacions[0] = partyIdentificacion;
                partySign.PartyIdentification = partyIdentificacions;
                Firma.SignatoryParty = partySign;

                MinimusaAPI.ModelosSunat.NoteType Nota = new MinimusaAPI.ModelosSunat.NoteType();
                MinimusaAPI.ModelosSunat.NoteType[] Notas = new MinimusaAPI.ModelosSunat.NoteType[2];
                Nota.Value = "Elaborado por Sistema de Emision Electronica NET SOLUTION DEVELOPER ";
                Notas[0] = Nota;
                Firma.Note = Notas;

                MinimusaAPI.ModelosSunat.PartyNameType partyName = new MinimusaAPI.ModelosSunat.PartyNameType();
                MinimusaAPI.ModelosSunat.PartyNameType[] partyNames = new MinimusaAPI.ModelosSunat.PartyNameType[2];

                MinimusaAPI.ModelosSunat.NameType1 RazonSocialFirma = new MinimusaAPI.ModelosSunat.NameType1();
                RazonSocialFirma.Value = Comprobante.EmpresaRazonSocial;
                partyName.Name = RazonSocialFirma;
                partyNames[0] = partyName;
                partySign.PartyName = partyNames;

                MinimusaAPI.ModelosSunat.AttachmentType attachType = new MinimusaAPI.ModelosSunat.AttachmentType();
                MinimusaAPI.ModelosSunat.ExternalReferenceType externaReferencia = new MinimusaAPI.ModelosSunat.ExternalReferenceType();
                MinimusaAPI.ModelosSunat.URIType uri = new MinimusaAPI.ModelosSunat.URIType();
                uri.Value = Comprobante.EmpresaRUC;
                externaReferencia.URI = uri;
                Firma.DigitalSignatureAttachment = attachType;

                attachType.ExternalReference = externaReferencia;
                Firma.DigitalSignatureAttachment = attachType;

                Firmas[0] = Firma;
                Factura.Signature = Firmas;

                MinimusaAPI.ModelosSunat.SupplierPartyType empresa = new MinimusaAPI.ModelosSunat.SupplierPartyType();
                MinimusaAPI.ModelosSunat.PartyType party = new MinimusaAPI.ModelosSunat.PartyType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType partyidentificacion = new MinimusaAPI.ModelosSunat.PartyIdentificationType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType[] partyidentificacions = new MinimusaAPI.ModelosSunat.PartyIdentificationType[2];
                MinimusaAPI.ModelosSunat.IDType idEmpresa = new MinimusaAPI.ModelosSunat.IDType();
                idEmpresa.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idEmpresa.schemeName = "Documento de Identidad";
                idEmpresa.schemeID = "6";
                idEmpresa.schemeAgencyName = "PE:SUNAT";
                idEmpresa.Value = Comprobante.EmpresaRUC;

                partyidentificacion.ID = idEmpresa;
                partyidentificacions[0] = partyidentificacion;
                party.PartyIdentification = partyidentificacions;

                MinimusaAPI.ModelosSunat.PartyNameType partyname = new MinimusaAPI.ModelosSunat.PartyNameType();
                List<MinimusaAPI.ModelosSunat.PartyNameType> partynames = new List<MinimusaAPI.ModelosSunat.PartyNameType>();
                MinimusaAPI.ModelosSunat.NameType1 nameEmisor = new MinimusaAPI.ModelosSunat.NameType1();
                nameEmisor.Value = Comprobante.EmpresaRazonSocial;
                partyname.Name = nameEmisor;
                partynames.Add(partyname);
                party.PartyName = partynames.ToArray();

                MinimusaAPI.ModelosSunat.PartyTaxSchemeType PartyTaxScheme = new MinimusaAPI.ModelosSunat.PartyTaxSchemeType();
                List<MinimusaAPI.ModelosSunat.PartyTaxSchemeType> PartyTaxSchemes = new List<MinimusaAPI.ModelosSunat.PartyTaxSchemeType>();

                MinimusaAPI.ModelosSunat.RegistrationNameType registerNameEmisor = new MinimusaAPI.ModelosSunat.RegistrationNameType();
                registerNameEmisor.Value = Comprobante.EmpresaRazonSocial;
                PartyTaxScheme.RegistrationName = registerNameEmisor;
                //Direccion emisor                
                ModelosSunat.CompanyIDType compañia = new ModelosSunat.CompanyIDType();
                compañia.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                compañia.schemeAgencyName = "PE:SUNAT";
                compañia.schemeName = "SUNAT:Identificador de Documento de Identidad";
                compañia.schemeID = "6";
                compañia.Value = Comprobante.EmpresaRUC;

                MinimusaAPI.ModelosSunat.AddressType direccion = new MinimusaAPI.ModelosSunat.AddressType();
                MinimusaAPI.ModelosSunat.AddressTypeCodeType addrestypecode = new MinimusaAPI.ModelosSunat.AddressTypeCodeType();
                addrestypecode.listName = "Establecimientos anexos";
                addrestypecode.listAgencyName = "PE:SUNAT";
                addrestypecode.Value = "0000";
                direccion.AddressTypeCode = addrestypecode;
                PartyTaxScheme.RegistrationAddress = direccion;
                //ModelosSunat.IDType tipo = new ModelosSunat.IDType();
                ModelosSunat.TaxSchemeType taxSchema = new ModelosSunat.TaxSchemeType();
                ModelosSunat.IDType idsupplier = new ModelosSunat.IDType();
                idsupplier.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idsupplier.schemeAgencyName = "PE:SUNAT";
                idsupplier.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idsupplier.schemeID = "6";
                idsupplier.Value = Comprobante.EmpresaRUC;
                taxSchema.ID = idsupplier;

                PartyTaxScheme.CompanyID = compañia;
                PartyTaxScheme.TaxScheme = taxSchema;
                PartyTaxSchemes.Add(PartyTaxScheme);
                //party.PartyTaxScheme = PartyTaxSchemes.ToArray();

                List<ModelosSunat.PartyLegalEntityType> partelegals = new List<ModelosSunat.PartyLegalEntityType>();
                ModelosSunat.PartyLegalEntityType partelegal = new ModelosSunat.PartyLegalEntityType();
                MinimusaAPI.ModelosSunat.RegistrationNameType registerNamePL = new MinimusaAPI.ModelosSunat.RegistrationNameType();
                registerNamePL.Value = Comprobante.EmpresaRazonSocial;
                partelegal.RegistrationName = registerNamePL;

                ModelosSunat.AddressType direccionPL = new ModelosSunat.AddressType();
                ModelosSunat.IDType iddireccionPL = new ModelosSunat.IDType();
                iddireccionPL.schemeAgencyName = "PE:INEI";
                iddireccionPL.schemeName = "Ubigeos";
                iddireccionPL.Value = Comprobante.ID_EmpresaDepartamento + Comprobante.ID_EmpresaProvincia + Comprobante.ID_EmpresaDistrito;
                direccionPL.ID = iddireccionPL;

                ModelosSunat.AddressTypeCodeType address_TypeCodeType = new ModelosSunat.AddressTypeCodeType();
                address_TypeCodeType.listName = "Establecimientos anexos";
                address_TypeCodeType.listAgencyName = "PE:SUNAT";
                address_TypeCodeType.Value = "0001";
                direccionPL.AddressTypeCode = address_TypeCodeType;

                MinimusaAPI.ModelosSunat.CityNameType Departamento = new MinimusaAPI.ModelosSunat.CityNameType();
                Departamento.Value = Comprobante.EmpresaDepartamento;
                direccionPL.CityName = Departamento;

                MinimusaAPI.ModelosSunat.CountrySubentityType Provincia = new MinimusaAPI.ModelosSunat.CountrySubentityType();
                Provincia.Value = Comprobante.EmpresaProvincia;
                direccionPL.CountrySubentity = Provincia;

                MinimusaAPI.ModelosSunat.DistrictType distrito = new MinimusaAPI.ModelosSunat.DistrictType();
                distrito.Value = Comprobante.EmpresaDistrito;
                direccionPL.District = distrito;
                List<ModelosSunat.AddressLineType> direcciones = new List<ModelosSunat.AddressLineType>();
                ModelosSunat.AddressLineType direccionEmisor = new ModelosSunat.AddressLineType();
                ModelosSunat.LineType local1 = new ModelosSunat.LineType();
                local1.Value = Comprobante.EmpresaDireccion;
                direccionPL.AddressLine = direcciones.ToArray();
                direccionEmisor.Line = local1;
                direcciones.Add(direccionEmisor);
                direccionPL.AddressLine = direcciones.ToArray();

                MinimusaAPI.ModelosSunat.CountryType pais = new MinimusaAPI.ModelosSunat.CountryType();
                MinimusaAPI.ModelosSunat.IdentificationCodeType codigoPais = new MinimusaAPI.ModelosSunat.IdentificationCodeType();

                codigoPais.listName = "Country";
                codigoPais.listAgencyName = "United Nations Economic Commission for Europe";
                codigoPais.listID = "ISO 3166-1";
                codigoPais.Value = "PE";
                pais.IdentificationCode = codigoPais;

                direccionPL.Country = pais;
                partelegal.RegistrationAddress = direccionPL;

                partelegals.Add(partelegal);
                party.PartyLegalEntity = partelegals.ToArray();


                party.PartyName = partynames.ToArray();
                party.PartyIdentification = partyidentificacions;
                empresa.Party = party;
                Factura.AccountingSupplierParty = empresa;

                //EMPRESA CLIENTE
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                //Tipo y número de documento de identidad del adquirente o usuario Apellidos y nombres, denominación o razón social del adquirente o usuario
                MinimusaAPI.ModelosSunat.TaxSchemeType taxschemeCliente = new MinimusaAPI.ModelosSunat.TaxSchemeType();
                MinimusaAPI.ModelosSunat.CustomerPartyType CustomerPartyCliente = new MinimusaAPI.ModelosSunat.CustomerPartyType();
                MinimusaAPI.ModelosSunat.PartyType partyCliente = new MinimusaAPI.ModelosSunat.PartyType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType partyIdentificion = new MinimusaAPI.ModelosSunat.PartyIdentificationType();
                List<MinimusaAPI.ModelosSunat.PartyIdentificationType> partyIdentificions = new List<MinimusaAPI.ModelosSunat.PartyIdentificationType>();
                MinimusaAPI.ModelosSunat.IDType idtipo = new MinimusaAPI.ModelosSunat.IDType();
                idtipo.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idtipo.schemeName = "Documento de Identidad";
                idtipo.schemeAgencyName = "PE:SUNAT";
                idtipo.schemeID = Comprobante.ClienteTipodocumento;
                idtipo.Value = Comprobante.ClienteNumeroDocumento;
                partyIdentificion.ID = idtipo;

                partyIdentificions.Add(partyIdentificion);
                partyCliente.PartyIdentification = partyIdentificions.ToArray();

                List<MinimusaAPI.ModelosSunat.PartyNameType> RazSocClientes = new List<MinimusaAPI.ModelosSunat.PartyNameType>();
                MinimusaAPI.ModelosSunat.PartyNameType RazSocCliente = new MinimusaAPI.ModelosSunat.PartyNameType();
                ModelosSunat.NameType1 razSocial = new ModelosSunat.NameType1();
                razSocial.Value = Comprobante.ClienteRazonSocial;
                RazSocCliente.Name = razSocial;
                RazSocClientes.Add(RazSocCliente);
                //partyCliente.PartyName = RazSocClientes.ToArray();


                List<MinimusaAPI.ModelosSunat.PartyTaxSchemeType> partySchemas = new List<ModelosSunat.PartyTaxSchemeType>();
                MinimusaAPI.ModelosSunat.PartyTaxSchemeType partySchema = new MinimusaAPI.ModelosSunat.PartyTaxSchemeType();
                ModelosSunat.RegistrationNameType RegistroNombre = new ModelosSunat.RegistrationNameType();
                RegistroNombre.Value = Comprobante.ClienteRazonSocial;
                partySchema.RegistrationName = RegistroNombre;

                ModelosSunat.CompanyIDType idcompañia = new ModelosSunat.CompanyIDType();
                idcompañia.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idcompañia.schemeAgencyName = "PE:SUNAT";
                idcompañia.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idcompañia.schemeID = Comprobante.ClienteTipodocumento;
                idcompañia.Value = Comprobante.ClienteNumeroDocumento;

                ModelosSunat.TaxSchemeType schemeType = new ModelosSunat.TaxSchemeType();
                ModelosSunat.IDType idc = new ModelosSunat.IDType();
                idc.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idc.schemeAgencyName = "PE:SUNAT";
                idc.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idc.schemeID = Comprobante.ClienteTipodocumento;
                idc.Value = Comprobante.ClienteNumeroDocumento;
                schemeType.ID = idc;

                idcompañia.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idcompañia.schemeAgencyName = "PE:SUNAT";
                idcompañia.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idcompañia.schemeID = Comprobante.ClienteTipodocumento;
                idcompañia.Value = Comprobante.ClienteNumeroDocumento;

                List<ModelosSunat.PartyLegalEntityType> partyLegals = new List<ModelosSunat.PartyLegalEntityType>();
                ModelosSunat.PartyLegalEntityType partyLegal = new ModelosSunat.PartyLegalEntityType();
                ModelosSunat.RegistrationNameType Registro_Nombre = new ModelosSunat.RegistrationNameType();
                Registro_Nombre.Value = Comprobante.ClienteRazonSocial;
                partyLegal.RegistrationName = Registro_Nombre;

                ModelosSunat.AddressType direccionCliente = new ModelosSunat.AddressType();
                List<ModelosSunat.AddressLineType> dirs = new List<ModelosSunat.AddressLineType>();
                ModelosSunat.AddressLineType dir = new ModelosSunat.AddressLineType();
                List<ModelosSunat.LineType> lineas = new List<ModelosSunat.LineType>();

                ModelosSunat.LineType linea = new ModelosSunat.LineType();
                linea.Value = Comprobante.ClienteDireccion;
                dir.Line = linea;
                dirs.Add(dir);
                direccionCliente.AddressLine = dirs.ToArray();
                //partyLegal.RegistrationAddress = direccionCliente;

                MinimusaAPI.ModelosSunat.CountryType paisC = new MinimusaAPI.ModelosSunat.CountryType();
                MinimusaAPI.ModelosSunat.IdentificationCodeType codigoPaisC = new MinimusaAPI.ModelosSunat.IdentificationCodeType();

                //codigoPaisC.listName = "Country";
                //codigoPaisC.listAgencyName = "United Nations Economic Commission for Europe";
                //codigoPaisC.listID = "ISO 3166-1";
                codigoPaisC.Value = "PE";
                paisC.IdentificationCode = codigoPaisC;
                //   partyLegal.RegistrationAddress.Country = paisC;
                partyLegals.Add(partyLegal);

                partySchema.CompanyID = idcompañia;
                partySchema.TaxScheme = schemeType;

                partySchemas.Add(partySchema);
                //partyCliente.PartyTaxScheme = partySchemas.ToArray();
                partyCliente.PartyLegalEntity = partyLegals.ToArray();
                CustomerPartyCliente.Party = partyCliente;

                ModelosSunat.CustomerPartyType accoutingCustomerParty = new ModelosSunat.CustomerPartyType();
                accoutingCustomerParty.Party = partyCliente;
                ///CustomerPartyCliente = partyCliente;
                Factura.AccountingCustomerParty = accoutingCustomerParty;


                /////////////////////////////////////////////////////////////////////////////////////FORMA DE PAGO
                List<ModelosSunat.PaymentTermsType> pagosCuotas = new List<ModelosSunat.PaymentTermsType>();

                if (Comprobante.FormaPago.ToLower() == "credito")
                {
                    var formapago = new ModelosSunat.PaymentTermsType();
                    var idformapago = new ModelosSunat.IDType();
                    idformapago.Value = "FormaPago";
                    formapago.ID = idformapago;
                    var tipoformapagos = new List<ModelosSunat.PaymentMeansIDType>();
                    var tipoformapago = new ModelosSunat.PaymentMeansIDType();
                    tipoformapago.Value = "Credito";
                    tipoformapagos.Add(tipoformapago);
                    formapago.PaymentMeansID = tipoformapagos.ToArray();
                    var formapagotipoMoneda = new ModelosSunat.AmountType2();
                    formapagotipoMoneda.currencyID = Comprobante.Idmoneda;
                    formapagotipoMoneda.Value = Convert.ToDecimal(Comprobante.TotNeto);
                    formapago.Amount = formapagotipoMoneda;
                    pagosCuotas.Add(formapago);

                    for (int i = 1, loopTo = Comprobante.NumeroCuotas; i <= loopTo; i++)
                    {
                        var pagoCuota = new ModelosSunat.PaymentTermsType();
                        var idpagoCuota = new ModelosSunat.IDType();
                        var tipoMoneda = new ModelosSunat.AmountType2();
                        tipoMoneda.currencyID = Comprobante.Idmoneda;
                        tipoMoneda.Value = Comprobante.MontoCuota;
                        pagoCuota.Amount = tipoMoneda;
                        var fechapagocuota = new ModelosSunat.PaymentDueDateType();
                        fechapagocuota.Value = Comprobante.Fechaemision.Value.AddDays(30 * i);
                        pagoCuota.PaymentDueDate = fechapagocuota;
                        idpagoCuota.Value = "Cuota" + i.ToString().PadLeft(3, '0');
                        pagoCuota.ID = idpagoCuota;
                        pagosCuotas.Add(pagoCuota);
                    }

                    Factura.PaymentTerms = pagosCuotas.ToArray();
                }
                else
                {
                    var pagos = new List<ModelosSunat.PaymentTermsType>();
                    var pago = new ModelosSunat.PaymentTermsType();
                    var idpago = new ModelosSunat.IDType();
                    idpago.Value = "FormaPago";
                    pago.ID = idpago;
                    var tipopagos = new List<ModelosSunat.PaymentMeansIDType>();
                    var tipopago = new ModelosSunat.PaymentMeansIDType();
                    tipopago.Value = Comprobante.FormaPago;
                    tipopagos.Add(tipopago);
                    pago.PaymentMeansID = tipopagos.ToArray();
                    pagos.Add(pago);
                    Factura.PaymentTerms = pagos.ToArray();
                }






                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Monto total de impuestos
                //Monto las operaciones gravadas
                //Monto las operaciones Exoneradas
                //Monto las operaciones inafectas del impuesto(Ver Ejemplo en la página 47)
                //Monto las operaciones gratuitas(Ver Ejemplo en la página 48)
                //Sumatoria de IGV
                //Sumatoria de ISC(Ver Ejemplo en la página 51)
                //Sumatoria de Otros Tributos(Ver Ejemplo en la página 52)

                MinimusaAPI.ModelosSunat.TaxTotalType TotalImptos = new MinimusaAPI.ModelosSunat.TaxTotalType();
                MinimusaAPI.ModelosSunat.TaxAmountType taxAmountImpto = new MinimusaAPI.ModelosSunat.TaxAmountType();
                taxAmountImpto.currencyID = Comprobante.Idmoneda;
                taxAmountImpto.Value = Convert.ToDecimal(Comprobante.TotIgv);
                TotalImptos.TaxAmount = taxAmountImpto;
                //////////////////////////////////////////////////////////////////////////////
                ///
                List<MinimusaAPI.ModelosSunat.TaxSubtotalType> subtotales = new
                                                                   List<MinimusaAPI.ModelosSunat.TaxSubtotalType>();
                MinimusaAPI.ModelosSunat.TaxSubtotalType subtotal = new MinimusaAPI.ModelosSunat.TaxSubtotalType();

                MinimusaAPI.ModelosSunat.TaxableAmountType taxsubtotal = new MinimusaAPI.ModelosSunat.TaxableAmountType();
                taxsubtotal.currencyID = Comprobante.Idmoneda;
                taxsubtotal.Value = Convert.ToDecimal(Comprobante.TotSubtotal);
                subtotal.TaxableAmount = taxsubtotal;

                MinimusaAPI.ModelosSunat.TaxAmountType TotalTaxAmountTotal = new MinimusaAPI.ModelosSunat.TaxAmountType();
                TotalTaxAmountTotal.currencyID = Comprobante.Idmoneda;
                TotalTaxAmountTotal.Value = Convert.ToDecimal(Comprobante.TotIgv);
                subtotal.TaxAmount = TotalTaxAmountTotal;

                ModelosSunat.TaxSubtotalType subTotalIGV = new ModelosSunat.TaxSubtotalType();
                subTotalIGV.TaxableAmount = taxsubtotal;

                subtotales.Add(subtotal);
                TotalImptos.TaxSubtotal = subtotales.ToArray();


                //PAgo de IGV
                MinimusaAPI.ModelosSunat.TaxCategoryType taxcategoryTotal = new MinimusaAPI.ModelosSunat.TaxCategoryType();
                MinimusaAPI.ModelosSunat.TaxSchemeType taxScheme = new MinimusaAPI.ModelosSunat.TaxSchemeType();
                MinimusaAPI.ModelosSunat.IDType idTotal = new MinimusaAPI.ModelosSunat.IDType();
                idTotal.schemeID = "UN/ECE 5305";
                idTotal.schemeName = "Tax Category Identifier";
                idTotal.schemeAgencyName = "United Nations Economic Commission for Europe";
                idTotal.Value = "S";
                //taxcategoryTotal.ID = idTotal;
                MinimusaAPI.ModelosSunat.NameType1 nametypeImpto = new MinimusaAPI.ModelosSunat.NameType1();
                nametypeImpto.Value = "IGV";
                MinimusaAPI.ModelosSunat.TaxTypeCodeType taxtypecodeImpto = new MinimusaAPI.ModelosSunat.TaxTypeCodeType();
                taxtypecodeImpto.Value = "VAT";

                MinimusaAPI.ModelosSunat.IDType idTot = new MinimusaAPI.ModelosSunat.IDType();
                //idTot.schemeID = "UN/ECE 5153";
                //idTot.schemeAgencyID = "6";
                idTot.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05";
                idTot.schemeAgencyName = "PE:SUNAT";
                idTot.schemeName = "Codigo de tributos";
                idTot.Value = "1000";
                taxScheme.ID = idTot;

                MinimusaAPI.ModelosSunat.NameType1 nametypeImptoIGV = new MinimusaAPI.ModelosSunat.NameType1();
                nametypeImptoIGV.Value = "IGV";
                MinimusaAPI.ModelosSunat.TaxTypeCodeType taxtypecodeImpuesto = new MinimusaAPI.ModelosSunat.TaxTypeCodeType();
                taxtypecodeImpuesto.Value = "VAT";

                taxScheme.Name = nametypeImpto;
                taxScheme.TaxTypeCode = taxtypecodeImpto;
                taxcategoryTotal.TaxScheme = taxScheme;
                subtotal.TaxCategory = taxcategoryTotal;

                List<MinimusaAPI.ModelosSunat.TaxSubtotalType> TaxSubtotals = new List<MinimusaAPI.ModelosSunat.TaxSubtotalType>();
                TaxSubtotals.Add(subtotal);

                //Si tiene Bolsa entonces                 
                foreach (Detalles det in Comprobante.Detalles)
                {
                    if (det.Codcom.Contains("BBBB"))
                    {
                        ModelosSunat.TaxSubtotalType TotalICBPER = new ModelosSunat.TaxSubtotalType();
                        ModelosSunat.TaxAmountType taxICBPER = new ModelosSunat.TaxAmountType();
                        taxICBPER.currencyID = "PEN";
                        //Calcular el impuesto a la bolsa                                     
                        taxICBPER.Value = Math.Round((det.Cantidad * 0.20m), 2);
                        TotalICBPER.TaxAmount = taxICBPER;
                        ModelosSunat.TaxCategoryType taxCategoria = new ModelosSunat.TaxCategoryType();
                        ModelosSunat.TaxSchemeType taxSchemaicb = new ModelosSunat.TaxSchemeType();
                        ModelosSunat.IDType idTaschema = new ModelosSunat.IDType();
                        idTaschema.Value = "7152";
                        ModelosSunat.NameType1 nombreICB = new ModelosSunat.NameType1();
                        nombreICB.Value = "ICBPER";
                        ModelosSunat.TaxTypeCodeType taxtypecodeICPER = new ModelosSunat.TaxTypeCodeType();
                        taxtypecodeICPER.Value = "OTH";

                        taxSchemaicb.ID = idTaschema;
                        taxSchemaicb.Name = nombreICB;
                        taxSchemaicb.TaxTypeCode = taxtypecodeICPER;

                        taxCategoria.TaxScheme = taxSchemaicb;
                        TotalICBPER.TaxCategory = taxCategoria;
                        TaxSubtotals.Add(TotalICBPER);
                        break;
                    }
                }

                TotalImptos.TaxSubtotal = TaxSubtotals.ToArray();
                List<MinimusaAPI.ModelosSunat.TaxTotalType> taxTotals = new List<MinimusaAPI.ModelosSunat.TaxTotalType>();
                taxTotals.Add(TotalImptos);
                Factura.TaxTotal = taxTotals.ToArray();
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////                                  
                ////Total valor de venta 
                ////Total precio de venta(incluye impuestos) 
                ////Monto total de descuentos del comprobante 
                ////Monto total de otros cargos del comprobante 
                ////Importe total de la venta, cesión en uso o del servicio prestado
                MinimusaAPI.ModelosSunat.MonetaryTotalType TotalValorVenta = new MinimusaAPI.ModelosSunat.MonetaryTotalType();
                MinimusaAPI.ModelosSunat.LineExtensionAmountType TValorVenta = new MinimusaAPI.ModelosSunat.LineExtensionAmountType();

                TValorVenta.currencyID = Comprobante.Idmoneda;
                TValorVenta.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.TotSubtotal));
                TotalValorVenta.LineExtensionAmount = TValorVenta;

                MinimusaAPI.ModelosSunat.TaxInclusiveAmountType TotalPrecioVenta = new MinimusaAPI.ModelosSunat.TaxInclusiveAmountType();
                TotalPrecioVenta.currencyID = Comprobante.Idmoneda;
                TotalPrecioVenta.Value = Convert.ToDecimal(Comprobante.Total);
                //TotalValorVenta.TaxInclusiveAmount = TotalPrecioVenta;

                MinimusaAPI.ModelosSunat.AllowanceTotalAmountType MtoTotalDsctos = new MinimusaAPI.ModelosSunat.AllowanceTotalAmountType();
                MtoTotalDsctos.currencyID = Comprobante.Idmoneda;
                MtoTotalDsctos.Value = Convert.ToDecimal(Comprobante.TotDsctos);
                //TotalValorVenta.AllowanceTotalAmount = MtoTotalDsctos;

                MinimusaAPI.ModelosSunat.ChargeTotalAmountType MtoTotalOtrosCargos = new MinimusaAPI.ModelosSunat.ChargeTotalAmountType();
                MtoTotalOtrosCargos.currencyID = Comprobante.Idmoneda;
                MtoTotalOtrosCargos.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.TotOtros));
                TotalValorVenta.ChargeTotalAmount = MtoTotalOtrosCargos;

                MinimusaAPI.ModelosSunat.PrepaidAmountType MtoCargos = new MinimusaAPI.ModelosSunat.PrepaidAmountType();
                MtoCargos.currencyID = Comprobante.Idmoneda;
                MtoCargos.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.TotOtros));
                MtoCargos.Value = Convert.ToDecimal(string.Format("{0:0.00}", 0));
                TotalValorVenta.PrepaidAmount = MtoCargos;

                MinimusaAPI.ModelosSunat.PayableAmountType ImporteTotalVenta = new MinimusaAPI.ModelosSunat.PayableAmountType();
                ImporteTotalVenta.currencyID = Comprobante.Idmoneda;
                ImporteTotalVenta.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.Total));

                TotalValorVenta.LineExtensionAmount = TValorVenta;
                TotalValorVenta.TaxInclusiveAmount = TotalPrecioVenta;
                TotalValorVenta.AllowanceTotalAmount = MtoTotalDsctos;
                TotalValorVenta.ChargeTotalAmount = MtoTotalOtrosCargos;
                TotalValorVenta.PrepaidAmount = MtoCargos;
                TotalValorVenta.PayableAmount = ImporteTotalVenta;
                Factura.LegalMonetaryTotal = TotalValorVenta;

                //Número de orden del Ítem 
                //Cantidad y Unidad de medida por ítem 
                //Valor de venta del ítem
                //Items de Factura
                List<MinimusaAPI.ModelosSunat.InvoiceLineType> items = new
                                                                                                List<MinimusaAPI.ModelosSunat.InvoiceLineType>();
                int iditem = 1;

                foreach (Detalles det in Comprobante.Detalles)
                {
                    MinimusaAPI.ModelosSunat.InvoiceLineType item = new MinimusaAPI.ModelosSunat.InvoiceLineType();
                    MinimusaAPI.ModelosSunat.IDType numeroItem = new MinimusaAPI.ModelosSunat.IDType();
                    numeroItem.Value = iditem.ToString();
                    item.ID = numeroItem;

                    MinimusaAPI.ModelosSunat.InvoicedQuantityType cantidad = new MinimusaAPI.ModelosSunat.InvoicedQuantityType();
                    cantidad.unitCodeListAgencyName = "United Nations Economic Commission for Europe";
                    cantidad.unitCodeListID = "UN/ECE rec 20";
                    cantidad.unitCode = det.UnidadMedida;
                    item.InvoicedQuantity = cantidad;
                    cantidad.Value = Convert.ToInt32(det.Cantidad);
                    item.InvoicedQuantity = cantidad;

                    MinimusaAPI.ModelosSunat.LineExtensionAmountType ValorVenta = new MinimusaAPI.ModelosSunat.LineExtensionAmountType();
                    ValorVenta.currencyID = Comprobante.Idmoneda;
                    ValorVenta.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.Total / 1.18m));
                    item.LineExtensionAmount = ValorVenta;

                    //Precio de venta unitario por item y código 
                    MinimusaAPI.ModelosSunat.PricingReferenceType ValorReferenUnitario = new MinimusaAPI.ModelosSunat.PricingReferenceType();
                    //ValorReferenUnitario.AlternativeConditionPrice
                    List<MinimusaAPI.ModelosSunat.PriceType> TipoPrecios = new List<MinimusaAPI.ModelosSunat.PriceType>();
                    MinimusaAPI.ModelosSunat.PriceType TipoPrecio = new MinimusaAPI.ModelosSunat.PriceType();
                    MinimusaAPI.ModelosSunat.PriceAmountType PrecioMonto = new MinimusaAPI.ModelosSunat.PriceAmountType();

                    PrecioMonto.currencyID = Comprobante.Idmoneda;
                    PrecioMonto.Value = Convert.ToDecimal(string.Format("{0:0.000}", det.Precio));
                    TipoPrecio.PriceAmount = PrecioMonto;

                    MinimusaAPI.ModelosSunat.PriceTypeCodeType TipoPrecioCode = new MinimusaAPI.ModelosSunat.PriceTypeCodeType();
                    TipoPrecioCode.listName = "Tipo de Precio";
                    TipoPrecioCode.listAgencyName = "PE:SUNAT";
                    TipoPrecioCode.listURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo16";
                    TipoPrecioCode.Value = "01";

                    TipoPrecio.PriceTypeCode = TipoPrecioCode;
                    TipoPrecios.Add(TipoPrecio);
                    ValorReferenUnitario.AlternativeConditionPrice = TipoPrecios.ToArray();
                    item.PricingReference = ValorReferenUnitario;

                    //Crear una coleccion de impuestos (IGV , ICBPER, Otros)
                    List<MinimusaAPI.ModelosSunat.TaxTotalType> Totales_Items = new List<MinimusaAPI.ModelosSunat.TaxTotalType>();
                    MinimusaAPI.ModelosSunat.TaxTotalType Totales_Item = new MinimusaAPI.ModelosSunat.TaxTotalType();
                    MinimusaAPI.ModelosSunat.TaxAmountType Total_Item = new MinimusaAPI.ModelosSunat.TaxAmountType();

                    Total_Item.currencyID = Comprobante.Idmoneda;
                    Total_Item.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.mtoValorVentaItem - (det.mtoValorVentaItem / 1.18m)));
                    Totales_Item.TaxAmount = Total_Item;


                    List<MinimusaAPI.ModelosSunat.TaxSubtotalType> subtotal_Items =
                                                                                                     new List<MinimusaAPI.ModelosSunat.TaxSubtotalType>();

                    MinimusaAPI.ModelosSunat.TaxSubtotalType subtotal_Item = new MinimusaAPI.ModelosSunat.TaxSubtotalType();
                    MinimusaAPI.ModelosSunat.TaxableAmountType taxsubtotal_IGVItem = new MinimusaAPI.ModelosSunat.TaxableAmountType();
                    taxsubtotal_IGVItem.currencyID = Comprobante.Idmoneda;
                    taxsubtotal_IGVItem.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.mtoValorVentaItem / 1.18m));
                    subtotal_Item.TaxableAmount = taxsubtotal_IGVItem;

                    MinimusaAPI.ModelosSunat.TaxAmountType TotalTaxAmount_IGVItem = new MinimusaAPI.ModelosSunat.TaxAmountType();
                    TotalTaxAmount_IGVItem.currencyID = Comprobante.Idmoneda;
                    TotalTaxAmount_IGVItem.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.mtoValorVentaItem - (det.mtoValorVentaItem / 1.18m)));
                    subtotal_Item.TaxAmount = TotalTaxAmount_IGVItem;

                    subtotal_Items.Add(subtotal_Item);
                    Totales_Item.TaxSubtotal = subtotal_Items.ToArray();

                    MinimusaAPI.ModelosSunat.TaxCategoryType taxcategory_IGVItem = new MinimusaAPI.ModelosSunat.TaxCategoryType();
                    ModelosSunat.IDType idTaxCategoria = new ModelosSunat.IDType();
                    idTaxCategoria.schemeAgencyName = "United Nations Economic Commission for Europe";
                    idTaxCategoria.schemeName = "Tax Category Identifier";
                    idTaxCategoria.schemeID = "UN/ECE 5305";
                    idTaxCategoria.Value = "S";
                    ModelosSunat.PercentType1 porcentaje = new ModelosSunat.PercentType1();
                    porcentaje.Value = Convert.ToDecimal(det.porIgvItem) * 100;
                    taxcategory_IGVItem.Percent = porcentaje;
                    subtotal_Item.TaxCategory = taxcategory_IGVItem;

                    ModelosSunat.TaxExemptionReasonCodeType ReasonCode = new ModelosSunat.TaxExemptionReasonCodeType();
                    ReasonCode.listURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07";
                    ReasonCode.listName = "Afectacion del IGV";
                    ReasonCode.listAgencyName = "PE:SUNAT";
                    ReasonCode.Value = "10";

                    taxcategory_IGVItem.TaxExemptionReasonCode = ReasonCode;

                    MinimusaAPI.ModelosSunat.TaxSchemeType taxscheme_IGVItem = new MinimusaAPI.ModelosSunat.TaxSchemeType();
                    MinimusaAPI.ModelosSunat.IDType id2_IGVItem = new MinimusaAPI.ModelosSunat.IDType();

                    id2_IGVItem.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05";
                    id2_IGVItem.schemeAgencyName = "PE:SUNAT";
                    id2_IGVItem.schemeName = "Codigo de tributos";
                    id2_IGVItem.Value = "1000";
                    taxscheme_IGVItem.ID = id2_IGVItem;

                    MinimusaAPI.ModelosSunat.NameType1 nombreImpto_IGVItem = new MinimusaAPI.ModelosSunat.NameType1();
                    nombreImpto_IGVItem.Value = "IGV";
                    taxscheme_IGVItem.Name = nombreImpto_IGVItem;

                    MinimusaAPI.ModelosSunat.TaxTypeCodeType nombreImpto_IGVItemInter = new MinimusaAPI.ModelosSunat.TaxTypeCodeType();
                    nombreImpto_IGVItemInter.Value = "VAT";
                    taxscheme_IGVItem.TaxTypeCode = nombreImpto_IGVItemInter;
                    taxscheme_IGVItem.Name = nombreImpto_IGVItem;
                    taxcategory_IGVItem.TaxScheme = taxscheme_IGVItem;

                    //Si es Bolsa 
                    if (det.Codcom.Contains("BBBB"))
                    {
                        ModelosSunat.TaxSubtotalType TotalIcb = new ModelosSunat.TaxSubtotalType();
                        ModelosSunat.TaxAmountType taxAmounticb = new ModelosSunat.TaxAmountType();
                        taxAmounticb.currencyID = Comprobante.Idmoneda;
                        taxAmounticb.Value = Math.Round((det.Cantidad * 0.20m), 2);
                        ModelosSunat.BaseUnitMeasureType baseicb = new ModelosSunat.BaseUnitMeasureType();
                        baseicb.unitCode = det.UnidadMedida;
                        baseicb.Value = Convert.ToInt32(det.Cantidad);
                        ModelosSunat.PerUnitAmountType perunicb = new ModelosSunat.PerUnitAmountType();
                        perunicb.currencyID = Comprobante.Idmoneda;
                        perunicb.Value = det.Precio;

                        TotalIcb.TaxAmount = taxAmounticb;
                        TotalIcb.BaseUnitMeasure = baseicb;

                        ModelosSunat.TaxCategoryType categoryicb = new ModelosSunat.TaxCategoryType();
                        ModelosSunat.TaxSchemeType taxicb = new ModelosSunat.TaxSchemeType();
                        ModelosSunat.IDType idtaxcat = new ModelosSunat.IDType();
                        idtaxcat.schemeID = "UN/ECE 5305";
                        idtaxcat.schemeName = "Codigo de tributos";
                        idtaxcat.schemeAgencyName = "PE:SUNAT";
                        idtaxcat.Value = "S";
                        //TotalIcb.PerUnitAmount = perunicb;
                        categoryicb.ID = idtaxcat;
                        categoryicb.PerUnitAmount = perunicb;


                        ModelosSunat.IDType idicp = new ModelosSunat.IDType();
                        idicp.Value = "7152";
                        ModelosSunat.NameType1 nombreicb = new ModelosSunat.NameType1();
                        nombreicb.Value = "ICBPER";
                        ModelosSunat.TaxTypeCodeType codicb = new ModelosSunat.TaxTypeCodeType();
                        codicb.Value = "OTH";

                        taxicb.ID = idicp;
                        taxicb.Name = nombreicb;
                        taxicb.TaxTypeCode = codicb;
                        categoryicb.TaxScheme = taxicb;
                        TotalIcb.TaxCategory = categoryicb;
                        subtotal_Items.Add(TotalIcb);
                        //Totales_Item.TaxSubtotal = subtotal_Items.ToArray();
                        //Totales_Items.Add(Totales_Item); //Agregar el ICBPER 

                        //subtotal_Items.Add(subtotal_Item);
                        //   Totales_Item.TaxSubtotal = subtotal_Items.ToArray();
                        //   Totales_Items.Add(Totales_Item); //Agregar el IGV del producto
                    }
                    // subtotal_Items.Add(subtotal_Item);
                    Totales_Item.TaxSubtotal = subtotal_Items.ToArray();
                    Totales_Items.Add(Totales_Item); //Agregar el IGV del producto
                    item.TaxTotal = Totales_Items.ToArray();

                    //Descripcion del producto
                    List<MinimusaAPI.ModelosSunat.DescriptionType> descriptions =
                                                                                                    new List<MinimusaAPI.ModelosSunat.DescriptionType>();
                    MinimusaAPI.ModelosSunat.DescriptionType description = new MinimusaAPI.ModelosSunat.DescriptionType();
                    description.Value = det.DescripcionProducto;
                    MinimusaAPI.ModelosSunat.ItemIdentificationType codigoProd = new MinimusaAPI.ModelosSunat.ItemIdentificationType();
                    MinimusaAPI.ModelosSunat.IDType id = new MinimusaAPI.ModelosSunat.IDType();
                    id.Value = det.Codcom;
                    codigoProd.ID = id;

                    MinimusaAPI.ModelosSunat.PriceType PrecioProducto = new MinimusaAPI.ModelosSunat.PriceType();
                    MinimusaAPI.ModelosSunat.PriceAmountType PrecioMontoTipo = new MinimusaAPI.ModelosSunat.PriceAmountType();
                    PrecioMontoTipo.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.Precio / (det.porIgvItem + 1)));
                    //PrecioMontoTipo.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.Precio));
                    PrecioMontoTipo.currencyID = Comprobante.Idmoneda;
                    PrecioProducto.PriceAmount = PrecioMontoTipo;

                    MinimusaAPI.ModelosSunat.ItemType itemTipo = new MinimusaAPI.ModelosSunat.ItemType();
                    descriptions.Add(description);
                    itemTipo.Description = descriptions.ToArray();
                    itemTipo.SellersItemIdentification = codigoProd;

                    List<ModelosSunat.CommodityClassificationType> codSunats = new List<ModelosSunat.CommodityClassificationType>();
                    ModelosSunat.CommodityClassificationType codSunat = new ModelosSunat.CommodityClassificationType();
                    ModelosSunat.ItemClassificationCodeType codClas = new ModelosSunat.ItemClassificationCodeType();
                    codClas.listName = "Item Classification";
                    codClas.listAgencyName = "GS1 US";
                    codClas.listID = "UNSPSC";
                    codClas.Value = "25172405";
                    codSunat.ItemClassificationCode = codClas;
                    codSunats.Add(codSunat);
                    itemTipo.CommodityClassification = codSunats.ToArray();

                    item.Item = itemTipo;
                    item.Price = PrecioProducto;
                    //Item_Adicional.AdditionalItemProperty = Propiedades;
                    //Item_Adicionales.Item = Item_Adicional;            

                    //items[1] = item_OpeOnerosa;
                    // items[2] = item_DsctoItem;
                    // items[3] = item_DsctoCargoItem;
                    //items[4] = item_IGVItem;
                    //items[5] = item_ISCItem;
                    // items[6] = Item_Descripcion;
                    //  items[7] = Item_CodProSUNAT;
                    //   items[8] = Item_Adicionales;
                    items.Add(item);
                    iditem += 1;
                }
                Factura.InvoiceLine = items.ToArray();
                //
                string archXML = GenerarComprobante(Factura, Comprobante.EmpresaRUC, Comprobante.Idtipocomp, Comprobante.Serie, Comprobante.Numero);
                FirmarXML(archXML, Ruta_Certificado, Password_Certificado);
                string strEnvio = Ruta_ENVIOS + Path.GetFileName(archXML).Replace(".xml", ".zip");
                Comprimir(archXML, strEnvio);
                EnviarDocumento(strEnvio);

                //generar el nombre del archivo xml firmado
                string xmlfirmado = Comprobante.EmpresaRUC + "-" + Comprobante.Idtipocomp.PadLeft(2, '0') + "-" +
                                                Comprobante.Serie + "-" + Comprobante.Numero + ".xml";

                string rutafirmado = Ruta_XML + xmlfirmado;

                //Obtener firma
                string firma = Util.Helper.ObtenerDigestValueSunat(rutafirmado);

                Util.Helper.GenerarQR(Comprobante.EmpresaRUC, Comprobante.Idtipocomp.PadLeft(2, '0'),
                                                       Comprobante.Serie + '-' + Comprobante.Numero,
                                                       Comprobante.TotIgv.ToString(),
                                                       Comprobante.TotNeto.ToString(),
                                                       Comprobante.Fechaemision.ToString(),
                                                       Comprobante.ClienteTipodocumento,
                                                       Comprobante.ClienteNumeroDocumento,
                                                        firma,
                                                        Ruta_QR,
                                                        Comprobante.Idcabecera);

                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int GenerarFBGuiaRemision_XML(MinimusaAPI.ViewModels.GuiaRemision guia, 
            MinimusaAPI.ViewModels.Cabecera Comprobante)
        {
           // GenerarComprobanteFB_XML(Comprobante);
            GenerarGuiaRemision_XML(guia, Comprobante.Detalles, Comprobante.EmpresaRUC);
            return 1;
        }

            //public  int GenerarGuiaRemision_XML(GuiaRemision Comprobante, ICollection<DETALLE> Detalles ,String EmpresaRUC)
            public  int GenerarGuiaRemision_XML(GuiaRemision Comprobante, ICollection<Detalles> Detalles, String EmpresaRUC)
            {               
           MinimusaAPI.Guias.DespatchAdviceType Guia = new MinimusaAPI.Guias.DespatchAdviceType();
            try
            {
                //------Namespace necesarios para el UBL
                Guia.Cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                Guia.Cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                Guia.Ccts = "urn:un:unece:uncefact:documentation:2";
                Guia.Ds = "http://www.w3.org/2000/09/xmldsig#";
                Guia.Ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";
                Guia.Qdt = "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2";
                Guia.Udt = "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2";
                //------
                //-----Datos de pruebas de Guias
                //UBLExtensionsType[] ublExtensiones = new UBLExtensionsType[5];

                MinimusaAPI.Guias.UBLExtensionType[] ublExtensiones = new MinimusaAPI.Guias.UBLExtensionType[5];
                MinimusaAPI.Guias.UBLExtensionType ublExtension = new MinimusaAPI.Guias.UBLExtensionType();

                ublExtensiones[0] = ublExtension;
                Guia.UBLExtensions = ublExtensiones;

                Guia.UBLVersionID = new MinimusaAPI.Guias.UBLVersionIDType();
                Guia.UBLVersionID.Value = "2.1";

                Guia.CustomizationID = new MinimusaAPI.Guias.CustomizationIDType();
                //Guia.CustomizationID.schemeAgencyName = "PE:SUNAT";
                Guia.CustomizationID.Value = "1.0";
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Código de tipo de operación
                Guia.ID = new MinimusaAPI.Guias.IDType();
                Guia.ID.Value = Comprobante.NumeroGuiaRemision;
                //Fecha de emisión y hora de emision
                Guia.IssueDate = new MinimusaAPI.Guias.IssueDateType();
                string fechaemision = Convert.ToDateTime(Comprobante.FechaEmision).ToString("dd/MM/yyyy");
                Guia.IssueDate.Value = Convert.ToDateTime(fechaemision);
                //Guia.IssueTime = new Guias.IssueTimeType();
                //string hora = Convert.ToDateTime(Comprobante.FechaEmision).ToString("HH:mm:ss");
                //Guia.IssueTime.Value = Convert.ToDateTime(hora);
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////               
                //Tipo de documento(Guia)
                MinimusaAPI.Guias.DespatchAdviceTypeCodeType TipoDoc = new MinimusaAPI.Guias.DespatchAdviceTypeCodeType();
                TipoDoc.Value = "09";                
                Guia.DespatchAdviceTypeCode = TipoDoc;
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////                                
                MinimusaAPI.Guias.NoteType Nota = new MinimusaAPI.Guias.NoteType();
                MinimusaAPI.Guias.NoteType[] Notas = new MinimusaAPI.Guias.NoteType[2];
                Nota.Value = "GUIA DE PRUEBA";
                Notas[0] = Nota;
                Guia.Note=Notas;
                //Tipo de Moneda                
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                //Nombre Comercial del emisor Apellidos y nombres, denominación o razón social del emisor Tipo y Número de RUC del emisor Código del domicilio fiscal 
                //o de local anexo del emisor 
                MinimusaAPI.Guias.SignatureType[] Firmas = new MinimusaAPI.Guias.SignatureType[2];
                MinimusaAPI.Guias.SignatureType Firma = new MinimusaAPI.Guias.SignatureType();

                MinimusaAPI.Guias.IDType NumeroGuia = new MinimusaAPI.Guias.IDType();
                NumeroGuia.Value =Comprobante.NumeroGuiaRemision ;
                Firma.ID = NumeroGuia;

                MinimusaAPI.Guias.PartyType partySign = new MinimusaAPI.Guias.PartyType();
                MinimusaAPI.Guias.PartyIdentificationType partyIdentificacion = new MinimusaAPI.Guias.PartyIdentificationType();
                MinimusaAPI.Guias.PartyIdentificationType[] partyIdentificacions = new MinimusaAPI.Guias.PartyIdentificationType[2];

                MinimusaAPI.Guias.IDType idType = new MinimusaAPI.Guias.IDType();
                idType.Value = Comprobante.EmpresaRUC ;
                partyIdentificacion.ID = idType;
                partyIdentificacions[0] = partyIdentificacion;
                partySign.PartyIdentification = partyIdentificacions;
                Firma.SignatoryParty = partySign;
                                                
                MinimusaAPI.Guias.PartyNameType partyName = new MinimusaAPI.Guias.PartyNameType();
                MinimusaAPI.Guias.PartyNameType[] partyNames = new MinimusaAPI.Guias.PartyNameType[2];

                MinimusaAPI.Guias.NameType1 RazonSocialFirma = new MinimusaAPI.Guias.NameType1();
                RazonSocialFirma.Value = Comprobante.EmpresaRazonSocial;
                partyName.Name = RazonSocialFirma;
                partyNames[0] = partyName;
                partySign.PartyName = partyNames;
                                   
                MinimusaAPI.Guias.AttachmentType attachType = new MinimusaAPI.Guias.AttachmentType();
                MinimusaAPI.Guias.ExternalReferenceType externaReferencia = new MinimusaAPI.Guias.ExternalReferenceType();
                MinimusaAPI.Guias.URIType uri = new MinimusaAPI.Guias.URIType();
                uri.Value =  EmpresaRUC + "-"  + Comprobante.NumeroGuiaRemision;
                externaReferencia.URI = uri;
                Firma.DigitalSignatureAttachment = attachType;
                attachType.ExternalReference = externaReferencia;
                Firma.DigitalSignatureAttachment = attachType;
                Firmas[0] = Firma;
                Guia.Signature = Firmas;

                //Remitente
                
                MinimusaAPI.Guias.SupplierPartyType Remitente = new MinimusaAPI.Guias.SupplierPartyType();

                MinimusaAPI.Guias.PartyType party = new MinimusaAPI.Guias.PartyType();                            
                MinimusaAPI.Guias.PartyIdentificationType partyidentificacion = new MinimusaAPI.Guias.PartyIdentificationType();
                MinimusaAPI.Guias.PartyIdentificationType[] partyidentificacions = new MinimusaAPI.Guias.PartyIdentificationType[2];
                MinimusaAPI.Guias.IDType idEmpresa = new MinimusaAPI.Guias.IDType();
                
                MinimusaAPI.Guias. CustomerAssignedAccountIDType RemitenteTipoDocumento = new MinimusaAPI.Guias.CustomerAssignedAccountIDType();
                RemitenteTipoDocumento.schemeID = Comprobante.TipoDocRemite;
                RemitenteTipoDocumento.Value = Comprobante.NumDocRemite;
                Remitente.CustomerAssignedAccountID = RemitenteTipoDocumento;
                                               
                MinimusaAPI.Guias.PartyLegalEntityType pLEntityRemite = new MinimusaAPI.Guias.PartyLegalEntityType();
                MinimusaAPI.Guias.PartyLegalEntityType[] pLEntityRemites = new MinimusaAPI.Guias.PartyLegalEntityType[2];

                MinimusaAPI.Guias.RegistrationNameType registerNameEmisor = new MinimusaAPI.Guias.RegistrationNameType();
                registerNameEmisor.Value = Comprobante.EmpresaRazonSocial;
                pLEntityRemite.RegistrationName = registerNameEmisor;
                pLEntityRemites[0] = pLEntityRemite;
                party.PartyLegalEntity = pLEntityRemites;
                
                Remitente.Party = party;
                Guia.DespatchSupplierParty = Remitente;

                //Destinatario de Envio
                MinimusaAPI.Guias.CustomerPartyType Destinatario = new MinimusaAPI.Guias.CustomerPartyType();                
                MinimusaAPI.Guias.CustomerAssignedAccountIDType DestinatarioTipoDocumento = new MinimusaAPI.Guias.CustomerAssignedAccountIDType();
                DestinatarioTipoDocumento.schemeID = Comprobante.TipoDocDestinatario;
                DestinatarioTipoDocumento.Value = Comprobante.NumDocDestinatario;

                Destinatario.CustomerAssignedAccountID = DestinatarioTipoDocumento;
                MinimusaAPI.Guias.PartyType DestinatarioRazonSocial = new MinimusaAPI.Guias.PartyType();
                MinimusaAPI.Guias.PartyLegalEntityType DestinatarioRazon = new MinimusaAPI.Guias.PartyLegalEntityType();
                MinimusaAPI.Guias.PartyLegalEntityType[] DestinatariosRazon = new MinimusaAPI.Guias.PartyLegalEntityType[2];

                MinimusaAPI.Guias.RegistrationNameType DestinararioNombre = new MinimusaAPI.Guias.RegistrationNameType();
                DestinararioNombre.Value = Comprobante.RazonSocialDestinatario;

                DestinatarioRazon.RegistrationName = DestinararioNombre;
                DestinatariosRazon[0] = DestinatarioRazon;
                DestinatarioRazonSocial.PartyLegalEntity = DestinatariosRazon;

                Destinatario.Party= DestinatarioRazonSocial;
                Guia.DeliveryCustomerParty = Destinatario;

                MinimusaAPI.Guias.ShipmentType Envio = new MinimusaAPI.Guias.ShipmentType();
                MinimusaAPI.Guias.IDType idEnvio = new MinimusaAPI.Guias.IDType();
                idEnvio.Value = "001";
                Envio.ID = idEnvio;

                MinimusaAPI.Guias.HandlingCodeType TipoEnvio = new MinimusaAPI.Guias.HandlingCodeType();
                TipoEnvio.Value=Comprobante.MotivoTraslado;
                Envio.HandlingCode = TipoEnvio;

                MinimusaAPI.Guias.InformationType[] Motivos = new MinimusaAPI.Guias.InformationType[2];
                MinimusaAPI.Guias.InformationType Motivo = new MinimusaAPI.Guias.InformationType();
                Motivo.Value = Comprobante.DescMotivo;
                Motivos[0] = Motivo;
                Envio.Information = Motivos;
                
                MinimusaAPI.Guias.GrossWeightMeasureType Peso =new MinimusaAPI.Guias.GrossWeightMeasureType();
                Peso.unitCode = Comprobante.UniMedPesoBruto;
                Peso.Value =Convert.ToDecimal( Comprobante.Peso);
                Envio.GrossWeightMeasure = Peso;

                MinimusaAPI.Guias.SplitConsignmentIndicatorType Indicador = new MinimusaAPI.Guias.SplitConsignmentIndicatorType();
                Indicador.Value = false;
                Envio.SplitConsignmentIndicator = Indicador;

                MinimusaAPI.Guias.ShipmentStageType[] Transportes = new MinimusaAPI.Guias.ShipmentStageType[2];
                MinimusaAPI.Guias.ShipmentStageType Transporte = new MinimusaAPI.Guias.ShipmentStageType();
                MinimusaAPI.Guias.TransportModeCodeType ModoTransporte = new MinimusaAPI.Guias.TransportModeCodeType();
                ModoTransporte.Value = Comprobante.MovilTraslado;
                Transporte.TransportModeCode = ModoTransporte;

                MinimusaAPI.Guias.PeriodType Periodo = new MinimusaAPI.Guias.PeriodType();
                MinimusaAPI.Guias.StartDateType FechaInicio = new MinimusaAPI.Guias.StartDateType();
                //FechaInicio.Value = DateTime.ParseExact(Comprobante.FechaInicioTraslado.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                FechaInicio.Value =Convert.ToDateTime( Comprobante.FechaInicioTraslado);
                Periodo.StartDate = FechaInicio;
                Transporte.TransitPeriod = Periodo;
                              
                MinimusaAPI.Guias.PartyType CompañiaTransporte = new MinimusaAPI.Guias.PartyType();
                MinimusaAPI.Guias.PartyType[] CompañiasTransportes = new MinimusaAPI.Guias.PartyType[2];

                MinimusaAPI.Guias.PartyIdentificationType RUCTransporte = new MinimusaAPI.Guias.PartyIdentificationType();
                MinimusaAPI.Guias.PartyIdentificationType[] RUCsTransportes = new MinimusaAPI.Guias.PartyIdentificationType[2];

                MinimusaAPI.Guias.IDType idDocumento = new MinimusaAPI.Guias.IDType();
                idDocumento.Value = Comprobante.NumRucTransportista;
                RUCTransporte.ID = idDocumento;
                RUCsTransportes[0] = RUCTransporte;
                CompañiaTransporte.PartyIdentification = RUCsTransportes;
                CompañiasTransportes[0] = CompañiaTransporte;
                Transporte.CarrierParty = CompañiasTransportes;

                MinimusaAPI.Guias.PartyNameType[] NombresTransportes = new MinimusaAPI.Guias.PartyNameType[2];
                MinimusaAPI.Guias.PartyNameType NombreTransporte = new MinimusaAPI.Guias.PartyNameType();                
                MinimusaAPI.Guias.NameType1 Nombre = new MinimusaAPI.Guias.NameType1();

                Nombre.Value = Comprobante.RazonSocialTransportista;
                NombreTransporte.Name = Nombre;
                NombresTransportes[0] = NombreTransporte;
                CompañiaTransporte.PartyName = NombresTransportes;
                CompañiasTransportes[0] = CompañiaTransporte;
                Transporte.CarrierParty = CompañiasTransportes;

                MinimusaAPI.Guias.TransportMeansType Transportista = new MinimusaAPI.Guias.TransportMeansType();
                MinimusaAPI.Guias.RoadTransportType TransportistaPlaca = new MinimusaAPI.Guias.RoadTransportType();
                MinimusaAPI.Guias.LicensePlateIDType NumeroPlaca = new MinimusaAPI.Guias.LicensePlateIDType();
                NumeroPlaca.Value = Comprobante.Placa;
                TransportistaPlaca.LicensePlateID = NumeroPlaca;
                Transportista.RoadTransport = TransportistaPlaca;
                Transporte.TransportMeans = Transportista;

                MinimusaAPI.Guias.PersonType[] conductores = new MinimusaAPI.Guias.PersonType[2];
                MinimusaAPI.Guias.PersonType conductor = new MinimusaAPI.Guias.PersonType();

                MinimusaAPI.Guias.IDType idlicencia = new MinimusaAPI.Guias.IDType();
                idlicencia.Value =Comprobante.LicenciaConducir;                
                idlicencia.schemeID = "1";
                conductor.ID = idlicencia;
                conductores[0] = conductor;
                Transporte.DriverPerson = conductores;

                Transportes[0] = Transporte;
                Envio.ShipmentStage = Transportes;

                //Entrega
                MinimusaAPI.Guias.DeliveryType Entrega = new MinimusaAPI.Guias.DeliveryType();

                MinimusaAPI.Guias.AddressType DireccionEntrega = new MinimusaAPI.Guias.AddressType();
                MinimusaAPI.Guias.IDType UbigeoEntrega = new MinimusaAPI.Guias.IDType();
                UbigeoEntrega.Value = Comprobante.UbigeoPuntoLlegada;
                DireccionEntrega.ID = UbigeoEntrega;

                MinimusaAPI.Guias.StreetNameType calle = new MinimusaAPI.Guias.StreetNameType();
                calle.Value = Comprobante.Llegada;
                DireccionEntrega.StreetName = calle;

                MinimusaAPI.Guias.CountryType Pais = new MinimusaAPI.Guias.CountryType();
                MinimusaAPI.Guias.IdentificationCodeType Codigo = new MinimusaAPI.Guias.IdentificationCodeType();
                Codigo.Value = "PE";
                Pais.IdentificationCode = Codigo;
                DireccionEntrega.Country=Pais;
                Entrega.DeliveryAddress = DireccionEntrega;
                Envio.Delivery = Entrega;
               
                MinimusaAPI.Guias.AddressType DireccionPartida = new MinimusaAPI.Guias.AddressType();
                MinimusaAPI.Guias.IDType idDireccionPartida = new MinimusaAPI.Guias.IDType();
                idDireccionPartida.Value = Comprobante.UbigeoPuntoPartida;
                DireccionPartida.ID = idDireccionPartida;
               
                MinimusaAPI.Guias.StreetNameType CallePartida = new MinimusaAPI.Guias.StreetNameType();
                CallePartida.Value = Comprobante.Partida;
                DireccionPartida.StreetName=CallePartida;

                MinimusaAPI.Guias.CountryType PaisOrigen = new MinimusaAPI.Guias.CountryType();
                MinimusaAPI.Guias.IdentificationCodeType CodigoOrigen = new MinimusaAPI.Guias.IdentificationCodeType();
                CodigoOrigen.Value = "PE";
                ///Codigo.Value = "PE";
                PaisOrigen.IdentificationCode=CodigoOrigen;
                DireccionPartida.Country = PaisOrigen;                
                Envio.OriginAddress = DireccionPartida;
                
                Guia.Shipment = Envio;

                MinimusaAPI.Guias.DespatchLineType[] items = new MinimusaAPI.Guias.DespatchLineType[10];
                //For a los items de la guia:
                int i = 1;

                foreach (Detalles det in Detalles)
                {                 
                MinimusaAPI.Guias.DespatchLineType item = new MinimusaAPI.Guias.DespatchLineType();
                MinimusaAPI.Guias.IDType iditem = new MinimusaAPI.Guias.IDType();
                iditem.Value = i.ToString();
                item.ID = iditem;

                MinimusaAPI.Guias.DeliveredQuantityType Cantidad = new MinimusaAPI.Guias.DeliveredQuantityType();
                Cantidad.Value = Convert.ToDecimal(det.Cantidad);
                Cantidad.unitCode = det.UnidadMedida;
                item.DeliveredQuantity = Cantidad;

                MinimusaAPI.Guias.OrderLineReferenceType[] Ordenes = new MinimusaAPI.Guias.OrderLineReferenceType[2];
                MinimusaAPI.Guias.OrderLineReferenceType Orden = new MinimusaAPI.Guias.OrderLineReferenceType();

                MinimusaAPI.Guias.LineIDType Linea=new MinimusaAPI.Guias.LineIDType();
                Linea.Value = i.ToString();
                Orden.LineID = Linea;
                Ordenes[0] = Orden;
                item.OrderLineReference = Ordenes;

                MinimusaAPI.Guias.ItemType itemTipo = new MinimusaAPI.Guias.ItemType();
                MinimusaAPI.Guias.NameType1 NombreItem = new MinimusaAPI.Guias.NameType1();
                NombreItem.Value=det.DescripcionProducto;
                itemTipo.Name = NombreItem;
                                    
                MinimusaAPI.Guias.ItemIdentificationType Identificacion = new MinimusaAPI.Guias.ItemIdentificationType();
                    
                MinimusaAPI.Guias.IDType idTipo = new MinimusaAPI.Guias.IDType();
                idTipo.Value = det.Codcom; 
                Identificacion.ID = idTipo;
                itemTipo.SellersItemIdentification = Identificacion;
                item.Item = itemTipo;
                items[i] = item;                
                 i=i+1;
                }

                Guia.DespatchLine = items;
                string archXML = GenerarGuiaRemitente(Guia,EmpresaRUC);
                FirmarXML(archXML, Ruta_Certificado, Password_Certificado);
                string strEnvio = Ruta_ENVIOS + Path.GetFileName(archXML).Replace(".xml", ".zip");
                Comprimir(archXML, strEnvio);
                EnviarDocumentoGuiaRemision(strEnvio);
                
                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }                    
   
        public  int GenerarComprobanteNC_XML(Cabecera Comprobante)
        {
            MinimusaAPI.ModelosSunat.CreditNoteType Factura = new MinimusaAPI.ModelosSunat.CreditNoteType();

            try
            {
                //------Namespace necesarios para el UBL
                Factura.Cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                Factura.Cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                Factura.Ccts = "urn:un:unece:uncefact:documentation:2";
                Factura.Ds = "http://www.w3.org/2000/09/xmldsig#";
                Factura.Ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";
                Factura.Qdt = "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2";
                Factura.Udt = "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2";
                //------
                //-----Datos de pruebas de facturas
                //UBLExtensionsType[] ublExtensiones = new UBLExtensionsType[5];

               MinimusaAPI.ModelosSunat.UBLExtensionType[] ublExtensiones = new MinimusaAPI.ModelosSunat.UBLExtensionType[5];
                MinimusaAPI.ModelosSunat.UBLExtensionType ublExtension = new MinimusaAPI.ModelosSunat.UBLExtensionType();

                ublExtensiones[0] = ublExtension;
                Factura.UBLExtensions = ublExtensiones;

                Factura.UBLVersionID = new MinimusaAPI.ModelosSunat.UBLVersionIDType();
                Factura.UBLVersionID.Value = "2.1";

                Factura.CustomizationID = new MinimusaAPI.ModelosSunat.CustomizationIDType();
                //Factura.CustomizationID.schemeAgencyName = "PE:SUNAT";
                Factura.CustomizationID.Value = "2.0";
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Código de tipo de operación
                //Factura.ProfileID = new ProfileIDType();
                //Factura.ProfileID.schemeAgencyName = "PE:SUNAT";
                //Factura.ProfileID.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo51";
                //Factura.ProfileID.schemeName = "Tipo de Operacion";
                //Factura.ProfileID.Value = Comprobante.TipoOperacion;

                //Numeración, conformada por serie y número correlativo
                Factura.ID = new MinimusaAPI.ModelosSunat.IDType();
                Factura.ID.Value = Comprobante.Serie + "-" + Comprobante.Numero;
                //Fecha de emisión y hora de emision
                Factura.IssueDate = new MinimusaAPI.ModelosSunat.IssueDateType();
                string fechaemision = Convert.ToDateTime(Comprobante.Fechaemision).ToString("dd/MM/yyyy");
                Factura.IssueDate.Value = Convert.ToDateTime(fechaemision);
                Factura.IssueTime = new MinimusaAPI.ModelosSunat.IssueTimeType();
                string hora = Convert.ToDateTime(Comprobante.Fechaemision).ToString("HH:mm:ss");
                Factura.IssueTime.Value = hora;
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Tipo de Moneda
                MinimusaAPI.ModelosSunat.DocumentCurrencyCodeType moneda = new MinimusaAPI.ModelosSunat.DocumentCurrencyCodeType()
                {
                    listSchemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo51",
                    listID = "ISO 4217 Alpha",
                    listName = "Currency",
                    listAgencyName = "United Nations Economic Commission for Europe",
                    Value = Comprobante.Idmoneda
                };
                Factura.DocumentCurrencyCode = moneda;
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                MinimusaAPI.ModelosSunat.ResponseType DocumentoRel = new MinimusaAPI.ModelosSunat.ResponseType();
                MinimusaAPI.ModelosSunat.ResponseType[] DocumentoRels = new MinimusaAPI.ModelosSunat.ResponseType[2];
                MinimusaAPI.ModelosSunat.ReferenceIDType NumeroDocRel = new MinimusaAPI.ModelosSunat.ReferenceIDType();
                //NumeroDocRel.Value = Comprobante.Cab_Ref_Serie + "-" + Comprobante.Cab_Ref_Numero;
                NumeroDocRel.Value = Comprobante.Serie + "-" + Comprobante.Numero;
                MinimusaAPI.ModelosSunat.ResponseCodeType TipoDocRel = new MinimusaAPI.ModelosSunat.ResponseCodeType();
                TipoDocRel.Value = Comprobante.Idtipocomp;
                //TipoDocRel.Value = Comprobante.Cab_Ref_TipoNotaCredito;
                MinimusaAPI.ModelosSunat.DescriptionType Motivo = new MinimusaAPI.ModelosSunat.DescriptionType();
                MinimusaAPI.ModelosSunat.DescriptionType[] Motivos = new MinimusaAPI.ModelosSunat.DescriptionType[2];
                Motivos[0] = Motivo;
                Motivo.Value = Comprobante.Cab_Ref_Motivo;
                DocumentoRel.ReferenceID = NumeroDocRel;
                DocumentoRel.ResponseCode = TipoDocRel;
                DocumentoRel.Description = Motivos;
                DocumentoRels[0] = DocumentoRel;
                Factura.DiscrepancyResponse = DocumentoRels;

                MinimusaAPI.ModelosSunat.BillingReferenceType[] referencias = new MinimusaAPI.ModelosSunat.BillingReferenceType[2];
                MinimusaAPI.ModelosSunat.BillingReferenceType referencia = new MinimusaAPI.ModelosSunat.BillingReferenceType();

                MinimusaAPI.ModelosSunat.DocumentReferenceType documento = new MinimusaAPI.ModelosSunat.DocumentReferenceType();
                MinimusaAPI.ModelosSunat.IDType docRela = new MinimusaAPI.ModelosSunat.IDType();
                docRela.Value = Comprobante.Cab_Ref_Serie + "-" + Comprobante.Cab_Ref_Numero;
                MinimusaAPI.ModelosSunat.DocumentTypeCodeType TipoDocumentoRel = new MinimusaAPI.ModelosSunat.DocumentTypeCodeType();
                TipoDocumentoRel.Value = Comprobante.Cab_Ref_TipoDeDocumento;
                documento.DocumentTypeCode = TipoDocumentoRel;
                documento.ID = docRela;

                referencia.InvoiceDocumentReference = documento;
                referencias[0] = referencia;
                Factura.BillingReference = referencias;

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                //Nombre Comercial del emisor Apellidos y nombres, denominación o razón social del emisor Tipo y Número de RUC del emisor Código del domicilio fiscal 
                //o de local anexo del emisor 
                MinimusaAPI.ModelosSunat.SignatureType Firma = new MinimusaAPI.ModelosSunat.SignatureType();
                MinimusaAPI.ModelosSunat.SignatureType[] Firmas = new MinimusaAPI.ModelosSunat.SignatureType[2];

                MinimusaAPI.ModelosSunat.PartyType partySign = new MinimusaAPI.ModelosSunat.PartyType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType partyIdentificacion = new MinimusaAPI.ModelosSunat.PartyIdentificationType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType[] partyIdentificacions = new MinimusaAPI.ModelosSunat.PartyIdentificationType[2];
                MinimusaAPI.ModelosSunat.IDType idFirma = new MinimusaAPI.ModelosSunat.IDType();
                idFirma.Value = Comprobante.EmpresaRUC;
                Firma.ID = idFirma;

                partyIdentificacion.ID = idFirma;
                partyIdentificacions[0] = partyIdentificacion;
                partySign.PartyIdentification = partyIdentificacions;
                Firma.SignatoryParty = partySign;

                MinimusaAPI.ModelosSunat.SupplierPartyType empresa = new MinimusaAPI.ModelosSunat.SupplierPartyType();
                MinimusaAPI.ModelosSunat.PartyType party = new MinimusaAPI.ModelosSunat.PartyType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType partyidentificacion = new MinimusaAPI.ModelosSunat.PartyIdentificationType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType[] partyidentificacions = new MinimusaAPI.ModelosSunat.PartyIdentificationType[2];
                MinimusaAPI.ModelosSunat.IDType idEmpresa = new MinimusaAPI.ModelosSunat.IDType();

                idEmpresa.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idEmpresa.schemeName = "Documento de Identidad";
                idEmpresa.schemeID = "6";
                idEmpresa.schemeAgencyName = "PE:SUNAT";
                idEmpresa.Value = Comprobante.EmpresaRUC;

                partyidentificacion.ID = idEmpresa;
                partyidentificacions[0] = partyidentificacion;
                party.PartyIdentification = partyidentificacions;

                MinimusaAPI.ModelosSunat.PartyNameType partyname = new MinimusaAPI.ModelosSunat.PartyNameType();
                List<MinimusaAPI.ModelosSunat.PartyNameType> partynames = new List<MinimusaAPI.ModelosSunat.PartyNameType>();
                MinimusaAPI.ModelosSunat.NameType1 nameEmisor = new MinimusaAPI.ModelosSunat.NameType1();
                nameEmisor.Value = Comprobante.EmpresaRazonSocial;
                partyname.Name = nameEmisor;
                partynames.Add(partyname);
                party.PartyName = partynames.ToArray();

                //MinimusaAPI.ModelosSunat.PartyTaxSchemeType PartyTaxScheme = new MinimusaAPI.ModelosSunat.PartyTaxSchemeType();
                //List<MinimusaAPI.ModelosSunat.PartyTaxSchemeType> PartyTaxSchemes = new List<MinimusaAPI.ModelosSunat.PartyTaxSchemeType>();

                MinimusaAPI.ModelosSunat.RegistrationNameType registerNameEmisor = new MinimusaAPI.ModelosSunat.RegistrationNameType();
                registerNameEmisor.Value = Comprobante.EmpresaRazonSocial;
                ///PartyTaxScheme.RegistrationName = registerNameEmisor;
                //Direccion emisor                
                ModelosSunat.CompanyIDType compañia = new ModelosSunat.CompanyIDType();
                compañia.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                compañia.schemeAgencyName = "PE:SUNAT";
                compañia.schemeName = "SUNAT:Identificador de Documento de Identidad";
                compañia.schemeID = "6";
                compañia.Value = Comprobante.EmpresaRUC;

                MinimusaAPI.ModelosSunat.AddressType direccion = new MinimusaAPI.ModelosSunat.AddressType();
                MinimusaAPI.ModelosSunat.AddressTypeCodeType addrestypecode = new MinimusaAPI.ModelosSunat.AddressTypeCodeType();
                addrestypecode.listName = "Establecimientos anexos";
                addrestypecode.listAgencyName = "PE:SUNAT";
                addrestypecode.Value = "0000";
                //direccion.AddressTypeCode = addrestypecode;
                //PartyTaxScheme.RegistrationAddress = direccion;
                //ModelosSunat.IDType tipo = new ModelosSunat.IDType();
                ModelosSunat.TaxSchemeType taxSchema = new ModelosSunat.TaxSchemeType();
                ModelosSunat.IDType idsupplier = new ModelosSunat.IDType();
                idsupplier.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idsupplier.schemeAgencyName = "PE:SUNAT";
                idsupplier.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idsupplier.schemeID = "6";
                idsupplier.Value = Comprobante.EmpresaRUC;
                taxSchema.ID = idsupplier;

                //PartyTaxScheme.CompanyID = compañia;
                //PartyTaxScheme.TaxScheme = taxSchema;
                //PartyTaxSchemes.Add(PartyTaxScheme);
                //party.PartyTaxScheme = PartyTaxSchemes.ToArray();

                List<ModelosSunat.PartyLegalEntityType> partelegals = new List<ModelosSunat.PartyLegalEntityType>();
                ModelosSunat.PartyLegalEntityType partelegal = new ModelosSunat.PartyLegalEntityType();
                MinimusaAPI.ModelosSunat.RegistrationNameType registerNamePL = new MinimusaAPI.ModelosSunat.RegistrationNameType();
                registerNamePL.Value = Comprobante.EmpresaRazonSocial;
                partelegal.RegistrationName = registerNamePL;

                ModelosSunat.AddressType direccionPL = new ModelosSunat.AddressType();
                ModelosSunat.IDType iddireccionPL = new ModelosSunat.IDType();
                iddireccionPL.schemeAgencyName = "PE:INEI";
                iddireccionPL.schemeName = "Ubigeos";
                iddireccionPL.Value = Comprobante.ID_EmpresaDepartamento + Comprobante.ID_EmpresaProvincia + Comprobante.ID_EmpresaDistrito;
                direccionPL.ID = iddireccionPL;

                ModelosSunat.AddressTypeCodeType address_TypeCodeType = new ModelosSunat.AddressTypeCodeType();
                address_TypeCodeType.listName = "Establecimientos anexos";
                address_TypeCodeType.listAgencyName = "PE:SUNAT";
                address_TypeCodeType.Value = "0001";
                direccionPL.AddressTypeCode = address_TypeCodeType;

                MinimusaAPI.ModelosSunat.CityNameType Departamento = new MinimusaAPI.ModelosSunat.CityNameType();
                Departamento.Value = Comprobante.EmpresaDepartamento;
                direccionPL.CityName = Departamento;

                MinimusaAPI.ModelosSunat.CountrySubentityType Provincia = new MinimusaAPI.ModelosSunat.CountrySubentityType();
                Provincia.Value = Comprobante.EmpresaProvincia;
                direccionPL.CountrySubentity = Provincia;

                MinimusaAPI.ModelosSunat.DistrictType distrito = new MinimusaAPI.ModelosSunat.DistrictType();
                distrito.Value = Comprobante.EmpresaDistrito;
                direccionPL.District = distrito;
                List<ModelosSunat.AddressLineType> direcciones = new List<ModelosSunat.AddressLineType>();
                ModelosSunat.AddressLineType direccionEmisor = new ModelosSunat.AddressLineType();
                ModelosSunat.LineType local1 = new ModelosSunat.LineType();
                local1.Value = Comprobante.EmpresaDireccion;
                direccionPL.AddressLine = direcciones.ToArray();
                direccionEmisor.Line = local1;
                direcciones.Add(direccionEmisor);
                direccionPL.AddressLine = direcciones.ToArray();

                MinimusaAPI.ModelosSunat.CountryType pais = new MinimusaAPI.ModelosSunat.CountryType();
                MinimusaAPI.ModelosSunat.IdentificationCodeType codigoPais = new MinimusaAPI.ModelosSunat.IdentificationCodeType();

                codigoPais.listName = "Country";
                codigoPais.listAgencyName = "United Nations Economic Commission for Europe";
                codigoPais.listID = "ISO 3166-1";
                codigoPais.Value = "PE";
                pais.IdentificationCode = codigoPais;

                direccionPL.Country = pais;
                partelegal.RegistrationAddress = direccionPL;

                partelegals.Add(partelegal);
                party.PartyLegalEntity = partelegals.ToArray();

                party.PartyName = partynames.ToArray();
                party.PartyIdentification = partyidentificacions;
                empresa.Party = party;
                Factura.AccountingSupplierParty = empresa;

                //EMPRESA CLIENTE
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                //Tipo y número de documento de identidad del adquirente o usuario Apellidos y nombres, denominación o razón social del adquirente o usuario
                MinimusaAPI.ModelosSunat.TaxSchemeType taxschemeCliente = new MinimusaAPI.ModelosSunat.TaxSchemeType();
                MinimusaAPI.ModelosSunat.CustomerPartyType CustomerPartyCliente = new MinimusaAPI.ModelosSunat.CustomerPartyType();
                MinimusaAPI.ModelosSunat.PartyType partyCliente = new MinimusaAPI.ModelosSunat.PartyType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType partyIdentificion = new MinimusaAPI.ModelosSunat.PartyIdentificationType();
                List<MinimusaAPI.ModelosSunat.PartyIdentificationType> partyIdentificions = new List<MinimusaAPI.ModelosSunat.PartyIdentificationType>();
                MinimusaAPI.ModelosSunat.IDType idtipo = new MinimusaAPI.ModelosSunat.IDType();
                idtipo.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idtipo.schemeName = "Documento de Identidad";
                idtipo.schemeAgencyName = "PE:SUNAT";
                idtipo.schemeID = Comprobante.ClienteTipodocumento;
                idtipo.Value = Comprobante.ClienteNumeroDocumento;
                partyIdentificion.ID = idtipo;

                partyIdentificions.Add(partyIdentificion);
                partyCliente.PartyIdentification = partyIdentificions.ToArray();

                List<MinimusaAPI.ModelosSunat.PartyNameType> RazSocClientes = new List<MinimusaAPI.ModelosSunat.PartyNameType>();
                MinimusaAPI.ModelosSunat.PartyNameType RazSocCliente = new MinimusaAPI.ModelosSunat.PartyNameType();
                ModelosSunat.NameType1 razSocial = new ModelosSunat.NameType1();
                razSocial.Value = Comprobante.ClienteRazonSocial;
                RazSocCliente.Name = razSocial;
                RazSocClientes.Add(RazSocCliente);
                //partyCliente.PartyName = RazSocClientes.ToArray();


                List<MinimusaAPI.ModelosSunat.PartyTaxSchemeType> partySchemas = new List<ModelosSunat.PartyTaxSchemeType>();
                MinimusaAPI.ModelosSunat.PartyTaxSchemeType partySchema = new MinimusaAPI.ModelosSunat.PartyTaxSchemeType();
                ModelosSunat.RegistrationNameType RegistroNombre = new ModelosSunat.RegistrationNameType();
                RegistroNombre.Value = Comprobante.ClienteRazonSocial;
                partySchema.RegistrationName = RegistroNombre;

                ModelosSunat.CompanyIDType idcompañia = new ModelosSunat.CompanyIDType();
                idcompañia.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idcompañia.schemeAgencyName = "PE:SUNAT";
                idcompañia.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idcompañia.schemeID = Comprobante.ClienteTipodocumento;
                idcompañia.Value = Comprobante.ClienteNumeroDocumento;

                ModelosSunat.TaxSchemeType schemeType = new ModelosSunat.TaxSchemeType();
                ModelosSunat.IDType idc = new ModelosSunat.IDType();
                idc.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idc.schemeAgencyName = "PE:SUNAT";
                idc.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idc.schemeID = Comprobante.ClienteTipodocumento;
                idc.Value = Comprobante.ClienteNumeroDocumento;
                schemeType.ID = idc;

                idcompañia.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idcompañia.schemeAgencyName = "PE:SUNAT";
                idcompañia.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idcompañia.schemeID = Comprobante.ClienteTipodocumento;
                idcompañia.Value = Comprobante.ClienteNumeroDocumento;

                List<ModelosSunat.PartyLegalEntityType> partyLegals = new List<ModelosSunat.PartyLegalEntityType>();
                ModelosSunat.PartyLegalEntityType partyLegal = new ModelosSunat.PartyLegalEntityType();
                ModelosSunat.RegistrationNameType Registro_Nombre = new ModelosSunat.RegistrationNameType();
                Registro_Nombre.Value = Comprobante.ClienteRazonSocial;
                partyLegal.RegistrationName = Registro_Nombre;

                ModelosSunat.AddressType direccionCliente = new ModelosSunat.AddressType();
                List<ModelosSunat.AddressLineType> dirs = new List<ModelosSunat.AddressLineType>();
                ModelosSunat.AddressLineType dir = new ModelosSunat.AddressLineType();
                List<ModelosSunat.LineType> lineas = new List<ModelosSunat.LineType>();

                ModelosSunat.LineType linea = new ModelosSunat.LineType();
                linea.Value = Comprobante.ClienteDireccion;
                dir.Line = linea;
                dirs.Add(dir);
                direccionCliente.AddressLine = dirs.ToArray();
                //partyLegal.RegistrationAddress = direccionCliente;

                MinimusaAPI.ModelosSunat.CountryType paisC = new MinimusaAPI.ModelosSunat.CountryType();
                MinimusaAPI.ModelosSunat.IdentificationCodeType codigoPaisC = new MinimusaAPI.ModelosSunat.IdentificationCodeType();

                //codigoPaisC.listName = "Country";
                //codigoPaisC.listAgencyName = "United Nations Economic Commission for Europe";
                //codigoPaisC.listID = "ISO 3166-1";
                codigoPaisC.Value = "PE";
                paisC.IdentificationCode = codigoPaisC;
                //   partyLegal.RegistrationAddress.Country = paisC;
                partyLegals.Add(partyLegal);

                partySchema.CompanyID = idcompañia;
                partySchema.TaxScheme = schemeType;

                partySchemas.Add(partySchema);
                //partyCliente.PartyTaxScheme = partySchemas.ToArray();
                partyCliente.PartyLegalEntity = partyLegals.ToArray();
                CustomerPartyCliente.Party = partyCliente;

                ModelosSunat.CustomerPartyType accoutingCustomerParty = new ModelosSunat.CustomerPartyType();
                accoutingCustomerParty.Party = partyCliente;
                ///CustomerPartyCliente = partyCliente;
                Factura.AccountingCustomerParty = accoutingCustomerParty;
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Monto total de impuestos
                //Monto las operaciones gravadas
                //Monto las operaciones Exoneradas
                //Monto las operaciones inafectas del impuesto(Ver Ejemplo en la página 47)
                //Monto las operaciones gratuitas(Ver Ejemplo en la página 48)
                //Sumatoria de IGV
                //Sumatoria de ISC(Ver Ejemplo en la página 51)
                //Sumatoria de Otros Tributos(Ver Ejemplo en la página 52)

                MinimusaAPI.ModelosSunat.TaxTotalType TotalImptos = new MinimusaAPI.ModelosSunat.TaxTotalType();
                MinimusaAPI.ModelosSunat.TaxAmountType taxAmountImpto = new MinimusaAPI.ModelosSunat.TaxAmountType();
                taxAmountImpto.currencyID = Comprobante.Idmoneda;
                taxAmountImpto.Value = Convert.ToDecimal(Comprobante.TotIgv);
                TotalImptos.TaxAmount = taxAmountImpto;
                //////////////////////////////////////////////////////////////////////////////
                ///
                MinimusaAPI.ModelosSunat.TaxSubtotalType[] subtotales = new MinimusaAPI.ModelosSunat.TaxSubtotalType[2];
                MinimusaAPI.ModelosSunat.TaxSubtotalType subtotal = new MinimusaAPI.ModelosSunat.TaxSubtotalType();

                MinimusaAPI.ModelosSunat.TaxableAmountType taxsubtotal = new MinimusaAPI.ModelosSunat.TaxableAmountType();
                taxsubtotal.currencyID = Comprobante.Idmoneda;
                taxsubtotal.Value = Convert.ToDecimal(Comprobante.TotSubtotal);
                subtotal.TaxableAmount = taxsubtotal;

                MinimusaAPI.ModelosSunat.TaxAmountType TotalTaxAmountTotal = new MinimusaAPI.ModelosSunat.TaxAmountType();
                TotalTaxAmountTotal.currencyID = Comprobante.Idmoneda;
                TotalTaxAmountTotal.Value = Convert.ToDecimal(Comprobante.TotIgv);
                subtotal.TaxAmount = TotalTaxAmountTotal;

                ModelosSunat.TaxSubtotalType subTotalIGV = new ModelosSunat.TaxSubtotalType();
                subTotalIGV.TaxableAmount = taxsubtotal;

                subtotales[0] = subtotal;
                TotalImptos.TaxSubtotal = subtotales;


                //PAgo de IGV
                MinimusaAPI.ModelosSunat.TaxCategoryType taxcategoryTotal = new MinimusaAPI.ModelosSunat.TaxCategoryType();
                MinimusaAPI.ModelosSunat.TaxSchemeType taxScheme = new MinimusaAPI.ModelosSunat.TaxSchemeType();
                MinimusaAPI.ModelosSunat.IDType idTotal = new MinimusaAPI.ModelosSunat.IDType();
                idTotal.schemeID = "UN/ECE 5305";
                idTotal.schemeName = "Tax Category Identifier";
                idTotal.schemeAgencyName = "United Nations Economic Commission for Europe";
                idTotal.Value = "S";
                //taxcategoryTotal.ID = idTotal;
                MinimusaAPI.ModelosSunat.NameType1 nametypeImpto = new MinimusaAPI.ModelosSunat.NameType1();
                nametypeImpto.Value = "IGV";
                MinimusaAPI.ModelosSunat.TaxTypeCodeType taxtypecodeImpto = new MinimusaAPI.ModelosSunat.TaxTypeCodeType();
                taxtypecodeImpto.Value = "VAT";

                MinimusaAPI.ModelosSunat.IDType idTot = new MinimusaAPI.ModelosSunat.IDType();
                //idTot.schemeID = "UN/ECE 5153";
                //idTot.schemeAgencyID = "6";
                idTot.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05";
                idTot.schemeAgencyName = "PE:SUNAT";
                idTot.schemeName = "Codigo de tributos";
                idTot.Value = "1000";
                taxScheme.ID = idTot;

                MinimusaAPI.ModelosSunat.NameType1 nametypeImptoIGV = new MinimusaAPI.ModelosSunat.NameType1();
                nametypeImptoIGV.Value = "IGV";
                MinimusaAPI.ModelosSunat.TaxTypeCodeType taxtypecodeImpuesto = new MinimusaAPI.ModelosSunat.TaxTypeCodeType();
                taxtypecodeImpuesto.Value = "VAT";

                taxScheme.Name = nametypeImpto;
                taxScheme.TaxTypeCode = taxtypecodeImpto;
                taxcategoryTotal.TaxScheme = taxScheme;
                subtotal.TaxCategory = taxcategoryTotal;

                List<MinimusaAPI.ModelosSunat.TaxSubtotalType> TaxSubtotals = new List<MinimusaAPI.ModelosSunat.TaxSubtotalType>();
                TaxSubtotals.Add(subtotal);
                TotalImptos.TaxSubtotal = TaxSubtotals.ToArray();

                List<MinimusaAPI.ModelosSunat.TaxTotalType> taxTotals = new List<MinimusaAPI.ModelosSunat.TaxTotalType>();

                taxTotals.Add(TotalImptos);
                Factura.TaxTotal = taxTotals.ToArray();

                foreach (Detalles det in Comprobante.Detalles)
                {
                }
                    
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////                                  
                ////Total valor de venta 
                ////Total precio de venta(incluye impuestos) 
                ////Monto total de descuentos del comprobante 
                ////Monto total de otros cargos del comprobante 
                ////Importe total de la venta, cesión en uso o del servicio prestado
                MinimusaAPI.ModelosSunat.MonetaryTotalType TotalValorVenta = new MinimusaAPI.ModelosSunat.MonetaryTotalType();
                MinimusaAPI.ModelosSunat.LineExtensionAmountType TValorVenta = new MinimusaAPI.ModelosSunat.LineExtensionAmountType();

                TValorVenta.currencyID = Comprobante.Idmoneda;
                TValorVenta.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.TotSubtotal));
                TotalValorVenta.LineExtensionAmount = TValorVenta;

                MinimusaAPI.ModelosSunat.TaxInclusiveAmountType TotalPrecioVenta = new MinimusaAPI.ModelosSunat.TaxInclusiveAmountType();
                TotalPrecioVenta.currencyID = Comprobante.Idmoneda;
                TotalPrecioVenta.Value = Convert.ToDecimal(Comprobante.Total);
                //TotalValorVenta.TaxInclusiveAmount = TotalPrecioVenta;


                MinimusaAPI.ModelosSunat.AllowanceTotalAmountType MtoTotalDsctos = new MinimusaAPI.ModelosSunat.AllowanceTotalAmountType();
                MtoTotalDsctos.currencyID = Comprobante.Idmoneda;
                MtoTotalDsctos.Value = Convert.ToDecimal(Comprobante.TotDsctos);
                //TotalValorVenta.AllowanceTotalAmount = MtoTotalDsctos;

                MinimusaAPI.ModelosSunat.ChargeTotalAmountType MtoTotalOtrosCargos = new MinimusaAPI.ModelosSunat.ChargeTotalAmountType();
                MtoTotalOtrosCargos.currencyID = Comprobante.Idmoneda;
                MtoTotalOtrosCargos.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.TotOtros));
                TotalValorVenta.ChargeTotalAmount = MtoTotalOtrosCargos;

                MinimusaAPI.ModelosSunat.PrepaidAmountType MtoCargos = new MinimusaAPI.ModelosSunat.PrepaidAmountType();
                MtoCargos.currencyID = Comprobante.Idmoneda;
                MtoCargos.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.TotOtros));
                MtoCargos.Value = Convert.ToDecimal(string.Format("{0:0.00}", 0));
                TotalValorVenta.PrepaidAmount = MtoCargos;

                MinimusaAPI.ModelosSunat.PayableAmountType ImporteTotalVenta = new MinimusaAPI.ModelosSunat.PayableAmountType();
                ImporteTotalVenta.currencyID = Comprobante.Idmoneda;
                ImporteTotalVenta.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.Total));

                TotalValorVenta.LineExtensionAmount = TValorVenta;
                TotalValorVenta.TaxInclusiveAmount = TotalPrecioVenta;
                TotalValorVenta.AllowanceTotalAmount = MtoTotalDsctos;
                TotalValorVenta.ChargeTotalAmount = MtoTotalOtrosCargos;
                TotalValorVenta.PrepaidAmount = MtoCargos;
                TotalValorVenta.PayableAmount = ImporteTotalVenta;
                Factura.LegalMonetaryTotal = TotalValorVenta;

                //Número de orden del Ítem 
                //Cantidad y Unidad de medida por ítem 
                //Valor de venta del ítem
                //Items de Factura
                MinimusaAPI.ModelosSunat.CreditNoteLineType[] items = 
                    new MinimusaAPI.ModelosSunat.CreditNoteLineType[10];
                int iditem = 1;

                foreach (Detalles det in Comprobante.Detalles)
                {
                    MinimusaAPI.ModelosSunat.CreditNoteLineType item = 
                        new MinimusaAPI.ModelosSunat.CreditNoteLineType();
                    MinimusaAPI.ModelosSunat.IDType numeroItem = new MinimusaAPI.ModelosSunat.IDType();
                    numeroItem.Value = iditem.ToString();
                    item.ID = numeroItem;

                    MinimusaAPI.ModelosSunat.CreditedQuantityType cantidad = new MinimusaAPI.ModelosSunat.CreditedQuantityType();
                    cantidad.unitCodeListAgencyName = "United Nations Economic Commission for Europe";
                    cantidad.unitCodeListID = "UN/ECE rec 20";
                    cantidad.unitCode = det.UnidadMedida;
                  //  item.CreditedQuantity = cantidad;
                    cantidad.Value = Convert.ToInt32(det.Cantidad);
                    item.CreditedQuantity = cantidad;

                    MinimusaAPI.ModelosSunat.LineExtensionAmountType ValorVenta = new MinimusaAPI.ModelosSunat.LineExtensionAmountType();
                    ValorVenta.currencyID = Comprobante.Idmoneda;
                    ValorVenta.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.Total / 1.18m));
                    item.LineExtensionAmount = ValorVenta;

                    //Precio de venta unitario por item y código 
                    MinimusaAPI.ModelosSunat.PricingReferenceType ValorReferenUnitario = new MinimusaAPI.ModelosSunat.PricingReferenceType();
                    //ValorReferenUnitario.AlternativeConditionPrice
                    MinimusaAPI.ModelosSunat.PriceType[] TipoPrecios = new MinimusaAPI.ModelosSunat.PriceType[2];
                    MinimusaAPI.ModelosSunat.PriceType TipoPrecio = new MinimusaAPI.ModelosSunat.PriceType();

                    MinimusaAPI.ModelosSunat.PriceAmountType PrecioMonto = new MinimusaAPI.ModelosSunat.PriceAmountType();

                    PrecioMonto.currencyID = Comprobante.Idmoneda;
                    PrecioMonto.Value = Convert.ToDecimal(string.Format("{0:0.000}", det.Precio));
                    TipoPrecio.PriceAmount = PrecioMonto;

                    MinimusaAPI.ModelosSunat.PriceTypeCodeType TipoPrecioCode = new MinimusaAPI.ModelosSunat.PriceTypeCodeType();
                    TipoPrecioCode.listName = "Tipo de Precio";
                    TipoPrecioCode.listAgencyName = "PE:SUNAT";
                    TipoPrecioCode.listURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo16";
                    TipoPrecioCode.Value = "01";


                    TipoPrecio.PriceTypeCode = TipoPrecioCode;
                    TipoPrecios[0] = TipoPrecio;
                    ValorReferenUnitario.AlternativeConditionPrice = TipoPrecios;
                    item.PricingReference = ValorReferenUnitario;

                    MinimusaAPI.ModelosSunat.TaxTotalType[] Totales_Items = new MinimusaAPI.ModelosSunat.TaxTotalType[2];
                    MinimusaAPI.ModelosSunat.TaxTotalType Totales_Item = new MinimusaAPI.ModelosSunat.TaxTotalType();

                    MinimusaAPI.ModelosSunat.TaxAmountType Total_Item = new MinimusaAPI.ModelosSunat.TaxAmountType();
                    Total_Item.currencyID = Comprobante.Idmoneda;
                    Total_Item.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.mtoValorVentaItem - (det.mtoValorVentaItem / 1.18m)));
                    Totales_Item.TaxAmount = Total_Item;

                    MinimusaAPI.ModelosSunat.TaxSubtotalType[] subtotal_Items = new MinimusaAPI.ModelosSunat.TaxSubtotalType[2];
                    MinimusaAPI.ModelosSunat.TaxSubtotalType subtotal_Item = new MinimusaAPI.ModelosSunat.TaxSubtotalType();

                    MinimusaAPI.ModelosSunat.TaxableAmountType taxsubtotal_IGVItem = new MinimusaAPI.ModelosSunat.TaxableAmountType();
                    taxsubtotal_IGVItem.currencyID = Comprobante.Idmoneda;
                    taxsubtotal_IGVItem.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.mtoValorVentaItem / 1.18m));
                    subtotal_Item.TaxableAmount = taxsubtotal_IGVItem;

                    MinimusaAPI.ModelosSunat.TaxAmountType TotalTaxAmount_IGVItem = new MinimusaAPI.ModelosSunat.TaxAmountType();
                    TotalTaxAmount_IGVItem.currencyID = Comprobante.Idmoneda;
                    TotalTaxAmount_IGVItem.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.mtoValorVentaItem - (det.mtoValorVentaItem / 1.18m)));
                    subtotal_Item.TaxAmount = TotalTaxAmount_IGVItem;

                    subtotal_Items[0] = subtotal_Item;
                    Totales_Item.TaxSubtotal = subtotal_Items;

                    MinimusaAPI.ModelosSunat.TaxCategoryType taxcategory_IGVItem = new MinimusaAPI.ModelosSunat.TaxCategoryType();
                    //taxcategory_IGVItem.ID = id_IGVItem;
                    ModelosSunat.IDType idTaxCategoria = new ModelosSunat.IDType();
                    idTaxCategoria.schemeAgencyName = "United Nations Economic Commission for Europe";
                    idTaxCategoria.schemeName = "Tax Category Identifier";
                    idTaxCategoria.schemeID = "UN/ECE 5305";
                    idTaxCategoria.Value = "S";
                    //taxcategory_IGVItem.ID = idTaxCategoria;

                    ModelosSunat.PercentType1 porcentaje = new ModelosSunat.PercentType1();
                    porcentaje.Value = Convert.ToDecimal(det.porIgvItem) * 100;
                    taxcategory_IGVItem.Percent = porcentaje;
                    subtotal_Item.TaxCategory = taxcategory_IGVItem;
                    // taxcategory_IGVItem.Percent= taxcategory_IGVItem;

                    ModelosSunat.TaxExemptionReasonCodeType ReasonCode = new ModelosSunat.TaxExemptionReasonCodeType();
                    ReasonCode.listURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07";
                    ReasonCode.listName = "Afectacion del IGV";
                    ReasonCode.listAgencyName = "PE:SUNAT";
                    ReasonCode.Value = "10";

                    taxcategory_IGVItem.TaxExemptionReasonCode = ReasonCode;

                    MinimusaAPI.ModelosSunat.TaxSchemeType taxscheme_IGVItem = new MinimusaAPI.ModelosSunat.TaxSchemeType();
                    MinimusaAPI.ModelosSunat.IDType id2_IGVItem = new MinimusaAPI.ModelosSunat.IDType();

                    id2_IGVItem.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05";
                    id2_IGVItem.schemeAgencyName = "PE:SUNAT";
                    id2_IGVItem.schemeName = "Codigo de tributos";


                    id2_IGVItem.Value = "1000";
                    taxscheme_IGVItem.ID = id2_IGVItem;

                    MinimusaAPI.ModelosSunat.NameType1 nombreImpto_IGVItem = new MinimusaAPI.ModelosSunat.NameType1();
                    nombreImpto_IGVItem.Value = "IGV";
                    taxscheme_IGVItem.Name = nombreImpto_IGVItem;

                    MinimusaAPI.ModelosSunat.TaxTypeCodeType nombreImpto_IGVItemInter = new MinimusaAPI.ModelosSunat.TaxTypeCodeType();
                    nombreImpto_IGVItemInter.Value = "VAT";
                    taxscheme_IGVItem.TaxTypeCode = nombreImpto_IGVItemInter;
                    taxscheme_IGVItem.Name = nombreImpto_IGVItem;

                    //taxcategory_IGVItem.TaxExemptionReasonCode = CodRazon_IGVItem;
                    taxcategory_IGVItem.TaxScheme = taxscheme_IGVItem;
                    //subtotal_Item.TaxableAmount = taxsubtotal_IGVItem;

                    subtotal_Items[0] = subtotal_Item;
                    Totales_Item.TaxSubtotal = subtotal_Items;
                    Totales_Items[0] = Totales_Item;

                    item.TaxTotal = Totales_Items;

                    MinimusaAPI.ModelosSunat.DescriptionType[] descriptions = new MinimusaAPI.ModelosSunat.DescriptionType[2];
                    MinimusaAPI.ModelosSunat.DescriptionType description = new MinimusaAPI.ModelosSunat.DescriptionType();
                    description.Value = det.DescripcionProducto;
                    MinimusaAPI.ModelosSunat.ItemIdentificationType codigoProd = new MinimusaAPI.ModelosSunat.ItemIdentificationType();
                    MinimusaAPI.ModelosSunat.IDType id = new MinimusaAPI.ModelosSunat.IDType();
                    id.Value = det.Codcom;
                    codigoProd.ID = id;

                    MinimusaAPI.ModelosSunat.PriceType PrecioProducto = new MinimusaAPI.ModelosSunat.PriceType();
                    MinimusaAPI.ModelosSunat.PriceAmountType PrecioMontoTipo = new MinimusaAPI.ModelosSunat.PriceAmountType();
                    PrecioMontoTipo.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.Precio / (det.porIgvItem + 1)));
                    //PrecioMontoTipo.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.Precio));
                    PrecioMontoTipo.currencyID = Comprobante.Idmoneda;
                    PrecioProducto.PriceAmount = PrecioMontoTipo;

                    MinimusaAPI.ModelosSunat.ItemType itemTipo = new MinimusaAPI.ModelosSunat.ItemType();
                    descriptions[0] = description;
                    itemTipo.Description = descriptions;
                    itemTipo.SellersItemIdentification = codigoProd;

                    List<ModelosSunat.CommodityClassificationType> codSunats = new List<ModelosSunat.CommodityClassificationType>();
                    ModelosSunat.CommodityClassificationType codSunat = new ModelosSunat.CommodityClassificationType();
                    ModelosSunat.ItemClassificationCodeType codClas = new ModelosSunat.ItemClassificationCodeType();
                    codClas.listName = "Item Classification";
                    codClas.listAgencyName = "GS1 US";
                    codClas.listID = "UNSPSC";
                    codClas.Value = "25172405";
                    codSunat.ItemClassificationCode = codClas;
                    codSunats.Add(codSunat);
                    itemTipo.CommodityClassification = codSunats.ToArray();

                    item.Item = itemTipo;
                    item.Price = PrecioProducto;
                    //Item_Adicional.AdditionalItemProperty = Propiedades;
                    //Item_Adicionales.Item = Item_Adicional;            

                    //items[1] = item_OpeOnerosa;
                    // items[2] = item_DsctoItem;
                    // items[3] = item_DsctoCargoItem;
                    //items[4] = item_IGVItem;
                    //items[5] = item_ISCItem;
                    // items[6] = Item_Descripcion;
                    //  items[7] = Item_CodProSUNAT;
                    //   items[8] = Item_Adicionales;
                    items[iditem] = item;
                    iditem += 1;
                }
                Factura.CreditNoteLine = items;

                string archXML = GenerarComprobante(Factura, Comprobante.EmpresaRUC, Comprobante.Idtipocomp, Comprobante.Serie, Comprobante.Numero);
                FirmarXML(archXML, Ruta_Certificado, Password_Certificado);
                string strEnvio = Ruta_ENVIOS + Path.GetFileName(archXML).Replace(".xml", ".zip");
                Comprimir(archXML, strEnvio);
                EnviarDocumento(strEnvio);
                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int GenerarComprobanteND_XML(Cabecera Comprobante)
        {
            MinimusaAPI.ModelosSunat.DebitNoteType Factura = new MinimusaAPI.ModelosSunat.DebitNoteType();

            try
            {
                //------Namespace necesarios para el UBL
                Factura.Cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                Factura.Cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                Factura.Ccts = "urn:un:unece:uncefact:documentation:2";
                Factura.Ds = "http://www.w3.org/2000/09/xmldsig#";
                Factura.Ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";
                Factura.Qdt = "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2";
                Factura.Udt = "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2";
                //------
                //-----Datos de pruebas de facturas
                //UBLExtensionsType[] ublExtensiones = new UBLExtensionsType[5];

                MinimusaAPI.ModelosSunat.UBLExtensionType[] ublExtensiones = new MinimusaAPI.ModelosSunat.UBLExtensionType[5];
                MinimusaAPI.ModelosSunat.UBLExtensionType ublExtension = new MinimusaAPI.ModelosSunat.UBLExtensionType();

                ublExtensiones[0] = ublExtension;
                Factura.UBLExtensions = ublExtensiones;

                Factura.UBLVersionID = new MinimusaAPI.ModelosSunat.UBLVersionIDType();
                Factura.UBLVersionID.Value = "2.1";

                Factura.CustomizationID = new MinimusaAPI.ModelosSunat.CustomizationIDType();
                //Factura.CustomizationID.schemeAgencyName = "PE:SUNAT";
                Factura.CustomizationID.Value = "2.0";
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Código de tipo de operación
                //Factura.ProfileID = new ProfileIDType();
                //Factura.ProfileID.schemeAgencyName = "PE:SUNAT";
                //Factura.ProfileID.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo51";
                //Factura.ProfileID.schemeName = "Tipo de Operacion";
                //Factura.ProfileID.Value = Comprobante.TipoOperacion;

                //Numeración, conformada por serie y número correlativo
                Factura.ID = new MinimusaAPI.ModelosSunat.IDType();
                Factura.ID.Value = Comprobante.Serie + "-" + Comprobante.Numero;
                //Fecha de emisión y hora de emision
                Factura.IssueDate = new MinimusaAPI.ModelosSunat.IssueDateType();
                string fechaemision = Convert.ToDateTime(Comprobante.Fechaemision).ToString("dd/MM/yyyy");
                Factura.IssueDate.Value = Convert.ToDateTime(fechaemision);
                Factura.IssueTime = new MinimusaAPI.ModelosSunat.IssueTimeType();
                string hora = Convert.ToDateTime(Comprobante.Fechaemision).ToString("HH:mm:ss");
                Factura.IssueTime.Value = hora;
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Tipo de Moneda
                MinimusaAPI.ModelosSunat.DocumentCurrencyCodeType moneda = new MinimusaAPI.ModelosSunat.DocumentCurrencyCodeType()
                {
                    listSchemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo51",
                    listID = "ISO 4217 Alpha",
                    listName = "Currency",
                    listAgencyName = "United Nations Economic Commission for Europe",
                    Value = Comprobante.Idmoneda
                };
                Factura.DocumentCurrencyCode = moneda;
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                MinimusaAPI.ModelosSunat.ResponseType DocumentoRel = new MinimusaAPI.ModelosSunat.ResponseType();
                MinimusaAPI.ModelosSunat.ResponseType[] DocumentoRels = new MinimusaAPI.ModelosSunat.ResponseType[2];
                MinimusaAPI.ModelosSunat.ReferenceIDType NumeroDocRel = new MinimusaAPI.ModelosSunat.ReferenceIDType();
                //NumeroDocRel.Value = Comprobante.Cab_Ref_Serie + "-" + Comprobante.Cab_Ref_Numero;
                NumeroDocRel.Value = Comprobante.Serie + "-" + Comprobante.Numero;
                MinimusaAPI.ModelosSunat.ResponseCodeType TipoDocRel = new MinimusaAPI.ModelosSunat.ResponseCodeType();
                //TipoDocRel.Value = Comprobante.Cab_Ref_TipoNotaCredito;
                TipoDocRel.Value = Comprobante.Cab_Ref_TipoNotaDebito;
                MinimusaAPI.ModelosSunat.DescriptionType Motivo = new MinimusaAPI.ModelosSunat.DescriptionType();
                MinimusaAPI.ModelosSunat.DescriptionType[] Motivos = new MinimusaAPI.ModelosSunat.DescriptionType[2];
                Motivos[0] = Motivo;
                Motivo.Value = Comprobante.Cab_Ref_Motivo;
                DocumentoRel.ReferenceID = NumeroDocRel;
                DocumentoRel.ResponseCode = TipoDocRel;
                DocumentoRel.Description = Motivos;
                DocumentoRels[0] = DocumentoRel;
                Factura.DiscrepancyResponse = DocumentoRels;

                MinimusaAPI.ModelosSunat.BillingReferenceType[] referencias = new MinimusaAPI.ModelosSunat.BillingReferenceType[2];
                MinimusaAPI.ModelosSunat.BillingReferenceType referencia = new MinimusaAPI.ModelosSunat.BillingReferenceType();

                MinimusaAPI.ModelosSunat.DocumentReferenceType documento = new MinimusaAPI.ModelosSunat.DocumentReferenceType();
                MinimusaAPI.ModelosSunat.IDType docRela = new MinimusaAPI.ModelosSunat.IDType();
                docRela.Value = Comprobante.Cab_Ref_Serie + "-" + Comprobante.Cab_Ref_Numero;
                MinimusaAPI.ModelosSunat.DocumentTypeCodeType TipoDocumentoRel = new MinimusaAPI.ModelosSunat.DocumentTypeCodeType();
                TipoDocumentoRel.Value = Comprobante.Cab_Ref_TipoDeDocumento;
                documento.DocumentTypeCode = TipoDocumentoRel;
                documento.ID = docRela;

                referencia.InvoiceDocumentReference = documento;
                referencias[0] = referencia;
                Factura.BillingReference = referencias;

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                //Nombre Comercial del emisor Apellidos y nombres, denominación o razón social del emisor Tipo y Número de RUC del emisor Código del domicilio fiscal 
                //o de local anexo del emisor 
                MinimusaAPI.ModelosSunat.SignatureType Firma = new MinimusaAPI.ModelosSunat.SignatureType();
                MinimusaAPI.ModelosSunat.SignatureType[] Firmas = new MinimusaAPI.ModelosSunat.SignatureType[2];

                MinimusaAPI.ModelosSunat.PartyType partySign = new MinimusaAPI.ModelosSunat.PartyType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType partyIdentificacion = new MinimusaAPI.ModelosSunat.PartyIdentificationType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType[] partyIdentificacions = new MinimusaAPI.ModelosSunat.PartyIdentificationType[2];
                MinimusaAPI.ModelosSunat.IDType idFirma = new MinimusaAPI.ModelosSunat.IDType();
                idFirma.Value = Comprobante.EmpresaRUC;
                Firma.ID = idFirma;

                partyIdentificacion.ID = idFirma;
                partyIdentificacions[0] = partyIdentificacion;
                partySign.PartyIdentification = partyIdentificacions;
                Firma.SignatoryParty = partySign;

                MinimusaAPI.ModelosSunat.SupplierPartyType empresa = new MinimusaAPI.ModelosSunat.SupplierPartyType();
                MinimusaAPI.ModelosSunat.PartyType party = new MinimusaAPI.ModelosSunat.PartyType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType partyidentificacion = new MinimusaAPI.ModelosSunat.PartyIdentificationType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType[] partyidentificacions = new MinimusaAPI.ModelosSunat.PartyIdentificationType[2];
                MinimusaAPI.ModelosSunat.IDType idEmpresa = new MinimusaAPI.ModelosSunat.IDType();

                idEmpresa.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idEmpresa.schemeName = "Documento de Identidad";
                idEmpresa.schemeID = "6";
                idEmpresa.schemeAgencyName = "PE:SUNAT";
                idEmpresa.Value = Comprobante.EmpresaRUC;

                partyidentificacion.ID = idEmpresa;
                partyidentificacions[0] = partyidentificacion;
                party.PartyIdentification = partyidentificacions;

                MinimusaAPI.ModelosSunat.PartyNameType partyname = new MinimusaAPI.ModelosSunat.PartyNameType();
                List<MinimusaAPI.ModelosSunat.PartyNameType> partynames = new List<MinimusaAPI.ModelosSunat.PartyNameType>();
                MinimusaAPI.ModelosSunat.NameType1 nameEmisor = new MinimusaAPI.ModelosSunat.NameType1();
                nameEmisor.Value = Comprobante.EmpresaRazonSocial;
                partyname.Name = nameEmisor;
                partynames.Add(partyname);
                party.PartyName = partynames.ToArray();

                //MinimusaAPI.ModelosSunat.PartyTaxSchemeType PartyTaxScheme = new MinimusaAPI.ModelosSunat.PartyTaxSchemeType();
                //List<MinimusaAPI.ModelosSunat.PartyTaxSchemeType> PartyTaxSchemes = new List<MinimusaAPI.ModelosSunat.PartyTaxSchemeType>();

                MinimusaAPI.ModelosSunat.RegistrationNameType registerNameEmisor = new MinimusaAPI.ModelosSunat.RegistrationNameType();
                registerNameEmisor.Value = Comprobante.EmpresaRazonSocial;
                ///PartyTaxScheme.RegistrationName = registerNameEmisor;
                //Direccion emisor                
                ModelosSunat.CompanyIDType compañia = new ModelosSunat.CompanyIDType();
                compañia.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                compañia.schemeAgencyName = "PE:SUNAT";
                compañia.schemeName = "SUNAT:Identificador de Documento de Identidad";
                compañia.schemeID = "6";
                compañia.Value = Comprobante.EmpresaRUC;

                MinimusaAPI.ModelosSunat.AddressType direccion = new MinimusaAPI.ModelosSunat.AddressType();
                MinimusaAPI.ModelosSunat.AddressTypeCodeType addrestypecode = new MinimusaAPI.ModelosSunat.AddressTypeCodeType();
                addrestypecode.listName = "Establecimientos anexos";
                addrestypecode.listAgencyName = "PE:SUNAT";
                addrestypecode.Value = "0000";
                //direccion.AddressTypeCode = addrestypecode;
                //PartyTaxScheme.RegistrationAddress = direccion;
                //ModelosSunat.IDType tipo = new ModelosSunat.IDType();
                ModelosSunat.TaxSchemeType taxSchema = new ModelosSunat.TaxSchemeType();
                ModelosSunat.IDType idsupplier = new ModelosSunat.IDType();
                idsupplier.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idsupplier.schemeAgencyName = "PE:SUNAT";
                idsupplier.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idsupplier.schemeID = "6";
                idsupplier.Value = Comprobante.EmpresaRUC;
                taxSchema.ID = idsupplier;

                //PartyTaxScheme.CompanyID = compañia;
                //PartyTaxScheme.TaxScheme = taxSchema;
                //PartyTaxSchemes.Add(PartyTaxScheme);
                //party.PartyTaxScheme = PartyTaxSchemes.ToArray();

                List<ModelosSunat.PartyLegalEntityType> partelegals = new List<ModelosSunat.PartyLegalEntityType>();
                ModelosSunat.PartyLegalEntityType partelegal = new ModelosSunat.PartyLegalEntityType();
                MinimusaAPI.ModelosSunat.RegistrationNameType registerNamePL = new MinimusaAPI.ModelosSunat.RegistrationNameType();
                registerNamePL.Value = Comprobante.EmpresaRazonSocial;
                partelegal.RegistrationName = registerNamePL;

                ModelosSunat.AddressType direccionPL = new ModelosSunat.AddressType();
                ModelosSunat.IDType iddireccionPL = new ModelosSunat.IDType();
                iddireccionPL.schemeAgencyName = "PE:INEI";
                iddireccionPL.schemeName = "Ubigeos";
                iddireccionPL.Value = Comprobante.ID_EmpresaDepartamento + Comprobante.ID_EmpresaProvincia + Comprobante.ID_EmpresaDistrito;
                direccionPL.ID = iddireccionPL;

                ModelosSunat.AddressTypeCodeType address_TypeCodeType = new ModelosSunat.AddressTypeCodeType();
                address_TypeCodeType.listName = "Establecimientos anexos";
                address_TypeCodeType.listAgencyName = "PE:SUNAT";
                address_TypeCodeType.Value = "0001";
                direccionPL.AddressTypeCode = address_TypeCodeType;

                MinimusaAPI.ModelosSunat.CityNameType Departamento = new MinimusaAPI.ModelosSunat.CityNameType();
                Departamento.Value = Comprobante.EmpresaDepartamento;
                direccionPL.CityName = Departamento;

                MinimusaAPI.ModelosSunat.CountrySubentityType Provincia = new MinimusaAPI.ModelosSunat.CountrySubentityType();
                Provincia.Value = Comprobante.EmpresaProvincia;
                direccionPL.CountrySubentity = Provincia;

                MinimusaAPI.ModelosSunat.DistrictType distrito = new MinimusaAPI.ModelosSunat.DistrictType();
                distrito.Value = Comprobante.EmpresaDistrito;
                direccionPL.District = distrito;
                List<ModelosSunat.AddressLineType> direcciones = new List<ModelosSunat.AddressLineType>();
                ModelosSunat.AddressLineType direccionEmisor = new ModelosSunat.AddressLineType();
                ModelosSunat.LineType local1 = new ModelosSunat.LineType();
                local1.Value = Comprobante.EmpresaDireccion;
                direccionPL.AddressLine = direcciones.ToArray();
                direccionEmisor.Line = local1;
                direcciones.Add(direccionEmisor);
                direccionPL.AddressLine = direcciones.ToArray();

                MinimusaAPI.ModelosSunat.CountryType pais = new MinimusaAPI.ModelosSunat.CountryType();
                MinimusaAPI.ModelosSunat.IdentificationCodeType codigoPais = new MinimusaAPI.ModelosSunat.IdentificationCodeType();

                codigoPais.listName = "Country";
                codigoPais.listAgencyName = "United Nations Economic Commission for Europe";
                codigoPais.listID = "ISO 3166-1";
                codigoPais.Value = "PE";
                pais.IdentificationCode = codigoPais;

                direccionPL.Country = pais;
                partelegal.RegistrationAddress = direccionPL;

                partelegals.Add(partelegal);
                party.PartyLegalEntity = partelegals.ToArray();

                party.PartyName = partynames.ToArray();
                party.PartyIdentification = partyidentificacions;
                empresa.Party = party;
                Factura.AccountingSupplierParty = empresa;

                //EMPRESA CLIENTE
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                //Tipo y número de documento de identidad del adquirente o usuario Apellidos y nombres, denominación o razón social del adquirente o usuario
                MinimusaAPI.ModelosSunat.TaxSchemeType taxschemeCliente = new MinimusaAPI.ModelosSunat.TaxSchemeType();
                MinimusaAPI.ModelosSunat.CustomerPartyType CustomerPartyCliente = new MinimusaAPI.ModelosSunat.CustomerPartyType();
                MinimusaAPI.ModelosSunat.PartyType partyCliente = new MinimusaAPI.ModelosSunat.PartyType();
                MinimusaAPI.ModelosSunat.PartyIdentificationType partyIdentificion = new MinimusaAPI.ModelosSunat.PartyIdentificationType();
                List<MinimusaAPI.ModelosSunat.PartyIdentificationType> partyIdentificions = new List<MinimusaAPI.ModelosSunat.PartyIdentificationType>();
                MinimusaAPI.ModelosSunat.IDType idtipo = new MinimusaAPI.ModelosSunat.IDType();
                idtipo.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idtipo.schemeName = "Documento de Identidad";
                idtipo.schemeAgencyName = "PE:SUNAT";
                idtipo.schemeID = Comprobante.ClienteTipodocumento;
                idtipo.Value = Comprobante.ClienteNumeroDocumento;
                partyIdentificion.ID = idtipo;

                partyIdentificions.Add(partyIdentificion);
                partyCliente.PartyIdentification = partyIdentificions.ToArray();

                List<MinimusaAPI.ModelosSunat.PartyNameType> RazSocClientes = new List<MinimusaAPI.ModelosSunat.PartyNameType>();
                MinimusaAPI.ModelosSunat.PartyNameType RazSocCliente = new MinimusaAPI.ModelosSunat.PartyNameType();
                ModelosSunat.NameType1 razSocial = new ModelosSunat.NameType1();
                razSocial.Value = Comprobante.ClienteRazonSocial;
                RazSocCliente.Name = razSocial;
                RazSocClientes.Add(RazSocCliente);
                //partyCliente.PartyName = RazSocClientes.ToArray();


                List<MinimusaAPI.ModelosSunat.PartyTaxSchemeType> partySchemas = new List<ModelosSunat.PartyTaxSchemeType>();
                MinimusaAPI.ModelosSunat.PartyTaxSchemeType partySchema = new MinimusaAPI.ModelosSunat.PartyTaxSchemeType();
                ModelosSunat.RegistrationNameType RegistroNombre = new ModelosSunat.RegistrationNameType();
                RegistroNombre.Value = Comprobante.ClienteRazonSocial;
                partySchema.RegistrationName = RegistroNombre;

                ModelosSunat.CompanyIDType idcompañia = new ModelosSunat.CompanyIDType();
                idcompañia.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idcompañia.schemeAgencyName = "PE:SUNAT";
                idcompañia.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idcompañia.schemeID = Comprobante.ClienteTipodocumento;
                idcompañia.Value = Comprobante.ClienteNumeroDocumento;

                ModelosSunat.TaxSchemeType schemeType = new ModelosSunat.TaxSchemeType();
                ModelosSunat.IDType idc = new ModelosSunat.IDType();
                idc.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idc.schemeAgencyName = "PE:SUNAT";
                idc.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idc.schemeID = Comprobante.ClienteTipodocumento;
                idc.Value = Comprobante.ClienteNumeroDocumento;
                schemeType.ID = idc;

                idcompañia.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06";
                idcompañia.schemeAgencyName = "PE:SUNAT";
                idcompañia.schemeName = "SUNAT:Identificador de Documento de Identidad";
                idcompañia.schemeID = Comprobante.ClienteTipodocumento;
                idcompañia.Value = Comprobante.ClienteNumeroDocumento;

                List<ModelosSunat.PartyLegalEntityType> partyLegals = new List<ModelosSunat.PartyLegalEntityType>();
                ModelosSunat.PartyLegalEntityType partyLegal = new ModelosSunat.PartyLegalEntityType();
                ModelosSunat.RegistrationNameType Registro_Nombre = new ModelosSunat.RegistrationNameType();
                Registro_Nombre.Value = Comprobante.ClienteRazonSocial;
                partyLegal.RegistrationName = Registro_Nombre;

                ModelosSunat.AddressType direccionCliente = new ModelosSunat.AddressType();
                List<ModelosSunat.AddressLineType> dirs = new List<ModelosSunat.AddressLineType>();
                ModelosSunat.AddressLineType dir = new ModelosSunat.AddressLineType();
                List<ModelosSunat.LineType> lineas = new List<ModelosSunat.LineType>();

                ModelosSunat.LineType linea = new ModelosSunat.LineType();
                linea.Value = Comprobante.ClienteDireccion;
                dir.Line = linea;
                dirs.Add(dir);
                direccionCliente.AddressLine = dirs.ToArray();
                //partyLegal.RegistrationAddress = direccionCliente;

                MinimusaAPI.ModelosSunat.CountryType paisC = new MinimusaAPI.ModelosSunat.CountryType();
                MinimusaAPI.ModelosSunat.IdentificationCodeType codigoPaisC = new MinimusaAPI.ModelosSunat.IdentificationCodeType();

                //codigoPaisC.listName = "Country";
                //codigoPaisC.listAgencyName = "United Nations Economic Commission for Europe";
                //codigoPaisC.listID = "ISO 3166-1";
                codigoPaisC.Value = "PE";
                paisC.IdentificationCode = codigoPaisC;
                //   partyLegal.RegistrationAddress.Country = paisC;
                partyLegals.Add(partyLegal);

                partySchema.CompanyID = idcompañia;
                partySchema.TaxScheme = schemeType;

                partySchemas.Add(partySchema);
                //partyCliente.PartyTaxScheme = partySchemas.ToArray();
                partyCliente.PartyLegalEntity = partyLegals.ToArray();
                CustomerPartyCliente.Party = partyCliente;

                ModelosSunat.CustomerPartyType accoutingCustomerParty = new ModelosSunat.CustomerPartyType();
                accoutingCustomerParty.Party = partyCliente;
                ///CustomerPartyCliente = partyCliente;
                Factura.AccountingCustomerParty = accoutingCustomerParty;
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Monto total de impuestos
                //Monto las operaciones gravadas
                //Monto las operaciones Exoneradas
                //Monto las operaciones inafectas del impuesto(Ver Ejemplo en la página 47)
                //Monto las operaciones gratuitas(Ver Ejemplo en la página 48)
                //Sumatoria de IGV
                //Sumatoria de ISC(Ver Ejemplo en la página 51)
                //Sumatoria de Otros Tributos(Ver Ejemplo en la página 52)

                MinimusaAPI.ModelosSunat.TaxTotalType TotalImptos = new MinimusaAPI.ModelosSunat.TaxTotalType();
                MinimusaAPI.ModelosSunat.TaxAmountType taxAmountImpto = new MinimusaAPI.ModelosSunat.TaxAmountType();
                taxAmountImpto.currencyID = Comprobante.Idmoneda;
                taxAmountImpto.Value = Convert.ToDecimal(Comprobante.TotIgv);
                TotalImptos.TaxAmount = taxAmountImpto;
                //////////////////////////////////////////////////////////////////////////////
                ///
                MinimusaAPI.ModelosSunat.TaxSubtotalType[] subtotales = new MinimusaAPI.ModelosSunat.TaxSubtotalType[2];
                MinimusaAPI.ModelosSunat.TaxSubtotalType subtotal = new MinimusaAPI.ModelosSunat.TaxSubtotalType();

                MinimusaAPI.ModelosSunat.TaxableAmountType taxsubtotal = new MinimusaAPI.ModelosSunat.TaxableAmountType();
                taxsubtotal.currencyID = Comprobante.Idmoneda;
                taxsubtotal.Value = Convert.ToDecimal(Comprobante.TotSubtotal);
                subtotal.TaxableAmount = taxsubtotal;

                MinimusaAPI.ModelosSunat.TaxAmountType TotalTaxAmountTotal = new MinimusaAPI.ModelosSunat.TaxAmountType();
                TotalTaxAmountTotal.currencyID = Comprobante.Idmoneda;
                TotalTaxAmountTotal.Value = Convert.ToDecimal(Comprobante.TotIgv);
                subtotal.TaxAmount = TotalTaxAmountTotal;

                ModelosSunat.TaxSubtotalType subTotalIGV = new ModelosSunat.TaxSubtotalType();
                subTotalIGV.TaxableAmount = taxsubtotal;

                subtotales[0] = subtotal;
                TotalImptos.TaxSubtotal = subtotales;


                //PAgo de IGV
                MinimusaAPI.ModelosSunat.TaxCategoryType taxcategoryTotal = new MinimusaAPI.ModelosSunat.TaxCategoryType();
                MinimusaAPI.ModelosSunat.TaxSchemeType taxScheme = new MinimusaAPI.ModelosSunat.TaxSchemeType();
                MinimusaAPI.ModelosSunat.IDType idTotal = new MinimusaAPI.ModelosSunat.IDType();
                idTotal.schemeID = "UN/ECE 5305";
                idTotal.schemeName = "Tax Category Identifier";
                idTotal.schemeAgencyName = "United Nations Economic Commission for Europe";
                idTotal.Value = "S";
                //taxcategoryTotal.ID = idTotal;
                MinimusaAPI.ModelosSunat.NameType1 nametypeImpto = new MinimusaAPI.ModelosSunat.NameType1();
                nametypeImpto.Value = "IGV";
                MinimusaAPI.ModelosSunat.TaxTypeCodeType taxtypecodeImpto = new MinimusaAPI.ModelosSunat.TaxTypeCodeType();
                taxtypecodeImpto.Value = "VAT";

                MinimusaAPI.ModelosSunat.IDType idTot = new MinimusaAPI.ModelosSunat.IDType();
                //idTot.schemeID = "UN/ECE 5153";
                //idTot.schemeAgencyID = "6";
                idTot.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05";
                idTot.schemeAgencyName = "PE:SUNAT";
                idTot.schemeName = "Codigo de tributos";
                idTot.Value = "1000";
                taxScheme.ID = idTot;

                MinimusaAPI.ModelosSunat.NameType1 nametypeImptoIGV = new MinimusaAPI.ModelosSunat.NameType1();
                nametypeImptoIGV.Value = "IGV";
                MinimusaAPI.ModelosSunat.TaxTypeCodeType taxtypecodeImpuesto = new MinimusaAPI.ModelosSunat.TaxTypeCodeType();
                taxtypecodeImpuesto.Value = "VAT";

                taxScheme.Name = nametypeImpto;
                taxScheme.TaxTypeCode = taxtypecodeImpto;
                taxcategoryTotal.TaxScheme = taxScheme;
                subtotal.TaxCategory = taxcategoryTotal;

                List<MinimusaAPI.ModelosSunat.TaxSubtotalType> TaxSubtotals = new List<MinimusaAPI.ModelosSunat.TaxSubtotalType>();
                TaxSubtotals.Add(subtotal);
                TotalImptos.TaxSubtotal = TaxSubtotals.ToArray();

                List<MinimusaAPI.ModelosSunat.TaxTotalType> taxTotals = new List<MinimusaAPI.ModelosSunat.TaxTotalType>();

                taxTotals.Add(TotalImptos);
                Factura.TaxTotal = taxTotals.ToArray();
                              
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////                                  
                ////Total valor de venta 
                ////Total precio de venta(incluye impuestos) 
                ////Monto total de descuentos del comprobante 
                ////Monto total de otros cargos del comprobante 
                ////Importe total de la venta, cesión en uso o del servicio prestado
                
               
                MinimusaAPI.ModelosSunat.MonetaryTotalType TotalValorVenta = new MinimusaAPI.ModelosSunat.MonetaryTotalType();
                MinimusaAPI.ModelosSunat.PayableAmountType ImporteTotalVenta = new MinimusaAPI.ModelosSunat.PayableAmountType();
                ImporteTotalVenta.currencyID = Comprobante.Idmoneda;
                ImporteTotalVenta.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.Total));
                TotalValorVenta.PayableAmount = ImporteTotalVenta;
                Factura.RequestedMonetaryTotal = TotalValorVenta;
                //MinimusaAPI.ModelosSunat.LineExtensionAmountType TValorVenta = new MinimusaAPI.ModelosSunat.LineExtensionAmountType();

                //TValorVenta.currencyID = Comprobante.Idmoneda;
                //TValorVenta.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.TotSubtotal));
                //TotalValorVenta.LineExtensionAmount = TValorVenta;

                //MinimusaAPI.ModelosSunat.TaxInclusiveAmountType TotalPrecioVenta = new MinimusaAPI.ModelosSunat.TaxInclusiveAmountType();
                //TotalPrecioVenta.currencyID = Comprobante.Idmoneda;
                //TotalPrecioVenta.Value = Convert.ToDecimal(Comprobante.Total);
                ////TotalValorVenta.TaxInclusiveAmount = TotalPrecioVenta;


                //MinimusaAPI.ModelosSunat.AllowanceTotalAmountType MtoTotalDsctos = new MinimusaAPI.ModelosSunat.AllowanceTotalAmountType();
                //MtoTotalDsctos.currencyID = Comprobante.Idmoneda;
                //MtoTotalDsctos.Value = Convert.ToDecimal(Comprobante.TotDsctos);
                ////TotalValorVenta.AllowanceTotalAmount = MtoTotalDsctos;

                //MinimusaAPI.ModelosSunat.ChargeTotalAmountType MtoTotalOtrosCargos = new MinimusaAPI.ModelosSunat.ChargeTotalAmountType();
                //MtoTotalOtrosCargos.currencyID = Comprobante.Idmoneda;
                //MtoTotalOtrosCargos.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.TotOtros));
                //TotalValorVenta.ChargeTotalAmount = MtoTotalOtrosCargos;

                //MinimusaAPI.ModelosSunat.PrepaidAmountType MtoCargos = new MinimusaAPI.ModelosSunat.PrepaidAmountType();
                //MtoCargos.currencyID = Comprobante.Idmoneda;
                //MtoCargos.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.TotOtros));
                //MtoCargos.Value = Convert.ToDecimal(string.Format("{0:0.00}", 0));
                //TotalValorVenta.PrepaidAmount = MtoCargos;

                //MinimusaAPI.ModelosSunat.PayableAmountType ImporteTotalVenta = new MinimusaAPI.ModelosSunat.PayableAmountType();
                //ImporteTotalVenta.currencyID = Comprobante.Idmoneda;
                //ImporteTotalVenta.Value = Convert.ToDecimal(string.Format("{0:0.00}", Comprobante.Total));

                //TotalValorVenta.LineExtensionAmount = TValorVenta;
                //TotalValorVenta.TaxInclusiveAmount = TotalPrecioVenta;
                //TotalValorVenta.AllowanceTotalAmount = MtoTotalDsctos;
                //TotalValorVenta.ChargeTotalAmount = MtoTotalOtrosCargos;
                //TotalValorVenta.PrepaidAmount = MtoCargos;
                //TotalValorVenta.PayableAmount = ImporteTotalVenta;
                //Factura.LegalMonetaryTotal = TotalValorVenta;




                //Número de orden del Ítem 
                //Cantidad y Unidad de medida por ítem 
                //Valor de venta del ítem
                //Items de Factura
                MinimusaAPI.ModelosSunat.DebitNoteLineType[] items =
                    new MinimusaAPI.ModelosSunat.DebitNoteLineType[10];
                int iditem = 1;

                foreach (Detalles det in Comprobante.Detalles)
                {
                    MinimusaAPI.ModelosSunat.DebitNoteLineType item =
                        new MinimusaAPI.ModelosSunat.DebitNoteLineType();
                    MinimusaAPI.ModelosSunat.IDType numeroItem = new MinimusaAPI.ModelosSunat.IDType();
                    numeroItem.Value = iditem.ToString();
                    item.ID = numeroItem;

                    MinimusaAPI.ModelosSunat.DebitedQuantityType cantidad = new MinimusaAPI.ModelosSunat.DebitedQuantityType();
                    cantidad.unitCodeListAgencyName = "United Nations Economic Commission for Europe";
                    cantidad.unitCodeListID = "UN/ECE rec 20";
                    cantidad.unitCode = det.UnidadMedida;
                    //  item.DebitedQuantity = cantidad;
                    cantidad.Value = Convert.ToInt32(det.Cantidad);
                    item.DebitedQuantity = cantidad;

                    MinimusaAPI.ModelosSunat.LineExtensionAmountType ValorVenta = new MinimusaAPI.ModelosSunat.LineExtensionAmountType();
                    ValorVenta.currencyID = Comprobante.Idmoneda;
                    ValorVenta.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.Total / 1.18m));
                    item.LineExtensionAmount = ValorVenta;

                    //Precio de venta unitario por item y código 
                    MinimusaAPI.ModelosSunat.PricingReferenceType ValorReferenUnitario = new MinimusaAPI.ModelosSunat.PricingReferenceType();
                    //ValorReferenUnitario.AlternativeConditionPrice
                    MinimusaAPI.ModelosSunat.PriceType[] TipoPrecios = new MinimusaAPI.ModelosSunat.PriceType[2];
                    MinimusaAPI.ModelosSunat.PriceType TipoPrecio = new MinimusaAPI.ModelosSunat.PriceType();

                    MinimusaAPI.ModelosSunat.PriceAmountType PrecioMonto = new MinimusaAPI.ModelosSunat.PriceAmountType();

                    PrecioMonto.currencyID = Comprobante.Idmoneda;
                    PrecioMonto.Value = Convert.ToDecimal(string.Format("{0:0.000}", det.Precio));
                    TipoPrecio.PriceAmount = PrecioMonto;

                    MinimusaAPI.ModelosSunat.PriceTypeCodeType TipoPrecioCode = new MinimusaAPI.ModelosSunat.PriceTypeCodeType();
                    TipoPrecioCode.listName = "Tipo de Precio";
                    TipoPrecioCode.listAgencyName = "PE:SUNAT";
                    TipoPrecioCode.listURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo16";
                    TipoPrecioCode.Value = "01";


                    TipoPrecio.PriceTypeCode = TipoPrecioCode;
                    TipoPrecios[0] = TipoPrecio;
                    ValorReferenUnitario.AlternativeConditionPrice = TipoPrecios;
                    item.PricingReference = ValorReferenUnitario;

                    MinimusaAPI.ModelosSunat.TaxTotalType[] Totales_Items = new MinimusaAPI.ModelosSunat.TaxTotalType[2];
                    MinimusaAPI.ModelosSunat.TaxTotalType Totales_Item = new MinimusaAPI.ModelosSunat.TaxTotalType();

                    MinimusaAPI.ModelosSunat.TaxAmountType Total_Item = new MinimusaAPI.ModelosSunat.TaxAmountType();
                    Total_Item.currencyID = Comprobante.Idmoneda;
                    Total_Item.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.mtoValorVentaItem - (det.mtoValorVentaItem / 1.18m)));
                    Totales_Item.TaxAmount = Total_Item;

                    MinimusaAPI.ModelosSunat.TaxSubtotalType[] subtotal_Items = new MinimusaAPI.ModelosSunat.TaxSubtotalType[2];
                    MinimusaAPI.ModelosSunat.TaxSubtotalType subtotal_Item = new MinimusaAPI.ModelosSunat.TaxSubtotalType();

                    MinimusaAPI.ModelosSunat.TaxableAmountType taxsubtotal_IGVItem = new MinimusaAPI.ModelosSunat.TaxableAmountType();
                    taxsubtotal_IGVItem.currencyID = Comprobante.Idmoneda;
                    taxsubtotal_IGVItem.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.mtoValorVentaItem / 1.18m));
                    subtotal_Item.TaxableAmount = taxsubtotal_IGVItem;

                    MinimusaAPI.ModelosSunat.TaxAmountType TotalTaxAmount_IGVItem = new MinimusaAPI.ModelosSunat.TaxAmountType();
                    TotalTaxAmount_IGVItem.currencyID = Comprobante.Idmoneda;
                    TotalTaxAmount_IGVItem.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.mtoValorVentaItem - (det.mtoValorVentaItem / 1.18m)));
                    subtotal_Item.TaxAmount = TotalTaxAmount_IGVItem;

                    subtotal_Items[0] = subtotal_Item;
                    Totales_Item.TaxSubtotal = subtotal_Items;

                    MinimusaAPI.ModelosSunat.TaxCategoryType taxcategory_IGVItem = new MinimusaAPI.ModelosSunat.TaxCategoryType();
                    //taxcategory_IGVItem.ID = id_IGVItem;
                    ModelosSunat.IDType idTaxCategoria = new ModelosSunat.IDType();
                    idTaxCategoria.schemeAgencyName = "United Nations Economic Commission for Europe";
                    idTaxCategoria.schemeName = "Tax Category Identifier";
                    idTaxCategoria.schemeID = "UN/ECE 5305";
                    idTaxCategoria.Value = "S";
                    //taxcategory_IGVItem.ID = idTaxCategoria;

                    ModelosSunat.PercentType1 porcentaje = new ModelosSunat.PercentType1();
                    porcentaje.Value = Convert.ToDecimal(det.porIgvItem) * 100;
                    taxcategory_IGVItem.Percent = porcentaje;
                    subtotal_Item.TaxCategory = taxcategory_IGVItem;
                    // taxcategory_IGVItem.Percent= taxcategory_IGVItem;

                    ModelosSunat.TaxExemptionReasonCodeType ReasonCode = new ModelosSunat.TaxExemptionReasonCodeType();
                    ReasonCode.listURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07";
                    ReasonCode.listName = "Afectacion del IGV";
                    ReasonCode.listAgencyName = "PE:SUNAT";
                    ReasonCode.Value = "10";

                    taxcategory_IGVItem.TaxExemptionReasonCode = ReasonCode;

                    MinimusaAPI.ModelosSunat.TaxSchemeType taxscheme_IGVItem = new MinimusaAPI.ModelosSunat.TaxSchemeType();
                    MinimusaAPI.ModelosSunat.IDType id2_IGVItem = new MinimusaAPI.ModelosSunat.IDType();

                    id2_IGVItem.schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05";
                    id2_IGVItem.schemeAgencyName = "PE:SUNAT";
                    id2_IGVItem.schemeName = "Codigo de tributos";


                    id2_IGVItem.Value = "1000";
                    taxscheme_IGVItem.ID = id2_IGVItem;

                    MinimusaAPI.ModelosSunat.NameType1 nombreImpto_IGVItem = new MinimusaAPI.ModelosSunat.NameType1();
                    nombreImpto_IGVItem.Value = "IGV";
                    taxscheme_IGVItem.Name = nombreImpto_IGVItem;

                    MinimusaAPI.ModelosSunat.TaxTypeCodeType nombreImpto_IGVItemInter = new MinimusaAPI.ModelosSunat.TaxTypeCodeType();
                    nombreImpto_IGVItemInter.Value = "VAT";
                    taxscheme_IGVItem.TaxTypeCode = nombreImpto_IGVItemInter;
                    taxscheme_IGVItem.Name = nombreImpto_IGVItem;

                    //taxcategory_IGVItem.TaxExemptionReasonCode = CodRazon_IGVItem;
                    taxcategory_IGVItem.TaxScheme = taxscheme_IGVItem;
                    //subtotal_Item.TaxableAmount = taxsubtotal_IGVItem;

                    subtotal_Items[0] = subtotal_Item;
                    Totales_Item.TaxSubtotal = subtotal_Items;
                    Totales_Items[0] = Totales_Item;

                    item.TaxTotal = Totales_Items;

                    MinimusaAPI.ModelosSunat.DescriptionType[] descriptions = new MinimusaAPI.ModelosSunat.DescriptionType[2];
                    MinimusaAPI.ModelosSunat.DescriptionType description = new MinimusaAPI.ModelosSunat.DescriptionType();
                    description.Value = det.DescripcionProducto;
                    MinimusaAPI.ModelosSunat.ItemIdentificationType codigoProd = new MinimusaAPI.ModelosSunat.ItemIdentificationType();
                    MinimusaAPI.ModelosSunat.IDType id = new MinimusaAPI.ModelosSunat.IDType();
                    id.Value = det.Codcom;
                    codigoProd.ID = id;

                    MinimusaAPI.ModelosSunat.PriceType PrecioProducto = new MinimusaAPI.ModelosSunat.PriceType();
                    MinimusaAPI.ModelosSunat.PriceAmountType PrecioMontoTipo = new MinimusaAPI.ModelosSunat.PriceAmountType();
                    PrecioMontoTipo.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.Precio / (det.porIgvItem + 1)));
                    //PrecioMontoTipo.Value = Convert.ToDecimal(string.Format("{0:0.00}", det.Precio));
                    PrecioMontoTipo.currencyID = Comprobante.Idmoneda;
                    PrecioProducto.PriceAmount = PrecioMontoTipo;

                    MinimusaAPI.ModelosSunat.ItemType itemTipo = new MinimusaAPI.ModelosSunat.ItemType();
                    descriptions[0] = description;
                    itemTipo.Description = descriptions;
                    itemTipo.SellersItemIdentification = codigoProd;

                    List<ModelosSunat.CommodityClassificationType> codSunats = new List<ModelosSunat.CommodityClassificationType>();
                    ModelosSunat.CommodityClassificationType codSunat = new ModelosSunat.CommodityClassificationType();
                    ModelosSunat.ItemClassificationCodeType codClas = new ModelosSunat.ItemClassificationCodeType();
                    codClas.listName = "Item Classification";
                    codClas.listAgencyName = "GS1 US";
                    codClas.listID = "UNSPSC";
                    codClas.Value = "25172405";
                    codSunat.ItemClassificationCode = codClas;
                    codSunats.Add(codSunat);
                    itemTipo.CommodityClassification = codSunats.ToArray();

                    item.Item = itemTipo;
                    item.Price = PrecioProducto;
                    //Item_Adicional.AdditionalItemProperty = Propiedades;
                    //Item_Adicionales.Item = Item_Adicional;            

                    //items[1] = item_OpeOnerosa;
                    // items[2] = item_DsctoItem;
                    // items[3] = item_DsctoCargoItem;
                    //items[4] = item_IGVItem;
                    //items[5] = item_ISCItem;
                    // items[6] = Item_Descripcion;
                    //  items[7] = Item_CodProSUNAT;
                    //   items[8] = Item_Adicionales;
                    items[iditem] = item;
                    iditem += 1;
                }
                Factura.DebitNoteLine = items;

                string archXML = GenerarComprobante(Factura, Comprobante.EmpresaRUC, Comprobante.Idtipocomp, Comprobante.Serie, Comprobante.Numero);
                FirmarXML(archXML, Ruta_Certificado, Password_Certificado);
                string strEnvio = Ruta_ENVIOS + Path.GetFileName(archXML).Replace(".xml", ".zip");
                Comprimir(archXML, strEnvio);
                EnviarDocumento(strEnvio);
                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public string GenerarResumenDiario_XML(DateTime Fecha, string EmpresaRUC, string EmpresaRazonSocial,
                                                                            List<ViewModels.VMResumenDiario> dsResumen,
                                                                            string RUTA_XML,
                                                                            string Ruta_Certificado, string Password_Certificado)
        {
            MinimusaAPI.Resumenes.SummaryDocumentsType Resumen = new MinimusaAPI.Resumenes.SummaryDocumentsType();
            string numTicket = "";
            try
            {
                //------Namespace necesarios para el UBL
                Resumen.Sac = "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1";
                Resumen.Ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";
                Resumen.Ds = "http://www.w3.org/2000/09/xmldsig#";
                Resumen.Cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                Resumen.Cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";

                //Resumen.Ccts = "urn:un:unece:uncefact:documentation:2";                           
                //Resumen.Qdt = "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2";
                //Resumen.Udt = "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2";
                //Resumen.Xsi = "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2";

                //------
                //-----Datos
               // CabeceraDAO db = new CabeceraDAO();
                
                //System.Data.DataSet dsResumen = new System.Data.DataSet();
                ////ARREGLAR LA CONSULTA DE RESUMEN DIARIO
               // dsResumen = db.ListarResumenDiario(Fecha);

                if (dsResumen.Count > 0)
                {
                    MinimusaAPI.Resumenes.UBLExtensionType[] ublExtensiones = new MinimusaAPI.Resumenes.UBLExtensionType[5];
                    MinimusaAPI.Resumenes.UBLExtensionType ublExtension = new MinimusaAPI.Resumenes.UBLExtensionType();

                    ublExtensiones[0] = ublExtension;
                    Resumen.UBLExtensions = ublExtensiones;

                    Resumen.UBLVersionID = new MinimusaAPI.Resumenes.UBLVersionIDType();
                    Resumen.UBLVersionID.Value = "2.0";

                    Resumen.CustomizationID = new MinimusaAPI.Resumenes.CustomizationIDType();
                    //Resumen.CustomizationID.schemeAgencyName = "PE:SUNAT";
                    Resumen.CustomizationID.Value = "1.1";
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////                        
                    //Numeración, conformada por serie y número correlativo
                    Resumen.ID = new MinimusaAPI.Resumenes.IDType();
                    Resumen.ID.Value = "RC-" + DateTime.Now.ToString("yyyyMMdd") + "-001";
                    //Fecha de emisión y hora de emision
                    MinimusaAPI.Resumenes.ReferenceDateType FechaEmision = new MinimusaAPI.Resumenes.ReferenceDateType();
                    FechaEmision.Value = Convert.ToDateTime(dsResumen[0].FechaEmision);
                    Resumen.ReferenceDate = FechaEmision;

                    Resumen.IssueDate = new MinimusaAPI.Resumenes.IssueDateType();
                    DateTime fechaGeneracion = DateTime.Now.Date;
                    Resumen.IssueDate.Value = Convert.ToDateTime(fechaGeneracion);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                    //Nombre Comercial del emisor Apellidos y nombres, denominación o razón social del emisor Tipo y Número de RUC del emisor Código del domicilio fiscal 
                    //o de local anexo del emisor 
                    MinimusaAPI.Resumenes.SignatureType Firma = new MinimusaAPI.Resumenes.SignatureType();
                    MinimusaAPI.Resumenes.SignatureType[] Firmas = new MinimusaAPI.Resumenes.SignatureType[2];

                    MinimusaAPI.Resumenes.PartyType partySign = new MinimusaAPI.Resumenes.PartyType();
                    MinimusaAPI.Resumenes.PartyIdentificationType partyIdentificacion = new MinimusaAPI.Resumenes.PartyIdentificationType();
                    MinimusaAPI.Resumenes.PartyIdentificationType[] partyIdentificacions = new MinimusaAPI.Resumenes.PartyIdentificationType[2];
                    MinimusaAPI.Resumenes.IDType idFirma = new MinimusaAPI.Resumenes.IDType();
                    idFirma.Value = EmpresaRUC;
                    Firma.ID = idFirma;

                    partyIdentificacion.ID = idFirma;
                    partyIdentificacions[0] = partyIdentificacion;
                    partySign.PartyIdentification = partyIdentificacions;
                    Firma.SignatoryParty = partySign;

                    MinimusaAPI.Resumenes.NoteType Nota = new MinimusaAPI.Resumenes.NoteType();

                    Nota.Value = "Elaborado por Sistema de Emision Electronica NET SOLUTION DEVELOPER ";
                    Firma.Note = Nota;

                    MinimusaAPI.Resumenes.PartyNameType partyName = new MinimusaAPI.Resumenes.PartyNameType();
                    MinimusaAPI.Resumenes.PartyNameType[] partyNames = new MinimusaAPI.Resumenes.PartyNameType[2];

                    MinimusaAPI.Resumenes.NameType1 RazonSocialFirma = new MinimusaAPI.Resumenes.NameType1();
                    RazonSocialFirma.Value = EmpresaRazonSocial;
                    partyName.Name = RazonSocialFirma;
                    partyNames[0] = partyName;
                    partySign.PartyName = partyNames;

                    MinimusaAPI.Resumenes.AttachmentType attachType = new MinimusaAPI.Resumenes.AttachmentType();
                    MinimusaAPI.Resumenes.ExternalReferenceType externaReferencia = new MinimusaAPI.Resumenes.ExternalReferenceType();
                    MinimusaAPI.Resumenes.URIType uri = new MinimusaAPI.Resumenes.URIType();
                    uri.Value = "SIGN";
                    externaReferencia.URI = uri;
                    Firma.DigitalSignatureAttachment = attachType;

                    attachType.ExternalReference = externaReferencia;
                    Firma.DigitalSignatureAttachment = attachType;
                    Firmas[0] = Firma;
                    Resumen.Signature = Firmas;

                    MinimusaAPI.Resumenes.SupplierPartyType empresa = new MinimusaAPI.Resumenes.SupplierPartyType();
                    MinimusaAPI.Resumenes.PartyType party = new MinimusaAPI.Resumenes.PartyType();

                    MinimusaAPI.Resumenes.AdditionalAccountIDType TipoDocumentoEmisor = new MinimusaAPI.Resumenes.AdditionalAccountIDType();
                    MinimusaAPI.Resumenes.AdditionalAccountIDType[] TipoDocumentoEmisors = new MinimusaAPI.Resumenes.AdditionalAccountIDType[2];
                    TipoDocumentoEmisors[0] = TipoDocumentoEmisor;
                    TipoDocumentoEmisor.Value = "6";
                    empresa.AdditionalAccountID = TipoDocumentoEmisors;

                    MinimusaAPI.Resumenes.CustomerAssignedAccountIDType RUCEmisor = new MinimusaAPI.Resumenes.CustomerAssignedAccountIDType();
                    RUCEmisor.Value = EmpresaRUC;
                    empresa.CustomerAssignedAccountID = RUCEmisor;

                    MinimusaAPI.Resumenes.PartyLegalEntityType parteLegalEntity = new MinimusaAPI.Resumenes.PartyLegalEntityType();
                    MinimusaAPI.Resumenes.PartyLegalEntityType[] parteLegalEntitys = new MinimusaAPI.Resumenes.PartyLegalEntityType[2];

                    MinimusaAPI.Resumenes.RegistrationNameType registerNameEmisor = new MinimusaAPI.Resumenes.RegistrationNameType();
                    registerNameEmisor.Value = EmpresaRazonSocial;
                    parteLegalEntity.RegistrationName = registerNameEmisor;

                    parteLegalEntitys[0] = parteLegalEntity;
                    party.PartyLegalEntity = parteLegalEntitys;
                    empresa.Party = party;

                    Resumen.AccountingSupplierParty = empresa;
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                    //Número de orden del Ítem 
                    //Cantidad y Unidad de medida por ítem 
                    //Valor de venta del ítem
                    //Items de Resumen

                    MinimusaAPI.Resumenes.SummaryDocumentsLineType[] items = new MinimusaAPI.Resumenes.SummaryDocumentsLineType[100];
                    int iditem = 1;
                    MinimusaAPI.Resumenes.TaxTotalType[] TotalesTributos = new MinimusaAPI.Resumenes.TaxTotalType[100];

                    foreach (var reg in dsResumen)
                    {
                        MinimusaAPI.Resumenes.SummaryDocumentsLineType item = new MinimusaAPI.Resumenes.SummaryDocumentsLineType();

                        MinimusaAPI.Resumenes.LineIDType numeroItem = new MinimusaAPI.Resumenes.LineIDType();
                        numeroItem.Value = iditem.ToString();
                        item.LineID = numeroItem;

                        MinimusaAPI.Resumenes.DocumentTypeCodeType TipoDocumento = new MinimusaAPI.Resumenes.DocumentTypeCodeType();
                        TipoDocumento.Value = reg.IdTipoComp;
                        item.DocumentTypeCode = TipoDocumento;

                        MinimusaAPI.Resumenes.IDType NumDocumento = new MinimusaAPI.Resumenes.IDType();
                        NumDocumento.Value = reg.NumeroComprobante;
                        item.ID = NumDocumento;

                        MinimusaAPI.Resumenes.CustomerPartyType Cliente = new MinimusaAPI.Resumenes.CustomerPartyType();
                        MinimusaAPI.Resumenes.CustomerAssignedAccountIDType NumeroDocumento = new MinimusaAPI.Resumenes.CustomerAssignedAccountIDType();
                        NumeroDocumento.Value = reg.NumDoc;
                        Cliente.CustomerAssignedAccountID = NumeroDocumento;

                        MinimusaAPI.Resumenes.AdditionalAccountIDType TipoDocumentoCliente = new MinimusaAPI.Resumenes.AdditionalAccountIDType();
                        MinimusaAPI.Resumenes.AdditionalAccountIDType[] TipoDocumentoClientes = new MinimusaAPI.Resumenes.AdditionalAccountIDType[2];
                        TipoDocumentoClientes[0] = TipoDocumentoCliente;
                        TipoDocumentoCliente.Value = reg.IdTipoDoc;
                        Cliente.AdditionalAccountID = TipoDocumentoClientes;
                        item.AccountingCustomerParty = Cliente;

                        //< !--Documento que modifica -->    
                        //< !--Datos de Percepcion -PER-- >
                        //< !--PER-- >
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ///
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        MinimusaAPI.Resumenes.StatusType Estado = new MinimusaAPI.Resumenes.StatusType();
                        MinimusaAPI.Resumenes.ConditionCodeType condicion = new MinimusaAPI.Resumenes.ConditionCodeType();
                        condicion.Value = reg.Adicionar;
                        Estado.ConditionCode = condicion;
                        item.Status = Estado;

                        MinimusaAPI.Resumenes.AmountType1 Total = new MinimusaAPI.Resumenes.AmountType1();
                        if (reg.IdMoneda == "PEN")
                        {
                            Total.currencyID = CurrencyCodeContentType.PEN;
                        }
                        else if (reg.IdMoneda == "USD")
                        {
                            Total.currencyID = CurrencyCodeContentType.USD;
                        }
                        //< !--Total Importe Total-->
                        Total.Value = Convert.ToDecimal(reg.TOT_NETO);
                        item.TotalAmount = Total;
                        //< !--Total Venta Operaciones Gravadas - 01-- >
                        MinimusaAPI.Resumenes.PaymentType PagoSubtotal = new MinimusaAPI.Resumenes.PaymentType();
                        MinimusaAPI.Resumenes.PaymentType[] PagoSubtotals = new MinimusaAPI.Resumenes.PaymentType[2];
                        MinimusaAPI.Resumenes.PaidAmountType SubTotal = new MinimusaAPI.Resumenes.PaidAmountType();
                        //Identificación del tipo de importe total
                        //01: Valor de venta de las operaciones gravadas con el IGV
                        //02: Valores de venta de las operaciones exoneradas del IGV
                        //03: Valores de venta de las operaciones inafectas del IGV
                        //04: Valor de venta de las exportaciones del item
                        //05: Valor de venta de las operaciones gratuitas
                        MinimusaAPI.Resumenes.InstructionIDType TipoImporteTotal = new MinimusaAPI.Resumenes.InstructionIDType();
                        TipoImporteTotal.Value = "01";

                        if (reg.IdMoneda == "PEN")
                        {
                            SubTotal.currencyID = CurrencyCodeContentType.PEN;
                        }
                        else if (reg.IdMoneda == "USD")
                        {
                            SubTotal.currencyID = CurrencyCodeContentType.USD;
                        }
                        SubTotal.Value = reg.TOT_VALOR_VENTA;
                        MinimusaAPI.Resumenes.InstructionIDType Tipo = new MinimusaAPI.Resumenes.InstructionIDType();
                        Tipo.Value = "01";
                        ////< !--fin 01-- >
                        ////< !--Total Venta Operaciones Exoneradas - 02-- > 
                        ////< !--fin 02-- >
                        ////< !--Total Venta Operaciones Inafectas - 03-- >
                        ////< !--fin 03-- >
                        ////< !--Total Venta Operaciones Gratuitas - 05-- >  
                        ////< !--fin 05-- >   
                        ////< !--Total SUMATORIO OTROS CARGOS - Cargos-- >
                        ////< !--fin Cargos-- >
                        ////< !--TOTAL ISC-- >
                        ///
                        PagoSubtotal.PaidAmount = SubTotal;
                        PagoSubtotals[0] = PagoSubtotal;
                        PagoSubtotal.InstructionID = Tipo;

                        // Total ISC                                
                        MinimusaAPI.Resumenes.TaxTotalType[] Totals_ISCItems = new MinimusaAPI.Resumenes.TaxTotalType[2];
                        MinimusaAPI.Resumenes.TaxTotalType Total_ISCItem = new MinimusaAPI.Resumenes.TaxTotalType();

                        MinimusaAPI.Resumenes.TaxAmountType Total_ItemISC = new MinimusaAPI.Resumenes.TaxAmountType();
                        Total_ItemISC.Value =reg.TOT_ISC;
                        if (reg.IdMoneda == "PEN")
                        {
                            Total_ItemISC.currencyID = CurrencyCodeContentType.PEN;
                        }
                        else if (reg.IdMoneda == "USD")
                        {
                            Total_ItemISC.currencyID = CurrencyCodeContentType.USD;
                        }
                        Total_ISCItem.TaxAmount = Total_ItemISC;
                        Totals_ISCItems[0] = Total_ISCItem;

                        MinimusaAPI.Resumenes.TaxCategoryType Category_ISCItem = new MinimusaAPI.Resumenes.TaxCategoryType();

                        MinimusaAPI.Resumenes.TaxSchemeType TaxScheme_ISCItem = new MinimusaAPI.Resumenes.TaxSchemeType();
                        MinimusaAPI.Resumenes.IDType id_ISCitem = new MinimusaAPI.Resumenes.IDType();
                        id_ISCitem.Value = "2000";
                        TaxScheme_ISCItem.ID = id_ISCitem;

                        MinimusaAPI.Resumenes.NameType1 nombreImpto_ISCItem = new MinimusaAPI.Resumenes.NameType1();
                        nombreImpto_ISCItem.Value = "ISC";
                        TaxScheme_ISCItem.Name = nombreImpto_ISCItem;

                        MinimusaAPI.Resumenes.TaxTypeCodeType nombreImpto_ISCItemInter = new MinimusaAPI.Resumenes.TaxTypeCodeType();
                        nombreImpto_ISCItemInter.Value = "EXC";
                        TaxScheme_ISCItem.TaxTypeCode = nombreImpto_ISCItemInter;

                        Category_ISCItem.TaxScheme = TaxScheme_ISCItem;
                        MinimusaAPI.Resumenes.TaxSubtotalType[] subtotal_ISCs = new MinimusaAPI.Resumenes.TaxSubtotalType[2];
                        MinimusaAPI.Resumenes.TaxSubtotalType subtotal_ISC = new MinimusaAPI.Resumenes.TaxSubtotalType();

                        subtotal_ISC.TaxCategory = Category_ISCItem;
                        subtotal_ISC.TaxAmount = Total_ItemISC;
                        subtotal_ISCs[0] = subtotal_ISC;

                        Total_ISCItem.TaxSubtotal = subtotal_ISCs;
                        TotalesTributos[0] = Total_ISCItem;

                        //< !--TOTAL IGV-- >

                        MinimusaAPI.Resumenes.TaxTotalType[] Totals_IGVItems = new MinimusaAPI.Resumenes.TaxTotalType[2];
                        MinimusaAPI.Resumenes.TaxTotalType Total_IGVItem = new MinimusaAPI.Resumenes.TaxTotalType();

                        MinimusaAPI.Resumenes.TaxAmountType Total_ItemIGV = new MinimusaAPI.Resumenes.TaxAmountType();
                        Total_ItemIGV.Value = reg.TOT_IGV;
                        if (reg.IdMoneda == "PEN")
                        {
                            Total_ItemIGV.currencyID = MinimusaAPI.Resumenes.CurrencyCodeContentType.PEN;
                        }
                        else if (reg.IdMoneda == "USD")
                        {
                            Total_ItemIGV.currencyID = MinimusaAPI.Resumenes.CurrencyCodeContentType.USD;
                        }
                        Total_IGVItem.TaxAmount = Total_ItemIGV;
                        Totals_IGVItems[0] = Total_IGVItem;

                        MinimusaAPI.Resumenes.TaxCategoryType Category_IGVItem = new MinimusaAPI.Resumenes.TaxCategoryType();
                        MinimusaAPI.Resumenes.TaxSchemeType TaxScheme_IGVItem = new MinimusaAPI.Resumenes.TaxSchemeType();
                        MinimusaAPI.Resumenes.IDType id_IGVitem = new MinimusaAPI.Resumenes.IDType();
                        id_IGVitem.Value = "1000";
                        TaxScheme_IGVItem.ID = id_IGVitem;

                        MinimusaAPI.Resumenes.NameType1 nombreImpto_IGVItem = new MinimusaAPI.Resumenes.NameType1();
                        nombreImpto_IGVItem.Value = "IGV";
                        TaxScheme_IGVItem.Name = nombreImpto_IGVItem;

                        MinimusaAPI.Resumenes.TaxTypeCodeType nombreImpto_IGVItemInter = new MinimusaAPI.Resumenes.TaxTypeCodeType();
                        nombreImpto_IGVItemInter.Value = "VAT";
                        TaxScheme_IGVItem.TaxTypeCode = nombreImpto_IGVItemInter;

                        Category_IGVItem.TaxScheme = TaxScheme_IGVItem;

                        MinimusaAPI.Resumenes.TaxSubtotalType[] subtotal_IGVs = new MinimusaAPI.Resumenes.TaxSubtotalType[2];
                        MinimusaAPI.Resumenes.TaxSubtotalType subtotal_IGV = new MinimusaAPI.Resumenes.TaxSubtotalType();

                        subtotal_IGV.TaxCategory = Category_IGVItem;
                        subtotal_IGV.TaxAmount = Total_ItemIGV;
                        subtotal_IGVs[0] = subtotal_IGV;

                        Total_IGVItem.TaxSubtotal = subtotal_IGVs;
                        TotalesTributos[1] = Total_IGVItem;

                        //< !--TOTAL OTROS-- >

                        MinimusaAPI.Resumenes.TaxTotalType[] Totals_OtrosItems = new MinimusaAPI.Resumenes.TaxTotalType[2];
                        MinimusaAPI.Resumenes.TaxTotalType Total_OtrosItem = new MinimusaAPI.Resumenes.TaxTotalType();

                        MinimusaAPI.Resumenes.TaxAmountType Total_ItemOtros = new MinimusaAPI.Resumenes.TaxAmountType();
                        Total_ItemOtros.Value = reg.TOT_OPOT;

                        if (reg.IdMoneda == "PEN")
                        {
                            Total_ItemOtros.currencyID = MinimusaAPI.Resumenes.CurrencyCodeContentType.PEN;
                        }
                        else if (reg.IdMoneda == "USD")
                        {
                            Total_ItemOtros.currencyID = MinimusaAPI.Resumenes.CurrencyCodeContentType.USD;
                        }
                        Total_OtrosItem.TaxAmount = Total_ItemOtros;
                        Totals_OtrosItems[0] = Total_OtrosItem;

                        MinimusaAPI.Resumenes.TaxSubtotalType[] subtotal_Otross = new MinimusaAPI.Resumenes.TaxSubtotalType[2];
                        MinimusaAPI.Resumenes.TaxSubtotalType subtotal_Otros = new MinimusaAPI.Resumenes.TaxSubtotalType();

                        MinimusaAPI.Resumenes.TaxAmountType Total_ItemOtrosSub = new MinimusaAPI.Resumenes.TaxAmountType();
                        if (reg.IdMoneda == "PEN")
                        {
                            Total_ItemOtrosSub.currencyID = MinimusaAPI.Resumenes.CurrencyCodeContentType.PEN;
                        }
                        else if (reg.IdMoneda == "USD")
                        {
                            Total_ItemOtrosSub.currencyID = MinimusaAPI.Resumenes.CurrencyCodeContentType.USD;
                        }
                        subtotal_Otros.TaxAmount = Total_ItemOtrosSub;

                        MinimusaAPI.Resumenes.TaxCategoryType Category_OtrosItem = new MinimusaAPI.Resumenes.TaxCategoryType();
                        MinimusaAPI.Resumenes.TaxSchemeType TaxScheme_OtrosItem = new MinimusaAPI.Resumenes.TaxSchemeType();
                        MinimusaAPI.Resumenes.IDType id_Otrositem = new MinimusaAPI.Resumenes.IDType();
                        id_Otrositem.Value = "9999";
                        TaxScheme_OtrosItem.ID = id_Otrositem;

                        MinimusaAPI.Resumenes.NameType1 nombreImpto_OtrosItem = new MinimusaAPI.Resumenes.NameType1();
                        nombreImpto_OtrosItem.Value = "OTROS";
                        TaxScheme_OtrosItem.Name = nombreImpto_OtrosItem;

                        MinimusaAPI.Resumenes.TaxTypeCodeType nombreImpto_OtrosItemInter = new MinimusaAPI.Resumenes.TaxTypeCodeType();
                        nombreImpto_OtrosItemInter.Value = "OTH";
                        TaxScheme_OtrosItem.TaxTypeCode = nombreImpto_OtrosItemInter;
                        Category_OtrosItem.TaxScheme = TaxScheme_OtrosItem;

                        subtotal_Otros.TaxCategory = Category_OtrosItem;
                        subtotal_Otros.TaxAmount = Total_ItemOtrosSub;
                        subtotal_Otross[0] = subtotal_Otros;
                        Total_OtrosItem.TaxSubtotal = subtotal_Otross;
                        TotalesTributos[2] = Total_OtrosItem;
                        item.TaxTotal = TotalesTributos;
                        //item.Item = itemTipo;
                        //item.Price = PrecioProducto;
                        ////Item_Adicional.AdditionalItemProperty = Propiedades;
                        ////Item_Adicionales.Item = Item_Adicional;            
                        items[iditem] = item;
                        iditem += 1;
                        ////items[1] = item_OpeOnerosa;
                        //// items[2] = item_DsctoItem;
                        //// items[3] = item_DsctoCargoItem;
                        ////items[4] = item_IGVItem;
                        ////items[5] = item_IGVItem;
                        //// items[6] = Item_Descripcion;
                        ////  items[7] = Item_CodProSUNAT;
                        ////   items[8] = Item_Adicionales;
                    }

                    Resumen.SummaryDocumentsLine = items;
                    string archXML = GenerarResumenDiario(Resumen, Fecha, "RC", 1, EmpresaRUC,RUTA_XML);
                    FirmarXML(archXML, Ruta_Certificado, Password_Certificado);
                    string strEnvio = Ruta_ENVIOS + Path.GetFileName(archXML).Replace(".xml", ".zip");
                    Comprimir(archXML, strEnvio);
                    numTicket = EnviarResumen(strEnvio);                
                }
                return numTicket;
            }                         
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public  string GenerarComunicacionBaja_XML(DateTime Fecha, string EmpresaRUC, 
                                                                                string EmpresaRazonSocial, string TipoDocumento, string SerieDocumento, 
                                                                                string NumeroDocumento, string MotivoBaja,
                                                                                string rutacert , string passwordcert , string rutaXML)
        {
            MinimusaAPI.Bajas.VoidedDocumentsType Baja = new MinimusaAPI.Bajas.VoidedDocumentsType();
            string numTicket = "";
            try
            {
                //------Namespace necesarios para el UBL
                Baja.Cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                Baja.Cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                Baja.Ds = "http://www.w3.org/2000/09/xmldsig#";
                Baja.Ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";
                Baja.Sac = "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1";
                //  Baja.Xsi = "http://www.w3.org/2001/XMLSchema-instance";

                MinimusaAPI.Bajas.UBLExtensionType[] ublExtensiones = new MinimusaAPI.Bajas.UBLExtensionType[5];
                MinimusaAPI.Bajas.UBLExtensionType ublExtension = new MinimusaAPI.Bajas.UBLExtensionType();

                ublExtensiones[0] = ublExtension;
                Baja.UBLExtensions = ublExtensiones;

                Baja.UBLVersionID = new MinimusaAPI.Bajas.UBLVersionIDType();
                Baja.UBLVersionID.Value = "2.0";

                Baja.CustomizationID = new MinimusaAPI.Bajas.CustomizationIDType();
                //Baja.CustomizationID.schemeAgencyName = "PE:SUNAT";
                Baja.CustomizationID.Value = "1.0";
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////                        
                //Numeración, conformada por serie y número correlativo
                Baja.ID = new MinimusaAPI.Bajas.IDType();
                Baja.ID.Value = "RA-" + DateTime.Now.ToString("yyyyMMdd") + "-001";
                //Fecha de emisión y hora de emision
                MinimusaAPI.Bajas.ReferenceDateType FechaEmision = new MinimusaAPI.Bajas.ReferenceDateType();
                FechaEmision.Value = Fecha;
                Baja.ReferenceDate = FechaEmision;

                Baja.IssueDate = new MinimusaAPI.Bajas.IssueDateType();
                DateTime fechaGeneracion = DateTime.Now.Date;
                Baja.IssueDate.Value = Convert.ToDateTime(fechaGeneracion);

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                //Nombre Comercial del emisor Apellidos y nombres, denominación o razón social del emisor Tipo y Número de RUC del emisor Código del domicilio fiscal 
                //o de local anexo del emisor 
                MinimusaAPI.Bajas.SignatureType Firma = new MinimusaAPI.Bajas.SignatureType();
                MinimusaAPI.Bajas.SignatureType[] Firmas = new MinimusaAPI.Bajas.SignatureType[2];

                MinimusaAPI.Bajas.PartyType partySign = new MinimusaAPI.Bajas.PartyType();
                MinimusaAPI.Bajas.PartyIdentificationType partyIdentificacion = new MinimusaAPI.Bajas.PartyIdentificationType();
                MinimusaAPI.Bajas.PartyIdentificationType[] partyIdentificacions = new MinimusaAPI.Bajas.PartyIdentificationType[2];
                MinimusaAPI.Bajas.IDType idFirma = new MinimusaAPI.Bajas.IDType();
                idFirma.Value = EmpresaRUC;
                Firma.ID = idFirma;

                partyIdentificacion.ID = idFirma;
                partyIdentificacions[0] = partyIdentificacion;
                partySign.PartyIdentification = partyIdentificacions;
                Firma.SignatoryParty = partySign;

                MinimusaAPI.Bajas.NoteType Nota = new MinimusaAPI.Bajas.NoteType();

                Nota.Value = "Elaborado por Sistema de Emision Electronica NET SOLUTION DEVELOPER ";
                Firma.Note = Nota;

                MinimusaAPI.Bajas.PartyNameType partyName = new MinimusaAPI.Bajas.PartyNameType();
                MinimusaAPI.Bajas.PartyNameType[] partyNames = new MinimusaAPI.Bajas.PartyNameType[2];

                MinimusaAPI.Bajas.NameType1 RazonSocialFirma = new MinimusaAPI.Bajas.NameType1();
                RazonSocialFirma.Value = EmpresaRazonSocial;
                partyName.Name = RazonSocialFirma;
                partyNames[0] = partyName;
                partySign.PartyName = partyNames;

                MinimusaAPI.Bajas.AttachmentType attachType = new MinimusaAPI.Bajas.AttachmentType();
                MinimusaAPI.Bajas.ExternalReferenceType externaReferencia = new MinimusaAPI.Bajas.ExternalReferenceType();
                MinimusaAPI.Bajas.URIType uri = new MinimusaAPI.Bajas.URIType();
                uri.Value = "SIGN";
                externaReferencia.URI = uri;
                Firma.DigitalSignatureAttachment = attachType;

                attachType.ExternalReference = externaReferencia;
                Firma.DigitalSignatureAttachment = attachType;
                Firmas[0] = Firma;
                Baja.Signature = Firmas;

                MinimusaAPI.Bajas.SupplierPartyType empresa = new MinimusaAPI.Bajas.SupplierPartyType();
                MinimusaAPI.Bajas.PartyType party = new MinimusaAPI.Bajas.PartyType();

                MinimusaAPI.Bajas.AdditionalAccountIDType TipoDocumentoEmisor = new MinimusaAPI.Bajas.AdditionalAccountIDType();
                MinimusaAPI.Bajas.AdditionalAccountIDType[] TipoDocumentoEmisors = new MinimusaAPI.Bajas.AdditionalAccountIDType[2];
                TipoDocumentoEmisors[0] = TipoDocumentoEmisor;
                TipoDocumentoEmisor.Value = "6";
                empresa.AdditionalAccountID = TipoDocumentoEmisors;

                MinimusaAPI.Bajas.CustomerAssignedAccountIDType RUCEmisor = new MinimusaAPI.Bajas.CustomerAssignedAccountIDType();
                RUCEmisor.Value = EmpresaRUC;
                empresa.CustomerAssignedAccountID = RUCEmisor;

                MinimusaAPI.Bajas.PartyLegalEntityType parteLegalEntity = new MinimusaAPI.Bajas.PartyLegalEntityType();
                MinimusaAPI.Bajas.PartyLegalEntityType[] parteLegalEntitys = new MinimusaAPI.Bajas.PartyLegalEntityType[2];

                MinimusaAPI.Bajas.RegistrationNameType registerNameEmisor = new MinimusaAPI.Bajas.RegistrationNameType();
                registerNameEmisor.Value = EmpresaRazonSocial;
                parteLegalEntity.RegistrationName = registerNameEmisor;

                parteLegalEntitys[0] = parteLegalEntity;
                party.PartyLegalEntity = parteLegalEntitys;
                empresa.Party = party;

                Baja.AccountingSupplierParty = empresa;
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                //Número de orden del Ítem 
                //Cantidad y Unidad de medida por ítem 
                //Valor de venta del ítem
                //Items de Baja
                MinimusaAPI.Bajas.VoidedDocumentsLineType ItemBaja = new MinimusaAPI.Bajas.VoidedDocumentsLineType();
                MinimusaAPI.Bajas.VoidedDocumentsLineType[] ItemsBajas = new MinimusaAPI.Bajas.VoidedDocumentsLineType[2];

                MinimusaAPI.Bajas.LineIDType numeroItem = new MinimusaAPI.Bajas.LineIDType();
                numeroItem.Value = "1";
                ItemBaja.LineID = numeroItem;

                MinimusaAPI.Bajas.DocumentTypeCodeType Tipo_Documento = new MinimusaAPI.Bajas.DocumentTypeCodeType();
                Tipo_Documento.Value = TipoDocumento.PadLeft(2,'0');
                ItemBaja.DocumentTypeCode = Tipo_Documento;

                MinimusaAPI.Bajas.IdentifierType Serie_Documento = new MinimusaAPI.Bajas.SerialIDType();
                Serie_Documento.Value = SerieDocumento;
                ItemBaja.DocumentSerialID = Serie_Documento;

                MinimusaAPI.Bajas.IdentifierType Numero_Documento = new MinimusaAPI.Bajas.SerialIDType();
                Numero_Documento.Value = NumeroDocumento;
                ItemBaja.DocumentNumberID = Numero_Documento;

                MinimusaAPI.Bajas.TextType Motivo_Baja = new MinimusaAPI.Bajas.TextType();
                Motivo_Baja.Value = MotivoBaja;
                ItemBaja.VoidReasonDescription = Motivo_Baja;
                ItemsBajas[0] = ItemBaja;
                Baja.VoidedDocumentsLine = ItemsBajas;
                
                string archXML = GenerarComunicacionBaja(Baja, Fecha, "RA", 1, EmpresaRUC,rutaXML);
                FirmarXML(archXML, rutacert, passwordcert);
                string strEnvio = Ruta_ENVIOS + Path.GetFileName(archXML).Replace(".xml", ".zip");
                Comprimir(archXML, strEnvio);
                numTicket = EnviarResumen(strEnvio);
                return numTicket;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
               
        private  string GenerarComprobante(MinimusaAPI.ModelosSunat.InvoiceType Factura, 
                                                                             string RUC, 
                                                                             string TipoDocumento, string Serie,
                                                                             string Numero)
        {
            //-----Generando el archivo XML 
            XmlWriterSettings setting = new XmlWriterSettings();
            //Especificar el uso de sangrias para los etiquetas XML
            setting.Indent = true;
            setting.IndentChars = "\t";

            //Generar el nombre del archivo
            string RUCEmpresa =  RUC;
            string ArchivoXML = RUC + "-" + TipoDocumento + "-" + Serie + "-" + Numero;
            string rutaXML = string.Format(@"{0}{1}.xml", this.Ruta_XML , ArchivoXML);

            //Generar el xml en la ruta espeficificada
            using (XmlWriter writer = XmlWriter.Create(rutaXML, setting))
            {
                Type typeToSerialize = typeof(MinimusaAPI.ModelosSunat.InvoiceType);
                XmlSerializer xs = new XmlSerializer(typeToSerialize);
                xs.Serialize(writer, Factura);
                return rutaXML;
            }
        }

        private string GenerarComprobante(MinimusaAPI.ModelosSunat.DebitNoteType Factura,
                                                                             string RUC,
                                                                             string TipoDocumento, string Serie,
                                                                             string Numero)
        {
            //-----Generando el archivo XML 
            XmlWriterSettings setting = new XmlWriterSettings();
            //Especificar el uso de sangrias para los etiquetas XML
            setting.Indent = true;
            setting.IndentChars = "\t";

            //Generar el nombre del archivo
            string RUCEmpresa = RUC;
            string ArchivoXML = RUC + "-" + TipoDocumento + "-" + Serie + "-" + Numero;
            string rutaXML = string.Format(@"{0}{1}.xml", this.Ruta_XML, ArchivoXML);

            //Generar el xml en la ruta espeficificada
            using (XmlWriter writer = XmlWriter.Create(rutaXML, setting))
            {
                Type typeToSerialize = typeof(MinimusaAPI.ModelosSunat.DebitNoteType);
                XmlSerializer xs = new XmlSerializer(typeToSerialize);
                xs.Serialize(writer, Factura);
                return rutaXML;
            }
        }
        private  string GenerarComprobante(MinimusaAPI.ModelosSunat.CreditNoteType Nota, string RUC, string TipoDocumento, string Serie,
                                                                             string Numero)
        {
            //-----Generando el archivo XML
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "\t";

            string RUCEmpresa = this.RUCEmpresa;
            string ArchivoXML = RUC + "-" + TipoDocumento + "-" + Serie + "-" + Numero;

            string rutaXML = string.Format(@"{0}{1}.xml", Ruta_XML, ArchivoXML);

            using (XmlWriter writer = XmlWriter.Create(rutaXML, setting))
            {
                Type typeToSerialize = typeof(MinimusaAPI.ModelosSunat.CreditNoteType);
                XmlSerializer xs = new XmlSerializer(typeToSerialize);
                xs.Serialize(writer, Nota);
                return rutaXML;
            }
        }

        private  string GenerarResumenDiario(MinimusaAPI.Resumenes.SummaryDocumentsType ResumenDiario, 
                                                                      DateTime Fecha, string TipoDocumento, int correlativo,
                                                                      string EmpresaRUC,string rutaxml)
        {
            //-----Generando el archivo XML
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "\t";
                  
            string ArchivoXML = EmpresaRUC + "-" + TipoDocumento + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + 
                                             correlativo.ToString().PadLeft(3, '0');

            string rutaXML = string.Format(@"{0}{1}.xml", rutaxml, ArchivoXML);

            using (XmlWriter writer = XmlWriter.Create(rutaXML, setting))
            {
                Type typeToSerialize = typeof(MinimusaAPI.Resumenes.SummaryDocumentsType);
                XmlSerializer xs = new XmlSerializer(typeToSerialize);
                xs.Serialize(writer, ResumenDiario);
                return rutaXML;
            }
        }

        private  string GenerarComunicacionBaja(MinimusaAPI.Bajas.VoidedDocumentsType Baja, 
                                                                              DateTime Fecha, string TipoDocumento,
                                                                              int correlativo , string RUCEmpresa , string pathXML)
        {
            //-----Generando el archivo XML
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "\t";
          
            string ArchivoXML = RUCEmpresa + "-" + TipoDocumento + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + 
                                             correlativo.ToString().PadLeft(3, '0');

            string rutaXML = string.Format(@"{0}{1}.xml", pathXML, ArchivoXML);

            using (XmlWriter writer = XmlWriter.Create(rutaXML, setting))
            {
                Type typeToSerialize = typeof(  MinimusaAPI.Bajas.VoidedDocumentsType);
                XmlSerializer xs = new XmlSerializer(typeToSerialize);
                xs.Serialize(writer, Baja);
                return rutaXML;
            }
        }

        private  string GenerarGuiaRemitente(MinimusaAPI.Guias.DespatchAdviceType guia, string RUC)
        {
            //-----Generando el archivo XML
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "\t";

            string RUCEmpresa = this.RUCEmpresa;
            //20257471609 - TAAA - 2344
            string ArchivoXML = RUC + "-09-" + guia.ID.Value;
            string rutaXML = string.Format(@"{0}{1}.xml", Ruta_XML, ArchivoXML);

            using (XmlWriter writer = XmlWriter.Create(rutaXML, setting))
            {
                Type typeToSerialize = typeof(MinimusaAPI.Guias.DespatchAdviceType);
                XmlSerializer xs = new XmlSerializer(typeToSerialize);
                xs.Serialize(writer, guia);
                return rutaXML;
            }
        }

        public  string FirmarXML(string cRutaArchivo, string cCertificado, string cClave)
        {

            string file = cRutaArchivo;
            //Corregir un error de xml en el archivo generado
            string text = File.ReadAllText(file);
            text = text.Replace(@"<ext:UBLExtension />", @"<ext:UBLExtension> <ext:ExtensionContent /></ext:UBLExtension>");
            text = text.Replace("xsi:type=", "");
            text = text.Replace("cbc:SerialIDType", "");
            text = text.Replace("\"\"", "");
            //Guardar las modificaciones
            File.WriteAllText(file, text);
            //Firmar el archivo xml
            string cRpta;
            string tipo = Path.GetFileName(cRutaArchivo);
            string local_typoDocumento = tipo.Substring(12, 2); // retorna 01 o 03 0 ...
            string l_xpath = "";
            string f_certificat = cCertificado;
            string f_pwd = cClave;
            string xmlFile = cRutaArchivo;

            X509Certificate2 MonCertificat = new X509Certificate2(f_certificat, f_pwd);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            //Leer el xml a firmar
            xmlDoc.Load(xmlFile);
            //Firmar el documento xml
            SignedXml signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = MonCertificat.PrivateKey;
            KeyInfo KeyInfo = new KeyInfo();
            Reference Reference = new Reference();
            Reference.Uri = "";
            Reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(Reference);
            X509Chain X509Chain = new X509Chain();
            X509Chain.Build(MonCertificat);
            X509ChainElement local_element = X509Chain.ChainElements[0];
            KeyInfoX509Data x509Data = new KeyInfoX509Data(local_element.Certificate);
            string subjectName = local_element.Certificate.Subject;
            x509Data.AddSubjectName(subjectName);
            KeyInfo.AddClause(x509Data);
            signedXml.KeyInfo = KeyInfo;
            signedXml.ComputeSignature();
            XmlElement signature = signedXml.GetXml();
            signature.Prefix = "ds";
            signedXml.ComputeSignature();
            foreach (XmlNode node in signature.SelectNodes("descendant-or-self::*[namespace-uri()='http://www.w3.org/2000/09/xmldsig#']"))
            {
                // node.Prefix = "ds"
                if (node.LocalName == "Signature")
                {
                    XmlAttribute newAttribute = xmlDoc.CreateAttribute("Id");
                    newAttribute.Value = "SignSUNAT";
                    node.Attributes.Append(newAttribute);
                }
            }
            XmlNamespaceManager nsMgr;
            nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1");
            nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2");
            nsMgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            switch (local_typoDocumento)
            {
                case "01":
                case "03" // factura y boleta
               :
                    {
                        nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
                        l_xpath = "/tns:Invoice/ext:UBLExtensions/ext:UBLExtension[1]/ext:ExtensionContent";
                        break;
                    }

                case "07": // n ota de credito :
                    {
                        nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2");
                        l_xpath = "/tns:CreditNote/ext:UBLExtensions/ext:UBLExtension[1]/ext:ExtensionContent";
                        break;
                    }

                case "08":// nota de debito:
                    {
                        nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2");
                        l_xpath = "/tns:DebitNote/ext:UBLExtensions/ext:UBLExtension[1]/ext:ExtensionContent";
                        break;
                    }

                case "RA" // COMUNICACION DE BAJA
                    :
                    {
                        nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:VoidedDocuments-1");
                        l_xpath = "/tns:VoidedDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
                        break;
                    }

                case "RC" // RESUMEN DIARIO
                     :
                    {
                        nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1");
                        l_xpath = "/tns:SummaryDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";

                        break;
                    }
        
                default  // GUIA REMISION
                    :
                    {                       
                        nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:DespatchAdvice-2");                    
                        l_xpath = "/tns:DespatchAdvice/ext:UBLExtensions/ext:UBLExtension[1]/ext:ExtensionContent";

                        break;
                    }
            }
            nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
            nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
            nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            nsMgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            xmlDoc.SelectSingleNode(l_xpath, nsMgr).AppendChild(xmlDoc.ImportNode(signature, true));
            //Guarda los cambios en el xml firmado
            xmlDoc.Save(xmlFile);
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("ds:Signature");
            if ((nodeList.Count != 1))
            {
                cRpta = "SE PRODUJO ERROR EN LA FIRMA";
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //FIRMAR COMPROBANTE
            signedXml.LoadXml((XmlElement)nodeList[0]);
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if ((signedXml.CheckSignature()) == false)
                cRpta = "SE PRODUJO UN ERROR EN LA FIRMA  DE DOCUMENTO";
            else
                cRpta = "OK";
            //string file = cRutaArchivo + cArchivo;              
            return cRpta;
        }

        public  string Comprimir(string cnombrearchivoOrigen, string cnombreArchivoDestino)
        {
            
            Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
            zip.AddFile(cnombrearchivoOrigen, ""); // se puede seguir agregando mas con a misma funcion
            zip.Save(cnombreArchivoDestino);
            string rpta = "OK";
            return rpta;
        }

        public string EnviarDocumento(string pArchivo)
        {
            string strRetorno = "";
            string filezip = pArchivo;
            //string filepath = Directory.GetCurrentDirectory() + "\\Envios\\" + filezip;
            string filepath = filezip;
            byte[] bitArray = File.ReadAllBytes(filepath);
            try
            {                
                    billServiceClient servicio = new billServiceClient(billServiceClient.EndpointConfiguration.BillServicePort);                                    
                    ServicePointManager.UseNagleAlgorithm = true;
                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.CheckCertificateRevocationList = true;
                                    
                    servicio.OpenAsync();
                    
                    //obtener el nombre del archivo sin ruta
                    filezip = Path.GetFileName(filezip);
                    //Enviar el archivo zipeado convertido a Bytes a la SUNAT
                    //servicio.ClientCredentials.UserName.UserName="MODDATOS";
                    //servicio.ClientCredentials.UserName.Password = "MODDATOS";                                                     
                    byte[] returnByte = servicio.sendBill(filezip, bitArray, "");

                    servicio.CloseAsync();

                    filezip = Path.GetFileName(filezip);
                    FileStream fs = new FileStream(Ruta_CDRS + "R-" + filezip, FileMode.Create);
                    fs.Write(returnByte, 0, returnByte.Length);
                    fs.Close();
                    strRetorno = "Archivo generado con exito";

                }
            catch (System.ServiceModel.FaultException ex)
            {
                strRetorno = ex.Code.Name;
                throw;
            }
            return strRetorno;
        }

        public  string EnviarDocumentoGuiaRemision(string pArchivo)
        {
            string strRetorno = "";
            string filezip = pArchivo;
            //string filepath = Directory.GetCurrentDirectory() + "\\Envios\\" + filezip;
            string filepath = filezip;
            byte[] bitArray = File.ReadAllBytes(filepath);
            try
            {
                   ServiceGUIA.billServiceClient servicio = new ServiceGUIA.billServiceClient(billServiceClient.EndpointConfiguration.BillServicePort_1);
                
                    ServicePointManager.UseNagleAlgorithm = true;
                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.CheckCertificateRevocationList = true;
                    servicio.OpenAsync();
                    //obtener el nombre del archivo sin ruta
                    filezip = Path.GetFileName(filezip);
                    //Enviar el archivo zipeado convertido a Bytes a la SUNAT
                    byte[] returnByte = servicio.sendBill(filezip, bitArray, "");
                    servicio.CloseAsync();

                    filezip = Path.GetFileName(filezip);
                    FileStream fs = new FileStream(Ruta_CDRS + "R-" + filezip, FileMode.Create);
                    fs.Write(returnByte, 0, returnByte.Length);
                    fs.Close();
                    strRetorno = "Archivo generado con exito";                
            }
            catch (System.ServiceModel.FaultException ex)
            {
                strRetorno = ex.Code.Name;
                throw;
            }
            return strRetorno;
        }


        public  string EnviarResumen(string pArchivo)
        {
            string strRetorno = "";
            string filezip = pArchivo;
            //string filepath = Directory.GetCurrentDirectory() + "\\Envios\\" + filezip;
            string filepath = filezip;
            byte[] bitArray = File.ReadAllBytes(filepath);
            try
            {
                  //Invocar el servicio
                        billServiceClient servicio = new billServiceClient(
                        billServiceClient.EndpointConfiguration.BillServicePort);
                                
                    ServicePointManager.UseNagleAlgorithm = true;
                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.CheckCertificateRevocationList = true;

                    servicio.OpenAsync();
                    //obtener el nombre del archivo sin ruta
                    filezip = Path.GetFileName(filezip);
                    //Enviar el archivo zipeado convertido a Bytes a la SUNAT
                    //string ticket = servicio.sendSummaryAsync(filezip, bitArray, "");
                    Task<sendSummaryResponse> rpta = servicio.sendSummaryAsync(filezip, bitArray, "");
                    servicio.CloseAsync();
                    strRetorno = rpta.Result.ticket.ToString();
                }         
                catch (System.ServiceModel.FaultException ex)
                    {
                        strRetorno = ex.Code.Name;
                        throw;
                    }   
            return strRetorno;
        }
        public  string ObtenerEstado(string ticket)
        {
            string strRetorno = "";
            try
            {
                billServiceClient servicio = new billServiceClient(billServiceClient.EndpointConfiguration.BillServicePort);
                {
                    servicio.OpenAsync();
                    statusResponse returnstring = servicio.getStatus(ticket);
                    strRetorno = returnstring.statusCode;
                    servicio.CloseAsync();
                    return strRetorno;                    
                }
            }
            catch (System.ServiceModel.FaultException ex)
            {
                throw new Exception(ex.Code.Name);                
            } 
         }
    }
}
  




