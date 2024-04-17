using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // Import this namespace for IConfiguration
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WbApp; // Assuming this namespace contains the Vehicle class

namespace WbApp.Pages.Clients
{
    // Page model for handling customer vehicles
    public class Index1Model : PageModel
    {
        private readonly IConfiguration _configuration;

        // Constructor to inject IConfiguration for accessing app settings
        public Index1Model(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Property to hold the list of customer vehicles
        public List<Vehicle> CustomerVehicles { get; set; }

        // Page handler for HTTP GET requests
        public IActionResult OnGet(string customerName)
        {
            // Check if customer name is provided
            if (string.IsNullOrEmpty(customerName))
            {
                TempData["AlertMessage"] = "Please enter a customer name.";
                return Page(); // Return the Razor Page
            }

            // Call the method to fetch customer vehicles
            CustomerVehicles = GetCustomerVehicles(customerName);
            return Page(); // Return the Razor Page
        }

        // Method to fetch customer vehicles from the database
        private List<Vehicle> GetCustomerVehicles(string customerName)
        {
            List<Vehicle> vehicles = new List<Vehicle>();

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // SQL query to fetch vehicles for a specific customer
                    string sqlQuery = "SELECT v.VehicleId, v.Make, v.Model, v.Year " +
                                      "FROM Vehicles v " +
                                      "INNER JOIN Customers c ON v.CustomerId = c.CustomerId " +
                                      "WHERE c.Name = @customerName";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@customerName", customerName);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if data is available in the reader
                            if (reader.HasRows)
                            {
                                // Read data from SqlDataReader and populate vehicles list
                                while (reader.Read())
                                {
                                    vehicles.Add(new Vehicle
                                    {
                                        VehicleId = Convert.ToInt32(reader["VehicleId"]),
                                        Make = reader["Make"].ToString(),
                                        Model = reader["Model"].ToString(),
                                        Year = Convert.ToInt32(reader["Year"])
                                    });
                                }
                            }
                            else
                            {
                                // If no data found for the customer, fetch all vehicles
                                string sqlQuery1 = "SELECT * FROM Vehicles";
                                using (SqlCommand command1 = new SqlCommand(sqlQuery1, connection))
                                {
                                    using (SqlDataReader reader1 = command1.ExecuteReader())
                                    {
                                        while (reader1.Read())
                                        {
                                            vehicles.Add(new Vehicle
                                            {
                                                VehicleId = Convert.ToInt32(reader1["VehicleId"]),
                                                Make = reader1["Make"].ToString(),
                                                Model = reader1["Model"].ToString(),
                                                Year = Convert.ToInt32(reader1["Year"])
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception, log error, or set TempData message for debugging
                TempData["Message"] = "Error fetching vehicles: " + ex.Message;
            }

            return vehicles; // Return the list of vehicles
        }
    }
}
