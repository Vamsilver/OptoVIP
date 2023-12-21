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

        List<Supply> supplies = new List<Supply>();

        public List<Supply> GetSupplies => supplies;

        public ViewManufacturer(Manufacturer manufacturer)
        {
            idManufacturer = manufacturer.idManufacturer;

            title = manufacturer.title;
            description = manufacturer.description;

            supplies = App.Connection.Supply.Where(z => z.idManufacturer.Equals(manufacturer.idManufacturer)).ToList();
        }
    }
}
