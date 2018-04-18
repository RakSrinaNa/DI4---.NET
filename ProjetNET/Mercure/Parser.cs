using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ProjetNET
{
    /// <summary>
    /// Parser from a XML file
    /// </summary>
    class Parser
    {
        /// <summary>
        /// Get the content of a XML file
        /// </summary>
        /// <param name="FileName">The path to the file to parse</param>
        /// <returns>The XmlDocument object containing the data</returns>
        public static XmlDocument ParseXML(String FileName)
        {
            XmlDocument Xml = new XmlDocument();
            Xml.Load(FileName);
            return Xml;
        }
    }

}
