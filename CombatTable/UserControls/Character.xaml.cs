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
    /// Interaction logic for Character.xaml
    /// </summary>
    public partial class Character : UserControl, ICellItem
    {
        public Character()
        {
            InitializeComponent();
        }

        public void CellSizeChanged(int newCellSize)
        {
            double half = (double)newCellSize / 2;
            double qr = (double)newCellSize / 6;

            this.Arrow.Points.Clear();
            this.Arrow.Points.Add(new Point(half, 0));
            this.Arrow.Points.Add(new Point(half - qr, qr));
            this.Arrow.Points.Add(new Point(half + qr, qr));
            this.Arrow.Points.Add(new Point(half, 0));
            (this.Arrow.RenderTransform as RotateTransform).CenterX = half;
            (this.Arrow.RenderTransform as RotateTransform).CenterY = half;
            //24,0 16,8 32,8 24,8        
        }
    }
}
