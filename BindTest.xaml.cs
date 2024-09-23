using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace C2Windows
{
    /// <summary>
    /// Interaction logic for BindTest.xaml
    /// </summary>
    public partial class BindTest
    {
        public BindTest()
        {
            InitializeComponent();

            Grid g = MainGrid;

            var b = new Button();
            b.Content = "Hello world!";

            b.Click += (s, e) =>
            {
                MessageBox.Show("Hello!");
            };

            // set b VerticalAlignment to Center
            b.VerticalAlignment = VerticalAlignment.Center;
            b.HorizontalAlignment  = HorizontalAlignment.Center;

            g.Children.Add(b);
        }
    }
}
