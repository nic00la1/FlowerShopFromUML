using FlowerShopFromUML;
using Microsoft.Data.Sqlite;

public class DatabaseManager
{
    private const string ConnectionString = "Data Source=flowershop.db";

    public async Task SaveDataAsync(Shop shop)
    {
        try
        {
            using SqliteConnection connection = new(ConnectionString);
            await connection.OpenAsync();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Flowers (
                    Name TEXT,
                    Color TEXT,
                    Price REAL,
                    InStock INTEGER
                );
                CREATE TABLE IF NOT EXISTS Bouquets (
                    Name TEXT,
                    Price REAL,
                    InStock INTEGER
                );
                CREATE TABLE IF NOT EXISTS FlowerCopies (
                    BouquetName TEXT,
                    Name TEXT,
                    Color TEXT,
                    Count INTEGER
                );
                CREATE TABLE IF NOT EXISTS Customers (
                    Name TEXT,
                    Email TEXT,
                    Phone TEXT
                );
                CREATE TABLE IF NOT EXISTS Orders (
                    OrderDate TEXT,
                    CustomerEmail TEXT,
                    TotalPrice REAL
                );
                CREATE TABLE IF NOT EXISTS OrderBouquets (
                    OrderId INTEGER,
                    BouquetName TEXT
                );
            ";
            await command.ExecuteNonQueryAsync();

            // Save Flowers
            command.CommandText = "DELETE FROM Flowers";
            await command.ExecuteNonQueryAsync();
            foreach (Flower flower in shop.Flowers)
            {
                command.CommandText = @"
                    INSERT INTO Flowers (Name, Color, Price, InStock)
                    VALUES ($name, $color, $price, $inStock)";
                command.Parameters.AddWithValue("$name", flower.Name);
                command.Parameters.AddWithValue("$color", flower.Color);
                command.Parameters.AddWithValue("$price", flower.Price);
                command.Parameters.AddWithValue("$inStock", flower.InStock);
                await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();
            }

            // Save Bouquets
            command.CommandText = "DELETE FROM Bouquets";
            await command.ExecuteNonQueryAsync();
            command.CommandText = "DELETE FROM FlowerCopies";
            await command.ExecuteNonQueryAsync();
            foreach (Bouquet bouquet in shop.Bouquets)
            {
                command.CommandText = @"
                    INSERT INTO Bouquets (Name, Price, InStock)
                    VALUES ($name, $price, $inStock)";
                command.Parameters.AddWithValue("$name", bouquet.Name);
                command.Parameters.AddWithValue("$price", bouquet.Price);
                command.Parameters.AddWithValue("$inStock", bouquet.InStock);
                await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();

                foreach (FlowerCopy flower in bouquet.Flowers)
                {
                    command.CommandText = @"
                        INSERT INTO FlowerCopies (BouquetName, Name, Color, Count)
                        VALUES ($bouquetName, $name, $color, $count)";
                    command.Parameters.AddWithValue("$bouquetName",
                        bouquet.Name);
                    command.Parameters.AddWithValue("$name", flower.Name);
                    command.Parameters.AddWithValue("$color", flower.Color);
                    command.Parameters.AddWithValue("$count", flower.Count);
                    await command.ExecuteNonQueryAsync();
                    command.Parameters.Clear();
                }
            }

            // Save Customers
            command.CommandText = "DELETE FROM Customers";
            await command.ExecuteNonQueryAsync();
            foreach (Customer customer in shop.Customers)
            {
                command.CommandText = @"
                    INSERT INTO Customers (Name, Email, Phone)
                    VALUES ($name, $email, $phone)";
                command.Parameters.AddWithValue("$name", customer.Name);
                command.Parameters.AddWithValue("$email", customer.Email);
                command.Parameters.AddWithValue("$phone", customer.Phone);
                await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();
            }

            // Save Orders
            command.CommandText = "DELETE FROM Orders";
            await command.ExecuteNonQueryAsync();
            command.CommandText = "DELETE FROM OrderBouquets";
            await command.ExecuteNonQueryAsync();
            foreach (Order order in shop.Orders)
            {
                command.CommandText = @"
                    INSERT INTO Orders (OrderDate, CustomerEmail, TotalPrice)
                    VALUES ($orderDate, $customerEmail, $totalPrice)";
                command.Parameters.AddWithValue("$orderDate",
                    order.OrderDate.ToString("o"));
                command.Parameters.AddWithValue("$customerEmail",
                    order.Customer.Email);
                command.Parameters.AddWithValue("$totalPrice",
                    order.TotalPrice);
                await command.ExecuteNonQueryAsync();

                command.CommandText = "SELECT last_insert_rowid()";
                long orderId = (long)await command.ExecuteScalarAsync();

                foreach (Bouquet bouquet in order.Bouquets)
                {
                    command.CommandText = @"
                        INSERT INTO OrderBouquets (OrderId, BouquetName)
                        VALUES ($orderId, $bouquetName)";
                    command.Parameters.AddWithValue("$orderId", orderId);
                    command.Parameters.AddWithValue("$bouquetName",
                        bouquet.Name);
                    await command.ExecuteNonQueryAsync();
                    command.Parameters.Clear();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wyst¹pi³ b³¹d: {ex.Message}");
        }
    }

    public async Task LoadDataAsync(Shop shop)
    {
        try
        {
            using SqliteConnection connection = new(ConnectionString);
            await connection.OpenAsync();

            SqliteCommand command = connection.CreateCommand();

            // Load Flowers
            command.CommandText =
                "SELECT Name, Color, Price, InStock FROM Flowers";
            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                shop.Flowers.Clear();
                while (await reader.ReadAsync())
                    shop.Flowers.Add(new Flower(
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetFloat(2),
                        reader.GetInt32(3)
                    ));
            }

            // Load Bouquets
            command.CommandText = "SELECT Name, Price, InStock FROM Bouquets";
            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                shop.Bouquets.Clear();
                while (await reader.ReadAsync())
                    shop.Bouquets.Add(new Bouquet(
                        reader.GetString(0),
                        new List<FlowerCopy>(), // Flowers will be loaded later
                        reader.GetFloat(1),
                        reader.GetInt32(2)
                    ));
            }

            // Load FlowerCopies
            command.CommandText =
                "SELECT BouquetName, Name, Color, Count FROM FlowerCopies";
            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Bouquet? bouquet =
                        shop.Bouquets.FirstOrDefault(b =>
                            b.Name == reader.GetString(0));
                    if (bouquet != null)
                        bouquet.Flowers.Add(new FlowerCopy(
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetInt32(3)
                        ));
                }
            }

