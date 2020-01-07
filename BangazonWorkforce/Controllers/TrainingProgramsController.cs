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

                        programs.Add(program);
                    }

                    reader.Close();

                    return View(programs);
                }
            }
        }

        // GET: TrainingProgram/Details/5
        public ActionResult Details(int id)
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
                        WHERE Id = @id";
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


        // GET: TrainingProgram/Create
        public ActionResult Create()
        {
            CreateProgramViewModel programViewModel = new CreateProgramViewModel(_config.GetConnectionString("DefaultConnection"));
            return View(programViewModel);
        }

        // POST: TrainingProgram/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(CreateProgramViewModel model)
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
                    cmd.Parameters.Add(new SqlParameter("@firstName", model.TrainingProgram.Name));
                    cmd.Parameters.Add(new SqlParameter("@lastName", model.TrainingProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", model.TrainingProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", model.TrainingProgram.MaxAttendees));
                    cmd.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // GET: TrainingProgram/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
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