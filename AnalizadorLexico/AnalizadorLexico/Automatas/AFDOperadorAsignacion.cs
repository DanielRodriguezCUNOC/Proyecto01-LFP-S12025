using AnalizadorLexico.util;

namespace AnalizadorLexico
{
    public class AFDOperadorAsignacion : IAutomata
    {
        private enum Estado { Inicio, AsignacionValida, Error }
        private Estado estado = Estado.Inicio;
        private string cadenaProcesada = "";
        
        public string TipoToken => "OperadorAsignacion";

        public void Procesar(char c)
        {
            cadenaProcesada += c;

            // Transiciones del autómata
            estado = estado switch
            {
                Estado.Inicio when c == '=' => Estado.AsignacionValida,
                Estado.Inicio => Estado.Error,
                _ => Estado.Error // Cualquier otro caso es error
            };
        }

        public bool EsValido() => estado == Estado.AsignacionValida && cadenaProcesada.Length == 1;
        public bool TieneError() => estado == Estado.Error;
        
        public void Reiniciar()
        {
            estado = Estado.Inicio;
            cadenaProcesada = "";
        }

        public string ObtenerLexema() => cadenaProcesada;
    }
}