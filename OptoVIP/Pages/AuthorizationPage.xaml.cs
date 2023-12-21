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

namespace OptoVIP.Pages
{
    /// <summary>
    /// Interaction logic for AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void InviteHyperlinkClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InvitePage());
        }

        private void AuthorizationButtonClick(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;

            var authorization = App.Connection.Authorization.Where(z => z.login.Equals(login) && z.password.Equals(password)).FirstOrDefault();
            if (authorization != null)
            {
                App.CurrentUser = authorization.User.FirstOrDefault();
                App.UserLogin = authorization.login;

                NavigationService.Navigate(new Pages.MainPage());
            }
            else
            {
                MessageBox.Show("Неправильные данные");
            }
        }
    }
}
