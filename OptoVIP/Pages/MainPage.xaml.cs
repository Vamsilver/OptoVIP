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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        List<ViewProduct> products;

        public MainPage()
        {
            InitializeComponent();

            UserNameTextBlock.Text = App.UserLogin;

            products = Converter.ConvertToListViewProducts(App.Connection.Product.ToList());

            ProductList.ItemsSource = products;

            var categoryList = App.Connection.ProductCategory.ToList();
            var allProductCategory = new ProductCategory()
            {
                title = "Все"
            };

            var manufacturerList = App.Connection.Manufacturer.ToList();
            var allManufacturer = new Manufacturer()
            {
                title = "Все"
            };

            categoryList.Add(allProductCategory);
            manufacturerList.Add(allManufacturer);

            CategoryComboBox.ItemsSource = categoryList;
            ManufacturerComboBox.ItemsSource = manufacturerList;

            CategoryComboBox.SelectedItem = allProductCategory;
            ManufacturerComboBox.SelectedItem = allManufacturer;

        }

        private void NameHyperlinkClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserPage());
        }

        private void LikeButtonClick(object sender, RoutedEventArgs e)
        {
            var newLike = new Like();

            if (ProductList.SelectedItem == null)
                return;

            var currentProduct = Converter.ConvertToProduct(ProductList.SelectedItem as ViewProduct);

            newLike.idProduct = currentProduct.idProduct;
            newLike.idUser = App.CurrentUser.idUser;

            var oldLike = App.Connection.Like.
                Where(z => z.idProduct.Equals(currentProduct.idProduct) &&
                           z.idUser.Equals(App.CurrentUser.idUser)).
                FirstOrDefault();

            if (oldLike != null)
            {
                MessageBox.Show("Уже у вас в изрбранном", "Упс");
                return;
            }
            else
            {
                App.Connection.Like.Add(newLike);
                App.Connection.SaveChanges();

                MessageBox.Show("Успешно добавлено в избранное!)");

                OrderProductList(products);
            }
        }

        private void CategoryComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            products = Converter.ConvertToListViewProducts(App.Connection.Product.ToList());

            var categorySortComboBoxSelectedItem = CategoryComboBox.SelectedItem as ProductCategory;
            var manufacturerComboBoxSelectedItem = ManufacturerComboBox.SelectedItem as Manufacturer;

            if (categorySortComboBoxSelectedItem == null || manufacturerComboBoxSelectedItem == null)
                return;

            if (categorySortComboBoxSelectedItem.title.Equals("Все") && manufacturerComboBoxSelectedItem.title.Equals("Все"))
                products = Converter.ConvertToListViewProducts(App.Connection.Product.ToList());
            else if (categorySortComboBoxSelectedItem.title.Equals("Все") && !manufacturerComboBoxSelectedItem.title.Equals("Все"))
                products = Converter.ConvertToListViewProducts(App.Connection.Product.Where(z => z.Manufacturer.title.Equals(manufacturerComboBoxSelectedItem.title)).ToList());
            else if (!categorySortComboBoxSelectedItem.title.Equals("Все") && manufacturerComboBoxSelectedItem.title.Equals("Все"))
                products = Converter.ConvertToListViewProducts(App.Connection.Product.Where(z => z.ProductCategory.title.Equals(categorySortComboBoxSelectedItem.title)).ToList());
            else
                products = products.Where(z => z.idProductCategory.Equals(categorySortComboBoxSelectedItem.idProdcutCategory) &&
                                               z.idManufacturer.Equals(manufacturerComboBoxSelectedItem.idManufacturer)).ToList();

            var newList = OrderProductList(products);

            if (newList != null)
                products = newList;

            ProductList.ItemsSource = products;
            ProductList.Items.Refresh();
        }

        private List<ViewProduct> OrderProductList(List<ViewProduct> list)
        {
            var categorySortComboBoxSelectedItem = CategoryComboBox.SelectedItem as ProductCategory;
            var manufacturerComboBoxSelectedItem = ManufacturerComboBox.SelectedItem as Manufacturer;

            if (categorySortComboBoxSelectedItem != null || manufacturerComboBoxSelectedItem != null)
                if(categorySortComboBoxSelectedItem.title.Equals("Все") && manufacturerComboBoxSelectedItem.title.Equals("Все"))
                    list = Converter.ConvertToListViewProducts(App.Connection.Product.ToList());

            switch ((SortComboBox.SelectedItem as ComboBoxItem).Content.ToString())
            {
                case "Популярные":
                    list = list.OrderBy(z => z.GetAmountOfLikes).ToList();
                    list.Reverse();
                    break;
                case "По возрастанию цены":
                    list = list.OrderBy(z => z.approximatePricePerUnit).ToList();
                    break;
                case "По убыванию цены":
                    list = list.OrderBy(z => z.approximatePricePerUnit).ToList();
                    list.Reverse();
                    break;
                case "По возрастанию ранга":
                    list = list.OrderBy(z => z.idPriceRange).ToList();
                    break;
                case "По убыванию ранга":
                    list = list.OrderBy(z => z.idPriceRange).ToList();
                    list.Reverse();
                    break;
            }

            if(SearchTextBox.Text != "")
                list = list.Where(z => z.title.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase)).ToList();

            if (MinCost.Text != "")
                list = list.Where(z => z.approximatePricePerUnit >= Decimal.Parse(MinCost.Text)).ToList();


            if (MaxCost.Text != "")
                list = list.Where(z => z.approximatePricePerUnit <= Decimal.Parse(MaxCost.Text)).ToList();

            return list;
        }   

        private void SortComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox == null)
                return;

            if (CategoryComboBox == null || ManufacturerComboBox == null)
                return;

            var categorySortComboBoxSelectedItem = CategoryComboBox.SelectedItem as ProductCategory;
            var manufacturerComboBoxSelectedItem = ManufacturerComboBox.SelectedItem as Manufacturer;

            if (categorySortComboBoxSelectedItem == null || manufacturerComboBoxSelectedItem == null)
                return;

            products = OrderProductList(products);

            ProductList.ItemsSource = products;
            ProductList.Items.Refresh();
        }

        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            products = Converter.ConvertToListViewProducts(App.Connection.Product.ToList());

            var categorySortComboBoxSelectedItem = CategoryComboBox.SelectedItem as ProductCategory;
            var manufacturerComboBoxSelectedItem = ManufacturerComboBox.SelectedItem as Manufacturer;

            if (categorySortComboBoxSelectedItem == null || manufacturerComboBoxSelectedItem == null)
                return;

            if (categorySortComboBoxSelectedItem.title.Equals("Все") && manufacturerComboBoxSelectedItem.title.Equals("Все"))
                products = Converter.ConvertToListViewProducts(App.Connection.Product.ToList());
            else if (categorySortComboBoxSelectedItem.title.Equals("Все") && !manufacturerComboBoxSelectedItem.title.Equals("Все"))
                products = Converter.ConvertToListViewProducts(App.Connection.Product.Where(z => z.Manufacturer.title.Equals(manufacturerComboBoxSelectedItem.title)).ToList());
            else if (!categorySortComboBoxSelectedItem.title.Equals("Все") && manufacturerComboBoxSelectedItem.title.Equals("Все"))
                products = Converter.ConvertToListViewProducts(App.Connection.Product.Where(z => z.ProductCategory.title.Equals(categorySortComboBoxSelectedItem.title)).ToList());
            else
                products = products.Where(z => z.idProductCategory.Equals(categorySortComboBoxSelectedItem.idProdcutCategory) &&
                                               z.idManufacturer.Equals(manufacturerComboBoxSelectedItem.idManufacturer)).ToList();

            var newList = OrderProductList(products);

            if (newList != null)
                products = newList;

            ProductList.ItemsSource = products;
            ProductList.Items.Refresh();
        }

        private void LogoButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void SearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var newList = OrderProductList(products);

            if (newList != null)
                products = newList;

            ProductList.ItemsSource = products;
            ProductList.Items.Refresh();
        }

        private void MinCostPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")
               && (!MinCost.Text.Contains(".")
               && MinCost.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }

        private void MinCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            var newList = OrderProductList(products);

            if (newList != null)
                products = newList;

            ProductList.ItemsSource = products;
            ProductList.Items.Refresh();
        }

        private void MaxCost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")
               && (!MaxCost.Text.Contains(".")
               && MaxCost.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }

        private void MaxCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            var newList = OrderProductList(products);

            if (newList != null)
                products = newList;

            ProductList.ItemsSource = products;
            ProductList.Items.Refresh();
        }
    }
}
