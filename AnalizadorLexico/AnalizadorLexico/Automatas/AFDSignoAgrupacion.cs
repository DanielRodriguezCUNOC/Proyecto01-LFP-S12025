using AnalizadorLexico.util;

namespace AnalizadorLexico
{
    public class AFDSignoAgrupacion : IAutomata
    {
        private enum Estado { Inicio, ParentesisAbierto, CorcheteAbierto, LlaveAbierta, Error }
        private Estado estado = Estado.Inicio;
        private string cadenaProcesada = "";
        
        public string TipoToken => "SignoAgrupacion";

        public void Procesar(char c)
        {
            cadenaProcesada += c;

            switch (estado)
            {
                case Estado.Inicio:
                    if (c == '(')
                        estado = Estado.ParentesisAbierto;
                    else if (c == '[')
                        estado = Estado.CorcheteAbierto;
                    else if (c == '{')
                        estado = Estado.LlaveAbierta;
                    else
                        estado = Estado.Error;
                    break;

                case Estado.ParentesisAbierto:
                    if (c == ')')
                        estado = Estado.Inicio; // Token completo
                    else
                        estado = Estado.Error;
                    break;

                case Estado.CorcheteAbierto:
                    if (c == ']')
                        estado = Estado.Inicio; // Token completo
                    else
                        estado = Estado.Error;
                    break;

                case Estado.LlaveAbierta:
                    if (c == '}')
                        estado = Estado.Inicio; // Token completo
                    else
                        estado = Estado.Error;
                    break;
            }
        }

        public bool EsValido()
        {
            // Valida pares completos: (), [], {}
            return (estado == Estado.Inicio && cadenaProcesada.Length == 2) ||
                   // O signos individuales: (, ), [, ], {, }
                   (cadenaProcesada.Length == 1 && "()[]{}".Contains(cadenaProcesada));
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

        public bool EsperaCierre()
        {
            return estado == Estado.ParentesisAbierto || 
                   estado == Estado.CorcheteAbierto || 
                   estado == Estado.LlaveAbierta;
        }
    }
}