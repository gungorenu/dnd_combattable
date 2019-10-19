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
    public class Wall : BaseObject, ICellModel
    {
        private Point first, second;
        private bool isBlock = true;

        public bool IsVertical
        {
            get { return first.X == second.X; }
        }
        public bool IsHorizontal
        {
            get { return first.Y == second.Y; }
        }

        public bool IsBlock
        {
            get { return isBlock; }
            set
            {
                if (isBlock != value)
                {
                    isBlock = value;
                    Trigger("IsBlock");
                }
            }
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

        Point ICellModel.Location
        {
            get { return First; }
        }

        protected internal override void ReadFromXml(System.Xml.XmlElement xel)
        {
            XmlElement xfirst = Read(xel, "wall.first");
            XmlElement xsecond = Read(xel, "wall.second");
            First = new Point(ReadIntAttribute(xfirst, "x"), ReadIntAttribute(xfirst, "y"));
            Second = new Point(ReadIntAttribute(xsecond, "x"), ReadIntAttribute(xsecond, "y"));
        }

        protected internal override void WriteToXml(System.Xml.XmlElement xel)
        {
            xel.RemoveAll();
            XmlElement xfirst = Write(xel, "wall.first");
            XmlElement xsecond = Write(xel, "wall.second");
            WriteAttribute(xfirst, "x", First.X.ToString());
            WriteAttribute(xfirst, "y", First.Y.ToString());
            WriteAttribute(xsecond, "x", Second.X.ToString());
            WriteAttribute(xsecond, "y", Second.Y.ToString());
        }

    }
}
