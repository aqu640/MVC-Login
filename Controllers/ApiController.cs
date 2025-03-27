// Swagger: https://localhost:5227/swagger/index.html
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using UsersApp.Models;

namespace UsersApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 添加身份驗證
    public class CustomersApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CustomersApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/CustomersApi
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetCustomers()
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
                                    Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    Address = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    Company = reader.GetString(6),
                                    Notes = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    CreatedAt = reader.GetDateTime(8)
                                };

                                customers.Add(customer);
                            }
                        }
                    }
                }

                return customers;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/CustomersApi/5
        [HttpGet("{id}")]
        public ActionResult<Customer> GetCustomer(int id)
        {
            try
            {
                Customer customer = null;
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
                                customer = new Customer
                                {
                                    Id = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    Address = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    Company = reader.GetString(6),
                                    Notes = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    CreatedAt = reader.GetDateTime(8)
                                };
                            }
                        }
                    }
                }

                if (customer == null)
                {
                    return NotFound($"Customer with ID {id} not found");
                }

                return customer;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/CustomersApi
        [HttpPost]
        public ActionResult<Customer> CreateCustomer([FromBody] Customer customer)
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
                        (@FirstName, @LastName, @Email, @Phone, @Address, @Company, @Notes, GETDATE());
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                        command.Parameters.AddWithValue("@LastName", customer.LastName);
                        command.Parameters.AddWithValue("@Email", customer.Email);
                        command.Parameters.AddWithValue("@Phone", customer.Phone ?? "");
                        command.Parameters.AddWithValue("@Address", customer.Address ?? "");
                        command.Parameters.AddWithValue("@Company", customer.Company);
                        command.Parameters.AddWithValue("@Notes", customer.Notes ?? "");

                        // 獲取新插入的 ID
                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        customer.Id = newId;
                        customer.CreatedAt = DateTime.Now;
                    }
                }

                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/CustomersApi/5
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, [FromBody] Customer customer)
        {
            try
            {
                // 檢查路徑 ID 是否與模型 ID 匹配
                if (id != customer.Id)
                {
                    return BadRequest("ID mismatch between URL and body");
                }

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
                        WHERE id = @Id;
                        SELECT @@ROWCOUNT;";

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

                        int rowsAffected = (int)command.ExecuteScalar();

                        if (rowsAffected == 0)
                        {
                            return NotFound($"Customer with ID {id} not found");
                        }
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/CustomersApi/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Customers WHERE id = @id; SELECT @@ROWCOUNT;";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        int rowsAffected = (int)command.ExecuteScalar();

                        if (rowsAffected == 0)
                        {
                            return NotFound($"Customer with ID {id} not found");
                        }
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}