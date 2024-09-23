using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

namespace C2Windows
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public partial class DNCResources
    {
        public string appStyle;
        public DNCResources()
        {
            appStyle = File.ReadAllText(System.IO.Path.Join(Environment.CurrentDirectory, @"./Resources/app.css"));;
        }
    }
}