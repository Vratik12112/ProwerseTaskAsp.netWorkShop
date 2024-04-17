using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace WbApp.Pages.Clients
{
    public class Index2Model : PageModel
    {
        private readonly IConfiguration _configuration;

        // Constructor to inject IConfiguration for accessing app settings
        public Index2Model(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<Vehicle1> Vehicles { get; set; }

        public IActionResult OnGet()
        {
            // Implement logic to fetch available vehicles for booking from the database
            Vehicles = GetAvailableVehicles(); // Example method to fetch vehicles, implement as needed
            return Page();
        }

        public IActionResult OnPost(int vehicleId, DateTime bookingDate)
        {
            // Implement logic to book the slot for the selected vehicle and date in the database
            bool bookingSuccess = BookSlot(vehicleId, bookingDate); // Example method to book slot, implement as needed

            if (bookingSuccess)
            {
                TempData["BookingMessage"] = $"Slot booked successfully for Vehicle ID {vehicleId} on {bookingDate.ToShortDateString()}!";
            }
            else
            {
                TempData["BookingMessage"] = "Booking failed. Please try again.";
            }

            // Fetch available vehicles again after booking
            Vehicles = GetAvailableVehicles();

            return Page();
        }


        private List<Vehicle1> GetAvailableVehicles()
        {
            List<Vehicle1> availableVehicles = new List<Vehicle1>();

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection"); 

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT * FROM Vehicles WHERE VehicleId NOT IN (SELECT VehicleId FROM Bookings WHERE BookingDate = @bookingDate)";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@bookingDate", DateTime.Today); // Assuming booking for today

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            availableVehicles.Add(new Vehicle1
                            {
                                VehicleId = Convert.ToInt32(reader["VehicleId"]),
                                Make = reader["Make"].ToString(),
                                Model = reader["Model"].ToString(),
                                Year = Convert.ToInt32(reader["Year"])
                            });
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception, log error, etc.
                TempData["Message"] = "Error fetching available vehicles. Please try again later.";
            }

            return availableVehicles;
        }


        private bool BookSlot(int vehicleId, DateTime bookingDate)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection"); 

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Check if there are already four bookings for the selected date
                    string checkSlotsQuery = "SELECT COUNT(*) FROM Bookings WHERE BookingDate = @bookingDate";
                    using (SqlCommand checkSlotsCommand = new SqlCommand(checkSlotsQuery, connection))
                    {
                        checkSlotsCommand.Parameters.AddWithValue("@bookingDate", bookingDate.Date);
                        connection.Open();
                        int bookedSlots = (int)checkSlotsCommand.ExecuteScalar();

                        if (bookedSlots >= 4)
                        {
                            TempData["BookingMessage"] = "Sorry, all slots for this date are booked. Please choose another date.";
                            return false; // Booking failed
                        }
                    }

                    // Proceed with booking since there are less than four bookings for the selected date
                    string insertBookingQuery = "INSERT INTO Bookings (VehicleId, BookingDate) VALUES (@vehicleId, @bookingDate)";
                    using (SqlCommand insertBookingCommand = new SqlCommand(insertBookingQuery, connection))
                    {
                        insertBookingCommand.Parameters.AddWithValue("@vehicleId", vehicleId);
                        insertBookingCommand.Parameters.AddWithValue("@bookingDate", bookingDate);
                        int rowsAffected = insertBookingCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            TempData["BookingMessage"] = $"Slot booked successfully for Vehicle ID {vehicleId} on {bookingDate.ToShortDateString()}!";
                            return true; // Booking successful
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error booking slot. Please try again later.";
            }

            return false; // Booking failed due to error
        }

    }

    public class Vehicle1
    {
        public int VehicleId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
    }
}