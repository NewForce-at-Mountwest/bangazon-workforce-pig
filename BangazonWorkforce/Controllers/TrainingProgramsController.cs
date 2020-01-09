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
    public class TrainingProgramsController : Controller
    {
       

            private readonly IConfiguration _config;

            public TrainingProgramsController(IConfiguration config)
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
        // GET single training program and create a list when 'training programs' clicked
        // GET: TrainingProgram
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                     SELECT t.Id,
                     t.Name,
                    t.StartDate,
                    t.EndDate,
                    t.MaxAttendees
                    FROM TrainingProgram t";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> programs = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        TrainingProgram program = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                        //If the Training Program has already happened dont show it on the list
                        DateTime currentTime = DateTime.Now;
                        if (program.StartDate > currentTime)
                        {
                            programs.Add(program);
                        }

                        
                    }

                    reader.Close();

                    return View(programs);
                }
            }
        }
        // Details view of training programs when 'details' clicked
        // GET: TrainingProgram/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                        TrainingProgram.Id,
                                        TrainingProgram.Name,
                                        TrainingProgram.StartDate,
                                        TrainingProgram.EndDate,
                                        TrainingProgram.MaxAttendees,
                                        EmployeeTraining.Id,
                                        EmployeeTraining.EmployeeId,
                                        EmployeeTraining.TrainingProgramId,
                                        Employee.Id,
                                        Employee.FirstName,
                                        Employee.LastName
                                        FROM TrainingProgram 
                                        FULL JOIN EmployeeTraining ON EmployeeTraining.TrainingProgramId = EmployeeTraining.Id 
                                        FULL JOIN Employee ON Employee.Id = EmployeeTraining.EmployeeId
                                        WHERE TrainingProgram.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram program = null;

                    while (reader.Read())
                    {

                        if (program == null)
                        {
                            program = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                                Employees = new List<Employee>()

                            };
                        };

                        // Bring back the list of employees in the detail view
                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            Employee employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            };

                            program.Employees.Add(employee);

                        }
                    }


                    reader.Close();
                    return View(program);
                }
            }
        }

        // Creating a new training program 
        // GET: TrainingProgram/Create
        public ActionResult Create()
        {
            TrainingProgram trainingProgram = new TrainingProgram();
            return View(trainingProgram);
        }
        // Posting the training program to the current list
        // POST: TrainingProgram/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(TrainingProgram trainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO TrainingProgram
                ( Name, StartDate, EndDate, MaxAttendees )
                VALUES
                ( @Name, @StartDate, @EndDate, @MaxAttendees )";
                    cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                    cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));
                    cmd.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // GET: TrainingProgram/Edit/5
        public ActionResult Edit(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, Name, StartDate, EndDate, MaxAttendees
                        FROM TrainingProgram
                        WHERE Id = @id
                       ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram program = new TrainingProgram();
                   
                    if (reader.Read())
                    {
                        program = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))

                        };

                    }
                    reader.Close();

                    return View(program);
                }
            }
        }
        

        // POST: TrainingProgram/Edit/5
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

        // GET: TrainingProgram/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingProgram/Delete/5
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

    internal class CreateProgramViewModel
    {
        private string v;

        public CreateProgramViewModel(string v)
        {
            this.v = v;
        }

        public object TrainingProgram { get; internal set; }
    }
}