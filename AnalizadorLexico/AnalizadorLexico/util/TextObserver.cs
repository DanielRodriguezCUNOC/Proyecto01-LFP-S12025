using System;

namespace AnalizadorLexico.util;

public class TextObserver : IObserver<string>
{
    private readonly MainWindow _window;

    public TextObserver(MainWindow window)
    {
        _window = window;
    }

    public void OnNext(string value)
    {
        _window.UpdateLineNumbers(value);
    }

    public void OnError(Exception error)
    {
        // Manejar errores si es necesario
    }

    public void OnCompleted()
    {
        // Opcional: manejar la finalización de la observación
    }
}