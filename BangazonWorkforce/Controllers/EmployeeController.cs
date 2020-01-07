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
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
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
        // GET: Employee
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string query = @"SELECT Employee.Id AS 'Id', DepartmentId, Department.Budget AS 'Budget', Department.Id AS 'DeptId', FirstName,  LastName, isSupervisor,  Department.Name AS 'DeptName' FROM Employee JOIN Department ON DepartmentId = Department.Id ";
                    cmd.CommandText = query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {

                        int idColumnPosition = reader.GetOrdinal("Id");

                        int idValue = reader.GetInt32(idColumnPosition);
                        int DepartmentIdColumnPosition = reader.GetOrdinal("DepartmentId");
                        int DepartmentIdValue = reader.GetInt32(DepartmentIdColumnPosition);

                        int FirstNameColumnPosition = reader.GetOrdinal("FirstName");
                        string FirstNameValue = reader.GetString(FirstNameColumnPosition);
                        int LastNameColumnPosition = reader.GetOrdinal("LastName");
                        string LastNameValue = reader.GetString(LastNameColumnPosition);
                        bool IsSupervisorValue = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"));
                        Employee employee = new Employee
                        {
                            Id = idValue,
                            FirstName = FirstNameValue,
                            LastName = LastNameValue,
                            DepartmentId = DepartmentIdValue,
                            IsSuperVisor = IsSupervisorValue,
                            CurrentDepartment = new Department{
                                Name = reader.GetString(reader.GetOrdinal("DeptName")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                                Id = reader.GetInt32(reader.GetOrdinal("DeptId"))

                            }
                        };
                        employees.Add(employee);
                    }
                    reader.Close();
                    return View(employees);
                }
            }
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT  Employee.Id AS 'Id', FirstName, 
LastName, Department.Name AS 'DeptName', isSupervisor ,  Computer.Make AS 'Make', TrainingProgram.Id AS 'TPID', TrainingProgram.Name AS 'TrainingName', Computer.Manufacturer AS 'Manufacturer'
FROM Employee  
 JOIN Department ON DepartmentId = Department.Id Join ComputerEmployee ON Employee.Id = ComputerEmployee.EmployeeId JOIN Computer ON Computer.Id = ComputerId
Join EmployeeTraining ON Employee.Id = EmployeeTraining.EmployeeId Join TrainingProgram ON TrainingProgram.Id = TrainingProgramId WHERE Employee.Id = @Id ";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Employee employee = null;
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor")),
                            CurrentComputer = new Computer
                            {
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            },
                            CurrentDepartment = new Department
                            {
                                Name = reader.GetString(reader.GetOrdinal("DeptName"))
                            }
                       
                        };
                        TrainingProgram training = new TrainingProgram
                        {
                            Name = reader.GetString(reader.GetOrdinal("TrainingName"))
                        };
                   if(     employee.TrainingPrograms.FirstOrDefault(program => program.Id == reader.GetInt32(reader.GetOrdinal("TPID")))== null)
                        {
                            employee.TrainingPrograms.Add(training);
                        }
                    }
                    reader.Close();

                    return View(employee);
                }
            }
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
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