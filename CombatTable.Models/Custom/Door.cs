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
using System.Xml;

namespace CombatTable.Models.Custom
{
    public class Door : BaseObject, ICellModel
    {
        private Point first, second;
        private bool isOpen;

        public bool IsVertical
        {
            get { return first.X == second.X; }
        }
        public bool IsHorizontal
        {
            get { return first.Y == second.Y; }
        }

        public Point First
        {
            get { return first; }
            set
            {
                if (first != value)
                {
                    first = value;
                    TriggerAll();
                }
            }
        }

        public Point Second
        {
            get { return second; }
            set
            {
                if (second != value)
                {
                    second = value;
                    TriggerAll();
                }
            }
        }

        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                if (isOpen != value)
                {
                    isOpen = value;
                    Trigger("IsOpen");
                }
            }
        }

        Point ICellModel.Location
        {
            get { return First; }
        }

        protected internal override void ReadFromXml(System.Xml.XmlElement xel)
        {
            IsOpen = Convert.ToBoolean(ReadAttribute(xel, "isOpen"));
            XmlElement xfirst = Read(xel, "door.first");
            XmlElement xsecond = Read(xel, "door.second");
            First = new Point(ReadIntAttribute(xfirst, "x"), ReadIntAttribute(xfirst, "y"));
            Second = new Point(ReadIntAttribute(xsecond, "x"), ReadIntAttribute(xsecond, "y"));
        }

        protected internal override void WriteToXml(System.Xml.XmlElement xel)
        {
            xel.RemoveAll();
            WriteAttribute(xel, "isOpen", IsOpen.ToString());
            XmlElement xfirst = Write(xel, "door.first");
            XmlElement xsecond = Write(xel, "door.second");
            WriteAttribute(xfirst, "x", First.X.ToString());
            WriteAttribute(xfirst, "y", First.Y.ToString());
            WriteAttribute(xsecond, "x", Second.X.ToString());
            WriteAttribute(xsecond, "y", Second.Y.ToString());
        }
    }
}
