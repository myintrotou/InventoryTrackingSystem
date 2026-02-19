using System;
using System.Data;
// CHANGE 1: Use the new namespace
using Microsoft.Data.SqlClient;

namespace InventoryTrackingSystem
{
    class Program
    {
        // CHANGE 2: Add "TrustServerCertificate=True" to the connection string.
        // The new library requires this for local development to trust your local SQL Server.
        static string connectionString = "Data Source=.;Initial Catalog=InventoryDB;Integrated Security=True;TrustServerCertificate=True";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== INVENTORY TRACKING SYSTEM (EXECUTIVE) ===");
                Console.WriteLine("1. Add Stock (Stored Proc)");
                Console.WriteLine("2. View Low Stock Report");
                Console.WriteLine("3. View Suppliers");
                Console.WriteLine("4. Place New Order");
                Console.WriteLine("5. Exit");
                Console.Write("Select: ");

                var choice = Console.ReadLine();

                if (choice == "1") AddStock();
                else if (choice == "2") GetLowStock();
                else if (choice == "3") GetSuppliers();
                else if (choice == "4") CreateOrder();
                else if (choice == "5") break;
            }
        }

        static void AddStock()
        {
            Console.Write("Enter Product ID: ");
            if (!int.TryParse(Console.ReadLine(), out int pId)) return;

            Console.Write("Enter Quantity to Add: ");
            if (!int.TryParse(Console.ReadLine(), out int qty)) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_AddStock", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@P_Id", pId);
                    cmd.Parameters.AddWithValue("@Qty", qty);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Stock Updated Successfully!");
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
                Console.ReadKey();
            }
        }

        static void GetLowStock()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_GetLowStock", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();

                    Console.WriteLine("\n--- Low Stock Report ---");
                    while (reader.Read())
                    {
                        Console.WriteLine($"Product: {reader["ProductName"]} | Stock: {reader["StockQuantity"]} | Reorder At: {reader["ReorderLevel"]}");
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
                Console.ReadKey();
            }
        }

        static void GetSuppliers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_GetSuppliers", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();

                    Console.WriteLine("\n--- Supplier List ---");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["SupplierID"]} | Name: {reader["SupplierName"]} | Contact: {reader["ContactName"]} | Ph: {reader["Phone"]}");
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
                Console.ReadKey();
            }
        }

        static void CreateOrder()
        {
            Console.Write("Enter Product ID to Order: ");
            if (!int.TryParse(Console.ReadLine(), out int pId)) return;

            Console.Write("Enter Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int qty)) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_CreateOrder", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@P_Id", pId);
                    cmd.Parameters.AddWithValue("@Qty", qty);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Order Placed Successfully!");
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
                Console.ReadKey();
            }
        }
    }
}
