using System.Data.SqlClient;
using System.Web;
using System.Web.Http;


namespace Sample_api_users.Controllers
{
    public class DataEFController : ApiController
    {

        [HttpGet]
        public IHttpActionResult Index()
        {

            string ConnectionString1 = @"Server=seed-srv.database.windows.net; Database=seeddb";
            
            
            //string ConnectionString1 = @"Server=seed-srv.database.windows.net; Authentication=Active Directory Managed Identity; Database=seeddb;";
            //string ConnectionString1 = @"Server=seed-srv.database.windows.net; Authentication=Active Directory Managed Identity; Database=seeddb; User Id=seed-api-windows2";
            //string ConnectionString1 = @"Server=seed-srv.database.windows.net; Authentication=Active Directory Managed Identity; Database=seeddb; User Id=8c806bcb-d998-4476-983c-9e99433d4296";
            //string ConnectionString1 = @"Server=seed-srv.database.windows.net; Authentication=Active Directory Managed Identity; Database=seeddb; User Id=779d44ae-dba6-46d1-8a19-729dc286d337";
            //string ConnectionString1 = @"Server=seed-srv.database.windows.net; Authentication=Active Directory Managed Identity; Database=seeddb;";
            //string ConnectionString1 = @"Data Source=seed-srv.database.windows.net;Initial Catalog=seeddb;user id=seed-app;password=p@$$w0rd;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;MultipleActiveResultSets=True";


            using (SqlConnection conn = new SqlConnection(ConnectionString1))
            {
                var tokenProv = new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider();
                conn.AccessToken = tokenProv.GetAccessTokenAsync("https://database.windows.net/").Result;
            }


        }

    }
}
