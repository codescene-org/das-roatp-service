using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Services
{
    public class DatabaseService
    {
        public DatabaseService()
        {

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("connectionStrings.Local.json")
                .Build();
            WebConfiguration = new TestWebConfiguration
            {
                SqlConnectionString = Configuration.GetConnectionString("SqlConnectionStringTest")
            };
        }

        public IConfiguration Configuration { get; }
        public TestWebConfiguration WebConfiguration;

        public void SetupDatabase()
        {
            DropDatabase();

            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionString")))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        $@"DBCC CLONEDATABASE ('SFA.DAS.RoatpService.Database', 'SFA.DAS.RoatpService.Database.Test'); ALTER DATABASE [SFA.DAS.RoatpService.Database.Test] SET READ_WRITE;"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }

        public void Execute(string sql)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql);
                connection.Close();
            }
        }

        public T Get<T>(string sql)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var result = connection.Query<T>(sql);
                connection.Close();
                return result.FirstOrDefault();
            }
        }

        public object ExecuteScalar(string sql)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var result = connection.ExecuteScalar(sql);
                connection.Close();

                return result;
            }
        }
        
        public void Execute(string sql, TestModel model)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql, model);
                connection.Close();
            }
        }

        public void DropDatabase()
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionString")))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        $@"if exists(select * from sys.databases where name = 'SFA.DAS.RoatpService.Database.Test') BEGIN ALTER DATABASE [SFA.DAS.RoatpService.Database.Test] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;  DROP DATABASE [SFA.DAS.RoatpService.Database.Test]; END"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }
    }
}
