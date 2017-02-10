using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web;

namespace refactor_me.Models
{
    public class Helpers
    {
        private const string ConnectionString = @"Data Source=(LocalDB)\{DBName};AttachDbFilename={DataDirectory}\Database.mdf;Integrated Security=True";

        public static SqlConnection NewConnection()
        {
			var connstr = HttpContext.Current != null ? 
				ConnectionString.Replace("{DataDirectory}", HttpContext.Current.Server.MapPath("~/App_Data")).Replace("{DBName}", "MSSQLLocalDB") :
				ConnectionString.Replace("{DataDirectory}", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).Replace("{DBName}", "MSSQLLocalDB");
            return new SqlConnection(connstr);
        }
    }
}