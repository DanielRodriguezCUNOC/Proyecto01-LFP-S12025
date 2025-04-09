using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AnalizadorLexico
{
    public partial class MessageBox : Window
    {
        public enum MessageBoxResult { Yes, No, Cancel }

        private MessageBoxResult _result = MessageBoxResult.Cancel;

        public MessageBoxResult Result => _result;

        public MessageBox()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public MessageBox(string title, string message) : this()
        {
            this.Title = title;
            this.Width = 300;
            this.Height = 150;
            this.FindControl<TextBlock>("MessageText").Text = message;
        }

        private void OnYesClick(object? sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Yes;
            this.Close();
        }

        private void OnNoClick(object? sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.No;
            this.Close();
        }

        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Cancel;
            this.Close();
        }

        public static async Task<MessageBoxResult> Show(Window parent, string message, string title)
        {
            var box = new MessageBox(title, message);
            await box.ShowDialog(parent);
            return box.Result;
        }
        
    }

}