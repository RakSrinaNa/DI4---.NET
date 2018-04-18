using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Xml;

namespace ProjetNET
{
    /// <summary>
    /// Controller for all interactions with the database
    /// </summary>
    public class DBConnect
    {
        private static DBConnect INSTANCE = null;
        private SQLiteConnection Connection;

        /// <summary>
        /// Get an instance of the controller (create it if necessary)
        /// </summary>
        /// <returns>The instance</returns>
        public static DBConnect GetInstance()
        {
            return INSTANCE ?? (INSTANCE = new DBConnect());
        }

        /// <summary>
        /// Get the connection with the database
        /// </summary>
        /// <returns>The connection</returns>
        public SQLiteConnection GetConnection()
        {
            return Connection;
        }

        /// <summary>
        /// Private constructor for the database 'Mercure.SQLite'
        /// </summary>
        private DBConnect()
        {
            Connection = new SQLiteConnection("Data Source=Mercure.SQLite;Version=3;");
            Connection.Open();
            Console.Out.WriteLine("DB Opened");
            MainWindow.ChangeStripText("Connected to DB");
        }

        /// <summary>
        /// Close the connection with the database
        /// </summary>
        public void Close()
        {
            Connection.Close();
            Console.Out.WriteLine("DB Closed");
            MainWindow.ChangeStripText("Disconnected from DB");
        }

        /// <summary>
        /// Add the given article to the database
        /// </summary>
        /// <param name="Article">The article</param>
        /// <returns>True if success, False otherwise</returns>
        public bool AddArticle(XmlNode Article)
        {
            long SFRef = CreateSubFamily(Article.SelectSingleNode("sousFamille").InnerText, Article.SelectSingleNode("famille").InnerText);

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@ID, @Desc, @SF, @M, @PHT, 1)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", Article.SelectSingleNode("refArticle").InnerText);
            CommandInsert.Parameters.AddWithValue("@Desc", Article.SelectSingleNode("description").InnerText);
            CommandInsert.Parameters.AddWithValue("@SF", SFRef);
            CommandInsert.Parameters.AddWithValue("@M", CreateBrand(Article.SelectSingleNode("marque").InnerText));
            CommandInsert.Parameters.AddWithValue("@PHT", Double.Parse(Article.SelectSingleNode("prixHT").InnerText));

            return CommandInsert.ExecuteNonQuery() == 1;
        }

