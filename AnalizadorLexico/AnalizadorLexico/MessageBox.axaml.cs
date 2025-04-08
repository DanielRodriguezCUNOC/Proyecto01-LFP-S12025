using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AnalizadorLexico
{
    public partial class MessageBox : Window
    {
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

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}