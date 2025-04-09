namespace AnalizadorLexico.util;

public interface IAutomata
{
    void Procesar(char c);
    bool EsValido();
    bool TieneError();
    void Reiniciar();
    string TipoToken { get; }
}