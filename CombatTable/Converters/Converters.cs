/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;
using CombatTable.Models.Base;
using System.Globalization;
using CombatTable.Models;
using System.Windows.Controls;

namespace CombatTable.Converters
{
    public class TypeCheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string type = System.Convert.ToString(parameter);
            if (type == "Attack")
                return value.GetType() == typeof(Models.Attack);
            else if (type == "SpellLevel")
                return value.GetType() == typeof(Models.SpellLevel);

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ColumnSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool boolValue = System.Convert.ToBoolean(value);
            if (boolValue) return parameter;
            else return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Bool2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool reverse = false;
            if (parameter != null) reverse = System.Convert.ToBoolean(parameter);

            bool boolValue = System.Convert.ToBoolean(value);
            if (boolValue) return reverse ? Visibility.Collapsed : Visibility.Visible;
            else return reverse ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntegerValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int i = 0;
            string val = System.Convert.ToString(value);
            if (int.TryParse(val, out i)) return i + System.Convert.ToInt32(parameter);
            else return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int i = 0;
            string val = System.Convert.ToString(value);
            if (int.TryParse(val, out i)) return i;
            else return 0;
        }
    }

    public class BooleanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool i = false;
            string val = System.Convert.ToString(value);
            if (bool.TryParse(val, out i)) return i;
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToString(value);
        }
    }

    public class ColorChoser : IValueConverter, IMultiValueConverter
    {
        public Color TrueCase { get; set; }

        public Color FalseCase { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool i = false;
            string val = System.Convert.ToString(value);
            if (bool.TryParse(val, out i)) if (i) return new SolidColorBrush(TrueCase);

            return new SolidColorBrush(FalseCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == values[1]) return new SolidColorBrush(TrueCase);
            else return new SolidColorBrush(FalseCase);
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringFormat : IMultiValueConverter, IValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Any(a => a == DependencyProperty.UnsetValue)) return string.Empty;

            string format = System.Convert.ToString(parameter);
            return string.Format(format, values);
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string format = System.Convert.ToString(parameter);
            return string.Format(format, value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !System.Convert.ToBoolean(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32(value) + System.Convert.ToInt32(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NotNullConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GenericNodeContainerTextConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string containerName = (string)value;
            if (string.IsNullOrEmpty(containerName))
                return null;

            if (containerName == Nodes.RacialTraits.ToString())
                return "Racial Traits: ";
            if (containerName == Nodes.ClassFeatures.ToString())
                return "Class Features: ";
            if (containerName == Nodes.Feats.ToString())
                return "Feats: ";
            if (containerName == Nodes.SpecialQualities.ToString())
                return "Special Qualities: ";

            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MaxWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double visibleSize = System.Convert.ToDouble(values[0]);

            TreeView tree = values[1] as TreeView;
            if (tree == null) return null;
            ItemsControl items = values[2] as ItemsControl;
            if (items == null) return null;
            Grid grid = values[3] as System.Windows.Controls.Grid;
            if (grid == null) return null;

            foreach (object property in tree.Items)
            {
                TreeViewItem tvi = tree.ItemContainerGenerator.ContainerFromItem(property) as TreeViewItem;
                if (tvi == null) continue;

                string key = (property as NodeContainer)?.Key;
                if (key == Nodes.RacialTraits.ToString() ||
                    key == Nodes.ClassFeatures.ToString() ||
                    key == Nodes.SpecialQualities.ToString() ||
                        key == Nodes.Feats.ToString())
                    continue;

                visibleSize = visibleSize > tvi.ActualWidth ? visibleSize : tvi.ActualWidth;
            }

            foreach (FrameworkElement child in grid.Children)
            {
                if (child == items) continue; // self
                if (child.ActualWidth != Double.NaN)
                    visibleSize = visibleSize - child.ActualWidth;
            }

            visibleSize -= 30; //expander for TreeViewItem

            if (visibleSize < 0)
                return 100;
            else
                return visibleSize;
        }

        public static T FindVisualParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AreEqualConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double value1 = System.Convert.ToDouble(values[0]);
            double value2 = System.Convert.ToDouble(values[1]);

            return Math.Ceiling(value1) == Math.Ceiling(value2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
