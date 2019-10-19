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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CombatTable.UserControls
{
    /// <summary>
    /// Interaction logic for Door.xaml
    /// </summary>
    public partial class Door : UserControl, ICellItem
    {
        public Door()
        {
            InitializeComponent();
        }

        public void CellSizeChanged(int newCellSize)
        {
            double qr = (double)newCellSize / 6;

            DoorVertical.Points.Clear();
            DoorVertical.Points.Add(new Point(0, 0));
            DoorVertical.Points.Add(new Point(-1 * qr, qr));
            DoorVertical.Points.Add(new Point(-1 * qr, (double)newCellSize - qr));
            DoorVertical.Points.Add(new Point(0, newCellSize));
            DoorVertical.Points.Add(new Point(qr, (double)newCellSize - qr));
            DoorVertical.Points.Add(new Point(qr, qr));
            DoorVertical.Points.Add(new Point(0, 0));

            DoorHorizontal.Points.Clear();
            DoorHorizontal.Points.Add(new Point(0, 0));
            DoorHorizontal.Points.Add(new Point(qr, qr * -1));
            DoorHorizontal.Points.Add(new Point((double)newCellSize - qr, qr * -1));
            DoorHorizontal.Points.Add(new Point(newCellSize, 0));
            DoorHorizontal.Points.Add(new Point((double)newCellSize - qr, qr));
            DoorHorizontal.Points.Add(new Point(qr, qr));
            DoorHorizontal.Points.Add(new Point(0, 0));
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                Models.Custom.Door dr = DataContext as Models.Custom.Door;
                dr.IsOpen = !dr.IsOpen;
                e.Handled = true;
            }
        }
    }
}
