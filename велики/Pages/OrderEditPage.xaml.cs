using Microsoft.EntityFrameworkCore;
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
using static велики.Pages.Orders;

namespace велики.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrderEditPage.xaml
    /// </summary>
    public partial class OrderEditPage : Page
    {
        Order OgOrder;
        OrderData Order;
        private bool _isEdit;
        List<String> Products;
        public class OrderItemRow
        {
            public long ProductId { get; set; }
            public string ProductName { get; set; }
            public long Amount { get; set; }
        }
        private readonly List<OrderItemRow> _items = new List<OrderItemRow>();
        public OrderEditPage()
        {
            InitializeComponent();

            _isEdit = false;
            Order = new OrderData();
            using (var context = new BicyclesContext())
            {
                Products = context.Products.Select(p => p.Name).ToList();
                StatusBox.ItemsSource = context.Statuses.Select(c => c.Name).ToList();
                PickupBox.ItemsSource = context.PickupPoints.Select(c => c.Address).ToList();
            }

            ProductBox.ItemsSource = Products;
            OrdersGrid.ItemsSource = _items;
        }

        public OrderEditPage(OrderData orderData)
        {
            InitializeComponent();

            _isEdit = true;

            using (var context = new BicyclesContext())
            {
                OgOrder = context.Orders.FirstOrDefault(o => o.Code == orderData.Article);

                Products = context.Products.Select(p => p.Name).ToList();
                var statuses = context.Statuses.ToList();
                var pickups = context.PickupPoints.ToList();

                StatusBox.ItemsSource = statuses.Select(s => s.Name).ToList();
                PickupBox.ItemsSource = pickups.Select(p => p.Address).ToList();

                if (OgOrder != null)
                {
                    CodeBox.Text = OgOrder.Code;
                    OrderBox.SelectedDate = OgOrder.DateOfOrdering.ToDateTime(TimeOnly.MinValue);
                    DeliveryBox.SelectedDate = OgOrder.DateOfDelivery.ToDateTime(TimeOnly.MinValue);
                    NameBox.Text = OgOrder.Name;
                    SurnameBox.Text = OgOrder.Surname;
                    MiddleNameBox.Text = OgOrder.MiddleName;

                    var status = statuses.FirstOrDefault(s => s.Id == OgOrder.StatusId);
                    if (status != null) StatusBox.SelectedItem = status.Name;

                    var pickup = pickups.FirstOrDefault(p => p.Id == OgOrder.PickupPointId);
                    if (pickup != null) PickupBox.SelectedItem = pickup.Address;

                    var productList = context.Products.ToList();
                    foreach (var oc in context.OrderContents.Where(x => x.OrderId == OgOrder.Id))
                    {
                        _items.Add(new OrderItemRow
                        {
                            ProductId = oc.ProductId,
                            ProductName = productList.FirstOrDefault(x => x.Id == oc.ProductId).Name,
                            Amount = oc.Amount
                        });
                    }
                }
            }

            ProductBox.ItemsSource = Products;
            OrdersGrid.ItemsSource = _items;
        }

        private void DeleteContentBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrdersGrid.SelectedItem as OrderItemRow;
            if (selected == null)
            {
                MessageBox.Show("Выберите строку для удаления!");
                return;
            }

            _items.Remove(selected);
            RefreshGrid();
        }
        private void AddContentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProductBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар!");
                return;
            }

            if (!long.TryParse(AmountBox.Text, out long amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректное количество!");
                return;
            }

            var productName = (string)ProductBox.SelectedItem;

            long productId;
            using (var context = new BicyclesContext())
            {
                productId = context.Products
                    .Where(p => p.Name == productName)
                    .Select(p => p.Id)
                    .FirstOrDefault();
            }

            var existing = _items.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
            {
                existing.Amount += amount;
            }
            else
            {
                _items.Add(new OrderItemRow
                {
                    ProductId = productId,
                    ProductName = productName,
                    Amount = amount
                });
            }

            RefreshGrid();
            AmountBox.Clear();
        }
        private void RefreshGrid()
        {
            OrdersGrid.ItemsSource = null;
            OrdersGrid.ItemsSource = _items;
        }
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_isEdit || OgOrder == null)
            {
                MessageBox.Show("Невозможно удалить несуществующий заказ!");
                return;
            }

            if (MessageBox.Show("Удалить заказ?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            using (var context = new BicyclesContext())
            {
                var items = context.OrderContents.Where(x => x.OrderId == OgOrder.Id);
                context.OrderContents.RemoveRange(items);

                var order = context.Orders.FirstOrDefault(o => o.Id == OgOrder.Id);
                if (order != null) context.Orders.Remove(order);

                context.SaveChanges();
            }

            var MainWindow = Window.GetWindow(this) as MainWindow;
            MainWindow.TitleBlock.Text = "Orders";
            MainWindow.MainFrame.Navigate(new Orders());
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            var MainWindow = Window.GetWindow(this) as MainWindow;
            MainWindow.TitleBlock.Text = "Orders";
            MainWindow.MainFrame.Navigate(new Orders());
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CodeBox.Text) ||
                StatusBox.SelectedItem == null ||
                PickupBox.SelectedItem == null ||
                OrderBox.SelectedDate == null ||
                DeliveryBox.SelectedDate == null ||
                string.IsNullOrWhiteSpace(SurnameBox.Text) ||
                string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!");
                return;
            }

            if (OrderBox.SelectedDate > DeliveryBox.SelectedDate)
            {
                MessageBox.Show("Дата выдачи не может быть раньше даты заказа!");
                return;
            }

            using (var context = new BicyclesContext())
            {
                Order orderEntity;

                if (_isEdit)
                {
                    orderEntity = context.Orders.FirstOrDefault(o => o.Id == OgOrder.Id);
                    if (orderEntity == null)
                    {
                        MessageBox.Show("Заказ не найден!");
                        return;
                    }

                    var oldItems = context.OrderContents.Where(x => x.OrderId == orderEntity.Id);
                    context.OrderContents.RemoveRange(oldItems);
                    context.Attach(orderEntity);
                }
                else
                {
                    orderEntity = new Order();
                    context.Orders.Add(orderEntity);
                }

                orderEntity.Code = CodeBox.Text;
                orderEntity.DateOfOrdering = DateOnly.FromDateTime(OrderBox.SelectedDate.Value);
                orderEntity.DateOfDelivery = DateOnly.FromDateTime(DeliveryBox.SelectedDate.Value);
                orderEntity.Name = NameBox.Text.Trim();
                orderEntity.Surname = SurnameBox.Text.Trim();
                orderEntity.MiddleName = string.IsNullOrWhiteSpace(MiddleNameBox.Text)
                    ? null
                    : MiddleNameBox.Text.Trim();

                var statusName = (string)StatusBox.SelectedItem;
                var status = context.Statuses.FirstOrDefault(s => s.Name == statusName);
                if (status != null) orderEntity.StatusId = status.Id;

                var pickupAddress = (string)PickupBox.SelectedItem;
                var pickup = context.PickupPoints.FirstOrDefault(p => p.Address == pickupAddress);
                if (pickup != null) orderEntity.PickupPointId = pickup.Id;

                context.SaveChanges();

                foreach (var row in _items)
                {
                    context.OrderContents.Add(new OrderContent
                    {
                        OrderId = orderEntity.Id,
                        ProductId = row.ProductId,
                        Amount = row.Amount
                    });
                }
                context.SaveChanges();
            }

            var MainWindow = Window.GetWindow(this) as MainWindow;
            MainWindow.TitleBlock.Text = "Orders";
            MainWindow.MainFrame.Navigate(new Orders());
        }
    }
}
