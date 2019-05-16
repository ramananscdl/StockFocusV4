using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StockFocus
{
    public static class Extensions
    {
        public static decimal ToDecimal(this object obj)
        {
            decimal d = 0;
            if (decimal.TryParse(obj.ToString(), out d))
            {
                return d;
            }
            return 0;
        }

        public static decimal ToDecimal(this string obj)
        {
            decimal d = 0;
            obj = obj.Replace("%", "").Replace("Rs", "").Replace("Rs.", "");
            if (decimal.TryParse(obj, out d))
            {
                return d;
            }
            return 0;
        }




        public static double ToDouble(this string obj)
        {
            double result = 0;
            obj = obj.Replace("%", "").Replace("Rs", "").Replace("Rs.", "");
            double.TryParse(obj, out result);
            return result;
        }


        public static void Clear(this UIElementCollection collection, Predicate<UIElement> condition)
        {
            List<UIElement> checkedElements = new List<UIElement>();

            foreach (UIElement element in collection)
            {
                if (condition.Invoke(element))
                {
                    checkedElements.Add(element);
                }
            }

            foreach (var item in checkedElements)
            {
                collection.Remove(item);
            }
        }

        public static T FindFirst<T>(this UIElementCollection collection, Predicate<UIElement> condition) where T : class
        {
            foreach (UIElement element in collection)
            {
                if (element is T && condition.Invoke(element))
                {
                    return (T)Convert.ChangeType(element, typeof(T));
                }
            }
            return null;
        }

        public static List<UIElement> FindAll(this UIElementCollection collection, Predicate<UIElement> condition)
        {
            List<UIElement> result = new List<UIElement>();
            foreach (UIElement element in collection)
            {
                if (condition.Invoke(element))
                {
                    result.Add(element);
                }
            }
            return result;
        }

        public static List<T> FindAllLevelChildrenByType<T>(this UIElementCollection collection, Predicate<T> condition)
        {
            List<T> result = new List<T>();
            foreach (UIElement element in collection)
            {
                if (element is Panel && (element as Panel).Children.Count > 0)
                {
                    result.AddRange(FindAllLevelChildrenByType<T>((element as Panel).Children, condition));
                }
                else if (element is T)
                {
                    T temp = (T)Convert.ChangeType(element, typeof(T));

                    if (condition.Invoke(temp))
                    {
                        result.Add(temp);
                    }
                }

            }
            return result;

        }

    }
}
