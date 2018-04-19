using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Xml;

namespace ProjetNET
{
    /// <summary>
    /// Controller for all interactions with the database
    /// </summary>
    public class DbConnect
    {
        private static DbConnect _Instance;
        private readonly SQLiteConnection _Connection;

        /// <summary>
        /// Get an instance of the controller (create it if necessary)
        /// </summary>
        /// <returns>The instance</returns>
        public static DbConnect GetInstance()
        {
            return _Instance ?? (_Instance = new DbConnect());
        }

        /// <summary>
        /// Get the connection with the database
        /// </summary>
        /// <returns>The connection</returns>
        public SQLiteConnection GetConnection()
        {
            return _Connection;
        }

        /// <summary>
        /// Private constructor for the database 'Mercure.SQLite'
        /// </summary>
        private DbConnect()
        {
            _Connection = new SQLiteConnection("Data Source=Mercure.SQLite;Version=3;");
            _Connection.Open();
            Console.Out.WriteLine("DB Opened");
            MainWindow.ChangeStripText("Connected to DB");
        }

        /// <summary>
        /// Close the connection with the database
        /// </summary>
        public void Close()
        {
            _Connection.Close();
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
            long SfRef = CreateSubFamily(Article.SelectSingleNode("sousFamille").InnerText, Article.SelectSingleNode("famille").InnerText);

            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@ID, @Desc, @SF, @M, @PHT, 1)", _Connection);
            CommandInsert.Parameters.AddWithValue("@ID", Article.SelectSingleNode("refArticle").InnerText);
            CommandInsert.Parameters.AddWithValue("@Desc", Article.SelectSingleNode("description").InnerText);
            CommandInsert.Parameters.AddWithValue("@SF", SfRef);
            CommandInsert.Parameters.AddWithValue("@M", CreateBrand(Article.SelectSingleNode("marque").InnerText));
            CommandInsert.Parameters.AddWithValue("@PHT", double.Parse(Article.SelectSingleNode("prixHT").InnerText));

            return CommandInsert.ExecuteNonQuery() == 1;
        }

        /// <summary>
        /// Update the given article into the database
        /// </summary>
        /// <param name="Article">The article</param>
        /// <returns>True if success, False otherwise</returns>
        public bool UpdateArticle(XmlNode Article)
        {
            long SfRef = CreateSubFamily(Article.SelectSingleNode("sousFamille").InnerText, Article.SelectSingleNode("famille").InnerText);

            SQLiteCommand CommandInsert = new SQLiteCommand("UPDATE Articles SET Description=@Desc, RefSousFamille=@SF, RefMarque=@M, PrixHT=@PHT WHERE RefArticle=@ID", _Connection);
            CommandInsert.Parameters.AddWithValue("@ID", Article.SelectSingleNode("refArticle").InnerText);
            CommandInsert.Parameters.AddWithValue("@Desc", Article.SelectSingleNode("description").InnerText);
            CommandInsert.Parameters.AddWithValue("@SF", SfRef);
            CommandInsert.Parameters.AddWithValue("@M", CreateBrand(Article.SelectSingleNode("marque").InnerText));
            CommandInsert.Parameters.AddWithValue("@PHT", double.Parse(Article.SelectSingleNode("prixHT").InnerText));

            return CommandInsert.ExecuteNonQuery() == 1;
        }

