using Avalonia.Controls;

namespace demo
{
    public partial class MainWindow : Window
    {
        private string _code = "0000";
        public MainWindow()
        {
            InitializeComponent();
        }
  
        private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Code.Text == _code)
            {
                new ServiceAdminWindow().Show();
                this.Close();
            }
        }

        private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            new ServiceWindow().Show();
            this.Close();
        }
    }
}