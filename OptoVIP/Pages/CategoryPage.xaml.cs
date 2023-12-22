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
    /// Interaction logic for CategoryPage.xaml
    /// </summary>
    public partial class CategoryPage : Page
    {
        ProductCategory category = new ProductCategory();

        public CategoryPage()
        {
            InitializeComponent();

            DataContext = category;

            UserNameTextBlock.Text = App.UserLogin;

            DeleteButton.IsEnabled = false;
            DeleteButton.Visibility = Visibility.Hidden;
        }

        public CategoryPage(ProductCategory category)
        {
            InitializeComponent();

            this.category = category;

            DataContext = this.category;

            TitleTextBox.Text = this.category.title;

            PageTitleTextBLock.Text = "Изменить категорию";
            EndOperationsButton.Content = "Изменить";

            DeleteButton.IsEnabled = true;
            DeleteButton.Visibility = Visibility.Visible;

            UserNameTextBlock.Text = App.UserLogin;
        }

        private void EndOperationsButtonClick(object sender, RoutedEventArgs e)
        {
            if (TitleTextBox.Text.Length > 150)
            {
                MessageBox.Show("Название должно быть не длинее 150 символов.", "Слишком длинное название");
                return;
            }

            if (String.IsNullOrEmpty(TitleTextBox.Text))
            {
                MessageBox.Show("Название должно быть заполнено!", "Заполните поле");
                return;
            }

            try
            {
                category.title = TitleTextBox.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло не так..");
                return;
            }

            if (App.Connection.Country.Where(z => z.title.Equals(category.title)).FirstOrDefault() != null)
            {
                MessageBox.Show("Такая категория уже сущесвует!");
                return;
            }

            App.Connection.ProductCategory.AddOrUpdate(category);

            App.Connection.SaveChanges();

            MessageBox.Show("Данные успешно сохранены!");

            NavigationService.Navigate(new CategoriesPage());
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить этот объект?", "Подтвержение операции", MessageBoxButton.OKCancel).Equals(MessageBoxResult.OK))
            {
                try
                {
                    App.Connection.ProductCategory.Remove(category);
                    App.Connection.SaveChanges();
                }
                catch(Exception)
                {
                    MessageBox.Show("Что-то пошло не так..");
                    return;
                }

                MessageBox.Show("Данные удалены!");
                NavigationService.Navigate(new CategoriesPage());
            }
        }
    }
}
