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
    /// Interaction logic for Wall.xaml
    /// </summary>
    public partial class Wall : UserControl, ICellItem
    {
        public Wall()
        {
            InitializeComponent();
        }

        public void CellSizeChanged(int newCellSize)
        {
            double qr = (double)newCellSize / 12;
            DoorVertical.Points.Clear();
            DoorVertical.Points.Add(new Point(0, 0));
            DoorVertical.Points.Add(new Point(qr, 0));
            DoorVertical.Points.Add(new Point(qr, newCellSize));
            DoorVertical.Points.Add(new Point(-1 * qr, newCellSize));
            DoorVertical.Points.Add(new Point(-1 * qr, 0));
            DoorVertical.Points.Add(new Point(0, 0));

            DoorHorizontal.Points.Clear();
            DoorHorizontal.Points.Add(new Point(0, 0));
            DoorHorizontal.Points.Add(new Point(0, -1 * qr));
            DoorHorizontal.Points.Add(new Point(newCellSize, -1 * qr));
            DoorHorizontal.Points.Add(new Point(newCellSize, qr));
            DoorHorizontal.Points.Add(new Point(0, qr));
            DoorHorizontal.Points.Add(new Point(0, 0));
        }
    }
}
