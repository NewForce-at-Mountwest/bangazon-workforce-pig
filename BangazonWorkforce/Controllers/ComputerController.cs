using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
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
                c.DecomissionDate
            FROM Computer c
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
                            DecomissionDate = reader.IsDBNull(reader.GetOrdinal("DecomissionDate")) ? nullDateTime : reader.GetDateTime(reader.GetOrdinal("DecomissionDate"))

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
            return View();
        }

        // POST: Computer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Computer computer)
        {
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Computer
                            ( Make, Manufacturer, PurchaseDate ) VALUES ( @Make, @Manufacturer, @PurchaseDate )";
                        cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@Manufacturer", computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@PurchaseDate", computer.PurchaseDate));
                        cmd.ExecuteNonQuery();

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
                    return View();
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