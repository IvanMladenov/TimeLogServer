using System.Data;
using TimeLogs.Interfaces;
using TimeLogs.ViewModels;
using System.Data.SqlClient;

namespace TimeLogs.Implementations
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;
        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Project> GetProgectsByTime(DateTime? dateFrom, DateTime? dateTo)
        {
            List<Project> projects = new List<Project>();
            var query = @"SELECT p.Name,
	                             SUM(l.TimeLog) TotalTime
                        FROM Project p
                        INNER JOIN TimeLog l ON l.ProjectId = p.Id
                        WHERE (@i_startDate IS NULL OR l.Date BETWEEN @i_startDate AND @i_endDate)
                        GROUP BY p.Name
                        ORDER BY 2 desc";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@i_startDate", dateFrom != null ? dateFrom : DBNull.Value);
                    command.Parameters.AddWithValue("@i_endDate", dateTo != null ? dateTo : DBNull.Value);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projects.Add(new Project
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                TimeWorked = reader.GetDouble(reader.GetOrdinal("TotalTime"))
                            });
                        }
                    }
                }
            }

            return projects;
        }
        public User GetCompareUserByTime(DateTime? dateFrom, DateTime? dateTo, int id)
        {
            User user = null;
            var query = @"SELECT u.Name,
	                               u.Surname,
                                   SUM(l.TimeLog) TotalTime
                            FROM TimeLog l
                            INNER JOIN [User] u ON l.UserId = u.Id
                            WHERE (@i_startDate IS NULL OR l.Date BETWEEN @i_startDate AND @i_endDate)
                            AND u.Id = @i_id
                            GROUP BY u.Id, u.Name, u.Surname";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@i_startDate", dateFrom != null ? dateFrom : DBNull.Value);
                    command.Parameters.AddWithValue("@i_endDate", dateTo != null ? dateTo : DBNull.Value);
                    command.Parameters.AddWithValue("@i_id", id);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            user = new User
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                TimeLogged = reader.GetDouble(reader.GetOrdinal("TotalTime"))
                            };
                        }
                    }
                }
            }
            return user;
        }

        public List<User> GetGridUsers(DataTablesRequest filters, ref int totalFilterd)
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                using (SqlCommand command = new SqlCommand("GetGridData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@i_startDate", filters.dateFrom != null ? filters.dateFrom : DBNull.Value);
                    command.Parameters.AddWithValue("@i_endDate", filters.dateTo != null ? filters.dateTo : DBNull.Value);
                    command.Parameters.AddWithValue("@i_startIndex", filters.start);
                    command.Parameters.AddWithValue("@i_pageSize", filters.length);
                    command.Parameters.AddWithValue("@i_sortColumn", filters.columns[filters.order[0].column].data);
                    command.Parameters.AddWithValue("@i_sortDirection", filters.order[0].dir);


                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                             users.Add(  new User 
                             { 
                                 Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                 Name = reader.GetString(reader.GetOrdinal("Name")),
                                 Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                 TimeLogged = reader.GetDouble(reader.GetOrdinal("TotalTime"))
                             }); 
                            
                             totalFilterd = reader.GetInt32(reader.GetOrdinal("TotalFiltered"));
                        }
                    }
                }
            }

            return users;
        }

        public List<User> GetTopUsersByTime(DateTime? dateFrom, DateTime? dateTo)
        {
            List<User> users = new List<User>();
            var query = @"SELECT TOP 10 
	                           u.Name,
	                           u.Surname,
                               SUM(l.TimeLog) TotalTime
                        FROM TimeLog l
                        INNER JOIN [User] u ON l.UserId = u.Id
                        WHERE (@i_startDate IS NULL OR l.Date BETWEEN @i_startDate AND @i_endDate)
                        GROUP BY u.Id, u.Name, u.Surname
                        ORDER BY 3 desc";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@i_startDate", dateFrom != null ? dateFrom : DBNull.Value);
                    command.Parameters.AddWithValue("@i_endDate", dateTo != null ? dateTo : DBNull.Value);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                TimeLogged = reader.GetDouble(reader.GetOrdinal("TotalTime"))
                            });
                        }
                    }
                }
            }

            return users; ;
        }

        public void CreateDatabase()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("CreateDbData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
