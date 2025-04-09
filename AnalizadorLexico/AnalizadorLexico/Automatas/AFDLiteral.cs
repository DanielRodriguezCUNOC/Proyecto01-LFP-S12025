using AnalizadorLexico.util;

namespace AnalizadorLexico;

public class AFDLiteral : IAutomata
{
    private enum Estado
    {
        Inicio,
        ComillasDobles,
        ComillasSimples,
        Fin,
        Error
    }

    private Estado estado = Estado.Inicio;
    private string cadenaEvaluada = "";
    private char comillaInicial;
    
    public void Procesar(char c)
    {
        cadenaEvaluada += c;

        switch (estado)
        {
            case Estado.Inicio:
                if (c == '"')
                {
                    comillaInicial = '"';
                    estado = Estado.ComillasDobles;
                }
                else if (c == '\'')
                {
                    comillaInicial = '\'';
                    estado = Estado.ComillasSimples;
                }
                else
                {
                    estado = Estado.Error;
                }
                break;

            case Estado.ComillasDobles:
            case Estado.ComillasSimples:
                // Verificar caracteres no permitidos
                if (c == '\n' || c == '\f' || c == '\t' || c == '\r')
                {
                    estado = Estado.Error;
                }
                else if ((estado == Estado.ComillasDobles && c == '"') || 
                         (estado == Estado.ComillasSimples && c == '\''))
                {
                    estado = Estado.Fin;
                }
                break;

            case Estado.Fin:
                // Si llegamos al fin y sigue procesando, es error
                estado = Estado.Error;
                break;
        }
    }

    public bool EsValido() => estado == Estado.Fin;
    public bool TieneError() => estado == Estado.Error;
    
    public string ObtenerResultado()
    {
        if (TieneError())
        {
            // Mostrar caracteres especiales como secuencias de escape
            string cadenaMostrar = cadenaEvaluada
                .Replace("\n", "\\n")
                .Replace("\t", "\\t")
                .Replace("\r", "\\r")
                .Replace("\f", "\\f");
            return $"{cadenaMostrar} → NO es un literal válido (contiene caracteres especiales)";
        }
        else if (EsValido())
            return $"{cadenaEvaluada} → Literal válido";
        else
            return $"{cadenaEvaluada} → Cadena incompleta";
    }

    public string ObtenerLiteral() => EsValido() ? cadenaEvaluada : null;

    public void Reiniciar()
    {
        estado = Estado.Inicio;
        cadenaEvaluada = "";
        comillaInicial = '\0';
    }

    public string TipoToken { get; }
    public string ObtenerLexema()
    {
        return cadenaEvaluada;
    }
}