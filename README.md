# Inventory App Remote API

## About the Project

The Inventory App Remote API is a C# ASP.NET Core Web API designed to manage inventory items stored in a SQL Server database. It provides endpoints to:

Retrieve inventory items for a specific user by userId.

Create new inventory items.

Update existing inventory items by id.

Delete inventory items by id.

The API uses a secure, encrypted API key for authentication and interacts with a SQL Server database to perform CRUD (Create, Read, Update, Delete) operations on inventory items, each consisting of an ID, title, description, quantity, and associated user ID.

## Motivation

Managing inventory data manually is time-consuming and error-prone, especially for users needing to track items across multiple contexts. The Inventory App Remote API automates inventory management by providing a secure, RESTful interface for creating, retrieving, updating, and deleting inventory items, ensuring data integrity and efficient access for authorized users.

## Getting Started
To get started with the Inventory App Remote API, follow these steps:
Ensure the required environment is set up (see Installation (#installation)).

Clone or download the project files to a local directory.

Configure the SQL Server database with the InventoryItemList table (schema: Id, Title, Description, Quantity, UserId).

Run the API using a development environment like Visual Studio or the .NET CLI.

Use the API endpoints via tools like Postman or Swagger UI to:

Retrieve Items: Get all items for a user by userId.


Create Item: Add a new inventory item.


Update Item: Modify an existing item by id.


Delete Item: Remove an item by id.


## Installation

To set up the environment:
Install .NET SDK (version compatible with ASP.NET Core, e.g., .NET 6 or later).

Ensure a SQL Server instance (e.g., LocalDB) is available and accessible. The default connection string in DBAccesser.cs is:
csharp. Add SQL authentication for the database and insert into connection string.

Data Source=(localdb)\ProjectModels;Initial Catalog=InventoryItems;Integrated Security=True

Create the InventoryItemList table in the database with the following schema:
```sql

CREATE TABLE InventoryItemList (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(30),
    Description NVARCHAR(50),
    Quantity INT,
    UserId INT
);
```
Download or clone the project files to a local directory.

For development with HTTPS, ensure the localhost.pfx certificate is available, or generate a new one using:
bash, if creating new, add information to program.cs in this application and add cert and information to Android's manifest.

Run the API using Visual Studio's run.

The API will be hosted at https://localhost:7113 by default.

Access the Swagger UI at https://localhost:7113/swagger for testing endpoints (available in development mode).

Note: All requests must include the X-Encrypted-Api-Key header with a valid encrypted key, as defined in DecryptKey.cs. Contact the project maintainer for the API key.

## Usage

The Inventory App Remote API supports four primary use cases:

Retrieve Items: Fetches all inventory items for a given userId via a GET request.
Endpoint: GET /api/Inventory?userId={userId}

Example Response: List of items with Id, Title, Description, Quantity, and UserId.

Create Item: Adds a new inventory item to the database via a POST request.
Endpoint: POST /api/Inventory

Body: JSON object with Title, Description, Quantity, and UserId.

Update Item: Modifies an existing item by id via a PUT request.
Endpoint: PUT /api/Inventory/{id}

Body: JSON object with item id and updated Title, Description, Quantity, and UserId.

Delete Item: Removes an item by id via a DELETE request.
Endpoint: DELETE /api/Inventory/{id}

Important: All requests require the X-Encrypted-Api-Key header for authentication. Invalid or missing keys result in a 401 Unauthorized response.

## Code Examples

Below are examples of key API operations based on the provided source code.

### Example A: Retrieve Items by User ID

The Get method retrieves all inventory items for a specified userId.
```csharp
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

```
This queries the InventoryItemList table, filters by UserId, and returns a list of matching items.

### Example B: Create a New Item

The Post method inserts a new inventory item into the database.
```csharp
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
```
This calls DBAccesser.CreateRecord to insert a new record with Title, Description, Quantity, and UserId. It returns 1 on success or 404 on failure.

### Example C: API Key Authentication

The Program.cs middleware enforces API key validation.
```csharp

 app.Use(async (context, next) =>
 {
     var decryptKeyService = context.RequestServices.GetRequiredService<DecryptKey>();

     // Check if the request contains a valid API key header this checks that the api key has been sent by android device
     // andn returns 401 Unauthorized if not present or invalid.
     if (!context.Request.Headers.TryGetValue("X-Encrypted-Api-Key", out var apiKey) || !decryptKeyService.IsValidKey(apiKey))
     {
         context.Response.StatusCode = 401; // Unauthorized
         await context.Response.WriteAsync("Invalid API Key");
         return;
     }

     // Proceed to next middleware in the pipeline
     await next.Invoke();
 });
```
This checks for a valid X-Encrypted-Api-Key header using AES decryption before allowing access to endpoints.

## Contact

Shannon Musgrave

mufg80@hotmail.com (mailto:mufg80@hotmail.com)

