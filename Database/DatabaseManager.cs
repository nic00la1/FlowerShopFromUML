using FlowerShopFromUML;
using Microsoft.Data.Sqlite;

public class DatabaseManager
{
    private const string ConnectionString = "Data Source=flowershop.db";

    public void SaveData(Shop shop)
    {
        try
        {
            using SqliteConnection connection = new(ConnectionString);
            connection.Open();

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
            command.ExecuteNonQuery();

            Console.WriteLine("Tabele zosta³y utworzone pomyœlnie.");

            // Save Flowers
            command.CommandText = "DELETE FROM Flowers";
            command.ExecuteNonQuery();
            foreach (Flower flower in shop.Flowers)
            {
                command.CommandText = @"
                    INSERT INTO Flowers (Name, Color, Price, InStock)
                    VALUES ($name, $color, $price, $inStock)";
                command.Parameters.AddWithValue("$name", flower.Name);
                command.Parameters.AddWithValue("$color", flower.Color);
                command.Parameters.AddWithValue("$price", flower.Price);
                command.Parameters.AddWithValue("$inStock", flower.InStock);
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }

            // Save Bouquets
            command.CommandText = "DELETE FROM Bouquets";
            command.ExecuteNonQuery();
            foreach (Bouquet bouquet in shop.Bouquets)
            {
                command.CommandText = @"
                    INSERT INTO Bouquets (Name, Price, InStock)
                    VALUES ($name, $price, $inStock)";
                command.Parameters.AddWithValue("$name", bouquet.Name);
                command.Parameters.AddWithValue("$price", bouquet.Price);
                command.Parameters.AddWithValue("$inStock", bouquet.InStock);
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }

            // Save Customers
            command.CommandText = "DELETE FROM Customers";
            command.ExecuteNonQuery();
            foreach (Customer customer in shop.Customers)
            {
                command.CommandText = @"
                    INSERT INTO Customers (Name, Email, Phone)
                    VALUES ($name, $email, $phone)";
                command.Parameters.AddWithValue("$name", customer.Name);
                command.Parameters.AddWithValue("$email", customer.Email);
                command.Parameters.AddWithValue("$phone", customer.Phone);
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }

            // Save Orders
            command.CommandText = "DELETE FROM Orders";
            command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM OrderBouquets";
            command.ExecuteNonQuery();
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
                command.ExecuteNonQuery();

                command.CommandText = "SELECT last_insert_rowid()";
                long orderId = (long)command.ExecuteScalar();

                foreach (Bouquet bouquet in order.Bouquets)
                {
                    command.CommandText = @"
                        INSERT INTO OrderBouquets (OrderId, BouquetName)
                        VALUES ($orderId, $bouquetName)";
                    command.Parameters.AddWithValue("$orderId", orderId);
                    command.Parameters.AddWithValue("$bouquetName",
                        bouquet.Name);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }

            Console.WriteLine("Dane zosta³y zapisane pomyœlnie.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wyst¹pi³ b³¹d: {ex.Message}");
        }
    }

    public void LoadData(Shop shop)
    {
        try
        {
            using SqliteConnection connection = new(ConnectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            // Load Flowers
            command.CommandText =
                "SELECT Name, Color, Price, InStock FROM Flowers";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                shop.Flowers.Clear();
                while (reader.Read())
                    shop.Flowers.Add(new Flower(
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetFloat(2),
                        reader.GetInt32(3)
                    ));
            }

            // Load Bouquets
            command.CommandText = "SELECT Name, Price, InStock FROM Bouquets";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                shop.Bouquets.Clear();
                while (reader.Read())
                    shop.Bouquets.Add(new Bouquet(
                        reader.GetString(0),
                        new List<FlowerCopy>(), // Flowers will be loaded later
                        reader.GetFloat(1),
                        reader.GetInt32(2)
                    ));
            }

            // Load Customers
            command.CommandText = "SELECT Name, Email, Phone FROM Customers";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                shop.Customers.Clear();
                while (reader.Read())
                    shop.Customers.Add(new Customer(
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2)
                    ));
            }

            // Load Orders
            command.CommandText =
                "SELECT OrderDate, CustomerEmail, TotalPrice FROM Orders";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                shop.Orders.Clear();
                while (reader.Read())
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
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Order order = shop.Orders[(int)reader.GetInt64(0) - 1];
                    Bouquet? bouquet =
                        shop.Bouquets.Find(b => b.Name == reader.GetString(1));
                    if (order != null && bouquet != null)
                        order.Bouquets.Add(bouquet);
                }
            }

            Console.WriteLine("Dane zosta³y wczytane pomyœlnie.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wyst¹pi³ b³¹d: {ex.Message}");
        }
    }

    public void RemoveEmptyBouquetsFromDatabase()
    {
        try
        {
            using SqliteConnection connection = new(ConnectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM Bouquets
                WHERE Name NOT IN (
                    SELECT DISTINCT BouquetName
                    FROM OrderBouquets
                );
            ";
            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine($"Usuniêto {rowsAffected} pustych bukietów.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wyst¹pi³ b³¹d: {ex.Message}");
        }
    }
}
