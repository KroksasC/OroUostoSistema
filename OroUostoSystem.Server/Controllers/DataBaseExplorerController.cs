// .Server/Controllers/DatabaseExplorerController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;

namespace OroUostoSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseExplorerController : ControllerBase
    {
        [HttpGet("explore")]
        public IActionResult ExploreDatabase()
        {
            var dbPath = "app2.db";
            var results = new Dictionary<string, object>();
            
            if (!System.IO.File.Exists(dbPath))
            {
                return Ok(new { error = $"Database file not found at: {Path.GetFullPath(dbPath)}" });
            }
            
            try
            {
                using var connection = new SqliteConnection($"Data Source={dbPath}");
                connection.Open();
                
                // 1. Get ALL table names
                var tables = new List<string>();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
                
                using (var reader = tableCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
                
                results.Add("Tables", tables);
                
                // 2. For EACH table, show structure and sample data
                var tableDetails = new Dictionary<string, object>();
                
                foreach (var tableName in tables)
                {
                    // Get column information
                    var columns = new List<object>();
                    var columnCommand = connection.CreateCommand();
                    columnCommand.CommandText = $"PRAGMA table_info('{tableName}');";
                    
                    using (var reader = columnCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(new
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Type = reader.GetString(2),
                                NotNull = reader.GetBoolean(3) ? "NOT NULL" : "NULL",
                                DefaultValue = reader.IsDBNull(4) ? null : reader.GetString(4),
                                IsPrimaryKey = reader.GetBoolean(5) ? "PK" : ""
                            });
                        }
                    }
                    
                    // Get row count
                    var countCommand = connection.CreateCommand();
                    countCommand.CommandText = $"SELECT COUNT(*) FROM {tableName};";
                    var rowCount = countCommand.ExecuteScalar();
                    
                    // Get first 5 rows (if any)
                    var sampleRows = new List<object>();
                    if ((long)rowCount > 0)
                    {
                        var sampleCommand = connection.CreateCommand();
                        sampleCommand.CommandText = $"SELECT * FROM {tableName} LIMIT 5;";
                        
                        using (var reader = sampleCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                                }
                                sampleRows.Add(row);
                            }
                        }
                    }
                    
                    tableDetails.Add(tableName, new
                    {
                        Columns = columns,
                        RowCount = rowCount,
                        SampleRows = sampleRows
                    });
                }
                
                results.Add("TableDetails", tableDetails);
                
                connection.Close();
                
                return Ok(results);
            }
            catch (Exception ex)
            {
                return Ok(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
        
        [HttpGet("file-info")]
        public IActionResult GetFileInfo()
        {
            var dbPath = "app2.db";
            
            if (!System.IO.File.Exists(dbPath))
            {
                return Ok(new { error = $"File not found at: {Path.GetFullPath(dbPath)}" });
            }
            
            var fileInfo = new FileInfo(dbPath);
            
            return Ok(new
            {
                FullPath = fileInfo.FullName,
                SizeInKB = fileInfo.Length / 1024,
                Created = fileInfo.CreationTime,
                LastModified = fileInfo.LastWriteTime,
                Exists = true
            });
        }
    }
}