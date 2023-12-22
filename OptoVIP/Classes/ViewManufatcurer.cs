using OptoVIP.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OptoVIP.Classes
{
    public class ViewManufacturer : Manufacturer
    {
        List<Supply> locatedCountriesList = new List<Supply>();
        List<Supply> tradedCountriesList = new List<Supply>();

        string locatedCountriesString = "";
        string tradedCountriesString = "";

        public string GetLocatedCountriesString => locatedCountriesString;
        public string GetTradedCountriesString => tradedCountriesString;

        public List<Supply> GetLocatedCountriesList => locatedCountriesList;
        public List<Supply> GetTradedCountriesList => tradedCountriesList;

        public ViewManufacturer(Manufacturer manufacturer)
        {
            idManufacturer = manufacturer.idManufacturer;

            title = manufacturer.title;
            description = manufacturer.description;

            var allSuplies = App.Connection.Supply.Where(z => z.idManufacturer.Equals(manufacturer.idManufacturer)).ToList();
            locatedCountriesList = allSuplies.Where(z => z.idSupplyCategory.Equals(1)).ToList();
            tradedCountriesList = allSuplies.Where(z => z.idSupplyCategory.Equals(2)).ToList();

            int count = 1;

            foreach(var country in locatedCountriesList)
            {
                if(count % 4 == 0)
                    locatedCountriesString += "\n";

                locatedCountriesString += country + "; ";
                count++;
            }

            count = 1;

            foreach (var country in tradedCountriesList)
            {
                if (count % 4 == 0)
                    tradedCountriesString += "\n";

                tradedCountriesString += country + "; ";
                count++;
            }
        }
    }
}
