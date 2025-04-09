using AnalizadorLexico.util;

namespace AnalizadorLexico
{
    public class AFDIdentificador : IAutomata
    {
        private enum Estado { Inicio, SignoDolar, Identificador, Error }
        private Estado estado = Estado.Inicio;
        private string cadenaProcesada = "";
        
        public string TipoToken => "Identificador";

        public void Procesar(char c)
        {
            cadenaProcesada += c;

            switch (estado)
            {
                case Estado.Inicio:
                    if (c == '$')
                    {
                        estado = Estado.SignoDolar;
                    }
                    else
                    {
                        estado = Estado.Error;
                    }
                    break;

                case Estado.SignoDolar:
                    if (EsCaracterValido(c))
                    {
                        estado = Estado.Identificador;
                    }
                    else
                    {
                        estado = Estado.Error;
                    }
                    break;

                case Estado.Identificador:
                    if (!EsCaracterValido(c))
                    {
                        estado = Estado.Error;
                    }
                    break;
            }
        }

        private bool EsCaracterValido(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == '-';
        }

        public bool EsValido()
        {
            return estado == Estado.Identificador && cadenaProcesada.Length > 1;
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