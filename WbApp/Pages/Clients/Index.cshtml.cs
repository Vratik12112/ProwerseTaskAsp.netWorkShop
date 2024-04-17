using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration; // Import this namespace for IConfiguration
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WbApp.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // Constructor to inject IConfiguration for accessing app settings
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Property to hold the list of customers
        public List<Customer> Customers { get; set; }

        // Page handler for HTTP GET requests
        public IActionResult OnGet(string searchQuery)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            Customers = new List<Customer>();

            // Using block to automatically dispose of SqlConnection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM Customers";
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    sqlQuery += " WHERE Name LIKE @searchQuery OR Email LIKE @searchQuery";
                }

                // Using block to automatically dispose of SqlCommand
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        command.Parameters.AddWithValue("@searchQuery", $"%{searchQuery}%");
                    }

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    // Read data from SqlDataReader and populate Customers list
                    while (reader.Read())
                    {
                        Customers.Add(new Customer
                        {
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString()
                        });
                    }

                    reader.Close(); // Close the SqlDataReader
                }
            }

            return Page(); // Return the Razor Page
        }
    }

    // Model class to represent a Customer
    public class Customer
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
