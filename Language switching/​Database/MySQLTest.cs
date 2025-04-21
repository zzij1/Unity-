using UnityEngine;
using MySqlConnector;
using System;

public class MySQLTest : MonoBehaviour
{
    void Start()
    {
        string connectionString = "Server=localhost;User=root;Password=abc123;Database=test;";

        using (var conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                Debug.Log("数据库连接成功！");

                // 示例：执行简单查询验证连接
                using (var cmd = new MySqlCommand("SELECT 1", conn))
                {
                    var result = cmd.ExecuteScalar();
                    Debug.Log($"查询验证结果: {result}");
                }
            }
            catch (MySqlException e)
            {
                Debug.LogError($"数据库连接失败！错误代码: {e.Number}, 错误信息: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"发生未知错误: {e.Message}");
            }
        }
    }
}