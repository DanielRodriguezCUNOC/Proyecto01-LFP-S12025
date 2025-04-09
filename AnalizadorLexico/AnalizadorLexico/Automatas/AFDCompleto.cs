using System;
using System.Collections.Generic;
using AnalizadorLexico.util;
using Avalonia.Threading;

namespace AnalizadorLexico;

public class AFDCompleto
{
    public event Action<List<TokenInfo>> ReporteTokensGenerado;
    private readonly AFDNumerico afdNumerico = new AFDNumerico();
    private readonly AFDIdentificador afdIdentificador = new AFDIdentificador();
    private readonly AFDPalabraReservada afdPalabraReservada = new AFDPalabraReservada();
    private readonly AFDLiteral afdLiteral = new AFDLiteral();
    private readonly AFDSignoPuntuacion afdSignoPuntuacion = new AFDSignoPuntuacion();
    private readonly AFDOperadorAritmetico afdOperadorAritmetico = new AFDOperadorAritmetico();
    private readonly AFDOperadorRelacional afdOperadorRelacional = new AFDOperadorRelacional();
    private readonly AFDOperadorLogico afdOperadorLogico = new AFDOperadorLogico();
    private readonly AFDOperadorAsignacion afdOperadorAsignacion = new AFDOperadorAsignacion();
    private readonly AFDSignoAgrupacion afdSignoAgrupacion = new AFDSignoAgrupacion();
    private readonly AFDComentarioLinea afdComentarioLinea = new AFDComentarioLinea();
    private readonly AFDComentarioBloque afdComentarioBloque = new AFDComentarioBloque();

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

        if (c == '\n')
        {
            fila++;
            columna = 1;
            inicioColumna = 1;

            if (automataActual is AFDComentarioLinea && automataActual.EsValido())
            {
                tokens.Add(afdComentarioLinea.GetTokenInfo());
                automataActual = null;
                lexemaActual = "";
            }
            continue;
        }

        if (char.IsWhiteSpace(c) && automataActual == null)
        {
            columna++;
            continue;
        }

        if (automataActual is AFDOperadorRelacional opRel && opRel.EsperarOperadorCompuesto()) continue;
        if (automataActual is AFDOperadorLogico opLog && opLog.EstaProcesandoTexto()) continue;

        if (automataActual == null)
        {
            inicioFila = fila;
            inicioColumna = columna;
            automataActual = SeleccionarAutomata(c, i, texto);

            if (automataActual is AFDComentarioBloque)
            {
                afdComentarioBloque.SetPosicionInicio(fila, columna);
            }

            if (automataActual is AFDComentarioLinea)
            {
                afdComentarioLinea.SetPosicionInicio(fila, columna);
            }

            if (automataActual == null)
            {
                columna++;
                continue;
            }

            lexemaActual = c.ToString();
            automataActual.Procesar(c);
            columna++;
            continue;
        }

        // Procesar con el autómata actual
        automataActual.Procesar(c);
        lexemaActual += c;

        // Manejo especial para identificadores
        if (automataActual is AFDIdentificador)
        {
            bool esFinIdentificador = (i + 1 >= texto.Length) || !EsCaracterIdentificador(texto[i + 1]);
            
            if (esFinIdentificador)
            {
                if (automataActual.EsValido())
                {
                    tokens.Add(new TokenInfo
                    {
                        Tipo = automataActual.TipoToken,
                        Lexema = lexemaActual,
                        Fila = inicioFila,
                        Columna = inicioColumna
                    });
                }
                automataActual = null;
                lexemaActual = "";
                inicioColumna = columna + 1;
            }
            columna++;
            continue;
        }

        // Comentario de bloque
        if (automataActual is AFDComentarioBloque bloque)
        {
            if (bloque.EsValido())
            {
                tokens.Add(bloque.GetTokenInfo());
                automataActual = null;
                lexemaActual = "";
            }
            else if (bloque.TieneError())
            {
                automataActual = null;
                lexemaActual = "";
            }
            columna++;
            continue;
        }

        if (automataActual is AFDComentarioLinea)
        {
            columna++;
            continue;
        }

        // Para todos los demás autómatas
        if (automataActual.EsValido())
        {
            tokens.Add(new TokenInfo
            {
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
            automataActual = null;
            lexemaActual = "";
            inicioColumna = columna + 1;
        }

        columna++;
    }

    // Capturar último token si quedó pendiente
    if (automataActual != null && automataActual.EsValido())
    {
        tokens.Add(new TokenInfo
        {
            Tipo = automataActual.TipoToken,
            Lexema = lexemaActual,
            Fila = inicioFila,
            Columna = inicioColumna
        });
    }

    ReporteTokensGenerado?.Invoke(tokens);
    return tokens;
}
    private IAutomata SeleccionarAutomata(char c, int i, string texto)
    {
        if (c == '$') return afdIdentificador;
        if (EsDigito(c)) return afdNumerico;
        if (c == '"' || c == '\'') return afdLiteral;
        if (EsSignoPuntuacion(c)) return afdSignoPuntuacion;
        if (EsOperadorAritmetico(c)) return afdOperadorAritmetico;
        if (EsLetra(c)) return afdPalabraReservada;
        if (c == '>' || c == '<') return afdOperadorRelacional;
        if (c == '&' || c == '|' || ToLower(c) == 'a' || ToLower(c) == 'o') return afdOperadorLogico;
        if (c == '=') return afdOperadorAsignacion;
        if (EsSignoAgrupacion(c)) return afdSignoAgrupacion;
        if (c == '#') return afdComentarioLinea;

        if (c == '/' && i + 1 < texto.Length && texto[i + 1] == '*') return afdComentarioBloque;

        return null;
    }
    
    private bool EsCaracterIdentificador(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_' || c == '-';
    }

    private bool EsDigito(char c)
    {
        return c >= '0' && c <= '9';
    }

    private bool EsLetra(char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    }

    private bool EsSignoPuntuacion(char c)
    {
        char[] signos = { '.', ',', ';', ':' };
        for (int i = 0; i < signos.Length; i++)
        {
            if (c == signos[i]) return true;
        }
        return false;
    }

    private bool EsOperadorAritmetico(char c)
    {
        char[] ops = { '^', '*', '/', '+', '-' };
        for (int i = 0; i < ops.Length; i++)
        {
            if (c == ops[i]) return true;
        }
        return false;
    }

    private bool EsSignoAgrupacion(char c)
    {
        char[] signos = { '(', ')', '[', ']', '{', '}' };
        for (int i = 0; i < signos.Length; i++)
        {
            if (c == signos[i]) return true;
        }
        return false;
    }

    private char ToLower(char c)
    {
        if (c >= 'A' && c <= 'Z')
            return (char)(c + 32);
        return c;
    }
}
