using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using BangazonWorkforce.Models;
namespace BangazonWorkforce.Controllers
{
    public class DepartmentsController : Controller
    {
        //Making connection to server
        private readonly IConfiguration _config;
        public DepartmentsController(IConfiguration config)
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
        // GET: Department
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                     SELECT d.Id AS 'Department Id',
                     d.Name,
                     d.Budget,
                     e.Id AS 'Employee Id',
                     e.FirstName,
                     e.LastName,
                     e.DepartmentId
                     FROM Department d
                     LEFT JOIN Employee e ON e.DepartmentId = d.Id
                ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        Department department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Department Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            Employees = new List<Employee>()
                        };
                        //Add the Employees
                        if (!reader.IsDBNull(reader.GetOrdinal("Employee Id")))
                        {
                            Employee employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Employee Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                            };
                            if (departments.Any(d => d.Id == department.Id))
                            {
                                Department departmentToReference = departments.Where(d => d.Id == department.Id).FirstOrDefault();
                                if (!departmentToReference.Employees.Any(s => s.Id == employee.Id))
                                {
                                    departmentToReference.Employees.Add(employee);
                                }
                            }
                            else
                            {
                                department.Employees.Add(employee);
                                departments.Add(department);
                            }
                        }
                       
                        else if (department.Employees.Count() == 0)
                        {
                            departments.Add(department);
                        };
                    }
                    reader.Close();
                    return View(departments);
                }
            }
        }
        // GET: Department/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT d.Id AS 'Department Id',
                                         d.Name,
                                         d.Budget,
                                         e.Id AS 'Employee Id',
                                         e.FirstName,
                                         e.LastName,
                                         e.DepartmentId
                                         FROM Department d
                                         FULL JOIN Employee e ON e.DepartmentId = d.Id
                                         WHERE d.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Department department = null;
                    while (reader.Read())
                    {
                        if (department == null)
                        {
                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Department Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                                Employees = new List<Employee>()
                            };
                        };
                        // add Employees to Employee List
                        if (!reader.IsDBNull(reader.GetOrdinal("Employee Id")))
                        {
                            Employee employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Employee Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                            };
                            department.Employees.Add(employee);
                        }
                    }

                    reader.Close();
                    return View(department);

                }
            }
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Department
                                            (Name, Budget)
                                            VALUES
                                            (@name, @budget)";
                    cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));

                    cmd.ExecuteNonQuery();
                    return RedirectToAction(nameof(Index));
                }
            }
        }
        // GET: Department/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }
        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        // GET: Department/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }
        // POST: Department/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
