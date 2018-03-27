using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ProjetNET
{
    class Parser
    {

        public static XmlDocument ParseXML(String FileName)
        {
            XmlDocument Xml = new XmlDocument();
            Xml.Load(FileName);
            return Xml;
        }
    }

}
