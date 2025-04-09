using AnalizadorLexico.util;

namespace AnalizadorLexico;

public class AFDNumerico : IAutomata 
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
    public bool tieneError ;
    
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

    public bool EsValido()
    {
        if (esDecimalValido || esEnteroValido)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public bool TieneError() => estado == Estado.Error;

    public void Reiniciar()
    {
        estado = Estado.Inicio;
        cadenaEvaluada = "";
        esDecimal = false;
    }

    public string TipoToken { get; }

    public string obtenerResultado()
    {
        if (tieneError)
            return $"{cadenaEvaluada} → Formato numérico inválido";
        
        string resultado = cadenaEvaluada;
        
        // Eliminar ceros no significativos solo si es un número válido
        if (esEnteroValido || esDecimalValido)
        {
            // Dividir en partes si es decimal
            string[] partes = resultado.Split('.');
            bool esNegativo = partes[0].StartsWith("-");
            
            // Procesar parte entera
            //Eliminar el signo
            string parteEntera = partes[0].TrimStart('-', '+');
            // Eliminar ceros no significativos
            parteEntera = parteEntera.TrimStart('0');
            parteEntera = parteEntera == "" ? "0" : parteEntera; // Evitar cadena vacía si es 000 o 000.0
            parteEntera = (esNegativo ? "-" : "") + parteEntera;
            
            // Reconstruir el número
            resultado = partes.Length > 1 ? $"{parteEntera}.{partes[1]}" : parteEntera;
        }

        if (esDecimalValido)
            return $"{resultado} → Decimal válido (original: {cadenaEvaluada})";
        else if (esEnteroValido)
            return $"{resultado} → Entero válido (original: {cadenaEvaluada})";
        else
            return $"{cadenaEvaluada} → Entrada incompleta";
    }
    
}