using AnalizadorLexico.util;

namespace AnalizadorLexico;

public class AFDSignoPuntuacion : IAutomata
{
    private enum Estado
    {
        Inicio,
        Signo,
        Error
    }

    private char[] signoPuntuacion =
    {
        '.', ',', ';', ':'
    };
    public string TipoToken { get; }
    private Estado estado = Estado.Inicio;
    private string cadenaEvaluada = "";
    
    public void Procesar(char c)
    {
        cadenaEvaluada += c;

        switch (estado)
        {
            case Estado.Inicio:
                if (EsSigno(c))
                    estado = Estado.Signo;
                else
                    estado = Estado.Error;
                break;

            case Estado.Signo:
                if (!EsSigno(c))
                    estado = Estado.Error;
                break;
        }
    }

    public bool EsValido()
    {
        return estado == Estado.Signo;
    }

    bool IAutomata.TieneError()
    {
        return estado == Estado.Error;
    }

    private bool EsSigno(char c)
    {
        foreach (char signo in signoPuntuacion)
        {
            if (c == signo)
                return true;
        }
        return false;
    }
    public void Reiniciar()
    {
        estado = Estado.Inicio;
        cadenaEvaluada = "";
    }
    public string ObtenerLexema()
    {
        return cadenaEvaluada;
    }
  
}