        /// <summary>
        /// Add the given subfamily to the database
        /// </summary>
        /// <param name="Name">The name of the new subfamily</param>
        /// <param name="Famille">The id of its family</param>
        /// <returns>The id of the newly created subfamily</returns>
        public long CreateSubFamily(string Name, long Famille)
        {
            //Get already existing sub family
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefSousFamille FROM SousFamilles WHERE Nom = @Name", _Connection);
            CommandSelect.Parameters.AddWithValue("@Name", Name);

            SQLiteDataReader Result = CommandSelect.ExecuteReader();
            if (Result != null)
            {
                if (Result.Read())
                {
                    object Obj = Result["RefSousFamille"];
                    if (Obj != DBNull.Value)
                        return Convert.ToInt64(Obj);
                }
                Result.Close();
            }
            else
            {
                throw new Exception("Getting SF failed");
            }

            //Get next ID
            long Id = 0;
            SQLiteCommand CommandId = new SQLiteCommand("SELECT MAX(RefSousFamille) AS ID FROM SousFamilles", _Connection);
            SQLiteDataReader ResultId = CommandId.ExecuteReader();
            if (ResultId != null)
            {
                if (ResultId.Read())
                {
                    object Obj = ResultId["ID"];
                    if (Obj != DBNull.Value)
                        Id = Convert.ToInt64(Obj);
                }
                ResultId.Close();
            }
            else
            {
                throw new Exception("Getting SF failed");
            }

            //Insert subfamily
            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO SousFamilles (RefSousFamille, RefFamille, Nom) VALUES (@ID, @RefF, @Name)", _Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ++Id);
            CommandInsert.Parameters.AddWithValue("@RefF", Famille);
            CommandInsert.Parameters.AddWithValue("@Name", Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new Exception("Inserting SF failed");
            return Id;
        }

        /// <summary>
        /// Add the given subfamily to the database
        /// </summary>
        /// <param name="Name">The name of the new subfamily</param>
        /// <param name="Famille">The name of its family</param>
        /// <returns>The id of the newly created subfamily</returns>
        public long CreateSubFamily(string Name, string Famille)
        {
            return CreateSubFamily(Name, CreateFamily(Famille));
        }

        /// <summary>
        /// Add the given brand to the database
        /// </summary>
        /// <param name="Name">The name of the new brand</param>
        /// <returns>The id of the newmy created brand</returns>
        public long CreateBrand(string Name)
        {
            //Get the existing brand
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefMarque FROM Marques WHERE Nom = @Name", _Connection);
            CommandSelect.Parameters.AddWithValue("@Name", Name);

            SQLiteDataReader Result = CommandSelect.ExecuteReader();
            if (Result != null)
            {
                if (Result.Read())
                {
                    object Obj = Result["RefMarque"];
                    if (Obj != DBNull.Value)
                        return Convert.ToInt64(Obj);
                }
                Result.Close();
            }
            else
            {
                throw new Exception("Getting M failed");
            }

            //Get the next ID
            long Id = 0;
            SQLiteCommand CommandId = new SQLiteCommand("SELECT MAX(RefMarque) AS ID FROM Marques", _Connection);
            SQLiteDataReader ResultId = CommandId.ExecuteReader();
            if (ResultId != null)
            {
                if (ResultId.Read())
                {
                    object Obj = ResultId["ID"];
                    if (Obj != DBNull.Value)
                        Id = Convert.ToInt64(Obj);
                }
                ResultId.Close();
            }
            else
            {
                throw new Exception("Getting M failed");
            }

            //Insert brand
            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO Marques (RefMarque, Nom) VALUES (@ID, @Name)", _Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ++Id);
            CommandInsert.Parameters.AddWithValue("@Name", Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new Exception("Inserting M failed");
            return Id;
        }

        /// <summary>
        /// Add the given family to the database
        /// </summary>
        /// <param name="Name">The name of the new family</param>
        /// <returns>The id of the newly created family</returns>
        public long CreateFamily(string Name)
        {
            //Get existign family
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefFamille FROM Familles WHERE Nom = @Name", _Connection);
            CommandSelect.Parameters.AddWithValue("@Name", Name);

            SQLiteDataReader Result = CommandSelect.ExecuteReader();
            if (Result != null)
            {
                if (Result.Read())
                {
                    object Obj = Result["RefFamille"];
                    if (Obj != DBNull.Value)
                        return Convert.ToInt64(Obj);
                }
                Result.Close();
            }
            else
            {
                throw new Exception("Getting F failed");
            }

            //Get next ID
            long Id = 0;
            SQLiteCommand CommandId = new SQLiteCommand("SELECT MAX(RefFamille) AS ID FROM Familles", _Connection);
            SQLiteDataReader ResultId = CommandId.ExecuteReader();
            if (ResultId != null)
            {
                if (ResultId.Read())
                {
                    object Obj = ResultId["ID"];
                    if (Obj != DBNull.Value)
                        Id = Convert.ToInt64(Obj);
                }
                ResultId.Close();
            }
            else
            {
                throw new Exception("Getting F failed");
            }

            //Insert family
            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT INTO Familles (RefFamille, Nom) VALUES (@ID, @Name)", _Connection);
            CommandInsert.Parameters.AddWithValue("@ID", ++Id);
            CommandInsert.Parameters.AddWithValue("@Name", Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new Exception("Inserting F failed");
            return Id;
        }

        /// <summary>
        /// Empty all the database
        /// </summary>
        public void Clear()
        {
            string[] Tables = {"Articles", "Familles", "Marques", "SousFamilles"};
            foreach(string Table in Tables)
            {
                SQLiteCommand CommandClear = new SQLiteCommand("DELETE FROM " + Table, _Connection);
                CommandClear.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Tells if an article exists by its reference
        /// </summary>
        /// <param name="Id">The reference to look for</param>
        /// <returns>True if the article exists, False otherwise</returns>
        public bool ArticleExists(string Id)
        {
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefArticle FROM Articles WHERE RefArticle = @ID", _Connection);
            CommandSelect.Parameters.AddWithValue("@ID", Id);

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
            SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Articles WHERE RefArticle = @Ref", _Connection);
            CommandDelete.Parameters.AddWithValue("@Ref", Ref);
            CommandDelete.ExecuteNonQuery();
        }

        /// <summary>
        /// Delete a brand by its reference
        /// </summary>
        /// <param name="Ref">The reference</param>
        public void DeleteBrand(long Ref)
        {
            var Transaction = _Connection.BeginTransaction();
            { //Delete articles
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Articles WHERE RefMarque = @Ref", _Connection);
                CommandDelete.Parameters.AddWithValue("@Ref", Ref);
                int Modified = CommandDelete.ExecuteNonQuery();
                if (Modified > 0)
                {
                    Transaction.Rollback();
                    return;
                }
            }
            { //Delete brand
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Marques WHERE RefMarque = @Ref", _Connection);
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
            var Transaction = _Connection.BeginTransaction();
            { //Delete family
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Familles WHERE RefFamille = @Ref", _Connection);
                CommandDelete.Parameters.AddWithValue("@Ref", Ref);
                CommandDelete.ExecuteNonQuery();
            }

            //Get all subfamilies to delete
            LinkedList<long> SfToDel = new LinkedList<long>();
            {
                SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefSousFamille FROM SousFamilles WHERE RefFamille = @Ref", _Connection);
                CommandSelect.Parameters.AddWithValue("@Ref", Ref);

                SQLiteDataReader Result = CommandSelect.ExecuteReader();
                if (Result != null)
                {
                    while (Result.Read())
                    {
                        object Obj = Result["RefSousFamille"];
                        if (Obj != DBNull.Value)
                        {
                            SfToDel.AddLast(Convert.ToInt64(Obj));
                        }
                    }
                    Result.Close();
                }
            }

            if (SfToDel.Count > 0)
            {
                Transaction.Rollback();
                return;
            }
            foreach (long RefSf in SfToDel)
            {
                { //Delete sub families
                    SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM SousFamilles WHERE RefSousFamille = @Ref", _Connection);
                    CommandDelete.Parameters.AddWithValue("@Ref", RefSf);
                    CommandDelete.ExecuteNonQuery();
                }
                { //Delete articles
                    SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Articles WHERE RefSousFamille = @Ref", _Connection);
                    CommandDelete.Parameters.AddWithValue("@Ref", RefSf);
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
            var Transaction = _Connection.BeginTransaction();
            { //Delete sub family
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM SousFamilles WHERE RefSousFamille = @Ref", _Connection);
                CommandDelete.Parameters.AddWithValue("@Ref", Ref);
                CommandDelete.ExecuteNonQuery();
            }

            { //Delete articles
                SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Articles WHERE RefSousFamille = @Ref", _Connection);
                CommandDelete.Parameters.AddWithValue("@Ref", Ref);
                int Modified = CommandDelete.ExecuteNonQuery();
                if (Modified > 0)
                {
                    Transaction.Rollback();
                    return;
                }
            }

            //Get families to delete
            LinkedList<long> FToDel = new LinkedList<long>();
            {
                SQLiteCommand CommandSelect = new SQLiteCommand("SELECT RefFamille FROM SousFamilles WHERE RefSousFamille = @Ref", _Connection);
                CommandSelect.Parameters.AddWithValue("@Ref", Ref);

                SQLiteDataReader Result = CommandSelect.ExecuteReader();
                if (Result != null)
                {
                    if(Result.Read())
                    {
                        object Obj = Result["RefFamille"];
                        if (Obj != DBNull.Value)
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

            //Delete families
            foreach (long RefF in FToDel)
            {
                    SQLiteCommand CommandDelete = new SQLiteCommand("DELETE FROM Familles WHERE RefFamille = @Ref", _Connection);
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
            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT OR REPLACE INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@ID, @Desc, @SF, @M, @PHT, @Q)", _Connection);
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
            long Id = -1;
            if (Brand.Reference == -1) //If no ID, get the next one
            {
                SQLiteCommand CommandId = new SQLiteCommand("SELECT MAX(RefMarque) AS ID FROM Marques", _Connection);
                SQLiteDataReader ResultId = CommandId.ExecuteReader();
                if (ResultId != null)
                {
                    if (ResultId.Read())
                    {
                        object Obj = ResultId["ID"];
                        if (Obj != DBNull.Value)
                            Id = Convert.ToInt64(Obj);
                    }
                    ResultId.Close();
                    Id++;
                }
                else
                    throw new Exception("Getting brand ID failed");
            }
            else
                Id = Brand.Reference;

            //Insert brand
            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT OR REPLACE INTO Marques (RefMarque, Nom) VALUES (@ID, @Name)", _Connection);
            CommandInsert.Parameters.AddWithValue("@ID", Id);
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
            long Id = -1;
            if (Family.Reference == -1)//If no ID, get the next one
            {
                SQLiteCommand CommandId = new SQLiteCommand("SELECT MAX(RefFamily) AS ID FROM Familles", _Connection);
                SQLiteDataReader ResultId = CommandId.ExecuteReader();
                if (ResultId != null)
                {
                    if (ResultId.Read())
                    {
                        object Obj = ResultId["ID"];
                        if (Obj != DBNull.Value)
                            Id = Convert.ToInt64(Obj);
                    }
                    ResultId.Close();
                    Id++;
                }
                else
                    throw new Exception("Getting family ID failed");
            }
            else
                Id = Family.Reference;

            //Insert family
            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT OR REPLACE INTO Familles (RefFamille, Nom) VALUES (@ID, @Name)", _Connection);
            CommandInsert.Parameters.AddWithValue("@ID", Id);
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
            long Id = -1;
            if (SubFamily.Reference == -1)//If no ID, get the next one
            {
                SQLiteCommand CommandId = new SQLiteCommand("SELECT MAX(RefSousFamille) AS ID FROM SousFamilles", _Connection);
                SQLiteDataReader ResultId = CommandId.ExecuteReader();
                if (ResultId != null)
                {
                    if (ResultId.Read())
                    {
                        object Obj = ResultId["ID"];
                        if (Obj != DBNull.Value)
                            Id = Convert.ToInt64(Obj);
                    }
                    ResultId.Close();
                    Id++;
                }
                else
                    throw new Exception("Getting subfamily ID failed");
            }
            else
                Id = SubFamily.Reference;

            //Insert sub family
            SQLiteCommand CommandInsert = new SQLiteCommand("INSERT OR REPLACE INTO SousFamilles (RefSousFamille, RefFamille, Nom) VALUES (@ID, @IDFamille, @Name)", _Connection);
            CommandInsert.Parameters.AddWithValue("@ID", Id);
            CommandInsert.Parameters.AddWithValue("@IDFamille", SubFamily.FamilyReference);
            CommandInsert.Parameters.AddWithValue("@Name", SubFamily.Name);

            if (CommandInsert.ExecuteNonQuery() != 1)
                throw new Exception("Inserting SF failed");
        }
    }
}