        /// <summary>
        /// Update the given article into the database
        /// </summary>
        /// <param name="Article">The article</param>
        /// <returns>True if success, False otherwise</returns>
        public bool UpdateArticle(XmlNode Article)
        {
            long SFRef = CreateSubFamily(Article.SelectSingleNode("sousFamille").InnerText, Article.SelectSingleNode("famille").InnerText);

            SQLiteCommand CommandInsert = new SQLiteCommand("UPDATE Articles SET Description=@Desc, RefSousFamille=@SF, RefMarque=@M, PrixHT=@PHT WHERE RefArticle=@ID", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", Article.SelectSingleNode("refArticle").InnerText);
            CommandInsert.Parameters.AddWithValue("@Desc", Article.SelectSingleNode("description").InnerText);
            CommandInsert.Parameters.AddWithValue("@SF", SFRef);
            CommandInsert.Parameters.AddWithValue("@M", CreateBrand(Article.SelectSingleNode("marque").InnerText));
            CommandInsert.Parameters.AddWithValue("@PHT", Double.Parse(Article.SelectSingleNode("prixHT").InnerText));

            return CommandInsert.ExecuteNonQuery() == 1;
        }

        /// <summary>
        /// Add the given subfamily to the database
        /// </summary>
        /// <param name="Name">The name of the new subfamily</param>
        /// <param name="Famille">The id of its family</param>
        /// <returns>The id of the newly created subfamily</returns>
        public long CreateSubFamily(String Name, long Famille)
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
                throw new Exception("Getting SF failed");
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
                throw new Exception("Getting SF failed");
            }

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO SousFamilles (RefSousFamille, RefFamille, Nom) VALUES (@ID, @RefF, @Name)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ++ID);
            CommandInsert.Parameters.AddWithValue("@RefF", Famille);
            CommandInsert.Parameters.AddWithValue("@Name", Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new Exception("Inserting SF failed");
            return ID;
        }

        /// <summary>
        /// Add the given subfamily to the database
        /// </summary>
        /// <param name="Name">The name of the new subfamily</param>
        /// <param name="Famille">The name of its family</param>
        /// <returns>The id of the newly created subfamily</returns>
        public long CreateSubFamily(String Name, String Famille)
        {
            return CreateSubFamily(Name, CreateFamily(Famille));
        }

        /// <summary>
        /// Add the given brand to the database
        /// </summary>
        /// <param name="Name">The name of the new brand</param>
        /// <returns>The id of the newmy created brand</returns>
        public long CreateBrand(String Name)
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
                throw new Exception("Getting M failed");
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
                throw new Exception("Getting M failed");
            }

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO Marques (RefMarque, Nom) VALUES (@ID, @Name)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ++ID);
            CommandInsert.Parameters.AddWithValue("@Name", Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new Exception("Inserting M failed");
            return ID;
        }

        /// <summary>
        /// Add the given family to the database
        /// </summary>
        /// <param name="Name">The name of the new family</param>
        /// <returns>The id of the newly created family</returns>
        public long CreateFamily(String Name)
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
                throw new Exception("Getting F failed");
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
                throw new Exception("Getting F failed");
            }

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO Familles (RefFamille, Nom) VALUES (@ID, @Name)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ++ID);
            CommandInsert.Parameters.AddWithValue("@Name", Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new Exception("Inserting F failed");
            return ID;
        }

        /// <summary>
        /// Empty all the database
        /// </summary>
        public void Clear()
        {
            String[] Tables = {"Articles", "Familles", "Marques", "SousFamilles"};
            foreach(String Table in Tables)
            {
                SQLiteCommand CommandClear = new SQLiteCommand("DELETE FROM " + Table, Connection);
                CommandClear.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Tells if an article exists by its reference
        /// </summary>
        /// <param name="ID">The reference to look for</param>
        /// <returns>True if the article exists, False otherwise</returns>
        public bool ArticleExists(string ID)
        {
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefArticle FROM Articles WHERE RefArticle = @ID", Connection);
            CommandSelect.Parameters.AddWithValue("@ID", ID);

            SQLiteDataReader Result = CommandSelect.ExecuteReader();
            if (Result != null)
            {
                if (Result.Read())
                    return true;
                Result.Close();
            }
            else
            {
                throw new Exception("Getting A failed");
            }
            return false;
        }

        /// <summary>
        /// Delete an article by its reference
        /// </summary>
        /// <param name="Ref">The reference</param>
        public void DeleteArticle(string Ref)
        {
            SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Articles WHERE RefArticle = @Ref", Connection);
            CommandDelete.Parameters.AddWithValue("@Ref", Ref);
            CommandDelete.ExecuteNonQuery();
        }

        /// <summary>
        /// Delete a brand by its reference
        /// </summary>
        /// <param name="Ref">The reference</param>
        public void DeleteBrand(long Ref)
        {
            var Transaction = Connection.BeginTransaction();
            {
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Articles WHERE RefMarque = @Ref", Connection);
                CommandDelete.Parameters.AddWithValue("@Ref", Ref);
                int Modified = CommandDelete.ExecuteNonQuery();
                if (Modified > 0)
                {
                    Transaction.Rollback();
                    return;
                }
            }
            {
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Marques WHERE RefMarque = @Ref", Connection);
                CommandDelete.Parameters.AddWithValue("@Ref", Ref);
                CommandDelete.ExecuteNonQuery();
            }
            Transaction.Commit();
        }

        /// <summary>
        /// Delete a family by its reference
        /// </summary>
        /// <param name="Ref">The reference</param>
        public void DeleteFamily(long Ref)
        {
            var Transaction = Connection.BeginTransaction();
            {
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Familles WHERE RefFamille = @Ref", Connection);
                CommandDelete.Parameters.AddWithValue("@Ref", Ref);
                CommandDelete.ExecuteNonQuery();
            }

            LinkedList<long> SFToDel = new LinkedList<long>();
            {
                SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefSousFamille FROM SousFamilles WHERE RefFamille = @Ref", Connection);
                CommandSelect.Parameters.AddWithValue("@Ref", Ref);

                SQLiteDataReader Result = CommandSelect.ExecuteReader();
                if (Result != null)
                {
                    while (Result.Read())
                    {
                        Object Obj = Result["RefSousFamille"];
                        if (Obj != System.DBNull.Value)
                        {
                            SFToDel.AddLast(Convert.ToInt64(Obj));
                        }
                    }
                    Result.Close();
                }
            }

            if (SFToDel.Count > 0)
            {
                Transaction.Rollback();
                return;
            }
            foreach (long RefSF in SFToDel)
            {
                {
                    SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM SousFamilles WHERE RefSousFamille = @Ref", Connection);
                    CommandDelete.Parameters.AddWithValue("@Ref", RefSF);
                    CommandDelete.ExecuteNonQuery();
                }
                {
                    SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Articles WHERE RefSousFamille = @Ref", Connection);
                    CommandDelete.Parameters.AddWithValue("@Ref", RefSF);
                    CommandDelete.ExecuteNonQuery();
                }
            }
            Transaction.Commit();
        }

        /// <summary>
        /// Delete a subfamily by its reference
        /// </summary>
        /// <param name="Ref">The reference</param>
        public void DeleteSubFamily(long Ref)
        {
            var Transaction = Connection.BeginTransaction();
            {
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM SousFamilles WHERE RefSousFamille = @Ref", Connection);
                CommandDelete.Parameters.AddWithValue("@Ref", Ref);
                CommandDelete.ExecuteNonQuery();
            }

            {
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Articles WHERE RefSousFamille = @Ref", Connection);
                CommandDelete.Parameters.AddWithValue("@Ref", Ref);
                int Modified = CommandDelete.ExecuteNonQuery();
                if (Modified > 0)
                {
                    Transaction.Rollback();
                    return;
                }
            }

            LinkedList<long> FToDel = new LinkedList<long>();
            {
                SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefFamille FROM SousFamilles WHERE RefSousFamille = @Ref", Connection);
                CommandSelect.Parameters.AddWithValue("@Ref", Ref);

                SQLiteDataReader Result = CommandSelect.ExecuteReader();
                if (Result != null)
                {
                    if(Result.Read())
                    {
                        Object Obj = Result["RefFamille"];
                        if (Obj != System.DBNull.Value)
                        {
                            FToDel.AddLast(Convert.ToInt64(Obj));
                        }
                    }
                    Result.Close();
                }
            }
            if (FToDel.Count > 0)
            {
                Transaction.Rollback();
                return;
            }

            foreach (long RefF in FToDel)
            {
                    SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Familles WHERE RefFamille = @Ref", Connection);
                    CommandDelete.Parameters.AddWithValue("@Ref", RefF);
                    CommandDelete.ExecuteNonQuery();
            }

            Transaction.Commit();
        }

        /// <summary>
        /// Update (or insert if it doesn't exist) an article in the database
        /// </summary>
        /// <param name="Article">The article</param>
        public void UpdateOrCreateArticle(Article Article)
        {
            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT OR REPLACE INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@ID, @Desc, @SF, @M, @PHT, @Q)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", Article.Reference);
            CommandInsert.Parameters.AddWithValue("@Desc", Article.Description);
            CommandInsert.Parameters.AddWithValue("@SF", Article.SubFamily);
            CommandInsert.Parameters.AddWithValue("@M", Article.Brand);
            CommandInsert.Parameters.AddWithValue("@PHT", Article.Price);
            CommandInsert.Parameters.AddWithValue("@Q", Article.Quantity);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new Exception("Inserting A failed");
        }

        /// <summary>
        /// Update (or insert if it doesn't exist) a brand in the database
        /// </summary>
        /// <param name="Brand">The brand</param>
        public void UpdateOrCreateBrand(Brand Brand)
        {
            long ID = -1;
            if (Brand.Reference == -1)
            {
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
                    ID++;
                }
                else
                    throw new Exception("Getting brand ID failed");
            }
            else
                ID = Brand.Reference;

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT OR REPLACE INTO Marques (RefMarque, Nom) VALUES (@ID, @Name)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ID);
            CommandInsert.Parameters.AddWithValue("@Name", Brand.Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new Exception("Inserting M failed");
        }

        /// <summary>
        /// Update (or insert if it doesn't exist) a family in the database
        /// </summary>
        /// <param name="Family">The family</param>
        public void UpdateOrCreateFamily(Family Family)
        {
            long ID = -1;
            if (Family.Reference == -1)
            {
                SQLiteCommand CommandID = new SQLiteCommand("SELECT MAX(RefFamily) AS ID FROM Familles", Connection);
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
                    ID++;
                }
                else
                    throw new Exception("Getting family ID failed");
            }
            else
                ID = Family.Reference;

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT OR REPLACE INTO Familles (RefFamille, Nom) VALUES (@ID, @Name)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ID);
            CommandInsert.Parameters.AddWithValue("@Name", Family.Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new Exception("Inserting F failed");
        }

        /// <summary>
        /// Update (or insert if it doesn't exist) a subfamily in the database
        /// </summary>
        /// <param name="SubFamily">The subfamily</param>
        public void UpdateOrCreateSubFamily(SubFamily SubFamily)
        {
            long ID = -1;
            if (SubFamily.Reference == -1)
            {
                SQLiteCommand CommandID = new SQLiteCommand("SELECT MAX(RefSousFamily) AS ID FROM SousFamilles", Connection);
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
                    ID++;
                }
                else
                    throw new Exception("Getting subfamily ID failed");
            }
            else
                ID = SubFamily.Reference;

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT OR REPLACE INTO SousFamilles (RefSousFamille, RefFamille, Nom) VALUES (@ID, @IDFamille, @Name)", Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ID);
            CommandInsert.Parameters.AddWithValue("@IDFamille", SubFamily.FamilyReference);
            CommandInsert.Parameters.AddWithValue("@Name", SubFamily.Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new Exception("Inserting SF failed");
        }
    }
}
