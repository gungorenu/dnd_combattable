/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using CombatTable.Models.Base;
using System;
using System.Windows;
using System.Xml;
namespace CombatTable.Models.Custom
{
    public class Block : BaseObject, ICellModel
    {
        private Point location;
        private string notes;
        public string Notes
        {
            get
            {
                return this.notes;
            }
            set
            {
                if (this.notes != value)
                {
                    this.notes = value;
                    base.Trigger("Notes");
                }
            }
        }
        public Point Location
        {
            get
            {
                return this.location;
            }
            set
            {
                if (this.location != value)
                {
                    this.location = value;
                    base.Trigger("Location");
                }
            }
        }
        Point ICellModel.Location
        {
            get
            {
                return this.location;
            }
        }
        protected internal override void ReadFromXml(XmlElement xel)
        {
            this.Location = new Point((double)BaseObject.ReadIntAttribute(xel, "x"), (double)BaseObject.ReadIntAttribute(xel, "y"));
            this.Notes = BaseObject.ReadText(xel);
        }
        protected internal override void WriteToXml(XmlElement xel)
        {
            xel.RemoveAll();
            BaseObject.WriteAttribute(xel, "x", this.Location.X.ToString());
            BaseObject.WriteAttribute(xel, "y", this.Location.Y.ToString());
            BaseObject.WriteText(xel, this.Notes);
        }
    }
}
