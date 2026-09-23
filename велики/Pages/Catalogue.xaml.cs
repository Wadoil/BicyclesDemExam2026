using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using велики.Models;

namespace велики.Pages
{
    public partial class Catalogue : Page
    {
        public static string GetImagePath(string image)
        {
            if (string.IsNullOrWhiteSpace(image))
                image = "picture.png";

            return "C:\\college\\bicycles\\BicyclesDemoExam2026-master\\велики\\Assets\\"+image;
        }

        public class ProductData
        {
            public string CategoryName { get; set; }
            public string description { get; set; }
            public string producer { get; set; }
            public string supplier { get; set; }
            public decimal cost { get; set; }
            public string measurement { get; set; }
            public long amount { get; set; }
            public string discount { get; set; }
            public string image { get; set; }

            public ProductData(
                string v1,
                string description,
                string Producer,
                string Supplier,
                decimal cost,
                string measurement,
                long amount,
                long discount,
                string v2)
            {
                this.CategoryName = v1;
                this.description = description;
                this.producer = Producer;
                this.supplier = Supplier;
                this.cost = cost;
                this.measurement = measurement;
                this.amount = amount;
                this.discount = discount.ToString() + '%';
                this.image = v2;
            }
        }

        public Catalogue(Models.Auth auth)
        {
            InitializeComponent();
            LoadData();

            using (var context = new BicyclesContext())
            {
                var user = auth != null
                    ? context.Users.FirstOrDefault(x => x.AuthId == auth.Id)
                    : null;

                // 17 = Авторизированный клиент.
                // Админ и менеджер видят кнопку Add.
                if (user != null && user.RoleId != 17)
                    AddBtn.Visibility = Visibility.Visible;
            }
        }

        public void LoadData()
        {
            using (var context = new BicyclesContext())
            {
                ProductsList.ItemsSource = context.Products
                    .Select(x => new ProductData(
                        x.Category.Name + " | " + x.Name,
                        x.Description,
                        x.Producer.Name,
                        x.Supplier.Name,
                        x.Cost,
                        x.Measurement.Name,
                        x.Amount,
                        x.Discount,
                        GetImagePath(x.Image)))
                    .ToList();
            }
        }

        private void Edit(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;

            // 17 = клиент. Гость — session == null.
            // Админ и менеджер могут редактировать.
            if (mainWindow.session != null && mainWindow.session.RoleId != 17)
            {
                mainWindow.TitleBlock.Text = "Edit";
                mainWindow.MainFrame.Navigate(
                    new EditPage((sender as FrameworkElement)?.DataContext as ProductData));
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.TitleBlock.Text = "Edit";
            mainWindow.MainFrame.Navigate(new EditPage());
        }
    }
}