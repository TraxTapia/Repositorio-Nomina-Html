  public void GeneraArchivoLog(String Metodo, List<String> variables, List<String> parametros, String Mensaje)
        {
            string pathRuta = appSettings.Value.Logs["LogMetodos"];
            String rutaArchivo = pathRuta + "Log_" + DateTime.Now.ToShortDateString().Replace("/", "") + ".txt";
            String log = string.Empty;
            int indice = 0;

            log = string.Concat(DateTime.Now.ToString(), "--", Metodo, "--", "Parametros ingresados: ");
            Type t = parametros.GetType();
            PropertyInfo[] props = t.GetProperties();
           
            //foreach (var parametro in props)
            //{
            //    if (parametro.GetIndexParameters().Length == 0)
            //    {

            //    }
            //    log = string.Concat(log, " -- ", parametro.Name,parametro.PropertyType.Name,parametro.GetValue(parametros));
            //    indice++;
            //}

            log = string.Concat(log, "--", Mensaje);

            try
            {
                if (File.Exists(rutaArchivo))
                {
                    StreamWriter archivo = File.AppendText(rutaArchivo);
                    archivo.WriteLine(log);
                    archivo.Close();
                }
                else
                {
                    StreamWriter archivo = new StreamWriter(rutaArchivo, false, System.Text.Encoding.Default);
                    archivo.WriteLine(log);
                    archivo.Close();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }