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
    /// Interaction logic for Block .xaml
    /// </summary>
    public partial class Effect : UserControl, ICellItem
    {
        public Effect()
        {
            InitializeComponent();
        }

        public void CellSizeChanged(int newCellSize)
        {
        }

    }
}
