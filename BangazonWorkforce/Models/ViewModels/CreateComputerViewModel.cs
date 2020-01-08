using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class CreateComputerViewModel
    {
        public List<SelectListItem> employees { get; set; } = new List<SelectListItem>();

        public Computer computer { get; set; }

        private string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public CreateComputerViewModel() { }

        public CreateComputerViewModel(string connectionString)
        {
            _connectionString = connectionString;

            employees = GetAvailableEmployees()
               .Select(employee => new SelectListItem()
               {
                   Text = employee.LastName,
                   Value = employee.Id.ToString()

               })
               .ToList();

            // Add an option with instructions for how to use the dropdown
            employees.Insert(0, new SelectListItem
            {
                Text = "Choose an Employee",
                Value = "0"
            });
        }

        private List<Employee> GetAvailableEmployees()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Employee.Id, Employee.LastName FROM Employee JOIN ComputerEmployee ON Employee.Id=ComputerEmployee.EmployeeId WHERE ComputerEmployee.EmployeeId IS NULL";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        });
                    }

                    reader.Close();

                    return employees;
                }
            }
        }
    }
}
