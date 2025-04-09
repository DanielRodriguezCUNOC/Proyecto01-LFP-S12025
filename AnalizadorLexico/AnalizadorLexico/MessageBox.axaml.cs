using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AnalizadorLexico
{
    public partial class MessageBox : Window
    {
        public enum MessageBoxResult
        {
            Yes,
            No,
            Cancel
        }
        public MessageBoxResult Result { get; private set; }
        public MessageBox()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        // Constructor que recibe el título y mensaje
        public MessageBox(string title, string message) : this()
        {
            this.Title = title; // Establece el título de la ventana
            this.Width = 300;
            this.Height = 150;
            this.FindControl<TextBlock>("MessageText").Text = message; // Establece el mensaje en el TextBlock
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            this.Result = MessageBoxResult.Yes;
            this.Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Result = MessageBoxResult.Cancel;
            this.Close();
        }

        private void OnNoClick(object sender, RoutedEventArgs e)
        {
            this.Result = MessageBoxResult.No;
            this.Close();
        }
    }
}