


using InventoryAppRemoteAPI.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace InventoryAppRemoteAPI.DBAccess
{
    public class DBAccesser
    {
        string connectionString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=InventoryItems;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
         // Create: Insert a new record
        public bool CreateRecord(InventoryItem item)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO InventoryItemList (Title, Description, Quantity, UserId) VALUES (@Title, @Description, @Quantity, @UserId)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        //command.Parameters.AddWithValue("@ID", item.Id);
                        command.Parameters.AddWithValue("@Title", item.Title ?? (object)DBNull.Value); // Handle null strings
                        command.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Quantity", item.Quantity);
                        command.Parameters.AddWithValue("@UserId", item.UserId);
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine("Record created successfully.");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating record: {ex.Message}");
                    return false;
                }
            }
        }

        // Read: Retrieve all records
        public List<InventoryItem> ReadRecords()
        {
            var records = new List<InventoryItem>();
            using (SqlConnection connection = new SqlConnection(connectionString))
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
                                    UserId = reader.GetInt32(4) // Adjust to GetInt32 if UserId is int
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

        // Read: Retrieve a single record by ID
        public InventoryItem ReadRecordById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Id, Title, Description, Quantity, UserId FROM InventoryItemList WHERE Id = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new InventoryItem
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Quantity = reader.GetInt32(3),
                                    UserId = reader.GetInt32(4) // Adjust to GetInt32 if UserId is int
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading record: {ex.Message}");
                }
                return null;
            }
        }

        // Update: Update a record by ID
        public bool UpdateRecord(InventoryItem item)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
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
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Record updated successfully.");
                            return true;
                        }                            
                        else
                        {
                            Console.WriteLine("No record found with the specified ID.");
                            return false;
                        }
                           
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating record: {ex.Message}");
                    return false;
                }
            }
        }

        // Delete: Delete a record by ID
        public bool DeleteRecord(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM InventoryItemList WHERE Id = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Record deleted successfully.");
                            return true;
                        }                            
                        else
                        {
                            Console.WriteLine("No record found with the specified ID.");
                            return false;
                        }
                           
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
