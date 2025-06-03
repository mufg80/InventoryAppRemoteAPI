using InventoryAppRemoteAPI.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace InventoryAppRemoteAPI.DBAccess
{
    /// <summary>
    /// Handles database interactions for inventory items.
    /// Author: Shannon Musgrave
    /// </summary>
    public class DBAccesser
    {

        /// <summary>
        /// Configuration object used for retrieving connection string.
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// Constructor that initializes the configuration dependency.
        /// </summary>
        /// <param name="config">Injected configuration instance.</param>
        public DBAccesser(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Retrieves a value from the application's configuration settings.
        /// </summary>
        /// <param name="key">The configuration key to retrieve.</param>
        /// <returns>The corresponding configuration value.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the key is not found.</exception>
        private string GetJSONItem(string key)
        {
            string retrieved = _config[key];

            if (string.IsNullOrEmpty(retrieved))
            {
                throw new InvalidOperationException($"The key '{key}' is not configured in the application settings.");
            }

            return retrieved;
        }

        /// <summary>
        /// Creates a new inventory record in the database.
        /// </summary>
        /// <param name="item">The inventory item to be inserted.</param>
        /// <returns>True if insertion was successful, otherwise false.</returns>
        /// 
        public bool CreateRecord(InventoryItem item)
        {
            using (SqlConnection connection = new SqlConnection(GetJSONItem("ConnectionString")))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO InventoryItemList (Title, Description, Quantity, UserId) VALUES (@Title, @Description, @Quantity, @UserId)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Adding parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@Title", item.Title ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Quantity", item.Quantity);
                        command.Parameters.AddWithValue("@UserId", item.UserId);
                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating record: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Retrieves all inventory records from the database.
        /// </summary>
        /// <returns>List of inventory items.</returns>
        public List<InventoryItem> ReadRecords()
        {
            var records = new List<InventoryItem>();
            using (SqlConnection connection = new SqlConnection(GetJSONItem("ConnectionString")))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Id, Title, Description, Quantity, UserId FROM InventoryItemList";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                records.Add(new InventoryItem
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Quantity = reader.GetInt32(3),
                                    UserId = reader.GetInt32(4)
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading records: {ex.Message}");
                }
            }
            return records;
        }


        /// <summary>
        /// Updates an existing inventory record in the database.
        /// </summary>
        /// <param name="item">The updated inventory item.</param>
        /// <returns>True if update was successful, otherwise false.</returns>
        public bool UpdateRecord(InventoryItem item)
        {
            using (SqlConnection connection = new SqlConnection(GetJSONItem("ConnectionString")))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE InventoryItemList SET Title = @Title, Description = @Description, Quantity = @Quantity, UserId = @UserId WHERE Id = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", item.Id);
                        command.Parameters.AddWithValue("@Title", item.Title ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Quantity", item.Quantity);
                        command.Parameters.AddWithValue("@UserId", item.UserId);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating record: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Deletes an inventory record by ID.
        /// </summary>
        /// <param name="id">The ID of the record to delete.</param>
        /// <returns>True if deletion was successful, otherwise false.</returns>
        public bool DeleteRecord(int id)
        {
            using (SqlConnection connection = new SqlConnection(GetJSONItem("ConnectionString")))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM InventoryItemList WHERE Id = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting record: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
