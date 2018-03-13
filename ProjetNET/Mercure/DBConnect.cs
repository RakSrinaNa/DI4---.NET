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

        public void AddArticle(XmlNode Article)
        {
            Int64 SFRef = CreateSF(Article.SelectSingleNode("sousFamille").InnerText, Article.SelectSingleNode("famille").InnerText);
        }

        public Int64 CreateSF(String Name, String Famille)
        {
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefSousFamille FROM SousFamilles WHERE Nom = @Name", Connection);
            CommandSelect.Parameters.AddWithValue("@Name", Name);

            SQLiteDataReader Result = CommandSelect.ExecuteReader();
            if (Result != null)
            {
                if (Result.Read())
                {
                    return (int)Result.GetValue(0);
                }
                Result.Close();
            }
            else
            {
                throw new FieldAccessException("Getting SF failed");
            }

            Int64 ID = 0;
            SQLiteCommand CommandID = new SQLiteCommand("SELECT MAX(RefSousFamille) FROM SousFamilles", Connection);
            SQLiteDataReader ResultID = CommandID.ExecuteReader();
            if (ResultID != null)
            {
                if (ResultID.Read())
                {
                    ID = (Int64)ResultID.GetValue(0);
                }
                ResultID.Close();
            }
            else
            {
                throw new FieldAccessException("Getting SF failed");
            }

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO SousFamilles (RefSousFamille, RefFamille, Nom) VALUES (@ID, @RefF, @Name)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ++ID);
            CommandInsert.Parameters.AddWithValue("@RefF", CreateF(Famille));
            CommandInsert.Parameters.AddWithValue("@Name", Name);

            int ResultInsert = (int)CommandInsert.ExecuteScalar();
            if (ResultInsert != 1)
                throw new FieldAccessException("Inserting SF failed");
            return ID;
        }

        public Int64 CreateF(String Name)
        {
            return 1000;
        }
    }
}
