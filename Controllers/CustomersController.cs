using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UsersApp.Data;
using UsersApp.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace UsersApp.Controllers
{
    [Authorize] // 需要登入才能訪問
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public CustomersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: /Customers
        public IActionResult Index()
        {
            List<Customer> customers = new List<Customer>();

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Customers ORDER BY Id DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Customer customer = new Customer
                                {
                                    Id = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    Phone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Address = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    Company = reader.GetString(6),
                                    Notes = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                    CreatedAt = reader.GetDateTime(8)
                                };
                                customers.Add(customer);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 錯誤處理
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View(customers);
        }

        // GET: /Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = _configuration.GetConnectionString("DefaultConnection");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = @"
                            INSERT INTO Customers 
                            (FirstName, LastName, Email, Phone, Address, Company, Notes, CreatedAt) 
                            VALUES 
                            (@FirstName, @LastName, @Email, @Phone, @Address, @Company, @Notes, GETDATE());";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                            command.Parameters.AddWithValue("@LastName", customer.LastName);
                            command.Parameters.AddWithValue("@Email", customer.Email);
                            command.Parameters.AddWithValue("@Phone", customer.Phone ?? "");
                            command.Parameters.AddWithValue("@Address", customer.Address ?? "");
                            command.Parameters.AddWithValue("@Company", customer.Company);
                            command.Parameters.AddWithValue("@Notes", customer.Notes ?? "");

                            command.ExecuteNonQuery();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                }
            }

            return View(customer);
        }

        // GET: /Customers/Edit/5
        public IActionResult Edit(int id)
        {
            Customer customer = new Customer();

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Customers WHERE id = @id;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                customer.Id = reader.GetInt32(0);
                                customer.FirstName = reader.GetString(1);
                                customer.LastName = reader.GetString(2);
                                customer.Email = reader.GetString(3);
                                customer.Phone = reader.IsDBNull(4) ? null : reader.GetString(4);
                                customer.Address = reader.IsDBNull(5) ? null : reader.GetString(5);
                                customer.Company = reader.GetString(6);
                                customer.Notes = reader.IsDBNull(7) ? null : reader.GetString(7);
                                customer.CreatedAt = reader.GetDateTime(8);
                            }
                            else
                            {
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View(customer);
        }

        // POST: /Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = _configuration.GetConnectionString("DefaultConnection");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = @"
                            UPDATE Customers 
                            SET 
                                FirstName = @FirstName,
                                LastName = @LastName,
                                Email = @Email,
                                Phone = @Phone,
                                Address = @Address,
                                Company = @Company,
                                Notes = @Notes
                            WHERE id = @Id;";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);
                            command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                            command.Parameters.AddWithValue("@LastName", customer.LastName);
                            command.Parameters.AddWithValue("@Email", customer.Email);
                            command.Parameters.AddWithValue("@Phone", customer.Phone ?? "");
                            command.Parameters.AddWithValue("@Address", customer.Address ?? "");
                            command.Parameters.AddWithValue("@Company", customer.Company);
                            command.Parameters.AddWithValue("@Notes", customer.Notes ?? "");

                            command.ExecuteNonQuery();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                }
            }

            return View(customer);
        }

        // GET: /Customers/Delete/5
        public IActionResult Delete(int id)
        {
            Customer customer = new Customer();

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Customers WHERE id = @id;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                customer.Id = reader.GetInt32(0);
                                customer.FirstName = reader.GetString(1);
                                customer.LastName = reader.GetString(2);
                                customer.Email = reader.GetString(3);
                                customer.Phone = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                customer.Address = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                customer.Company = reader.GetString(6);
                                customer.Notes = reader.IsDBNull(7) ? "" : reader.GetString(7);
                            }
                            else
                            {
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View(customer);
        }

        // POST: /Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Customers WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting customer: {ex.Message}";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}