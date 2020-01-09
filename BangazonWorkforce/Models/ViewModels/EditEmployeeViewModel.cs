using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EditEmployeeViewModel
    {
        public Employee Employee { get; set; }
        public List<SelectListItem> Departments { get; set; }

        public List<SelectListItem> Computers { get; set; }
        protected string _connectionString;


        protected SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }
 
        public EditEmployeeViewModel() { }
        public EditEmployeeViewModel(int employeeId, string connectionString)
        {

            _connectionString = connectionString;
            Employee = GetOneEmployee(employeeId);
            Computers = GetAllComputers()
           .Select(computer => new SelectListItem()
           {
               Text = computer.Manufacturer,
               Value = computer.Id.ToString(),
               Selected = computer.Id == computer.Id
           })
           .ToList();
            Departments = GetAllDepartments()
             .Select(department => new SelectListItem()
             {
                 Text = department.Name,
                 Value = department.Id.ToString(),
                 Selected = Employee.DepartmentId == department.Id
             })
             .ToList();


        }
        private Employee GetOneEmployee(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT  Employee.Id AS 'Id', FirstName,  DepartmentId,
LastName, Department.Name AS 'DeptName', isSupervisor, ComputerId
FROM Employee FULL JOIN ComputerEmployee ON Employee.Id = EmployeeId 
 JOIN Department ON DepartmentId = Department.Id WHERE Employee.Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Employee employee = null;
                    if (reader.Read())
                    {
                      
                            employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),

                                CurrentDepartment = new Department
                                {
                                    Name = reader.GetString(reader.GetOrdinal("DeptName"))
                                },
                                CurrentComputer = new Computer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ComputerId"))
                                }

                            };
           


                    }
                    reader.Close();

                    return employee;
                }
            }
        }
        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Department";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }

                    reader.Close();

                    return departments;
                }
            }

        }
        private List<Computer> GetAllComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Manufacturer, Make FROM Computer";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> computers = new List<Computer>();
                    while (reader.Read())
                    {
                        computers.Add(new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        });
                    }

                    reader.Close();

                    return computers;
                }
            }

        }
    }
}
