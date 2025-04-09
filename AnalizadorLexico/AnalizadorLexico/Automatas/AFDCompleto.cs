using System.Collections.Generic;
using AnalizadorLexico.util;

namespace AnalizadorLexico;

public class AFDCompleto
{
    private readonly AFDNumerico afdNumerico = new AFDNumerico();
    private readonly AFDIdentificador afdIdentificador = new AFDIdentificador();
    private readonly AFDPalabraReservada afdPalabraReservada = new AFDPalabraReservada();
    private readonly AFDLiteral afdLiteral = new AFDLiteral();
    private readonly AFDSignoPuntuacion afdSignoPuntuacion = new AFDSignoPuntuacion();
    private readonly AFDOperadorAritmetico afdOperadorAritmetico = new AFDOperadorAritmetico();
    private readonly AFDOperadorRelacional afdOperadorRelacional = new AFDOperadorRelacional();

    public List<TokenInfo> AnalizarTexto(string texto)
    {
        var tokens = new List<TokenInfo>();
        int fila = 1;
        int columna = 1;
        int inicioColumna = 1;
        int inicioFila = 1;
        string lexemaActual = "";
        IAutomata automataActual = null;

        for (int i = 0; i < texto.Length; i++)
        {
            char c = texto[i];

            // Manejo de saltos de línea
            if (c == '\n')
            {
                fila++;
                columna = 1;
                continue;
            }
            
           // Manejo de operadores relacionales
            if (automataActual is AFDOperadorRelacional opRel && opRel.EsperarOperadorCompuesto())
            {
                // Esperar el siguiente carácter para ver si es '='
                continue;
            }

            // Determinar qué autómata usar basado en el primer carácter
            if (automataActual == null)
            {
                automataActual = SeleccionarAutomata(c, ref inicioFila, ref inicioColumna);
                if (automataActual == null) // Carácter no reconocido
                {
                    columna++;
                    continue;
                }
            }

            // Procesar el carácter en el autómata seleccionado
            automataActual.Procesar(c);
            lexemaActual += c;

            // Verificar si se completó un token
            if (automataActual.EsValido())
            {
                tokens.Add(new TokenInfo {
                    Tipo = automataActual.TipoToken,
                    Lexema = lexemaActual,
                    Fila = inicioFila,
                    Columna = inicioColumna
                });
                automataActual = null;
                lexemaActual = "";
                inicioColumna = columna + 1;
            }
            else if (automataActual.TieneError())
            {
                // Manejar error
                automataActual = null;
                lexemaActual = "";
                inicioColumna = columna + 1;
            }

            columna++;
        }

        return tokens;
    }

    private IAutomata SeleccionarAutomata(char c, ref int fila, ref int columna)
    {
        if (c == '$') return afdIdentificador;
        if (char.IsDigit(c)) return afdNumerico;
        if (c == '"' || c == '\'') return afdLiteral;
        if (".,;:".IndexOf(c) >= 0) return afdSignoPuntuacion;
        if ("^*/+-".IndexOf(c) >= 0) return afdOperadorAritmetico; 
        if (char.IsLetter(c)) return afdPalabraReservada;
        if (c == '>' || c == '<') return afdOperadorRelacional;
        
        return null; // Carácter no reconocido (espacios, operadores, etc.)
    }
}
