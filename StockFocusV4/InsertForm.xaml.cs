using StockFocus.Entites;
using StockFocus.Helper;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockFocus.UI
{
    /// <summary>
    /// Interaction logic for InsertForm.xaml
    /// </summary>
    public partial class InsertForm : UserControl
    {
        public InsertForm()
        {
            InitializeComponent();
        }

        public delegate void delgSaveClicked(object sender, InputSavedEventArgs Data);
        public event delgSaveClicked SaveClicked;
        public event EventHandler CancelClicked;


        public InputOperation  Operation { get; set; }


        public List<GridItem> DataItems { get; set; }

        public string Title { get; set; }

        public void DataBind()
        {
            txtTitle.Content = Title;
            this.Height = 130.00;
            if (DataItems != null && DataItems.Count > 0)
            {
                grdTopic.Children.Clear();
                grdControl.Children.Clear();
                int count = 0;
                DataItems.ForEach(di =>
                {
                    RowDefinition rd = new RowDefinition() { Height = new GridLength(35) };
                    grdTopic.RowDefinitions.Add(rd);
                    Label lbl = new Label() { Content = di.DisplayName + (di.IsMandatory ? "*" : ""), FontSize = 15, Foreground = Brushes.Teal, HorizontalContentAlignment = HorizontalAlignment.Right };
                    Grid.SetRow(lbl, count);
                    grdTopic.Children.Add(lbl);

                    RowDefinition rdContent = new RowDefinition() { Height = new GridLength(35) };
                    grdControl.RowDefinitions.Add(rdContent);
                    this.Height += 32;
                    switch (di.InputControl)
                    {
                        case Entites.InputType.Textbox:
                            TextBox tb = new TextBox() { FontSize = 15, Height = 30 };
                            tb.Tag = di;
                            Grid.SetRow(tb, count);
                            grdControl.Children.Add(tb);
                            break;
                        case Entites.InputType.NumericTextbox:
                            NumericTextBox ntb = new NumericTextBox() { FontSize = 15, DecimalNumbers = di.DecimalNumbers, Height = 30 };
                            ntb.Tag = di;
                            Grid.SetRow(ntb, count);
                            grdControl.Children.Add(ntb);
                            break;
                        case Entites.InputType.DropDown:
                            ComboBox cb = new ComboBox() { FontSize = 15, Height = 30 };
                            di.ControlValues.ForEach(i => cb.Items.Add(i));
                           
                            cb.DisplayMemberPath = "Value";
                            cb.SelectedValuePath = "Key";
                            cb.Tag = di;
                            Grid.SetRow(cb, count);
                            grdControl.Children.Add(cb);
                            break;
                        case Entites.InputType.Calendar:
                            DatePicker dp = new DatePicker() { FontSize = 15, Height = 30, SelectedDate = DateTime.Today };
                            dp.Tag = di;
                            Grid.SetRow(dp, count);
                            grdControl.Children.Add(dp);
                            break;
                        default:
                            break;
                    }

                    count++;
                });
            }
        }



        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClicked?.Invoke(this, e);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            InputSavedEventArgs result = new InputSavedEventArgs();
            result.Operation = Operation;

            lblMessage.Content = ""; 

            List<GridItem> data = new List<GridItem>();
            foreach (var c in grdControl.Children)
            {
                if (c is TextBox)
                {
                    TextBox tb = c as TextBox;
                    var item = (GridItem)(tb).Tag;
                    if (item.IsMandatory && string.IsNullOrEmpty(tb.Text))
                    {
                        lblMessage.Content = " Fill all mandatory fields";
                        return;
                    }
                    item.ReturnValue = tb.Text;
                    data.Add(item);
                }
                else if (c is DatePicker)
                {
                    DatePicker tb = c as DatePicker;
                    var item = (GridItem)(tb).Tag;
                    if (item.IsMandatory && !tb.SelectedDate.HasValue)
                    {
                        lblMessage.Content = " Fill all mandatory fields";
                        return;
                    }
                    if (tb.SelectedDate.HasValue)
                        item.ReturnValue = tb.SelectedDate.Value.ToString("dd-MMM-yyyy");
                    data.Add(item);
                }
                else if (c is ComboBox)
                {

                    ComboBox tb = c as ComboBox;
                    var item = (GridItem)(tb).Tag;
                    if (item.IsMandatory && string.IsNullOrEmpty(tb.Text))
                    {
                        lblMessage.Content = " Fill all mandatory fields";
                        return;
                    }
                    item.ReturnValue = tb.SelectedValue;
                    data.Add(item);
                }
            }
            result.Data = data;

            SaveClicked?.Invoke(this, result);
        }
    }
}
