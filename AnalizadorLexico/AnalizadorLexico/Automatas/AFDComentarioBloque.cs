using AnalizadorLexico.util;

namespace AnalizadorLexico
{
    public class AFDComentarioBloque : IAutomata
    {
        private enum Estado { Inicio, Abierto, PosibleCierre, Fin, Error }
        private Estado estado = Estado.Inicio;
        private string cadenaProcesada = "";
        private int filaInicio = 1;
        private int columnaInicio = 1;
        private bool ultimoEsAsterisco = false;
        
        public string TipoToken => "ComentarioBloque";

        public void Procesar(char c)
        {
            if (estado == Estado.Fin || estado == Estado.Error)
                return;

            cadenaProcesada += c;

            switch (estado)
            {
                case Estado.Inicio:
                    if (c == '/')
                        estado = Estado.Abierto;
                    else
                        estado = Estado.Error;
                    break;

                case Estado.Abierto:
                    if (c == '*')
                        estado = Estado.PosibleCierre;
                    else
                        estado = Estado.Error;
                    break;

                case Estado.PosibleCierre:
                    if (c == '*')
                        ultimoEsAsterisco = true;
                    else if (ultimoEsAsterisco && c == '/')
                        estado = Estado.Fin;
                    else
                        ultimoEsAsterisco = false;
                    break;
            }
        }

        public bool EsValido()
        {
            return estado == Estado.Fin;
        }

        public bool TieneError()
        {
            return estado == Estado.Error;
        }

        public void Reiniciar()
        {
            estado = Estado.Inicio;
            cadenaProcesada = "";
            ultimoEsAsterisco = false;
        }

        public string ObtenerLexema()
        {
            return cadenaProcesada;
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

        public bool EstaEnComentario()
        {
            return estado == Estado.PosibleCierre || estado == Estado.Abierto;
        }
    }
}