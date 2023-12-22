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
    /// Interaction logic for NotationsPage.xaml
    /// </summary>
    public partial class NotationsPage : Page
    {
        List<Notation> notations;

        public NotationsPage()
        {
            InitializeComponent();

            UserNameTextBlock.Text = App.UserLogin;

            notations = App.Connection.Notation.ToList();

            notations = OrderCountriesList();

            NotationsList.ItemsSource = notations;
        }

        private void AddNewManufacturerButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NotationPage());
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
                NotationsList.ItemsSource = newList;
                NotationsList.Items.Refresh();
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
                NotationsList.ItemsSource = newList;
                NotationsList.Items.Refresh();
            }
        }

        private List<Notation> OrderCountriesList()
        {
            var list = new List<Notation>();

            list = notations;

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
            if (NotationsList.SelectedItem == null)
                return;

            NavigationService.Navigate(new NotationPage(NotationsList.SelectedItem as Notation));
        }
    }
}
