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
 JOIN Department ON DepartmentId = Department.Id FULL Join ComputerEmployee ON Employee.Id = ComputerEmployee.EmployeeId  FUll JOIN Computer ON Computer.Id = ComputerId
LEFT Join EmployeeTraining ON Employee.Id = EmployeeTraining.EmployeeId LEFT Join TrainingProgram ON TrainingProgram.Id = TrainingProgramId WHERE Employee.Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Employee employee = null;
                    if (reader.Read())
                    {
                        if (reader.IsDBNull(reader.GetOrdinal("Make")) == false)
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
                        }
                        else
                        {
                            employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor")),

                                CurrentDepartment = new Department
                                {
                                    Name = reader.GetString(reader.GetOrdinal("DeptName"))
                                }

                            };
                        }
                        if (reader.IsDBNull(reader.GetOrdinal("TrainingName")) == false)
                        {
                            TrainingProgram training = new TrainingProgram
                            {
                                Name = reader.GetString(reader.GetOrdinal("TrainingName"))
                            };

                            if (employee.TrainingPrograms.FirstOrDefault(program => program.Id == reader.GetInt32(reader.GetOrdinal("TPID"))) == null)
                            {
                                employee.TrainingPrograms.Add(training);
                            }

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
            EmployeeViewModel EmployeeViewModel = new EmployeeViewModel(_config.GetConnectionString("DefaultConnection"));

            // Once we've created it, we can pass it to the view
            return View(EmployeeViewModel);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee
                ( FirstName, LastName, DepartmentId, isSupervisor )
                VALUES
                ( @firstName, @lastName, @DepartmentId, @isSupervisor )";
                        cmd.Parameters.Add(new SqlParameter("@firstName", model.employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", model.employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@DepartmentId", model.employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", model.employee.IsSuperVisor));
                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View(model);
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            EditEmployeeViewModel viewModel = new EditEmployeeViewModel(id, _config.GetConnectionString("DefaultConnection"));
            return View(viewModel);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EditEmployeeViewModel viewModel)
        {
            try
            {
                // TODO: Add update logic here
               
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            string command = $@"UPDATE Employee SET
                                            lastName=@lastName, 
                                            departmentId=@departmentId 
                                            WHERE Id = @id;
                                            UPDATE ComputerEmployee SET UnassignDate= {DateTime.Now.ToString("MM/dd/yyyy")} WHERE employeeId =@id ";

                                command += $" INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate) VALUES (@id, @computerId, {DateTime.Now.ToString("MM/dd/yyyy")})";

                            cmd.CommandText = command;
                            cmd.Parameters.Add(new SqlParameter("@lastName", viewModel.Employee.LastName));
                            cmd.Parameters.Add(new SqlParameter("@departmentId", viewModel.Employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@computerId", viewModel.Employee.CurrentComputer.Id));
                            cmd.Parameters.Add(new SqlParameter("@id", id));

                            int rowsAffected = cmd.ExecuteNonQuery();
                     
                    }

                }
                return RedirectToAction(nameof(Index));


            }
            catch(Exception e)
            {
                return View(viewModel);
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