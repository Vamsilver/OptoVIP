using OptoVIP.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Annotations;

namespace OptoVIP.Classes
{
    public class Converter
    {
        public static Product ConvertToProduct(ViewProduct viewProduct)
        {
            Product product = new Product()
            {
                idProduct = viewProduct.idProduct,
                title = viewProduct.title,    
                description = viewProduct.description,
                idProductCategory = viewProduct.idProductCategory,
                image = viewProduct.image,
                minCount = viewProduct.minCount,
                idNotation = viewProduct.idNotation,
                idManufacturer = viewProduct.idManufacturer,
                idPriceRange = viewProduct.idPriceRange,
                approximatePricePerUnit = viewProduct.approximatePricePerUnit
            };

            return product;
        }

        public static List<ViewProduct> ConvertToListViewProducts(List<Product> list)
        {
            List<ViewProduct> viewProductList = new List<ViewProduct>();

            foreach (Product product in list)
                viewProductList.Add(new ViewProduct(product));

            return viewProductList;
        }
    }
}
