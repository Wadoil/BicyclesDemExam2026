using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace велики.Pages
{
    /// <summary>
    /// Логика взаимодействия для Orders.xaml
    /// </summary>
    public partial class Orders : Page
    {
        public class OrderData
        {
            public string Article { get; set; }
            public string Status { get; set; }
            public string Address { get; set; }
            public DateOnly DateOfOrder { get; set; }
            public DateOnly DateOfDelivery { get; set; }
            public OrderData(){}
            public OrderData(string Article, string Status, string Address, DateOnly DateOfOrder, DateOnly DateOfDelivery)
            {
                this.Article = Article;
                this.Status = Status;
                this.Address = Address;
                this.DateOfOrder = DateOfOrder;
                this.DateOfDelivery = DateOfDelivery;
            }
        }
        public Orders()
        {
            InitializeComponent();

            List<OrderData> orders = new List<OrderData>();
            using (var context = new BicyclesContext()) 
            {
                OrdersList.ItemsSource = context.Orders.Select(x => new OrderData(x.Code, x.Status.Name, x.PickupPoint.Address, x.DateOfOrdering, x.DateOfDelivery)).ToList();
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var MainWindow = Window.GetWindow(this) as MainWindow;
            MainWindow.TitleBlock.Text = "Edit";
            MainWindow.MainFrame.Navigate(new OrderEditPage());
        }

        private void Edit(object sender, MouseButtonEventArgs e)
        {
            var MainWindow = Window.GetWindow(this) as MainWindow;
            MainWindow.TitleBlock.Text = "Edit";
            MainWindow.MainFrame.Navigate(new OrderEditPage((sender as FrameworkElement)?.DataContext as OrderData));
        }
    }
}
