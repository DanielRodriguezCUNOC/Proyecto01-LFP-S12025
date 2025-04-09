using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using AnalizadorLexico.util;
using Avalonia;

namespace AnalizadorLexico
{
    public partial class MainWindow : Window
    {
        private string? currentFilePath = null;


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
        private async void OnUploadClick(object sender, RoutedEventArgs e)
        {
            // Si hay texto ya en el editor, preguntar si quiere sobrescribirlo
            if (!string.IsNullOrEmpty(Editor.Text))
            {
                var confirmDialog = await MessageBox.Show(this,
                    "Hay cambios en el editor. ¿Deseas guardar antes de abrir otro archivo?",
                    "Confirmar");

                if (confirmDialog == MessageBox.MessageBoxResult.Cancel)
                    return; // Cancelar acción

                if (confirmDialog == MessageBox.MessageBoxResult.Yes)
                {
                    // Aquí puedes agregar lógica para guardar el archivo actual
                    // Ejemplo: await GuardarArchivo(Editor.Text);
                }
            }

            // Mostrar diálogo para seleccionar archivo
            var openFileDialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Title = "Selecciona un archivo de texto",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Archivos de texto", Extensions = { "txt" } }
                }
            };

            string[]? result = await openFileDialog.ShowAsync(this);

            if (result != null && result.Length > 0)
            {
                string path = result[0];
                string fileContent = await System.IO.File.ReadAllTextAsync(path);

                Editor.Text = fileContent;
                UpdateLineNumbers(fileContent);
                currentFilePath = path;
            }
        }

        
        private async void OnSaveClick(object sender, RoutedEventArgs e)
        {
            string content = Editor.Text;

            if (!string.IsNullOrEmpty(currentFilePath))
            {
                // Guardar directamente en el archivo actual
                await System.IO.File.WriteAllTextAsync(currentFilePath, content);
            }
            else
            {
                // Mostrar diálogo para guardar archivo nuevo
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Guardar como...",
                    Filters = new List<FileDialogFilter>
                    {
                        new FileDialogFilter { Name = "Archivos de texto", Extensions = { "txt" } }
                    }
                };

                string? result = await saveFileDialog.ShowAsync(this);

                if (!string.IsNullOrEmpty(result))
                {
                    await System.IO.File.WriteAllTextAsync(result, content);
                    currentFilePath = result; // guardar ruta nueva
                }
            }
        }
        
        public async void OnSaveAsClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Guardar como...",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Archivos de texto", Extensions = { "txt" } }
                }
            };

            string? path = await saveFileDialog.ShowAsync(this);

            if (!string.IsNullOrEmpty(path))
            {
                await System.IO.File.WriteAllTextAsync(path, Editor.Text);
                currentFilePath = path; // actualiza el archivo actual a este nuevo
            }
        }

        // Método para manejar el click del botón "Cerrar"
        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            // Cierra completamente la aplicación
            this.Close();
        }

    }
}