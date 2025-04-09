using AnalizadorLexico.util;

namespace AnalizadorLexico
{
    public class AFDOperadorAritmetico : IAutomata
    {
        private enum Estado { Inicio, Operador, Error }
        private Estado estado = Estado.Inicio;
        private string cadenaProcesada = "";
        
        // Operadores válidos
        private readonly char[] operadoresValidos = { '^', '*', '/', '+', '-' };
        
        public string TipoToken => "OperadorAritmetico";

        public void Procesar(char c)
        {
            cadenaProcesada += c;

            switch (estado)
            {
                case Estado.Inicio:
                    if (EsOperadorValido(c))
                        estado = Estado.Operador;
                    else
                        estado = Estado.Error;
                    break;

                case Estado.Operador:
                    // Los operadores aritméticos son de un solo carácter
                    estado = Estado.Error;
                    break;
            }
        }

        private bool EsOperadorValido(char c)
        {
            foreach (char op in operadoresValidos)
            {
                if (c == op)
                    return true;
            }
            return false;
        }

        public bool EsValido()
        {
            return estado == Estado.Operador && cadenaProcesada.Length == 1;
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