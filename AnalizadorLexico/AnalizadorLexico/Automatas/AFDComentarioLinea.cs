using AnalizadorLexico.util;

namespace AnalizadorLexico
{
    public class AFDComentarioLinea : IAutomata
    {
        private enum Estado { Inicio, Comentario, Fin, Error }
        private Estado estado = Estado.Inicio;
        private string cadenaProcesada = "";
        private int filaInicio = 1;
        private int columnaInicio = 1;
        
        public string TipoToken => "Comentario";

        public void Procesar(char c)
        {
            if (estado == Estado.Fin)
            {
                estado = Estado.Error;
                return;
            }

            cadenaProcesada += c;

            switch (estado)
            {
                case Estado.Inicio:
                    if (c == '#')
                        estado = Estado.Comentario;
                    else
                        estado = Estado.Error;
                    break;

                case Estado.Comentario:
                    if (c == '\n')
                        estado = Estado.Fin;
                    break;
            }
        }

        public bool EsValido()
        {
            return estado == Estado.Fin || 
                  (estado == Estado.Comentario && cadenaProcesada.Length >= 1);
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
            // Eliminar el \n final si existe
            string lexema = cadenaProcesada;
            if (lexema.EndsWith("\n"))
                lexema = lexema.Substring(0, lexema.Length - 1);
            return lexema;
        }

        public void SetPosicionInicio(int fila, int columna)
        {
            filaInicio = fila;
            columnaInicio = columna;
        }

        public TokenInfo GetTokenInfo()
        {
            return new TokenInfo {
                Tipo = TipoToken,
                Lexema = ObtenerLexema(),
                Fila = filaInicio,
                Columna = columnaInicio
            };
        }
    }
}