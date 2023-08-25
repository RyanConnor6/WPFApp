using MaterialDesignThemes.Wpf;
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
using System.Windows.Shapes;
using MaterialDesignColors;

namespace WPFAppProject
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        HomePage my = new HomePage();
        BrushConverter bc = new BrushConverter();

        public HomeWindow()
        {
            InitializeComponent();
        }

        private void homeButton_Click(object sender, RoutedEventArgs e)
        {
            navframe.Navigate(my);
        }

        private void mouseEnterButton(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Background = (Brush)bc.ConvertFrom("#FF34648A");
        }

        private void mouseLeaveButton(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Background = (Brush)bc.ConvertFrom("#FF38434C");
        }
    }
}
