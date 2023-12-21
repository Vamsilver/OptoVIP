﻿using OptoVIP.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OptoVIP.Classes
{
    public class ViewProduct : Product
    {
        int AmountOfLikes = 0;

        public int GetAmountOfLikes => AmountOfLikes;

        public ViewProduct(Product product)
        {
            idProduct = product.idProduct;

            title = product.title;
            description = product.description;
            idProductCategory = product.idProductCategory;
            image = product.image;
            minCount = product.minCount;
            idNotation = product.idNotation;
            idManufacturer = product.idManufacturer;
            idPriceRange = product.idPriceRange;
            approximatePricePerUnit = product.approximatePricePerUnit;

            AmountOfLikes = App.Connection.Like.Where(z => z.idProduct.Equals(product.idProduct)).Count();
        }
    }
}
