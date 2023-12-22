using OptoVIP.ADO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
    /// Interaction logic for AddNewClientPage.xaml
    /// </summary>
    public partial class AddNewClientPage : Page
    {
        private static Random random = new Random();

        User newCLient = new User();
        Authorization authorization = new Authorization();

        public AddNewClientPage()
        {
            InitializeComponent();

            DataContext = newCLient;

            newCLient.idRole = 2;

            UserNameTextBlock.Text = App.UserLogin;

        }

        private void EndOperationsButtonClick(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text.Length > 50)
            {
                MessageBox.Show("Логин должен быть не длинее 50 символов.", "Слишком длинный логин");
                return;
            }

            if (PasswordTextBox.Text.Length > 50)
            {
                MessageBox.Show("Пароль должен быть не длинее 50 символов.", "Слишком длинный пароль");
                return;
            }

            if (String.IsNullOrEmpty(LoginTextBox.Text))
            {
                MessageBox.Show("Поле логин должно быть заполнено!", "Заполните поле");
                return;
            }


            if (String.IsNullOrEmpty(PasswordTextBox.Text))
            {
                MessageBox.Show("Поле пароль должно быть заполнено!", "Заполните поле");
                return;
            }

            try
            {
                authorization.login = LoginTextBox.Text;
                authorization.password = PasswordTextBox.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло не так..");
                return;
            }

            if (App.Connection.Authorization.Where(z => z.login.Equals(authorization.login)).FirstOrDefault() != null)
            {
                MessageBox.Show("Такой логин уже сущесвует!");
                return;
            }

            try
            {
                App.Connection.Authorization.AddOrUpdate(authorization);
                App.Connection.SaveChanges();

                newCLient.idAuthorization = App.Connection.Authorization.Where(z => z.login.Equals(authorization.login)).FirstOrDefault().idAuthorization;

                App.Connection.User.AddOrUpdate(newCLient);
                App.Connection.SaveChanges();
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло не так..");
                return;
            }

            MessageBox.Show("Данные успешно сохранены!");

            NavigationService.Navigate(new CountriesPage());
        }
        private void NameHyperlinkClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserPage());
        }

        private void LogoButtonClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Новые данные будут удалены.", "Вы точно хотите выйти?", MessageBoxButton.OKCancel).Equals(MessageBoxResult.OK))
                NavigationService.Navigate(new MainPage());
        }

        private void LoginGenerateButtonCLick(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = StringGenerate();
        }

        private string StringGenerate()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, random.Next(10, 50))
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void PasswordGenerateButtonCLick(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = StringGenerate();
        }
    }
}
