using AnalizadorLexico.util;

namespace AnalizadorLexico
{
    public class AFDOperadorRelacional : IAutomata
    {
        private enum Estado { Inicio, MayorQue, MenorQue, OperadorCompleto, Error }
        private Estado estado = Estado.Inicio;
        private string cadenaProcesada = "";
        
        public string TipoToken => "OperadorRelacional";

        public void Procesar(char c)
        {
            cadenaProcesada += c;

            switch (estado)
            {
                case Estado.Inicio:
                    if (c == '>')
                        estado = Estado.MayorQue;
                    else if (c == '<')
                        estado = Estado.MenorQue;
                    else
                        estado = Estado.Error;
                    break;

                case Estado.MayorQue:
                case Estado.MenorQue:
                    if (c == '=')
                        estado = Estado.OperadorCompleto;
                    else
                        estado = Estado.Error; // Solo operadores de un carácter
                    break;

                case Estado.OperadorCompleto:
                    // No se permiten más caracteres después de >= o <=
                    estado = Estado.Error;
                    break;
            }
        }

        public bool EsperarOperadorCompuesto()
        {
            return estado == Estado.MayorQue || estado == Estado.MenorQue;
        }

        public bool EsValido()
        {
            // Valida: >, <, >=, <=
            return estado == Estado.MayorQue || 
                   estado == Estado.MenorQue || 
                   estado == Estado.OperadorCompleto;
        }

        public bool TieneError()
        {
            return estado == Estado.Error;
        }

        public void Reiniciar()
        {
            estado = Estado.Inicio;
            cadenaProcesada = "";
        }

        public string ObtenerLexema()
        {
            return cadenaProcesada;
        }
    }
}