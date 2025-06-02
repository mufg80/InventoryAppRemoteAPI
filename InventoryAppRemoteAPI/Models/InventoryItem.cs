namespace InventoryAppRemoteAPI.Models
{
    public class InventoryItem
    {

        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private int quantity;

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }


        private int userId;

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }



        public InventoryItem()
        {
        }
    }
}
