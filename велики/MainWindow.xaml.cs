using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using велики.Models;
using велики.Pages;

namespace велики
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User session = null;
        public MainWindow()
        {
            InitializeComponent();

            TitleBlock.Text = "Authorization";
            MainFrame.Navigate(new Pages.Auth());
        }
        
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            UserTextBlock.Text = "";
            TitleBlock.Text = "Authorization";
            ExitBtn.Visibility = Visibility.Hidden;
            MainFrame.Navigate(new Pages.Auth());
        }
        private void OrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            TitleBlock.Text = "Orders";
            MainFrame.Navigate(new Orders());
        }
    }
}