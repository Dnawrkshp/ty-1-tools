using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TyModManager.Extension
{
    public static class XmlNodeExtensions
    {

        public static bool HasTextChild(this XmlNode node)
        {
            if (!node.HasChildNodes)
                return false;

            foreach (XmlNode child in node.ChildNodes)
                if (child.NodeType == XmlNodeType.Text)
                    return true;

            return false;
        }

        public static string GetFirstTextChild(this XmlNode node)
        {
            if (!node.HasChildNodes)
                return null;

            foreach (XmlNode child in node.ChildNodes)
                if (child.NodeType == XmlNodeType.Text)
                    return child.Value;

            return null;
        }
    }
}
