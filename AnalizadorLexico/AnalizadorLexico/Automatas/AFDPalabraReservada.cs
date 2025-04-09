using System;
using AnalizadorLexico.util;

namespace AnalizadorLexico;

public class AFDPalabraReservada : IAutomata
{
    private enum Estado
    {
        Inicio,
        Siguiente,
        Error
    }
    private readonly string[] palabrasReservadas =
    {
        "if", "class", "for", "then", "else", "public", "private", "package", "import", "static", "void",
        "int", "true", "false", "extends", "short", "boolean", "float", "interface", "final", "protected", "return", "while", "case",
        "implements"
    };

    private readonly char[] letrasClave =
    {
        'i', 'c', 'f', 't', 'e', 'p', 'b', 's', 'v', 'r', 'w'
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
                if (EsLetraInicialValida(c))
                    estado = Estado.Siguiente;
                else
                    estado = Estado.Error;
                break;

            case Estado.Siguiente:
                if (!SigueSiendoValida())
                    estado = Estado.Error;
                break;
        }
    }

    private bool EsLetraInicialValida(char c)
    {
        // Verifica si la letra inicial es una de las letras clave
        for (int i = 0; i < letrasClave.Length; i++)
        {
            if (letrasClave[i] == c)
                return true;
        }
        return false;
    }

    private bool SigueSiendoValida()
    {
        // Verifica si la cadena actual es prefijo de alguna palabra reservada
        for (int i = 0; i < palabrasReservadas.Length; i++)
        {
            // asignamos la palabra reservada a una variable
            string palabra = palabrasReservadas[i];
            // creamos una variable para verificar si coincide
            bool coincide = true;

            // Recorremos la cadenaEvaluada para buscar coincidencias
            for (int j = 0; j < cadenaEvaluada.Length; j++)
            {
                /*
                 * Si la longitud de la cadenaEvaluada es mayor que la longitud de la palabra reservada es porque no coincide
                 * Si la letra de la cadenaEvaluada no coincide con la letra de la palabra reservada entonces no coincide
                 */
                if (j >= palabra.Length || cadenaEvaluada[j] != palabra[j])
                {
                    coincide = false;
                    break;
                }
            }
            if (coincide)
                return true;
        }
        return false;
    }

    public bool EsValido()
    {
        // Verifica si la cadena completa es una palabra reservada
        for (int i = 0; i < palabrasReservadas.Length; i++)
        {
            if (palabrasReservadas[i] == cadenaEvaluada)
                return true;
        }
        return false;
    }

    public bool TieneError() => estado == Estado.Error;
        
    public string ObtenerResultado()
    {
        if (TieneError())
            return $"{cadenaEvaluada} → NO es palabra reservada";
        else if (EsValido())
            return $"{cadenaEvaluada} → Palabra reservada válida";
        else
            return $"{cadenaEvaluada} → Cadena incompleta";
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