using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace StockFocus.Helper
{
     
        public class NumericTextBox : TextBox
        {
            public NumericTextBox()
            {
                this.KeyDown += NumericTextBox_KeyDown;
                this.LostFocus += NumericTextBox_LostFocus;
            }

            private void NumericTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
            {
                if (this.Text.Length > 0)
                {
                    Text = Math.Round((double.Parse(Text)), decimalNumbers).ToString();
                }
                else
                {
                    Text = Math.Round((double.Parse("0")), decimalNumbers).ToString();
                }
            }


            public new string Text
            {
                get
                {
                    return Math.Round(base.Text.ToDecimal(), decimalNumbers).ToString();
                }
                set
                {
                    base.Text = Math.Round(value.ToDecimal(), decimalNumbers).ToString();
                }
            }
            private void NumericTextBox_KeyDown(object sender, KeyEventArgs e)
            {
                if (DecimalNumbers > 0)
                {
                    if (!(WholenumberKeys.Contains(e.Key) || ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && !this.Text.Contains("."))))
                    {
                        e.Handled = true;
                    }


                }
                else if (!WholenumberKeys.Contains(e.Key))
                {
                    e.Handled = true;
                }




            }

            Key[] WholenumberKeys = { Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5,
            Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6,
            Key.D7, Key.D8, Key.D9, Key.D0 };


            public int DecimalNumbers
            {
                get
                {
                    return decimalNumbers;
                }

                set
                {
                    decimalNumbers = value;
                }
            }

            int decimalNumbers = 2;


        }
     
}
