/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CombatTable.Models.Base;
using System.Windows;

namespace CombatTable.Models.Custom
{
    public class PointOfInterest : BaseObject, ICellModel
    {
        private Point location;
        private string notes;

        public string Notes
        {
            get { return notes; }
            set
            {
                if (notes != value)
                {
                    notes = value;
                    Trigger("Notes");
                }
            }
        }

        public Point Location
        {
            get { return location; }
            set
            {
                if (location != value)
                {
                    location = value;
                    Trigger("Location");
                }
            }
        }

        Point ICellModel.Location
        {
            get { return this.location; }
        }

        protected internal override void ReadFromXml(System.Xml.XmlElement xel)
        {
            Location = new Point(ReadIntAttribute(xel, "x"), ReadIntAttribute(xel, "y"));
            Notes = ReadText(xel);
        }

        protected internal override void WriteToXml(System.Xml.XmlElement xel)
        {
            xel.RemoveAll();
            WriteAttribute(xel, "x", Location.X.ToString());
            WriteAttribute(xel, "y", Location.Y.ToString());
            WriteText(xel, Notes);
        }

    }
}
