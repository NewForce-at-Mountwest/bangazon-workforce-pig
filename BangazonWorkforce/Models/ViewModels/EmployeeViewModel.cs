using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public List<SelectListItem> Departments { get; set; }
        protected string _connectionString;
        public Employee employee { get; set; }
        protected SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }
        public EmployeeViewModel() { }
        public EmployeeViewModel(string connectionString)
        {
            _connectionString = connectionString;

            // When we create a new instance of this view model, we'll call the internal methods to get all the cohorts from the database
            // Then we'll map over them and convert the list of cohorts to a list of select list items
            Departments = GetAllDepartments()
                .Select(department => new SelectListItem()
                {
                    Text = department.Name,
                    Value = department.Id.ToString()

                })
                .ToList();

            // Add an option with instructiosn for how to use the dropdown
            Departments.Insert(0, new SelectListItem
            {
                Text = "Choose a department",
                Value = "0"
            });

        }
        protected List<Department> GetAllDepartments()
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
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        });
                    }
                    reader.Close();
                    return departments;
                }
            }
        }

    }
}
