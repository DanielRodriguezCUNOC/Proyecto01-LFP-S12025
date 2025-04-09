namespace AnalizadorLexico.util;

public class TokenInfo
{
    // Tipo de token (Identificador, Palabra Reservada, Literal, etc.)
    public string Tipo { get; set; }

    // Texto exacto del token encontrado
    public string Lexema { get; set; }

    // Número de línea/fila donde empieza el token (base 1)
    public int Fila { get; set; }

    // Número de columna donde empieza el token (base 1)
    public int Columna { get; set; }

    // Opcional: Valor interpretado (para números, literales, etc.)
    public object Valor { get; set; }

    // Constructor completo
    public TokenInfo(string tipo, string lexema, int fila, int columna, object valor = null)
    {
        Tipo = tipo;
        Lexema = lexema;
        Fila = fila;
        Columna = columna;
        Valor = valor;
    }

    // Constructor mínimo
    public TokenInfo() { }

    // Para facilitar la visualización en consola
    public override string ToString()
    {
        return $"{Tipo,-20} {Lexema,-15} {Fila,5} {Columna,5}";
    }
}