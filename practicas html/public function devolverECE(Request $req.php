public function devolverECE(Request $request)
    {
        try {

            $f_factura = Factura::Where('SecuenciaMac', $request->Secuencia)->update(['CodEstatusFactura' => 'C']);
            $res = Factura::NOLOCK()->select('secuenciaMac')->where('secuenciaMac', $request->Secuencia)->get();
            // $f = Factura::noLock()->where('SecuenciaMac', '2136862')->get();
            foreach ($res as &$valor) {
                $sec = $valor->secuenciaMac;
            }
            $urlToken = env('ServiceUrl');
            $urlService = env('serviceChageEAD');

            $response = $this->servicGetToken($urlToken, $isDebug = false);
            $jsonResponse = json_decode($response);
            $tkn = $jsonResponse->access_token;
            $data = array(
                'SecuenciaMac' => $sec,
                'Observations' => 'Prueba devolucion'
            );
            //     echo ($data);

            $ServiceResponse = $this->ServicesApiSase($urlService, $data, $tkn, $isDebug = false);
            $jsonResponseService = json_decode($ServiceResponse);

            var_dump($jsonResponseService);

            if ($jsonResponseService->Result->Code == "200") {
                return json_encode(array('status' => 'OK', 'msg' => 'Se realizó la devolución de la factura.'));
            } else {
                return json_encode(array('status' => '', 'msg' => $jsonResponseService->Result->Errors[0]->Message));
            }
        } catch (Exception $e) {
            return json_encode(array('status' => 'ERROR', 'msg' => $e->getMessage()));
        }
    }
