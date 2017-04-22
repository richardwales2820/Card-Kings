using Mono.Data.Sqlite;
using System;
using System.Data;
using UnityEngine;

public class DatabaseConnect : MonoBehaviour
{
    private static string con = "URI=file:" + Application.dataPath + "PokerDB.s3db"; //Path to database.
    private static IDbConnection dbConnect;
    private static IDbCommand dbCmd;
    private static IDataReader reader;


    // Use this for initialization
    void Start()
    {
        OpenDB();
        ExecuteCreate();
    }

    static bool ValidLogin(string name, string password)
    {
        
        if( LookupInDB(name) != 0 )
        {
            dbCmd.CommandText = "SELECT PASSWORD FROM Player WHERE NAME= @id";
            dbCmd.Parameters.Add(new SqliteParameter("@id", name));
            string result = (string)dbCmd.ExecuteScalar();
            if (result.Equals(password))
            {
                  return true;
            }
        }

        return false;


    }

    static void OpenDB()
    {
        dbConnect = new SqliteConnection(con);
        dbConnect.Open(); //Open connection to the database.
    }

    static void ExecuteCreate()
    {
        dbCmd = dbConnect.CreateCommand();
        dbCmd.CommandText = "CREATE TABLE IF NOT EXISTS Player (ID INTEGER NOT NULL PRIMARY KEY, NAME VARCHAR(256) NOT NULL, PASSWORD VARCHAR(256) NOT NULL, WINS INTEGER NOT NULL, LOSSES INTEGER NOT NULL, BIGPOT INTEGER NOT NULL, CHIPS INTEGER NOT NULL)";
        reader = dbCmd.ExecuteReader();
        reader.Close();
    }

    static void InsertInDB(int id, string name, string password, int wins, int losses, int bigPot, int chips)
    {
        dbCmd.CommandText = "INSERT INTO Player (ID, NAME, PASSWORD, WINS, LOSSES, BIGPOT, CHIPS) VALUES (@id , @name , @password, @wins , @losses , @bigPot , @chips )";
        dbCmd.Parameters.Add(new SqliteParameter("@id", id));
        dbCmd.Parameters.Add(new SqliteParameter("@name", name));
        dbCmd.Parameters.Add(new SqliteParameter("@password", password));
        dbCmd.Parameters.Add(new SqliteParameter("@wins", wins));
        dbCmd.Parameters.Add(new SqliteParameter("@losses", losses));
        dbCmd.Parameters.Add(new SqliteParameter("@bigPot", bigPot));
        dbCmd.Parameters.Add(new SqliteParameter("@chips", chips));
        reader = dbCmd.ExecuteReader();
        reader.Close();
    }

    
    // Update is called once per frame
    void Update()
    {
    }

    bool DeleteFromDB(string name, string password)
    {
        try
        {
            int id = LookupID(name);
            dbCmd.CommandText = "SELECT PASSWORD FROM Player WHERE ID= @id";
            dbCmd.Parameters.Add(new SqliteParameter("@id", id));
            string result = (string)dbCmd.ExecuteScalar();
            if (result.Equals(password))
            {
                dbCmd.CommandText = "DELETE FROM Player WHERE ID= @id";
                reader = dbCmd.ExecuteReader();
                reader.Close();
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }
    static void UpdateDB(int id, int win, int loss, int bigPot, int chips)
    {
        dbCmd.CommandText = "UPDATE Player SET WINS= @win, LOSSES= @loss, BIGPOT= @bigPot , CHIPS= @chips WHERE ID= @id";
        dbCmd.Parameters.Add(new SqliteParameter("@id", id));
        dbCmd.Parameters.Add(new SqliteParameter("@win", win));
        dbCmd.Parameters.Add(new SqliteParameter("@loss", loss));
        dbCmd.Parameters.Add(new SqliteParameter("@bigPot", bigPot));
        dbCmd.Parameters.Add(new SqliteParameter("@chips", chips));
        reader = dbCmd.ExecuteReader();
        reader.Close();
    }

    static int LookupInDB(string name)
    { 
        dbCmd.CommandText = "SELECT COUNT(*) FROM Player WHERE NAME like @id";
        int ret = (int) dbCmd.ExecuteScalar();
        return ret;
    }

    static int LookupID(string name)
    {
        int id;

        dbCmd.CommandText = "SELECT ID FROM Player WHERE NAME= @name";
        dbCmd.Parameters.Add(new SqliteParameter("@name", name));
        id = (int) dbCmd.ExecuteScalar();

        return id;
    }

    static int MakeID()
    {
        dbCmd.CommandText = "SELECT COUNT(*) FROM Player";
        int ret = (int)dbCmd.ExecuteScalar();
        return ret+1;
    }
}
