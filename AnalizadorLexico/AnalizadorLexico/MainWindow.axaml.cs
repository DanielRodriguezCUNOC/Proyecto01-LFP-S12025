using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AnalizadorLexico.util;
using Avalonia;
using Avalonia.Threading;

namespace AnalizadorLexico
{
    public partial class MainWindow : Window
    {
        private AFDCompleto afdCompleto;
        
        private string? currentFilePath = null;
        private bool hasUnsavedChanges = false;
        public bool HasUnsavedChanges
        {
            get { return hasUnsavedChanges; }
            set { hasUnsavedChanges = value; }
        }


        public MainWindow()
        {
            afdCompleto = new AFDCompleto();
    
            // Suscribirse al evento ReporteTokensGenerado
            afdCompleto.ReporteTokensGenerado += MostrarReporteTokens; // MostrarReporteTokens es el manejador del evento

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
        private async void OnAnalyzeClick(object sender, RoutedEventArgs e)
        {
            string texto = GetTextoDelAreaDeTexto();
            try
            {
                // Ejecutar directamente en el hilo UI
                afdCompleto.AnalizarTexto(texto);
            }
            catch(Exception ex)
            {
                var messageBox = new MessageBox("Error", $"Ocurrió un error al analizar el texto: {ex.Message}");
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                _ = messageBox.ShowDialog<bool?>(this);
            }
        }

        // Método para manejar el click del botón "Subir Archivo"
        private async void OnUploadClick(object sender, RoutedEventArgs e)
        {
            // Si hay texto ya en el editor, preguntar si quiere sobrescribirlo
            if (!string.IsNullOrEmpty(Editor.Text))
            {
                var confirmDialog = new MessageBox("Confirmar", message: "¿Desea guardar los cambios antes de abrir un nuevo archivo?");
                confirmDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                await confirmDialog.ShowDialog<bool?>(this);

                if (confirmDialog.Result == MessageBox.MessageBoxResult.Cancel)
                    return; // Cancelar acción

                if (confirmDialog.Result == MessageBox.MessageBoxResult.Yes)
                {
                    // Aquí puedes agregar lógica para guardar el archivo actual
                    // Ejemplo: await GuardarArchivo(Editor.Text);
                }
                else if (confirmDialog.Result == MessageBox.MessageBoxResult.No)
                {
                    //Guardar sin cambios
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
        
        private async void OnSaveAsClick(object sender, RoutedEventArgs e)
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
        
        public async void OnNewClick(object sender, RoutedEventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = await ShowConfirmationDialog("¿Desea guardar los cambios antes de crear un nuevo archivo?");

                if (result == true) // Usuario eligió guardar
                {
                    await OnSaveAsync();
                }
            }

            // Limpiar el editor y resetear todo
            Editor.Text = "";
            currentFilePath = null;
            hasUnsavedChanges = false;
        }
        
        private async Task OnSaveAsync()
        {
            string content = Editor.Text;

            if (!string.IsNullOrEmpty(currentFilePath))
            {
                await System.IO.File.WriteAllTextAsync(currentFilePath, content);
            }
            else
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
                    await System.IO.File.WriteAllTextAsync(path, content);
                    currentFilePath = path;
                }
            }

            hasUnsavedChanges = false;
        }
        
        private async Task<bool?> ShowConfirmationDialog(string message)
        {
            var dialog = new MessageBox("Confirmación", message);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return await dialog.ShowDialog<bool?>(this);
        }
        
        // Este método se encuentra en MainWindow o donde desees mostrar el reporte.
        private void MostrarReporteTokens(List<TokenInfo> tokens)
        {
            if (tokens.Count == 0)
            {
                MessageBox messageBox = new MessageBox("Error", "No se encontraron tokens.");
                messageBox.ShowDialog(this);
                return;
            }

            // Versión simple con texto plano
            var reporte = new StringBuilder();
            reporte.AppendLine("TOKENS ENCONTRADOS:");
            reporte.AppendLine("------------------");
    
            foreach (var token in tokens)
            {
                reporte.AppendLine($"Tipo: {token.Tipo}");
                reporte.AppendLine($"Lexema: {token.Lexema}");
                reporte.AppendLine($"Posición: Fila {token.Fila}, Columna {token.Columna}");
                reporte.AppendLine();
            }

            TextBlockReporte.Text = reporte.ToString();
        }
        
        // En MainWindow, método para obtener el texto desde el área de texto
        private string GetTextoDelAreaDeTexto()
        {
            // Suponiendo que tienes un TextBox llamado 'TextBoxEntrada' en tu XAML
            return Editor.Text; // Obtén el texto del área de texto
        }



      


    }
}