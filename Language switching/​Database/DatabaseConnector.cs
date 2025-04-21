using System;
using System.Threading.Tasks;
using MySqlConnector;
using UnityEngine;

public static class DatabaseConnector
{
    [System.Serializable]
    private class DBConfig
    {
        public string server;
        public string database;
        public string user;
        public string password;
    }

    public static async Task<MySqlConnection> GetConnection()
    {
        var config = Resources.Load<TextAsset>("db_config");
        var settings = JsonUtility.FromJson<DBConfig>(config.text);
        var builder = new MySqlConnectionStringBuilder
        {
            Server = settings.server,
            Database = settings.database,
            UserID = settings.user,
            Password = settings.password,
            Pooling = true,
            MinimumPoolSize = 1,
            MaximumPoolSize = 10,
            ConnectionIdleTimeout = 30,
            ConnectionLifeTime = 300,
            CharacterSet = "utf8mb4"
        };

        var connection = new MySqlConnection(builder.ConnectionString);
        await connection.OpenAsync();
        return connection;
    }
}