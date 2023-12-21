﻿using OptoVIP.ADO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OptoVIP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static OptoVIPEntities1 Connection = new OptoVIPEntities1();
        public static ADO.User CurrentUser = new ADO.User();
        public static string UserLogin;
    }
}
