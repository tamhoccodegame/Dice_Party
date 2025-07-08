using System.Collections;
using System.Collections.Generic;
using System.IO;
using SQLite;
using UnityEngine;

public class SQL : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "mydatabase.db");
        using (var db = new SQLiteConnection(dbPath))
        {
            db.CreateTable<Player>();
            db.Insert(new Player { Name = "Senju", Score = 100 });
        }
    }
}

public class Player
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }
    public int Score { get; set; }
}

