public RespuestaSimple obtenerDatosXmlFactura(RequestXML xmlBase64)
        {
            RespuestaSimple respuesta = new RespuestaSimple();
            try
            {
                int pos = -1;
                var contentFile = Generic.contenidoArchivoBase64(xmlBase64.XML);
                var directoryXml = Generic.DatosXML(contentFile);
                var propiedas = XMLDAO.ObtenerDatoFiscal("cfdi:Comprobante", "SubTotal", directoryXml, ref pos);
                respuesta.result = 1;
                respuesta.mensaje = "Solicitud exitosa";
            }
            catch (Exception ex)
            {
                respuesta.result = 0;
                respuesta.mensaje = "Server Error" + ex.Message;
            }
            return respuesta;
        }
        public async Task<RespuestaData<List<LogFacturasModel>>> AgregarDatosFacturaXML(RequestXML request)
        {
            List<LogFacturasConError> ListaErrorFactura = new List<LogFacturasConError>();
            List<LogFacturasModel> resultLog = new List<LogFacturasModel>();
            RespuestaData<List<LogFacturasModel>> Respuesta =
                    new RespuestaData<List<LogFacturasModel>>
                    {
                        Datos = new List<LogFacturasModel>(),
                        Respuesta = new RespuestaSimple()
                    };
            int pos = -1;
            bool existe = false;
            double suma = 0;
            //string _satEstatus = string.Empty;
            bool existeError = false;
            try
            {
                var uuid = string.Empty;
                var rfcEmisor = string.Empty;
                var rfcReceptor = string.Empty;
                var fechaEmision = string.Empty;
                var fechaTimbrado = string.Empty;
                var xmlRuta = string.Empty;
                var pdfRuta = string.Empty;
                var totalFactura = string.Empty;
                var subTotalFactura = string.Empty;
                var descuentoFactura = string.Empty;
                var numeroFactura = string.Empty;
                var contentFile = Generic.contenidoArchivoBase64(request.XML);
                var directoryXml = Generic.DatosXML(contentFile);
                uuid = XMLDAO.ObtenerDatoFiscal("cfdi:Complemento", "UUID", directoryXml, ref pos);
                rfcEmisor = XMLDAO.ObtenerDatoFiscal("cfdi:Emisor", "Rfc", directoryXml, ref pos);
                rfcReceptor = XMLDAO.ObtenerDatoFiscal("cfdi:Receptor", "Rfc", directoryXml, ref pos);
                fechaEmision = XMLDAO.ObtenerDatoFiscal("cfdi:Comprobante", "Fecha", directoryXml, ref pos);
                fechaTimbrado = XMLDAO.ObtenerDatoFiscal("cfdi:Complemento", "FechaTimbrado", directoryXml, ref pos);
                totalFactura = XMLDAO.ObtenerDatoFiscal("cfdi:Comprobante", "_Total", directoryXml, ref pos);
                subTotalFactura = XMLDAO.ObtenerDatoFiscal("cfdi:Comprobante", "SubTotal", directoryXml, ref pos);
                numeroFactura = XMLDAO.ObtenerDatoFiscal("cfdi:Comprobante", "Folio", directoryXml, ref pos);
                var rfcMediaccess = EstatusFactura.MediaccessRfc;
                MaedicusDAO _DAO = new MaedicusDAO(UserId, appSettings.Value.ConnectionStrings["MaedicusCPT1433"], "");
                DAOCRUDGenerico<FacturasRemesas> repo = _DAO.DAOAgregarFacturaDeXML();
                DAOCRUDGenerico<LogFacturasConError> repoLog = _DAO.DAOAgregarFacturaError();
                DAOCRUDGenerico<FacturaxRemesa> repositorio = _DAO.DAOAgregarFacturaxRemesa();
                DAOCRUDGenerico<Facturas> _repoFacturas = _DAO.DAOFacturas();
                DAOCRUDGenerico<FacturasRemesas> _repoAgregaFacturaRemesa = _DAO.DAOAgregarFacturaDeXML();
                DAOCRUDGenerico<FoliosReceta> _repoFolioreceta = _DAO.DAOFoliosReceta();
                List<FoliosReceta> _ListCopago = new List<FoliosReceta>();
                NumeroRemesa _numeroRemesa = new NumeroRemesa();
                int _cveProveedor = 0;
                FacturasRemesas agregarFactura = new FacturasRemesas
                {
                    UUID = uuid.Trim(),
                    RFCEmisor = rfcEmisor.Trim(),
                    FechaEmision = DateTime.Parse(fechaEmision),
                    FechaTimbrado = DateTime.Parse(fechaTimbrado),
                    XML = request.rutaXML + uuid,
                    PDF = request.rutaPDF,
                    TotalFactura = decimal.Parse(totalFactura),
                    SubtotalFactura = decimal.Parse(subTotalFactura),
                    DescuentoFactura = 0,
                    FechaBaja = null,
                    FechaActualizacion = null,
                    Activo = EstatusFactura.Activo,
                    idEstatusFacturaRemesaSAT = EstatusFactura.SinValidacion,
                    idEstatusFacturaRemesa = EstatusFactura.Valida,
                    NumeroFactura = numeroFactura.Trim(),
                    SecuenciaMac = 0,
                    FechaPago = null
                };

                //se agregan los datos a facturas remesa antes de realizar validaciones
                _repoAgregaFacturaRemesa.Agregar(agregarFactura);





                using (var _ctx = new MaedicusContext(_DAO.ConParams))
                {
                    _numeroRemesa = _ctx.NumeroRemesa.Where(c => c.FolioRemesa == request.FolioRemesa).FirstOrDefault();

                }
                if (_numeroRemesa != null)
                {
                    _cveProveedor = _numeroRemesa.ClaveProveedor ?? default(int);
                }
                else
                {
                    Respuesta.Respuesta.mensaje = "No existen datos para el Folio de remesa " + request.FolioRemesa;
                    Respuesta.Respuesta.result = 1;
                    return Respuesta;
                }

               

                RequestElegibilidadRemesa requestElegibilidad = new RequestElegibilidadRemesa()
                {
                    FolioRemesa = request.FolioRemesa
                };
                var AutorizacionesRemesas = ObtenerElegibilidadesRemesas(requestElegibilidad);
                if (!AutorizacionesRemesas.Datos.Any())
                {
                    Respuesta.Respuesta.result = 0;
                    Respuesta.Respuesta.mensaje = "No existen datos relacionados al folio de la remesa";
                    return Respuesta;
                    //FechaCreacion = Convert.ToDateTime(AutorizacionesRemesas.Datos.Select(x => x.xDateInsert));
                }

                suma = AutorizacionesRemesas.Datos.Sum(x => x.Total);

                //resta con todos los copagos 
                //GetRemittanceDetailAsync
                LogFacturasConError logFacturaError = new LogFacturasConError()
                {
                    UUID = uuid.Trim(),
                    FechaAltaError = DateTime.Now,
                };
                FacturaxRemesa facturaxRemesa = new FacturaxRemesa()
                {
                    UUID = uuid.Trim(),
                    FolioRemesa = request.FolioRemesa.Trim(),
                    FechaCreacion = DateTime.Now
                };
                //validando fecha
                int resultFecha = AutorizacionesRemesas.Datos.Where(x => Convert.ToDateTime(x.xDateInsert) > Convert.ToDateTime(fechaTimbrado)).Count();
                if (resultFecha > 0)
                {
                    ListaErrorFactura.Add(new LogFacturasConError
                    {
                        UUID = uuid.Trim(),
                        FechaAltaError = DateTime.Now,
                        IdError = EstatusFactura.ErrorFechaTimbrado,
                        Activo = EstatusFactura.Activo
                    });
                    existeError = true;
                }
                //valida rfc receptor
                if ((rfcReceptor != rfcMediaccess))
                {
                    ListaErrorFactura.Add(new LogFacturasConError
                    {
                        UUID = uuid.Trim(),
                        FechaAltaError = DateTime.Now,
                        IdError = EstatusFactura.ErrorRFC,
                        Activo = EstatusFactura.Activo
                    });
                    existeError = true;
                }
                //valida subtotal
                if (suma.ToString("0.##") != subTotalFactura)
                {
                    ListaErrorFactura.Add(new LogFacturasConError
                    {
                        UUID = uuid.Trim(),
                        FechaAltaError = DateTime.Now,
                        IdError = EstatusFactura.ErrorSubtotal,
                        Activo = EstatusFactura.Activo
                    });
                    existeError = true;
                }
                //Agrega los errores que existan
                repoLog.AgregarRango(ListaErrorFactura);
                //si la factura tiene errores entra de manera automatica como inactiva
                if (ListaErrorFactura.Count() > 0)
                {
                    agregarFactura.Activo = EstatusFactura.Inactivo;
                }
                //valida si el registro existe o no 
                using (MaedicusContext _Context = new MaedicusContext(_DAO.ConParams))
                {
                    existe = _Context.FacturasRemesas.Any(x => x.UUID == agregarFactura.UUID);
                    List<CatErrores> query = new List<CatErrores>();
                    query = (from CE in _Context.CatErrores
                             select CE).ToList();
                    resultLog = (from LE in ListaErrorFactura
                                 join CE in query on LE.IdError equals CE.IdError
                                 select new LogFacturasModel
                                 {
                                     UUID = LE.UUID,
                                     IdError = LE.IdError,
                                     Descripcion = CE.Descripcion

                                 }).ToList();
                }

                if (existe)
                {
                    agregarFactura.FechaActualizacion = DateTime.Now;
                    repo.Actualizar(agregarFactura);
                }
                else
                {
                    //consumir servicio solo se consume cuando no hay errores
                    if (!existeError)
                    {
                        repo.Agregar(agregarFactura);
                        repositorio.Agregar(facturaxRemesa);
                        RequestvalidateCFDI requestService = new RequestvalidateCFDI()
                        {

                            clv = _cveProveedor,
                            pdf_b64 = request.PDF,
                            xml_b64 = request.XML,
                            validate_date = EstatusGeneral.validatedate,
                            Validate_payment_method = EstatusGeneral.validatepaymentmethod,
                            remesa = EstatusGeneral.remesa,
                            rfc = rfcEmisor
                        };
                        var _satEstatus = await ValidarFacturaEdifact(requestService);
                        //if (!string.IsNullOrEmpty(_satEstatus.mensaje))
                        //{
                        if (_satEstatus.result == 1)
                        {
                            RequestFacturaUpdate EstatusSat = new RequestFacturaUpdate()
                            {
                                UUID = agregarFactura.UUID,
                                idEstatusFacturaRemesaSAT = "C200"
                            };
                            ActualizarDatosFactura(EstatusSat);
                        }
                        else
                        {
                            Respuesta.Respuesta.mensaje = _satEstatus.mensaje + "con UUID  " + uuid;
                            Respuesta.Datos = resultLog;
                            Respuesta.Respuesta.result = 1;
                            return Respuesta;
                        }

                        //}
                    }
                    else
                    {
                        repo.Agregar(agregarFactura);
                    }
                }
                Respuesta.Respuesta.mensaje = (ListaErrorFactura.Count > 0) ? "La factura tiene errores" : "Se agrego el registro correctamente";
                Respuesta.Datos = resultLog;
                Respuesta.Respuesta.result = 1;
                repo.Dispose();
            }
            catch (Exception ex)
            {
                Respuesta.Respuesta.result = 0;
                Respuesta.Respuesta.mensaje = "Server Error" + ex.Message;
            }
            return Respuesta;
        }