using AnalizadorLexico.util;

public class AFDIdentificador : IAutomata
{
    private enum Estado { Inicio, SignoDolar, Identificador, Error }
    private Estado estado = Estado.Inicio;
    private string cadenaProcesada = "";
    
    public string TipoToken => "Identificador";

    public void Procesar(char c)
    {
        switch (estado)
        {
            case Estado.Inicio:
                if (c == '$')
                {
                    estado = Estado.SignoDolar;
                    cadenaProcesada += c;
                }
                else
                {
                    estado = Estado.Error;
                }
                break;

            case Estado.SignoDolar:
                if (EsCaracterValido(c, true)) // Primer carácter después del $
                {
                    estado = Estado.Identificador;
                    cadenaProcesada += c;
                }
                else
                {
                    estado = Estado.Error;
                }
                break;

            case Estado.Identificador:
                if (EsCaracterValido(c, false)) // Caracteres subsiguientes
                {
                    cadenaProcesada += c;
                }
                else
                {
                    estado = Estado.Error;
                }
                break;
        }
    }

    private bool EsCaracterValido(char c, bool primerCaracter)
    {
        // Para el primer carácter después del $: letras, números, _ o -
        if (primerCaracter)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == '-';
        }
        // Para caracteres subsiguientes: letras, números o _
        // (opcional: puedes permitir también '-' aquí si quieres)
        return char.IsLetterOrDigit(c) || c == '_' || c == '-';
    }

    public bool EsValido()
    {
        // Requiere al menos $ + un carácter válido
        return estado == Estado.Identificador && cadenaProcesada.Length >= 2;
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