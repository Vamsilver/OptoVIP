using Microsoft.Win32;
using OptoVIP.ADO;
using OptoVIP.Classes;
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
    /// Interaction logic for ManufacturerPage.xaml
    /// </summary>
    public partial class ManufacturerPage : Page
    {
        ViewManufacturer manufacturer = new ViewManufacturer(new Manufacturer());

        public ManufacturerPage()
        {
            InitializeComponent();

            DataContext = manufacturer;

            LocatedCountriesComboBox.ItemsSource = App.Connection.Country.ToList();
            TradedCountriesComboBox.ItemsSource = App.Connection.Country.ToList();

            DeleteButton.IsEnabled = false;
            DeleteButton.Visibility = Visibility.Hidden;

            UserNameTextBlock.Text = App.UserLogin;
        }

        public ManufacturerPage(ViewManufacturer viewManufacturer)
        {
            InitializeComponent();

            manufacturer = viewManufacturer;

            DataContext = manufacturer;

            TitleTextBox.Text = manufacturer.title;
            DesriptionTextBox.Text = manufacturer.description;
            LocatedTextBox.Text = manufacturer.GetLocatedCountriesString;
            TradedTextBox.Text = manufacturer.GetTradedCountriesString;

            if (App.CurrentUser.idRole.Equals(1))
            {
                PageTitleTextBLock.Text = "Изменить фабрику";
                EndOperationsButton.Content = "Изменить";
                DeleteButton.IsEnabled = true;
                DeleteButton.Visibility = Visibility.Visible;
            }
            else if(App.CurrentUser.idRole.Equals(2))
            {
                PageTitleTextBLock.Text = "Данные фабрики";
                EndOperationsButton.Visibility = Visibility.Hidden;

                TitleTextBox.IsEnabled = false;
                DesriptionTextBox.IsEnabled = false;
                LocatedTextBox.IsEnabled = false;
                TradedTextBox.IsEnabled = false;
                LocatedCountriesButton.IsEnabled = false;
                TradedCountriesButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                DeleteButton.Visibility = Visibility.Hidden;

                LocatedCountriesComboBox.Visibility = Visibility.Hidden;
                TradedCountriesComboBox.Visibility = Visibility.Hidden;

                LocatedCountriesButton.Visibility = Visibility.Hidden;
                TradedCountriesButton.Visibility = Visibility.Hidden;

                LocatedCoutriesTextBox.Visibility = Visibility.Hidden;
                TradedCoutriesTextBox.Visibility = Visibility.Hidden;
            }

            LocatedCountriesComboBox.ItemsSource = App.Connection.Country.ToList();
            TradedCountriesComboBox.ItemsSource = App.Connection.Country.ToList();

            UserNameTextBlock.Text = App.UserLogin;
        }

        private void EndOperationsButtonClick(object sender, RoutedEventArgs e)
        {
            if (TitleTextBox.Text.Length > 150)
            {
                MessageBox.Show("Название должно быть не длинее 150 символов.", "Слишком длинное название");
                return;
            }

            if (DesriptionTextBox.Text.Length > 500)
            {
                MessageBox.Show("Описание должно быть не длинее 500 символов.", "Слишком длинное описание");
                return;
            }

            if (String.IsNullOrEmpty(TitleTextBox.Text))
            {
                MessageBox.Show("Название должно быть заполнено!", "Заполните поле");
                return;
            }

            try
            {
                manufacturer.title = TitleTextBox.Text;
                manufacturer.description = DesriptionTextBox.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло не так..");
                return;
            }

            if (App.Connection.Manufacturer.Where(z => z.title.Equals(manufacturer.title)).FirstOrDefault() != null)
            {
                MessageBox.Show("Такой производитель уже сущесвует!");
                return;
            }

            App.Connection.Manufacturer.AddOrUpdate(Converter.ConvertToManufacturer(manufacturer));
            
            App.Connection.SaveChanges();

            var idManufacturer = App.Connection.Manufacturer.Where(z => z.title.Equals(manufacturer.title) && z.description.Equals(manufacturer.description)).FirstOrDefault().idManufacturer;

            foreach (var s in manufacturer.GetLocatedCountriesList)
            {
                if (!s.idSupply.Equals(0))
                    continue;

                Supply newSupply = new Supply();
                newSupply.idManufacturer = idManufacturer;
                newSupply.idCountry = s.idCountry;
                newSupply.idSupplyCategory = 1;

                App.Connection.Supply.Add(newSupply);
                App.Connection.SaveChanges();
            }

            foreach (var s in manufacturer.GetTradedCountriesList)
            {
                if (!s.idSupply.Equals(0))
                    continue;

                Supply newSupply = new Supply();
                newSupply.idManufacturer = idManufacturer;
                newSupply.idCountry = s.idCountry;
                newSupply.idSupplyCategory = 2;

                App.Connection.Supply.Add(newSupply);
                App.Connection.SaveChanges();
            }

            MessageBox.Show("Данные успешно сохранены!");

            NavigationService.Navigate(new ManufacturersPage());
        }
        private void NameHyperlinkClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserPage());
        }

        private void LogoButtonClick(object sender, RoutedEventArgs e)
        {
            if(App.CurrentUser.idRole.Equals(2))
            {
                NavigationService.Navigate(new MainPage());
                return;
            }

            if (MessageBox.Show("Новые данные будут удалены.", "Вы точно хотите выйти?", MessageBoxButton.OKCancel).Equals(MessageBoxResult.OK))
                NavigationService.Navigate(new MainPage());
        }

        private void LocatedCountriesButtonClick(object sender, RoutedEventArgs e)
        {
            if (LocatedCountriesComboBox == null)
                return;

            var selectedCountry = LocatedCountriesComboBox.SelectedItem as Country;

            if (selectedCountry == null)
                return;

            foreach (var supply in manufacturer.GetLocatedCountriesList)
            {
                if (supply.idCountry.Equals(selectedCountry.idCountry))
                {
                    return;
                }
            }

            var existSupply = App.Connection.Supply.Where(z => z.idManufacturer.Equals(manufacturer.idManufacturer) && z.idCountry.Equals(selectedCountry.idCountry) && z.idSupplyCategory.Equals(1)).FirstOrDefault();

            var newSupply = new Supply();

            if (existSupply == null)
            {
                newSupply.idCountry = selectedCountry.idCountry;
                newSupply.idManufacturer = manufacturer.idManufacturer;
                newSupply.idSupplyCategory = 1;

                manufacturer.GetLocatedCountriesList.Add(newSupply);

                LocatedTextBox.Text += selectedCountry.title + "; ";

            }
            else
                return;
        }

        private void TradedCountriesButtonClick(object sender, RoutedEventArgs e)
        {
            if (TradedCountriesComboBox == null)
                return;

            var selectedCountry = TradedCountriesComboBox.SelectedItem as Country;

            if (selectedCountry == null)
                return;

            foreach (var supply in manufacturer.GetTradedCountriesList)
            {
                if (supply.idCountry.Equals(selectedCountry.idCountry))
                {
                    return;
                }
            }

            var existSupply = App.Connection.Supply.Where(z => z.idManufacturer.Equals(manufacturer.idManufacturer) && z.idCountry.Equals(selectedCountry.idCountry) && z.idSupplyCategory.Equals(2)).FirstOrDefault();

            var newSupply = new Supply();

            if (existSupply == null)
            {
                newSupply.idCountry = selectedCountry.idCountry;
                newSupply.idManufacturer = manufacturer.idManufacturer;
                newSupply.idSupplyCategory = 2;

                manufacturer.GetTradedCountriesList.Add(newSupply);

                TradedTextBox.Text += selectedCountry.title + "; ";
            }
            else
                return;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить этот объект?", "Подтвержение операции", MessageBoxButton.OKCancel).Equals(MessageBoxResult.OK))
            {
                foreach(var sup in manufacturer.GetLocatedCountriesList)
                {
                    App.Connection.Supply.Remove(sup);
                }


                foreach (var sup in manufacturer.GetTradedCountriesList)
                {
                    App.Connection.Supply.Remove(sup);
                }

                var temp = App.Connection.Manufacturer.FirstOrDefault(z => z.idManufacturer.Equals(manufacturer.idManufacturer));

                App.Connection.Manufacturer.Remove(temp);
                App.Connection.SaveChanges();

                MessageBox.Show("Данные удалены!");
                NavigationService.Navigate(new ManufacturersPage());
            }
        }
    }
}
