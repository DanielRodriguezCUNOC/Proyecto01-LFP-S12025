namespace AnalizadorLexico;

public class AFDCompleto
{
    private AFDNumerico afdNumerico = new AFDNumerico();

    public string procesarCadena(string cadena)
    {
        afdNumerico.reiniciar();

        foreach ( char c in cadena)
        {
            afdNumerico.Procesar(c);
        }

        if (afdNumerico.tieneError)
            return "Cadena no valida: " + cadena;
        if (afdNumerico.esEnteroValido)
            return afdNumerico.obtenerResultado();
        if (afdNumerico.esDecimalValido)
            return afdNumerico.obtenerResultado();
        
        return afdNumerico.tieneError ? "" : afdNumerico.obtenerResultado();
    }
}