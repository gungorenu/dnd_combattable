/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CombatTable.Attached
{
    public class AttachedProperties : DependencyObject
    {
        public static bool GetHeaderEditor(DependencyObject obj)
        {
            return (bool)obj.GetValue(HeaderEditorProperty);
        }

        public static void SetHeaderEditor(DependencyObject obj, bool value)
        {
            obj.SetValue(HeaderEditorProperty, value);
        }

        public static bool GetDisableFocusBorder(DependencyObject obj)
        {
            return (bool)obj.GetValue(DisableFocusBorderProperty);
        }

        public static void SetDisableFocusBorder(DependencyObject obj, bool value)
        {
            obj.SetValue(DisableFocusBorderProperty, value);
        }

        public static bool GetSetReadOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(SetReadOnlyProperty);
        }

        public static void SetSetReadOnly(DependencyObject obj, bool value)
        {
            obj.SetValue(SetReadOnlyProperty, value);
        }


        private static void SetReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!(o is ComboBox)) return;

            if (Convert.ToBoolean(e.NewValue) && !Convert.ToBoolean(e.OldValue))
                (o as ComboBox).PreviewKeyDown += ComboBoxPreviewKeyDown;
            else if (!Convert.ToBoolean(e.NewValue) && Convert.ToBoolean(e.OldValue))
                (o as ComboBox).PreviewKeyDown -= ComboBoxPreviewKeyDown;
        }

        private static void ComboBoxPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo == null) return;

            if (combo.IsReadOnly) e.Handled = true;
        }

        public static readonly DependencyProperty SetReadOnlyProperty = DependencyProperty.RegisterAttached("SetReadOnly", typeof(bool), typeof(AttachedProperties), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(SetReadOnlyChanged)));
        public static readonly DependencyProperty DisableFocusBorderProperty = DependencyProperty.RegisterAttached("DisableFocusBorder", typeof(bool), typeof(AttachedProperties), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty HeaderEditorProperty = DependencyProperty.RegisterAttached("HeaderEditor", typeof(bool), typeof(AttachedProperties), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
    }
}
