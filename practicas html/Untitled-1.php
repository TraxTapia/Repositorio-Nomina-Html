

    public function doDevolver(Request $request)
    {
        var_dump('Hola aqui llegamos devolucion');
        //dd($request->infoCPT);

        $f = Factura::noLock()->where('secuenciaMac', $request->secuencia)->whereNotIn('codEstatusFactura', array('B', 'D', 'P'))->get();
        if (count($f) < 1) {
            return json_encode(array('status' => 'ERROR', 'msg' => 'No se encontró la factura o el estatus es inválido.'));
        } else {

            $cuenta_dev = $f[0]->movimientos_factura()->where('codestatusfactura', 'D')->where('secuenciaMac', $f[0]->secuenciaMac)->get();
            if (count($cuenta_dev) >= 5) {
                return json_encode(array('status' => 'ERROR', 'msg' => 'La Factura ya cuenta con 3 devoluciones y no puede ser devuelta, solo se cambiara el estatus en modificar.'));
            } else {

                //Guardar la info de la Lista de Motivos de Devolucion por Analisis de Devolución
                $listaMotivos = DB::select("EXEC SP_CreateReportAD " . $f[0]->secuenciaMac);

                //dd($listaMotivos);
                $arrMotivosDevolucion = array();
                //Obtener motivos AUTOMATICOS
                //dd($listaMotivos);
                if (count($listaMotivos) > 0) {
                    foreach ($listaMotivos as $row) {

                        if (isset($request->infoCPT[$row->Folio])) {

                            $arrMotivosDevolucion[] = array('Id' => $row->Id, 'Folio' => $row->Folio, 'CodConcepto' => $row->CodConcepto, 'subMotivo' => $row->SubMotivosDev_Id, 'observacion' => $request->infoCPT[$row->Folio]);
                            DB::beginTransaction();
                            try {
                                $devcpt = new DevolucionesCPT();
                                $devcpt->IdError = $row->Id;
                                $devcpt->secuenciaMac = $f[0]->secuenciaMac;
                                $devcpt->OperacionId = $row->Operacion_Id;
                                $devcpt->Folio = $row->Folio;
                                $devcpt->CodConcepto = $row->CodConcepto;
                                $devcpt->SubmotivoId = $row->SubMotivosDev_Id;
                                $devcpt->Fecha = DB::raw('convert(varchar, getdate(), 20)');
                                $devcpt->Activo = 1;
                                $devcpt->Observaciones = nl2br(htmlentities($request->infoCPT[$row->Folio], ENT_QUOTES, 'UTF-8'));
                                $devcpt->save();
                                DB::commit();
                            } catch (Exception $e) {
                                DB::rollBack();
                                return json_encode(array('status' => 'ERROR', 'msg' => 'Error al guardar en la base de datos en DevolucionesCPT. ' . $e->getMessage()));
                            }
                        }
                    }
                }

                $sub_cuenta = 0;

                if (count($request->info) > 0) {

                    foreach ($request->info as $k => $v) {
                        if (!empty($v)) {
                            if (!isset($v['submotivos']))
                                $v['submotivos'] = array(0);

                            foreach ($v['submotivos'] as $sub) {
                                DB::beginTransaction();

                                Storage::disk()->append('devtxt/devoluciones_' . $f[0]->secuenciaMac . '.txt', $f[0]->secuenciaMac . "-" . $v['motivo'] . "-" . $sub . "-" . $v['observacion']);

                                try {
                                    $dev = new DevolucionesFactura();
                                    $dev->cve_prov = $f[0]->cve_prov;
                                    $dev->no_fac = $f[0]->no_fac;
                                    $dev->codEstatusFactura = 'D';
                                    $dev->codConcepto = $v['motivo'];
                                    $dev->submotivo_id = $sub;
                                    $dev->fecha_alta =  DB::raw('convert(varchar, getdate(), 20)');
                                    $dev->activo = 1;
                                    $dev->observaciones = $v['observacion'];
                                    $dev->secuenciaMac = $f[0]->secuenciaMac;
                                    $dev->save();
                                    DB::commit();
                                    $sub_cuenta++;
                                } catch (Exception $e) {
                                    DB::rollBack();
                                    return json_encode(array('status' => 'ERROR', 'msg' => 'Error al guardar en la base de datos. ' . $e->getMessage()));
                                }
                            }
                        }
                    }
                }

                //if($sub_cuenta == 0){
                //return json_encode(array('status' => 'ERROR', 'msg' => 'Error al guardar en la base de datos. Motivos.'));
                //}

                DB::beginTransaction();

                try {

                    $f[0]->stickerEnvio = $request->guia;
                    $f[0]->codEstatusFactura = 'D';
                    $f[0]->SegLogin = trim(Auth::user()->cve_usuario);
                    $f[0]->codConcepto = count($request->info) > 0 ? $request->info[1]['motivo'] : $arrMotivosDevolucion[0]['CodConcepto'];
                    $f[0]->f_regresada = DB::raw('convert(varchar, getdate(), 20)');
                    $f[0]->observaciones = count($request->info) > 0 ? $request->info[1]['observacion'] : $arrMotivosDevolucion[0]['observacion'];
                    $f[0]->save();
                    $f[0]->movimientos_factura()->create([
                        'fechaMovimiento' => DB::raw('convert(varchar, getdate(), 20)'),  'usuario' => $f[0]->SegLogin, 'codestatusfactura' => 'D', 'codConcepto' => count($request->info) > 0 ? $request->info[1]['motivo'] : $arrMotivosDevolucion[0]['CodConcepto'],
                        'cve_prov' => $f[0]->cve_prov, 'observaciones' => count($request->info) > 0 ? $request->info[1]['observacion'] : $arrMotivosDevolucion[0]['observacion'], 'secuenciaMac' => $f[0]->secuenciaMac
                    ]);
                    //$f[0]->log_devoluciones()->create(['CodEstatus' => 'PD', 'secuenciaMac' => $f[0]->secuenciaMac, 'fecha_alta' => DB::raw('convert(varchar, getdate(), 20)'), 'activo' => 1, 'SegLogin' => trim(Auth::user()->cve_usuario) ]);
                    DB::commit();
                    return json_encode(array('status' => 'OK', 'msg' => 'Se realizó la devolución de la factura.', 'url' => '/facturas/devolucionMasiva/download/' . $f[0]->secuenciaMac));
                } catch (Exception $e) {
                    DB::rollBack();
                    return json_encode(array('status' => 'ERROR', 'msg' => '2Error al guardar en la base de datos. ' . $e->getMessage()));
                }
            } //else
        }
    }