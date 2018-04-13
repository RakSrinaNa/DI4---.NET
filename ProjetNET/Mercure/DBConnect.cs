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

        public SQLiteConnection GetConnection()
        {
            return Connection;
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
            long SFRef = CreateSF(Article.SelectSingleNode("sousFamille").InnerText, Article.SelectSingleNode("famille").InnerText);

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@ID, @Desc, @SF, @M, @PHT, 0)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", Article.SelectSingleNode("refArticle").InnerText);
            CommandInsert.Parameters.AddWithValue("@Desc", Article.SelectSingleNode("description").InnerText);
            CommandInsert.Parameters.AddWithValue("@SF", SFRef);
            CommandInsert.Parameters.AddWithValue("@M", CreateM(Article.SelectSingleNode("marque").InnerText));
            CommandInsert.Parameters.AddWithValue("@PHT", Double.Parse(Article.SelectSingleNode("prixHT").InnerText));

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new FieldAccessException("Inserting A failed");
        }

        public void UpdateArticle(XmlNode Article)
        {
            long SFRef = CreateSF(Article.SelectSingleNode("sousFamille").InnerText, Article.SelectSingleNode("famille").InnerText);

            SQLiteCommand CommandInsert = new SQLiteCommand("UPDATE Articles SET Description=@Desc, RefSousFamille=@SF, RefMarque=@M, PrixHT=@PHT WHERE RefArticle=@ID", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", Article.SelectSingleNode("refArticle").InnerText);
            CommandInsert.Parameters.AddWithValue("@Desc", Article.SelectSingleNode("description").InnerText);
            CommandInsert.Parameters.AddWithValue("@SF", SFRef);
            CommandInsert.Parameters.AddWithValue("@M", CreateM(Article.SelectSingleNode("marque").InnerText));
            CommandInsert.Parameters.AddWithValue("@PHT", Double.Parse(Article.SelectSingleNode("prixHT").InnerText));

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new FieldAccessException("Update A failed");
        }

        public long CreateSF(String Name, String Famille)
        {
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefSousFamille FROM SousFamilles WHERE Nom = @Name", Connection);
            CommandSelect.Parameters.AddWithValue("@Name", Name);

            SQLiteDataReader Result = CommandSelect.ExecuteReader();
            if (Result != null)
            {
                if (Result.Read())
                {
                    Object Obj = Result["RefSousFamille"];
                    if (Obj != System.DBNull.Value)
                        return Convert.ToInt64(Obj);
                }
                Result.Close();
            }
            else
            {
                throw new FieldAccessException("Getting SF failed");
            }

            long ID = 0;
            SQLiteCommand CommandID = new SQLiteCommand("SELECT MAX(RefSousFamille) AS ID FROM SousFamilles", Connection);
            SQLiteDataReader ResultID = CommandID.ExecuteReader();
            if (ResultID != null)
            {
                if (ResultID.Read())
                {
                    Object Obj = ResultID["ID"];
                    if (Obj != System.DBNull.Value)
                        ID = Convert.ToInt64(Obj);
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

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new FieldAccessException("Inserting SF failed");
            return ID;
        }

        public long CreateM(String Name)
        {
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefMarque FROM Marques WHERE Nom = @Name", Connection);
            CommandSelect.Parameters.AddWithValue("@Name", Name);

            SQLiteDataReader Result = CommandSelect.ExecuteReader();
            if (Result != null)
            {
                if (Result.Read())
                {
                    Object Obj = Result["RefMarque"];
                    if (Obj != System.DBNull.Value)
                        return Convert.ToInt64(Obj);
                }
                Result.Close();
            }
            else
            {
                throw new FieldAccessException("Getting M failed");
            }

            long ID = 0;
            SQLiteCommand CommandID = new SQLiteCommand("SELECT MAX(RefMarque) AS ID FROM Marques", Connection);
            SQLiteDataReader ResultID = CommandID.ExecuteReader();
            if (ResultID != null)
            {
                if (ResultID.Read())
                {
                    Object Obj = ResultID["ID"];
                    if (Obj != System.DBNull.Value)
                        ID = Convert.ToInt64(Obj);
                }
                ResultID.Close();
            }
            else
            {
                throw new FieldAccessException("Getting M failed");
            }

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO Marques (RefMarque, Nom) VALUES (@ID, @Name)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ++ID);
            CommandInsert.Parameters.AddWithValue("@Name", Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new FieldAccessException("Inserting M failed");
            return ID;
        }

        public long CreateF(String Name)
        {
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefFamille FROM Familles WHERE Nom = @Name", Connection);
            CommandSelect.Parameters.AddWithValue("@Name", Name);

            SQLiteDataReader Result = CommandSelect.ExecuteReader();
            if (Result != null)
            {
                if (Result.Read())
                {
                    Object Obj = Result["RefFamille"];
                    if (Obj != System.DBNull.Value)
                        return Convert.ToInt64(Obj);
                }
                Result.Close();
            }
            else
            {
                throw new FieldAccessException("Getting F failed");
            }

            long ID = 0;
            SQLiteCommand CommandID = new SQLiteCommand("SELECT MAX(RefFamille) AS ID FROM Familles", Connection);
            SQLiteDataReader ResultID = CommandID.ExecuteReader();
            if (ResultID != null)
            {
                if (ResultID.Read())
                {
                    Object Obj = ResultID["ID"];
                    if (Obj != System.DBNull.Value)
                        ID = Convert.ToInt64(Obj);
                }
                ResultID.Close();
            }
            else
            {
                throw new FieldAccessException("Getting F failed");
            }

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO Familles (RefFamille, Nom) VALUES (@ID, @Name)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ++ID);
            CommandInsert.Parameters.AddWithValue("@Name", Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new FieldAccessException("Inserting F failed");
            return ID;
        }

        public void Clear()
        {
            String[] Tables = {"Articles", "Familles", "Marques", "SousFamilles"};
            foreach(String Table in Tables)
            {
                SQLiteCommand CommandClear = new SQLiteCommand("DELETE FROM " + Table, Connection);
                CommandClear.ExecuteNonQuery();
            }
        }

        public bool ArticleExists(int ID)
        {
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefArticle FROM Articles WHERE RefArticle = @ID", Connection);
            CommandSelect.Parameters.AddWithValue("@ID", ID);

            SQLiteDataReader Result = CommandSelect.ExecuteReader();
            if (Result != null)
            {
                if (Result.Read())
                {
                    Object Obj = Result["RefArticle"];
                    if (Obj != System.DBNull.Value)
                        return Convert.ToInt64(Obj) != -1;
                }
                Result.Close();
            }
            else
            {
                throw new FieldAccessException("Getting A failed");
            }
            return false;
        }

        public void DeleteArticle(string Ref)
        {
            SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Articles WHERE RefArticle = @Ref", Connection);
            CommandDelete.Parameters.AddWithValue("@Ref", Ref);
            CommandDelete.ExecuteNonQuery();
        }

        public void DeleteBrand(long Ref)
        {
            {
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Articles WHERE RefMarque = @Ref", Connection);
                CommandDelete.Parameters.AddWithValue("@Ref", Ref);
                CommandDelete.ExecuteNonQuery();
            }
            {
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Marques WHERE RefMarque = @Ref", Connection);
                CommandDelete.Parameters.AddWithValue("@Ref", Ref);
                CommandDelete.ExecuteNonQuery();
            }
        }
    }
}
