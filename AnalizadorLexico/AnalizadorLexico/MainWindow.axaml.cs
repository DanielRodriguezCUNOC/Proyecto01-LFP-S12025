using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using AnalizadorLexico.util;
using Avalonia;

namespace AnalizadorLexico
{
    public partial class MainWindow : Window
    {
        //private TextBox Editor;
        //private TextBlock LineNumbers;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.Editor.GetObservable(TextBox.TextProperty).Subscribe(new TextObserver(this));
            // Para actualizar el número de líneas al inicio
            UpdateLineNumbers(this.Editor.Text);
        }

        public void UpdateLineNumbers(string text)
        {
            if (text is null) return;

            int lines = text.Split('\n').Length;
            this.LineNumbers.Text = string.Join('\n', System.Linq.Enumerable.Range(1, lines));
        }
        
        // Método para manejar el click del botón "Analizar"
        private void OnAnalyzeClick(object sender, RoutedEventArgs e)
        {
            // Aquí puedes implementar la lógica de análisis
            Console.WriteLine("Analizando el contenido...");
        }

        // Método para manejar el click del botón "Subir Archivo"
        private void OnUploadClick(object sender, RoutedEventArgs e)
        {
            //Obtener el texto
            string text = this.Editor.Text;
            
            // Enviamos el texto al automata
            AFDCompleto afd = new AFDCompleto();
            afd.AnalizarTexto(text);
        }

        // Método para manejar el click del botón "Cerrar"
        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            // Cierra completamente la aplicación
            this.Close();
        }

    }
}