            // Load Customers
            command.CommandText = "SELECT Name, Email, Phone FROM Customers";
            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                shop.Customers.Clear();
                while (await reader.ReadAsync())
                    shop.Customers.Add(new Customer(
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2)
                    ));
            }

            // Load Orders
            command.CommandText =
                "SELECT OrderDate, CustomerEmail, TotalPrice FROM Orders";
            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                shop.Orders.Clear();
                while (await reader.ReadAsync())
                {
                    Customer? customer =
                        shop.Customers.Find(c =>
                            c.Email == reader.GetString(1));
                    if (customer != null)
                        shop.Orders.Add(new Order(
                            DateTime.Parse(reader.GetString(0)),
                            customer,
                            new List<
                                Bouquet>(), // Bouquets will be loaded later
                            reader.GetFloat(2)
                        ));
                }
            }

            // Load OrderBouquets
            command.CommandText =
                "SELECT OrderId, BouquetName FROM OrderBouquets";
            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Order order = shop.Orders[(int)reader.GetInt64(0) - 1];
                    Bouquet? bouquet =
                        shop.Bouquets.Find(b => b.Name == reader.GetString(1));
                    if (order != null && bouquet != null)
                        order.Bouquets.Add(bouquet);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wyst¹pi³ b³¹d: {ex.Message}");
        }
    }
}
