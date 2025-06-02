namespace InventoryAppRemoteAPI.Models
{
    /// <summary>
    /// Represents an inventory item with various attributes.
    /// Author: Shannon Musgrave
    /// </summary>
    public class InventoryItem
    {
        /// <summary>
        /// Unique identifier for the inventory item.
        /// </summary>
        private int id;

        /// <summary>
        /// Gets or sets the item ID.
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Title of the inventory item.
        /// </summary>
        private string title;

        /// <summary>
        /// Gets or sets the item title.
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Description of the inventory item.
        /// </summary>
        private string description;

        /// <summary>
        /// Gets or sets the item description.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Quantity available for the inventory item.
        /// </summary>
        private int quantity;

        /// <summary>
        /// Gets or sets the item quantity.
        /// </summary>
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        /// <summary>
        /// User ID associated with this inventory item.
        /// </summary>
        private int userId;

        /// <summary>
        /// Gets or sets the user ID related to the item.
        /// </summary>
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        /// <summary>
        /// Default constructor for InventoryItem.
        /// </summary>
        public InventoryItem()
        {
        }
    }
}
