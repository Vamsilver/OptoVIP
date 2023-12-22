using Microsoft.Win32;
using OptoVIP.ADO;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddProductPage.xaml
    /// </summary>
    public partial class AddProductPage : Page
    {
        Product product = new Product();
        byte[] imageBytes;

        public AddProductPage()
        {
            InitializeComponent();
            DataContext = product;

            CategoryComboBox.ItemsSource = App.Connection.ProductCategory.ToList();
            NotationComboBox.ItemsSource = App.Connection.Notation.ToList();
            ManufacturerComboBox.ItemsSource = App.Connection.Manufacturer.ToList();
            PriceRangeComboBox.ItemsSource = App.Connection.PriceRange.ToList();

            UserNameTextBlock.Text = App.UserLogin;
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

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            SelectImage();
        }

        private void AddnewProductButtonClick(object sender, RoutedEventArgs e)
        {
            if(TitleTextBox.Text.Length > 150)
            {
                MessageBox.Show("Название должно быть не длинее 150 символов.", "Слишком длинное название");
                return;
            }

            if(DesriptionTextBox.Text.Length > 500)
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

            App.Connection.Product.Add(product);
            App.Connection.SaveChanges();

            NavigationService.GoBack();
        }
        private void NameHyperlinkClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserPage());
        }

        private void LogoButtonClick(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Данные будут удалены.","Вы точно хотите выйти?", MessageBoxButton.OKCancel).Equals(MessageBoxResult.OK))
                NavigationService.Navigate(new MainPage());
        }
    }
}
