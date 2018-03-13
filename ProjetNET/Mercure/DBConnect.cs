using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Xml;

namespace ProjetNET
{
    public class DBConnect
    {
        private static DBConnect INSTANCE = null;
        private SQLiteConnection Connection;

        public static DBConnect GetInstance()
        {
            if(INSTANCE == null)
                INSTANCE = new DBConnect();
            return INSTANCE;
        }

        private DBConnect()
        {
            Connection = new SQLiteConnection("Data Source=Mercure.SQLite;Version=3;");
            Connection.Open();
            Console.Out.WriteLine("DB Opened");
        }

        public void Close()
        {
            Connection.Close();
            Console.Out.WriteLine("DB Closed");
        }

        public void LoadXml(XmlDocument Xml)
        {
            XmlNodeList NodeList = Xml.SelectNodes("/materiels/article");

        }


    }

}
