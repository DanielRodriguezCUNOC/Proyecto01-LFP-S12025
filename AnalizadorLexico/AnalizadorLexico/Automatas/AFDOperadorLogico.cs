using AnalizadorLexico.util;

namespace AnalizadorLexico
{
    public class AFDOperadorLogico : IAutomata
    {
        private enum Estado { Inicio, And1, Or1, AndFinal, OrFinal, Textual, Error }
        private Estado estado = Estado.Inicio;
        private string cadenaProcesada = "";
        
        public string TipoToken => "OperadorLogico";

        public void Procesar(char c)
        {
            // Convertir a minúscula para manejar "AND"/"and" y "OR"/"or"
            char lowerC = char.ToLower(c);
            cadenaProcesada += c;

            switch (estado)
            {
                case Estado.Inicio:
                    if (lowerC == '&')
                        estado = Estado.And1;
                    else if (lowerC == '|')
                        estado = Estado.Or1;
                    else if (lowerC == 'a') // Inicio de "AND"
                        estado = Estado.Textual;
                    else if (lowerC == 'o') // Inicio de "OR"
                        estado = Estado.Textual;
                    else
                        estado = Estado.Error;
                    break;

                case Estado.And1:
                    if (lowerC == '&')
                        estado = Estado.AndFinal;
                    else
                        estado = Estado.Error;
                    break;

                case Estado.Or1:
                    if (lowerC == '|')
                        estado = Estado.OrFinal;
                    else
                        estado = Estado.Error;
                    break;

                case Estado.Textual:
                    // Construir "AND" o "OR" textual
                    string textoActual = cadenaProcesada.ToLower();
                    if (textoActual == "and")
                        estado = Estado.AndFinal;
                    else if (textoActual == "or")
                        estado = Estado.OrFinal;
                    else if (textoActual.Length > 3) // "AND" tiene 3 letras, "OR" tiene 2
                        estado = Estado.Error;
                    break;
            }
        }

        public bool EsValido()
        {
            return estado == Estado.AndFinal || estado == Estado.OrFinal;
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

        // Método para verificar si está en medio de un operador textual
        public bool EstaProcesandoTexto()
        {
            return estado == Estado.Textual;
        }
    }
}