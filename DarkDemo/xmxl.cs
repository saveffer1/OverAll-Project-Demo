using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace xmxl
{
        /*
    class xmml
    {
        (String pname, String pip, String pport, String pdb, String pusr, String ppass, String pevery) Readxmxl(String id)
        {
            // Start with XmlReader object  
            //here, we try to setup Stream between the XML file nad xmlReader  
            XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
            xmlDoc.Load("config.xml"); // Load the XML document from the specified file

            // Get elements
            XmlNodeList gname = xmlDoc.GetElementsByTagName("name");
            XmlNodeList gip = xmlDoc.GetElementsByTagName("ip");
            XmlNodeList gport = xmlDoc.GetElementsByTagName("port");
            XmlNodeList gdb = xmlDoc.GetElementsByTagName("database");
            XmlNodeList gusr = xmlDoc.GetElementsByTagName("usr");
            XmlNodeList gpass = xmlDoc.GetElementsByTagName("passwd");
            XmlNodeList gevery = xmlDoc.GetElementsByTagName("send_data_every_millisec");

            // Set Result

            var pname = id + gname[0].InnerText;
            var pip = id + gip[0].InnerText;
            var pport = id + gport[0].InnerText;
            var pdb = id + gdb[0].InnerText;
            var pusr = id + gusr[0].InnerText;
            var ppass = id + gpass[0].InnerText;
            var pevery = id + gevery[0].InnerText;

            return (pname, pip, pport, pdb, pusr, ppass, pevery);
        }

    }*/

}