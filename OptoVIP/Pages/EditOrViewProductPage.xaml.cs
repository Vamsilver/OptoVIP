using Microsoft.Win32;
using OptoVIP.ADO;
using OptoVIP.Classes;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
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
    /// Interaction logic for EditOrViewProductPage.xaml
    /// </summary>
    public partial class EditOrViewProductPage : Page
    {
        Product product = new Product();
        byte[] imageBytes;

        public EditOrViewProductPage()
        {
            InitializeComponent();
            DataContext = product;

            CategoryComboBox.ItemsSource = App.Connection.ProductCategory.ToList();
            NotationComboBox.ItemsSource = App.Connection.Notation.ToList();
            ManufacturerComboBox.ItemsSource = App.Connection.Manufacturer.ToList();
            PriceRangeComboBox.ItemsSource = App.Connection.PriceRange.ToList();

            UserNameTextBlock.Text = App.UserLogin;
        }

        public EditOrViewProductPage(ViewProduct productFromProductList)
        {
            InitializeComponent();

            this.product = App.Connection.Product.Where(z => z.idProduct.Equals(productFromProductList.idProduct)).FirstOrDefault();

            if(productFromProductList.GetIsLikedProductFromCurrentUser)
                HeartImage.Source = new BitmapImage(new Uri("/Images/free-icon-heart-2107845.png", UriKind.Relative));

            DataContext = product;

            CategoryComboBox.ItemsSource = App.Connection.ProductCategory.ToList();
            NotationComboBox.ItemsSource = App.Connection.Notation.ToList();
            ManufacturerComboBox.ItemsSource = App.Connection.Manufacturer.ToList();
            PriceRangeComboBox.ItemsSource = App.Connection.PriceRange.ToList();

            UserNameTextBlock.Text = App.UserLogin;

            TitleTextBox.Text = product.title;
            CategoryComboBox.SelectedItem = product.ProductCategory;
            LinkTextBox.Text = product.link;
            MinCountTextBox.Text = product.minCount.ToString();
            NotationComboBox.SelectedItem = product.Notation;
            ManufacturerComboBox.SelectedItem = product.Manufacturer;
            DesriptionTextBox.Text = product.description;
            PriceRangeComboBox.SelectedItem = product.PriceRange;
            BindingOperations.GetBindingExpressionBase(ProductImage, Image.SourceProperty).UpdateTarget();
            ApproximatePricePerUnitTextBox.Text = product.approximatePricePerUnit.ToString();

            if(App.CurrentUser.idRole.Equals(2))
            {
                TitleTextBox.IsReadOnly = true;
                CategoryComboBox.IsEnabled = false;
                LinkTextBox.IsReadOnly = true;
                MinCountTextBox.IsReadOnly = true;
                NotationComboBox.IsEnabled = false;
                ManufacturerComboBox.IsEnabled = false;
                DesriptionTextBox.IsReadOnly = true;
                PriceRangeComboBox.IsEnabled = false;
                ApproximatePricePerUnitTextBox.IsReadOnly = true;

                EditImageButton.IsEnabled = false;
                EditImageButton.Visibility = Visibility.Hidden;

                DeleteButton.IsEnabled = false;
                DeleteButton.Visibility = Visibility.Hidden;
            }
        }

        private void SelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Files|*.jpg;*.jpeg;*.png;";
            openFileDialog.Title = "Выберите изображение для товара.";
            openFileDialog.ShowDialog();

            if (String.IsNullOrEmpty(openFileDialog.FileName))
            {
                MessageBox.Show("Пустое изображение");
                return;
            }

            imageBytes = File.ReadAllBytes(openFileDialog.FileName);

            product.image = imageBytes;
            BindingOperations.GetBindingExpressionBase(ProductImage, Image.SourceProperty).UpdateTarget();
        }

        private void EditImageButtonClick(object sender, RoutedEventArgs e)
        {
            SelectImage();
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
                product.title = TitleTextBox.Text;
                product.minCount = Decimal.Parse(MinCountTextBox.Text.Replace('.', ','));
                product.description = DesriptionTextBox.Text;
                product.ProductCategory = (ProductCategory)CategoryComboBox.SelectedItem;
                product.Notation = (Notation)NotationComboBox.SelectedItem;
                product.Manufacturer = (Manufacturer)ManufacturerComboBox.SelectedItem;
                product.PriceRange = (PriceRange)PriceRangeComboBox.SelectedItem;
                product.approximatePricePerUnit = Decimal.Parse(ApproximatePricePerUnitTextBox.Text.Replace('.', ','));
                product.link = LinkTextBox.Text;
                product.image = imageBytes;
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло не так..");
                return;
            }

            App.Connection.Product.AddOrUpdate(product);
            App.Connection.SaveChanges();

            NavigationService.GoBack();
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

            if (MessageBox.Show("Данные будут удалены.", "Вы точно хотите выйти?", MessageBoxButton.OKCancel).Equals(MessageBoxResult.OK))
                NavigationService.Navigate(new MainPage());
        }

        private void CopyButtonClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(LinkTextBox.Text);
            MessageBox.Show("Ссылка скопирована в буфер обмена!");
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить этот объект?", "Подтвержение операции", MessageBoxButton.OKCancel).Equals(MessageBoxResult.OK))
            {
                try
                {
                    App.Connection.Product.Remove(product);
                    App.Connection.SaveChanges();
                }
                catch (Exception)
                {
                    MessageBox.Show("Что-то пошло не так..");
                    return;
                }

                MessageBox.Show("Данные удалены!");
                NavigationService.Navigate(new MainPage());
            }
        }

        private void ManufacturerButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManufacturerPage(new ViewManufacturer(ManufacturerComboBox.SelectedItem as Manufacturer)));
        }

        private void LikeButtonClick(object sender, RoutedEventArgs e)
        {
            var newLike = new Like();

            newLike.idProduct = product.idProduct;
            newLike.idUser = App.CurrentUser.idUser;

            var oldLike = App.Connection.Like.
                Where(z => z.idProduct.Equals(product.idProduct) &&
                           z.idUser.Equals(App.CurrentUser.idUser)).
                FirstOrDefault();

            if (oldLike != null)
            {
                var mesRes = MessageBox.Show("Вы хотите убрать этот товар из избранного?", "Удалить из избранного", MessageBoxButton.OKCancel);
                
                if(mesRes.Equals(MessageBoxResult.OK))
                {
                    App.Connection.Like.Remove(oldLike);
                    App.Connection.SaveChanges();

                    HeartImage.Source = new BitmapImage(new Uri("/Images/free-icon-heart-3502230.png", UriKind.Relative));

                    NavigationService.Navigate(new EditOrViewProductPage(new ViewProduct(product)));

                    MessageBox.Show("Товар убран из избранного!");
                }

                return;
            }
            else
            {
                App.Connection.Like.Add(newLike);
                App.Connection.SaveChanges();

                HeartImage.Source = new BitmapImage(new Uri("/Images/free-icon-heart-2107845.png", UriKind.Relative));

                NavigationService.Navigate(new EditOrViewProductPage(new ViewProduct(product)));

                MessageBox.Show("Успешно добавлено в избранное!)");
            }
        }
    }
}
