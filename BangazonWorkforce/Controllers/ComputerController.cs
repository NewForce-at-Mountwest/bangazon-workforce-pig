using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class ComputerController : Controller
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Computer
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT c.Id,
                c.Make,
                c.Manufacturer,
                c.PurchaseDate,
                c.DecomissionDate, Employee.FirstName AS 'FirstName', Employee.LastName AS 'LastName'
            FROM Computer c
FULL JOIN ComputerEmployee ON c.Id=ComputerEmployee.ComputerId FULL JOIN Employee ON ComputerEmployee.EmployeeId=Employee.Id
        ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    //create a list of computers
                    List<Computer> computers = new List<Computer>();
                    //create a variable for null entry
                    DateTime? nullDateTime = null;

                    while (reader.Read())
                    {
                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecomissionDate = reader.IsDBNull(reader.GetOrdinal("DecomissionDate")) ? nullDateTime : reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                            
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("LastName")))
                        {
                            computer.CurrentEmployee = new Employee { FirstName = reader.GetString(reader.GetOrdinal("FirstName")), LastName = reader.GetString(reader.GetOrdinal("LastName")) };
                        };
                        //add computer to the list
                        computers.Add(computer);
                    }

                    reader.Close();

                    return View(computers);
                }
            }
        }

        // GET: Computer/Details/id
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT c.Id,
                c.Make,
                c.Manufacturer,
                c.PurchaseDate,
                c.DecomissionDate
            FROM Computer c WHERE c.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    //variable for null
                    DateTime? nullDateTime = null;
                    //new computer instance
                    Computer computer = null;

                    if (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecomissionDate = reader.IsDBNull(reader.GetOrdinal("DecomissionDate")) ? nullDateTime : reader.GetDateTime(reader.GetOrdinal("DecomissionDate"))
                        };
                    }
                    reader.Close();

                    return View(computer);
                }
            }

        }

        // GET: Computer/Create
        //NEW FORM
        public ActionResult Create()
        {
            // Create a new instance of a CreateComputerViewModel
            // If we want to get the employees, we need to use the constructor that's expecting a connection string. 
            // When we create this instance, the constructor will run and get the employees.
            CreateComputerViewModel computerViewModel = new CreateComputerViewModel(_config.GetConnectionString("DefaultConnection"));

            // Once we've created it, we can pass it to the view
            return View(computerViewModel);
        }

        // POST: Computer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateComputerViewModel model)
        {
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Computer ( Make, Manufacturer, PurchaseDate) 
                        OUTPUT INSERTED.Id
                        VALUES ( @Make, @Manufacturer, @PurchaseDate)";
                        cmd.Parameters.Add(new SqlParameter("@Make", model.computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@Manufacturer", model.computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@PurchaseDate", model.computer.PurchaseDate));
                        cmd.ExecuteNonQuery();
                        int newId = (int)cmd.ExecuteScalar();
                        model.computer.Id = newId;

                        if (model.computer.CurrentEmployee.Id != 0)
                        {
                            cmd.CommandText += @" INSERT INTO ComputerEmployee ( EmployeeId, ComputerId, AssignDate, UnassignDate) 
                        VALUES ( @EmployeeId, @ComputerId, @AssignDate, NULL)";
                            cmd.Parameters.Add(new SqlParameter("@EmployeeId", model.computer.CurrentEmployee.Id));
                            cmd.Parameters.Add(new SqlParameter("@ComputerId", newId));
                            cmd.Parameters.Add(new SqlParameter("@AssignDate", DateTime.Now));
                            cmd.ExecuteNonQuery();
                        }
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
        }

        // GET: Computer/Delete/5
        public ActionResult Delete(int id)
        {
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        //bring up DELETE View
                        cmd.CommandText = @"
            SELECT c.Id,
                c.Make,
                c.Manufacturer,
                c.PurchaseDate,
                c.DecomissionDate
            FROM Computer c WHERE c.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();
                        //set the variable for a null entry
                        DateTime? nullDateTime = null;
                        //create new instance for a computer
                        Computer computer = null;
                        
                        if (reader.Read())
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                DecomissionDate = reader.IsDBNull(reader.GetOrdinal("DecomissionDate")) ? nullDateTime : reader.GetDateTime(reader.GetOrdinal("DecomissionDate"))
                            };
                        }
                        reader.Close();

                        return View(computer);
                    }
                }
            }
            }

        // POST: Computer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                //the computer should be deleted only if it is has never been assigned to an employee
                

                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                        //find out if this computer has any associations to employees in the employeecomputer dataset
                        cmd.CommandText = @"SELECT ComputerEmployee.ComputerId FROM ComputerEmployee WHERE ComputerEmployee.Id = @id";

                            cmd.Parameters.Add(new SqlParameter("@id", id));
                            SqlDataReader reader = cmd.ExecuteReader();


                            if (reader.Read())
                            {
                                //throw error
                                throw new Exception("Cannot delete due to association with one or more employees.");
                            }

                            else
                            {
                                //DELETE IT
                                cmd.CommandText = @"DELETE FROM Computer WHERE Id = @id";
                            }
                            reader.Close();
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return RedirectToAction(nameof(Index));
                        }
                            throw new Exception("No rows affected");
                        }
                    }
                
            }
            catch
            {

                if (!ComputerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
               
            }
        }
        private bool ComputerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Make, Manufacturer, PurchaseDate, DecomissionDate
                        FROM Computer
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}