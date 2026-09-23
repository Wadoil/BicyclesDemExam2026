using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using велики.Models;

namespace велики.Pages
{
    public partial class Auth : Page
    {
        public Auth()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBox.Password;

            using (var context = new BicyclesContext())
            {
                Models.Auth user = context.Auths
                    .FirstOrDefault(x => x.Login == login && x.Password == password);

                if (user == null)
                {
                    PasswordBox.Password = "";
                    return;
                }

                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow.session = context.Users.FirstOrDefault(x => x.AuthId == user.Id);

                if (mainWindow.session != null)
                {
                    mainWindow.UserTextBlock.Text = $"{mainWindow.session.Surname} {mainWindow.session.Name} {mainWindow.session.MiddleName}";

                    // 11 = Администратор
                    if (mainWindow.session.RoleId == 11)
                        mainWindow.OrdersBtn.Visibility = Visibility.Visible;
                }

                ToCatalogue(user);
            }
        }

        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.UserTextBlock.Text = "Гость";
            ToCatalogue();
        }

        private void ToCatalogue(Models.Auth auth = null)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.TitleBlock.Text = "Catalogue";
            mainWindow.ExitBtn.Visibility = Visibility.Visible;
            mainWindow.MainFrame.Navigate(new Catalogue(auth));
        }
    }
}