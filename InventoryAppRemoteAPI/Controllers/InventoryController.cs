using Microsoft.AspNetCore.Mvc;
using InventoryAppRemoteAPI.Models;
using InventoryAppRemoteAPI.DBAccess;

namespace InventoryAppRemoteAPI.Controllers
{
    /// <summary>
    /// InventoryController handles API requests related to inventory management.
    /// Author: Shannon Musgrave
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private DBAccesser db;

        /// <summary>
        /// Constructor for InventoryController.
        /// Initializes database access object.
        /// </summary>
        public InventoryController(DBAccesser databaseaccesser)
        {
            db = databaseaccesser;
        }

        /// <summary>
        /// Retrieves inventory items for a specific user.
        /// </summary>
        /// <param name="userId">User ID whose items are retrieved.</param>
        /// <returns>List of inventory items associated with the user.</returns>
        // GET: api/Inventory?userId=1
        [HttpGet]
        public ActionResult<IEnumerable<InventoryItem>> Get([FromQuery] int userId)
        {
            if (userId < 0)
            {
                return BadRequest("Invalid user ID provided.");
            }
            // Filter items based on user ID
            // If fails, returns newlist.
            var success = db.ReadRecords();
            if (success.Item1)
            {
                var userItems = from m in success.Item2
                                where m.UserId == userId
                                select m;
                if (userItems.Count() == 0)
                {
                    return NotFound("No items found for the specified user ID.");
                }
                return Ok(userItems);
            }
            else
            {
                return NotFound("No items found for the specified user ID.");
            }

        }

        /// <summary>
        /// Adds a new inventory item.
        /// </summary>
        /// <param name="item">Inventory item to be added.</param>
        /// <returns>Returns 1 if successful, 404 if failed.</returns>
        // POST api/Inventory
        [HttpPost]
        public ActionResult<int> Post([FromBody] InventoryItem item)
        {
            // Check if the item is valid
            if (!db.IsValidInventoryItem(item))
            {
                return BadRequest("Invalid inventory item data provided.");
            }
            // Attempt to create the new inventory item
            var isCreated = db.CreateRecord(item);

            if (isCreated)
            {
                return Ok(1);  // Item successfully added
            }
            else
            {
                return NotFound();  // Item creation failed
            }
        }

        /// <summary>
        /// Updates an existing inventory item.
        /// </summary>
        /// <param name="id">ID of the inventory item to update.</param>
        /// <param name="item">Updated inventory item details.</param>
        /// <returns>Returns 1 if successful, 404 if failed.</returns>
        // PUT api/Inventory/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] InventoryItem item)
        {
            // Check if the item is valid
            if (!db.IsValidInventoryItem(item) || id < 0)
            {
                return BadRequest("Invalid inventory item data provided.");
            }
            // Attempt to update the inventory item
            var isUpdated = db.UpdateRecord(item);

            if (isUpdated)
            {
                return Ok(1);  // Item successfully updated
            }
            else
            {
                return NotFound();  // Update failed
            }
        }

        /// <summary>
        /// Deletes an inventory item by its ID.
        /// </summary>
        /// <param name="id">ID of the inventory item to delete.</param>
        /// <returns>Returns 1 if successful, 404 if failed.</returns>
        // DELETE api/Inventory/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (id < 0)
            {
                return BadRequest("Invalid inventory item ID provided.");
            }
            // Attempt to delete the inventory item
            var isDeleted = db.DeleteRecord(id);

            if (isDeleted)
            {
                return Ok(1);  // Item successfully deleted
            }
            else
            {
                return NotFound();  // Deletion failed
            }
        }
    }
}
