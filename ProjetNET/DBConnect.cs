using System;
using Microsoft.Data.Sqlite;

public class DBConnect
{

    private SqliteConnection Connection;

	public DBConnect()
	{
        Connection = new SqliteConnection("jdbc:sqlite:Mercure.SQLite");
        Connection.Open();
	}
}
