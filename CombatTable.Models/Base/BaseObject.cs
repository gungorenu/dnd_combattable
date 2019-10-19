/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;

namespace CombatTable.Models.Base
{
    public abstract class BaseObject : Notifier
    {
        #region Virtual Members

        protected virtual BaseObject ParentObject 
        {
            get { return null; }
            set {  }
        }

        internal protected abstract void ReadFromXml(XmlElement xel);

        internal protected abstract void WriteToXml(XmlElement xel);
        #endregion

        #region Static Xml I/O Members
        protected static string ReadAttribute(XmlElement xel, string attribute)
        {
            XmlNode xn = xel.SelectSingleNode(string.Format("@{0}", attribute));
            if (xn == null) return string.Empty;
            else return xn.Value.Trim();
        }
        protected static int ReadIntAttribute(XmlElement xel, string attribute)
        {
            string text = ReadAttribute(xel, attribute);
            int value = 0;
            if (int.TryParse(text, out value)) return value;
            else return 0;
        }
        protected static string ReadText(XmlElement xel)
        {
            XmlNode xn = xel.SelectSingleNode("text()");
            if (xn == null) return string.Empty;
            else return xn.Value.Trim();
        }
        protected static int ReadInt(XmlElement xel)
        {
            string text = ReadText(xel);
            int value = 0;
            if (int.TryParse(text, out value)) return value;
            else return 0;
        }
        protected static string ReadCData(XmlElement xel, string tag)
        {
            XmlNode xn = xel.SelectSingleNode(string.Format("{0}/text()", tag));
            if (xn == null) return string.Empty;
            else return xn.Value.Trim();
        }
        protected static int ReadIntCData(XmlElement xel, string tag)
        {
            string text = ReadCData(xel, tag);
            int value = 0;
            if (int.TryParse(text, out value)) return value;
            else return 0;
        }
        protected static XmlElement Read(XmlElement xel, string tag)
        {
            XmlNode xn = xel.SelectSingleNode(tag);
            return xn as XmlElement;
        }
        protected static void ReadCollection<T>(XmlElement xel, string collectionTag, string childTag, ObservableCollection<T> collection, BaseObject parent)
            where T : BaseObject, new()
        {
            collection.Clear();

            XmlElement sel = xel;
            if (!string.IsNullOrEmpty(collectionTag))
                sel = xel.SelectSingleNode(collectionTag) as XmlElement;

            if (sel != null)
            {
                XmlNodeList xnl = sel.SelectNodes(childTag);
                if (xnl != null)
                {
                    foreach (XmlNode xn in xnl)
                    {
                        T item = new T();
                        item.ReadFromXml(xn as XmlElement);
                        collection.Add(item);
                        item.ParentObject = parent;
                    }
                }
            }
        }
        
        protected static void WriteAttribute(XmlElement xel, string attribute, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            XmlAttribute xat = xel.OwnerDocument.CreateAttribute(attribute);
            xat.Value = value;
            xel.Attributes.Append(xat);
        }
        protected static void WriteAttribute(XmlElement xel, string attribute, int value)
        {
            if (value == 0) return;

            WriteAttribute(xel, attribute, value.ToString());
        }
        protected static void WriteText(XmlElement xel, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            XmlText xt = xel.OwnerDocument.CreateTextNode(value);
            xel.AppendChild(xt);
        }
        protected static void WriteInt(XmlElement xel, int value)
        {
            if (value == 0) return;

            WriteText(xel, value.ToString());
        }
        protected static void WriteCData(XmlElement xel, string tag, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            XmlElement xel2 = xel.OwnerDocument.CreateElement(tag);
            xel.AppendChild(xel2);
            XmlCDataSection xcd = xel.OwnerDocument.CreateCDataSection(value);
            xel2.AppendChild(xcd);
        }
        protected static void WriteCData(XmlElement xel, string tag, int value)
        {
            if (value == 0) return;

            WriteCData(xel, tag, value.ToString());
        }
        protected static XmlElement Write(XmlElement xel, string tag)
        {
            XmlElement xel2 = xel.OwnerDocument.CreateElement(tag);
            xel.AppendChild(xel2);
            return xel2;
        }
        protected static void WriteCollection<T>(XmlElement xel, string collectionTag, string childTag, ObservableCollection<T> collection)
            where T : BaseObject, new()
        {
            XmlElement parent = xel;
            if (!string.IsNullOrEmpty(collectionTag))
            {
                parent = xel.OwnerDocument.CreateElement(collectionTag);
                xel.AppendChild(parent);
            }

            foreach (T item in collection)
            {
                XmlElement child = xel.OwnerDocument.CreateElement(childTag);
                item.WriteToXml(child);
                parent.AppendChild(child);
            }
        }
        #endregion
    }
}
