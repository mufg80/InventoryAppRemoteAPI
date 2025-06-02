using Microsoft.AspNetCore.Mvc;
using InventoryAppRemoteAPI.Models;
using System.Collections.Generic;
using System.Linq;
using InventoryAppRemoteAPI.DBAccess;

namespace InventoryAppRemoteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private DBAccesser db;

        public InventoryController()
        {
            db = new DBAccesser();
        }


        // GET: api/Inventory?userId=1
        [HttpGet]
        public ActionResult<IEnumerable<InventoryItem>> Get([FromQuery] int userId)
        {
            var userItems = from m in db.ReadRecords()
                                where m.UserId == userId
                                select m;
            return Ok(userItems);
        }


        // POST api/Inventory
        [HttpPost]
        public ActionResult<int> Post([FromBody] InventoryItem item)
        {

            // Simulate creating a new item
            var a = db.CreateRecord(item);
            if (a)
            {
                return Ok(1);
            }
            else
            {
                return NotFound();
            }
        
        }

        // PUT api/Inventory/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] InventoryItem item)
        {

            var a = db.UpdateRecord(item);
            if (a)
            {
                return Ok(1);
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE api/Inventory/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {

            var a = db.DeleteRecord(id);

            if (a)
            {
                return Ok(1);
            }
            else
            {
                return NotFound();
            }
        }
    }
}