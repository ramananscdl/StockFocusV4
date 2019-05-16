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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockFocus.UI
{
    /// <summary>
    /// Interaction logic for UcContextMenu.xaml
    /// </summary>
    public partial class UcContextMenu : UserControl
    {
        public UcContextMenu()
        {
            InitializeComponent();
            if(MenuItems !=null)
            {
                foreach (var kvp in MenuItems)
                {
                    WrapPanel wp = new WrapPanel();
                    RowDefinition rd = new RowDefinition() { Height = new GridLength(25) };
                    grdContext.RowDefinitions.Add(rd);
                    TextBlock tb = new TextBlock() { Text = kvp.Value };
                    wp.Children.Add(tb);
                    wp.Tag = kvp.Key;
                    grdContext.Children.Add(wp);
                }
            }

        }

        public List<KeyValuePair<int, string>> MenuItems;

        
    }
}
