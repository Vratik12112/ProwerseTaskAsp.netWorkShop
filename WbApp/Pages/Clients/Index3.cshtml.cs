using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration; // Add this namespace for IConfiguration
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WbApp.Pages.Clients
{
    public class Index3Model : PageModel
    {
        private readonly IConfiguration _configuration;

        // Add a constructor to inject IConfiguration
        public Index3Model(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Property to store the current sorting criteria
        public string SortBy { get; set; }

        // Property to store the list of due vehicles
        public List<Vehicle2> DueVehicles { get; set; }

        // GET request handler with optional sorting parameter
        public IActionResult OnGet(string sortBy = "ServiceDueDate")
        {
            // Set the current sorting criteria
            SortBy = sortBy;
            DueVehicles = GetDueVehicles(sortBy);
            return Page();
        }

        // Method to fetch vehicles due for service within a week based on the sorting criteria
        private List<Vehicle2> GetDueVehicles(string sortBy)
        {
            List<Vehicle2> vehicles = new List<Vehicle2>();

            try
            {
                // Get the connection string from appsettings.json
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Construct SQL query based on the sorting criteria
                    string sqlQuery = $"SELECT VehicleId, Make, Model, Year, ServiceDueDate FROM Vehicles WHERE DATEDIFF(day, GETDATE(), ServiceDueDate) <= 7 ORDER BY {sortBy}";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                vehicles.Add(new Vehicle2
                                {
                                    VehicleId = Convert.ToInt32(reader["VehicleId"]),
                                    Make = reader["Make"].ToString(),
                                    Model = reader["Model"].ToString(),
                                    Year = Convert.ToInt32(reader["Year"]),
                                    ServiceDueDate = Convert.ToDateTime(reader["ServiceDueDate"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }

            return vehicles;
        }
    }

    // Model class for a vehicle due for service
    public class Vehicle2
    {
        public int VehicleId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public DateTime ServiceDueDate { get; set; }
    }
}
