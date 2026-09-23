using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
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
    /// Логика взаимодействия для EditPage.xaml
    /// </summary>
    public partial class EditPage : Page
    {
        public class ProductData
        {
            public string Article {  get; set; }
            public string Category {  get; set; }
            public string Name {  get; set; }
            public string description {  get; set; }
            public string Producer {  get; set; }
            public string Supplier {  get; set; }
            public decimal cost {  get; set; }
            public string Measurement {  get; set; }
            public long amount {  get; set; }
            public long discount {  get; set; }
            public string image {  get; set; }
            public ProductData() { }
            public ProductData(string Article, string category, string name, string description, string producer, string supplier, decimal cost, string name5, long amount, long discount, string image)
            {
                this.Article = Article;
                this.Category = category;
                this.Name = name;
                this.description = description;
                this.Producer = producer;
                this.Supplier = supplier;
                this.cost = cost;
                this.Measurement = name5;
                this.amount = amount;
                this.discount = discount;
                this.image = image;
            }
        }
        Product OgProduct;
        ProductData Product;
        private bool _isEdit;

        public EditPage()
        {
            InitializeComponent();
            _isEdit = false;
            Product = new ProductData();
            using (var context = new BicyclesContext())
            {
                CategoryBox.ItemsSource = context.Categories.Select(c => c.Name).ToList();
                ProducerBox.ItemsSource = context.Producers.Select(c => c.Name).ToList();
                SupplierBox.ItemsSource = context.Suppliers.Select(c => c.Name).ToList();
                MeasurementBox.ItemsSource = context.Measurements.Select(c => c.Name).ToList();
            }
        }
        public EditPage(Catalogue.ProductData product)
        {
            InitializeComponent();
            _isEdit = true;
            Product = LoadData(product);
            using (var context = new BicyclesContext())
            {
                CategoryBox.ItemsSource = context.Categories.Select(c => c.Name).ToList();
                ProducerBox.ItemsSource = context.Producers.Select(c => c.Name).ToList();
                SupplierBox.ItemsSource = context.Suppliers.Select(c => c.Name).ToList();
                MeasurementBox.ItemsSource = context.Measurements.Select(c => c.Name).ToList();
            }
            ArticleBox.Text = Product.Article;
            CategoryBox.SelectedValue = Product.Category;
            ProducerBox.SelectedValue = Product.Producer;
            SupplierBox.SelectedValue = Product.Supplier;
            MeasurementBox.SelectedValue = Product.Measurement;

            NameBox.Text = Product.Name;
            DescriptionBox.Text = Product.description;

            BitmapImage image = new BitmapImage();
            image.BeginInit();

            if (!string.IsNullOrWhiteSpace(Product.image) && File.Exists(Product.image))
                image.UriSource = new Uri(Product.image);
            else
                image.UriSource = new Uri("C:\\college\\bicycles\\BicyclesDemoExam2026-master\\велики\\Assets\\picture.png");

            image.EndInit();
            ImageBox.Source = image;

            ImageBox.Source = image;
            CostBox.Text = Product.cost.ToString();
            AmountBox.Text = Product.amount.ToString();
            DiscountBox.Text = Product.discount.ToString();
        }

        public ProductData LoadData(Catalogue.ProductData product)
        {
            using (var context = new BicyclesContext())
            {
                OgProduct = context.Products.Where(x => x.Description == product.description).Include(y => y.Category).Include(y => y.Producer).Include(y => y.Measurement).Include(y => y.Supplier).FirstOrDefault(z => z.Description == product.description);
                return new ProductData(
                    OgProduct.Article,
                    OgProduct.Category.Name,
                    OgProduct.Name,
                    OgProduct.Description,
                    OgProduct.Producer.Name,
                    OgProduct.Supplier.Name,
                    OgProduct.Cost,
                    OgProduct.Measurement.Name,
                    OgProduct.Amount,
                    OgProduct.Discount,
                    Catalogue.GetImagePath(OgProduct.Image));
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            var MainWindow = Window.GetWindow(this) as MainWindow;
            MainWindow.TitleBlock.Text = "Catalogue";
            using (var context = new BicyclesContext())
            {
                MainWindow.MainFrame.Navigate(new Catalogue(context.Auths.FirstOrDefault(x => x.Id == MainWindow.session.AuthId)));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                                            string.IsNullOrWhiteSpace(ArticleBox.Text) ||
                                            CategoryBox.SelectedItem == null ||
                                            ProducerBox.SelectedItem == null ||
                                            SupplierBox.SelectedItem == null ||
                                            MeasurementBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните обязательные поля.");
                return;
            }

            if (!decimal.TryParse(CostBox.Text, out decimal cost) ||
                !long.TryParse(AmountBox.Text, out long amount) ||
                !long.TryParse(DiscountBox.Text, out long discount))
            {
                MessageBox.Show("Проверьте числовые поля: цена, количество, скидка.");
                return;
            }

            using (var context = new BicyclesContext())
            {
                if (!_isEdit)
                {
                    OgProduct = new Product();
                    context.Products.Add(OgProduct);
                }
                else
                    context.Attach(OgProduct);

                var category = context.Categories
                    .FirstOrDefault(c => c.Name == (string)CategoryBox.SelectedItem);

                var producer = context.Producers
                    .FirstOrDefault(p => p.Name == (string)ProducerBox.SelectedItem);

                var supplier = context.Suppliers
                    .FirstOrDefault(s => s.Name == (string)SupplierBox.SelectedItem);

                var measurement = context.Measurements
                    .FirstOrDefault(m => m.Name == (string)MeasurementBox.SelectedItem);

                if (category == null || producer == null || supplier == null || measurement == null)
                {
                    MessageBox.Show("Не удалось найти выбранные справочные значения.");
                    return;
                }

                OgProduct.Article = ArticleBox.Text;
                OgProduct.Name = NameBox.Text;
                OgProduct.Description = DescriptionBox.Text;
                OgProduct.Category = category;
                OgProduct.Producer = producer;
                OgProduct.Supplier = supplier;
                OgProduct.Measurement = measurement;
                OgProduct.Cost = cost;
                OgProduct.Amount = amount;
                OgProduct.Discount = discount;

                if (!string.IsNullOrWhiteSpace(Product.image) && File.Exists(Product.image))
                {
                    OgProduct.Image = System.IO.Path.GetFileName(Product.image);
                }

                context.SaveChanges();

                var MainWindow = Window.GetWindow(this) as MainWindow;
                MainWindow.TitleBlock.Text = "Catalogue";
                MainWindow.MainFrame.Navigate(new Catalogue(context.Auths.FirstOrDefault(x => x.Id == MainWindow.session.AuthId)));
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isEdit != true)
            {
                MessageBox.Show("Невозможно удалить несуществующий объект!");
            }
            else
            {
                using (var context = new BicyclesContext())
                {
                    context.Products.Remove(OgProduct);
                    context.SaveChanges();

                    var MainWindow = Window.GetWindow(this) as MainWindow;
                    MainWindow.TitleBlock.Text = "Catalogue";
                    MainWindow.MainFrame.Navigate(new Catalogue(context.Auths.FirstOrDefault(x => x.Id == MainWindow.session.AuthId)));
                }
            }
        }
    }
}
