using OptoVIP.ADO;
using OptoVIP.Classes;
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
    /// Interaction logic for CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        List<ProductCategory> categories;

        public CategoriesPage()
        {
            InitializeComponent();

            UserNameTextBlock.Text = App.UserLogin;

            categories = App.Connection.ProductCategory.ToList();

            categories = OrderCategotiesList();

            CategoriesList.ItemsSource = categories;
        }

        private void AddNewManufacturerButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CategoryPage());
        }

        private void LogoButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void SearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var newList = OrderCategotiesList();

            if (newList != null)
            {
                CategoriesList.ItemsSource = newList;
                CategoriesList.Items.Refresh();
            }
        }

        private void NameHyperlinkClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserPage());
        }

        private void SortComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox == null)
                return;

            var newList = OrderCategotiesList();

            if (newList != null)
            {
                CategoriesList.ItemsSource = newList;
                CategoriesList.Items.Refresh();
            }
        }

        private List<ProductCategory> OrderCategotiesList()
        {
            var list = new List<ProductCategory>();

            list = categories;

            if (SortComboBox == null || SortComboBox.SelectedItem == null || (SortComboBox.SelectedItem as ComboBoxItem).Content == null)
                return null;

            switch ((SortComboBox.SelectedItem as ComboBoxItem).Content.ToString())
            {
                case "От А до Я":
                    list = list.OrderBy(z => z.title).ToList();
                    break;
                case "От Я до А":
                    list = list.OrderBy(z => z.title).ToList();
                    list.Reverse();
                    break;
            }

            if (SearchTextBox.Text != "")
                list = list.Where(z => z.title.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase)).ToList();

            return list;
        }



        private void ManufacturersListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoriesList.SelectedItem == null)
                return;

            NavigationService.Navigate(new CategoryPage(CategoriesList.SelectedItem as ProductCategory));
        }
    }
}
