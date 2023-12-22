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
    /// Interaction logic for CountriesPage.xaml
    /// </summary>
    public partial class CountriesPage : Page
    {
        List<Country> countries;

        public CountriesPage()
        {
            InitializeComponent();

            UserNameTextBlock.Text = App.UserLogin;

            countries = App.Connection.Country.ToList();

            countries = OrderCountriesList();

            CountriesList.ItemsSource = countries;
        }

        private void AddNewManufacturerButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CountryPage());
        }

        private void LogoButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void SearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var newList = OrderCountriesList();

            if (newList != null)
            {
                CountriesList.ItemsSource = newList;
                CountriesList.Items.Refresh();
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

            var newList = OrderCountriesList();

            if (newList != null)
            {
                CountriesList.ItemsSource = newList;
                CountriesList.Items.Refresh();
            }
        }

        private List<Country> OrderCountriesList()
        {
            var list = new List<Country>();

            list = countries;

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
            if (CountriesList.SelectedItem == null)
                return;

            NavigationService.Navigate(new CountryPage(CountriesList.SelectedItem as Country));
        }
    }
}
