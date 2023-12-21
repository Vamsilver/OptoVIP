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
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        List<ViewProduct> currentProducts = new List<ViewProduct>();
        List<ViewProduct> allLikedProducts = new List<ViewProduct>();

        public UserPage()
        {
            InitializeComponent();

            switch(App.CurrentUser.idRole)
            {
                case 1:
                    
                    ClientUniformGrid.IsEnabled = false;
                    ClientUniformGrid.Visibility = Visibility.Hidden;

                    ProductList.IsEnabled = false;
                    ProductList.Visibility = Visibility.Hidden;

                    OperatorUniformGrid.IsEnabled = true;
                    OperatorUniformGrid.Visibility = Visibility.Visible;

                    break;
                case 2:
                    ClientUniformGrid.IsEnabled = true;
                    ClientUniformGrid.Visibility = Visibility.Visible;

                    ProductList.IsEnabled = true;
                    ProductList.Visibility = Visibility.Visible;

                    OperatorUniformGrid.IsEnabled = false;
                    OperatorUniformGrid.Visibility = Visibility.Hidden;

                    break;
            }

            UserNameTextBlock.Text = App.UserLogin;

            var likedProducts = App.Connection.Like.Where(z => z.idUser.Equals(App.CurrentUser.idUser)).ToList();

            foreach (var likedProduct in likedProducts)
            {
                var like = App.Connection.Product.Where(z => z.idProduct.Equals(likedProduct.idProduct)).FirstOrDefault();
                if (like != null)
                    allLikedProducts.Add(new ViewProduct(like));
            }

            currentProducts = allLikedProducts;

            ProductList.ItemsSource = currentProducts;

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

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProductPage());
        }

        private void NameHyperlinkClick(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new UserPage());
        }

        private void CategoryComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentProducts = allLikedProducts;

            var categorySortComboBoxSelectedItem = CategoryComboBox.SelectedItem as ProductCategory;
            var manufacturerComboBoxSelectedItem = ManufacturerComboBox.SelectedItem as Manufacturer;

            if (categorySortComboBoxSelectedItem == null || manufacturerComboBoxSelectedItem == null)
                return;

            if (categorySortComboBoxSelectedItem.title.Equals("Все") && manufacturerComboBoxSelectedItem.title.Equals("Все"))
                currentProducts = allLikedProducts;
            else if (categorySortComboBoxSelectedItem.title.Equals("Все") && !manufacturerComboBoxSelectedItem.title.Equals("Все"))
                currentProducts = allLikedProducts.Where(z => z.idManufacturer.Equals(manufacturerComboBoxSelectedItem.idManufacturer)).ToList();
            else if (!categorySortComboBoxSelectedItem.title.Equals("Все") && manufacturerComboBoxSelectedItem.title.Equals("Все"))
                currentProducts = allLikedProducts.Where(z => z.idProductCategory.Equals(categorySortComboBoxSelectedItem.idProdcutCategory)).ToList();
            else
                currentProducts = allLikedProducts.Where(z => z.idProductCategory.Equals(categorySortComboBoxSelectedItem.idProdcutCategory) &&
                                               z.idManufacturer.Equals(manufacturerComboBoxSelectedItem.idManufacturer)).ToList();

            var newList = OrderProductList(currentProducts);

            if (newList != null)
                currentProducts = newList;

            ProductList.ItemsSource = currentProducts;
            ProductList.Items.Refresh();
        }

        private List<ViewProduct> OrderProductList(List<ViewProduct> list)
        {
            var categorySortComboBoxSelectedItem = CategoryComboBox.SelectedItem as ProductCategory;
            var manufacturerComboBoxSelectedItem = ManufacturerComboBox.SelectedItem as Manufacturer;

            if (categorySortComboBoxSelectedItem != null || manufacturerComboBoxSelectedItem != null)
                if (categorySortComboBoxSelectedItem.title.Equals("Все") && manufacturerComboBoxSelectedItem.title.Equals("Все"))
                    list = allLikedProducts;

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

            if (SearchTextBox.Text != "")
                list = list.Where(z => z.title.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase)).ToList();

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

            currentProducts = OrderProductList(currentProducts);

            ProductList.ItemsSource = currentProducts;
            ProductList.Items.Refresh();
        }

        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentProducts = Converter.ConvertToListViewProducts(App.Connection.Product.ToList());

            var categorySortComboBoxSelectedItem = CategoryComboBox.SelectedItem as ProductCategory;
            var manufacturerComboBoxSelectedItem = ManufacturerComboBox.SelectedItem as Manufacturer;

            if (categorySortComboBoxSelectedItem == null || manufacturerComboBoxSelectedItem == null)
                return;

            if (categorySortComboBoxSelectedItem.title.Equals("Все") && manufacturerComboBoxSelectedItem.title.Equals("Все"))
                currentProducts = allLikedProducts;
            else if (categorySortComboBoxSelectedItem.title.Equals("Все") && !manufacturerComboBoxSelectedItem.title.Equals("Все"))
                currentProducts = allLikedProducts.Where(z => z.idManufacturer.Equals(manufacturerComboBoxSelectedItem.idManufacturer)).ToList();
            else if (!categorySortComboBoxSelectedItem.title.Equals("Все") && manufacturerComboBoxSelectedItem.title.Equals("Все"))
                currentProducts = allLikedProducts.Where(z => z.ProductCategory.Equals(categorySortComboBoxSelectedItem.idProdcutCategory)).ToList();
            else
                currentProducts = allLikedProducts.Where(z => z.idProductCategory.Equals(categorySortComboBoxSelectedItem.idProdcutCategory) &&
                                               z.idManufacturer.Equals(manufacturerComboBoxSelectedItem.idManufacturer)).ToList();

            var newList = OrderProductList(currentProducts);

            if (newList != null)
                currentProducts = newList;

            ProductList.ItemsSource = currentProducts;
            ProductList.Items.Refresh();
        }

        private void SearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var newList = OrderProductList(currentProducts);

            if (newList != null)
                currentProducts = newList;

            if(ProductList != null)
            {
                ProductList.ItemsSource = currentProducts;
                ProductList.Items.Refresh();
            }
        }

        private void LogoButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void AddManufacturerButtonClick(object sender, RoutedEventArgs e)
        {

        }

    }
}
