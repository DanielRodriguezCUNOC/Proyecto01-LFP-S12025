namespace AnalizadorLexico;

public class AFDNumerico
{
    private enum Estado
    {
        Inicio,
        Signo,
        Entero,
        Punto,
        Decimal,
        Error
    }
    private Estado estado = Estado.Inicio;
    private string cadenaEvaluada = "";
    private bool esDecimal = false;
    
    // Metodos de validacion
    public bool esEnteroValido => estado == Estado.Entero && !esDecimal;
    public bool esDecimalValido => estado == Estado.Decimal;
    public bool esDigito (char c) => c >= '0' && c <= '9'; 
    public bool tieneError => estado == Estado.Error;
    
    // Metodos de procesamiento

    public void Procesar(char c)
    {
        if (tieneError)
        {
            cadenaEvaluada += c;
            return;
        }

        switch (estado)
        {
            case Estado.Inicio:
                if (c == '+' || c == '-')
                {
                    estado = Estado.Signo;
                    cadenaEvaluada += c;
                }
                else if (esDigito(c))
                {
                    estado = Estado.Entero;
                    cadenaEvaluada += c;
                }
                else
                {
                    estado = Estado.Error;
                    cadenaEvaluada += $"[{c}]";
                }
                break;
            case Estado.Signo:
                if (esDigito(c))
                {
                    estado = Estado.Entero;
                    cadenaEvaluada += c;
                }
                else
                {
                    estado = Estado.Error;
                    cadenaEvaluada += $"[{c}]";
                }
                break;
            case Estado.Entero:
                if (esDigito(c))
                {
                    cadenaEvaluada += c;
                }
                else if (c == '.')
                {
                    estado = Estado.Punto;
                    cadenaEvaluada += c;
                }
                else
                {
                    estado = Estado.Error;
                    cadenaEvaluada += $"[{c}]";
                }
                break;
            case Estado.Punto:
                if (esDigito(c))
                {
                    estado = Estado.Decimal;
                    cadenaEvaluada += c;
                }
                else
                {
                    estado = Estado.Error;
                    cadenaEvaluada += $"[{c}]";
                }
                break;
            case Estado.Decimal:
                if (esDigito(c))
                {
                    cadenaEvaluada += c;
                }
                else
                {
                    estado = Estado.Error;
                    cadenaEvaluada += $"[{c}]";
                }
                break;
        }
    }

    public string obtenerResultado()
    {
        if (tieneError)
            return $"{cadenaEvaluada} -> Formato no valido";
        else if (esDecimalValido)
            return $"{cadenaEvaluada} -> Decimal valido";
        else if (esEnteroValido)
            return $"{cadenaEvaluada} -> Entero valido";
        else
            return $"{cadenaEvaluada} -> Formato no valido";
    }
    
    public void reiniciar()
    {
        estado = Estado.Inicio;
        cadenaEvaluada = "";
        esDecimal = false;
    }
}