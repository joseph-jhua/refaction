using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

using refactor_me.Infrastructure.Interface;

namespace refactor_me.Infrastructure.DataProvider
{
	/// <summary>
	/// Local sql in web server
	/// </summary>
	public class WebServerLocalSQLHelper : ISQLHelper
	{
		private const string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0}\Database.mdf;Integrated Security=True";

		private string GetConnectionString()
		{
			return string.Format(_connectionString, HttpContext.Current.Server.MapPath("~/App_Data"));
		}

		/// <summary>
		/// Execute none query
		/// </summary>
		/// <param name="sqlCommandText">sql command text</param>
		/// <param name="parameters">parameters to build sql command</param>
		public void ExcuteNonQuery(string sqlCommandText, IDictionary<string, object> parameters)
		{
			using (var conn = new SqlConnection(GetConnectionString()))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = sqlCommandText;
					foreach (var parameter in parameters)
					{
						cmd.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
					}

					cmd.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// Execute none query in a transaction scope
		/// </summary>
		/// <param name="sqlCommandText">sql command text</param>
		/// <param name="parameters">parameters to build sql command</param>
		public void ExcuteNonQueryInTransaction(string sqlCommandText, IDictionary<string, object> parameters)
		{
			// Here is using just one sql command as we just need it like that.
			// TODO: change parameter to List<string, Dictionary<string, object>> to support multiple command
			// TODO2: may consider to change this sql access layer completely to map all SqlClient things. So we can support transaction in a more complex scenario, like mixture of query and none query.
			using (var conn = new SqlConnection(GetConnectionString()))
			{
				conn.Open();
				using (var tran = conn.BeginTransaction())
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = sqlCommandText;
						cmd.Transaction = tran;
						foreach (var parameter in parameters)
						{
							cmd.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
						}

						try
						{
							cmd.ExecuteNonQuery();
							tran.Commit();
						}
						catch
						{
							// TODO: log exception

							tran.Rollback();
						}
					}
				}
			}
		}

		/// <summary>
		/// Execute query
		/// </summary>
		/// <param name="sqlCommandText">sql command text</param>
		/// <param name="parameters">parameters to build sql command</param>
		public DataTable ExcuteQuery(string sqlCommandText, IDictionary<string, object> parameters)
		{
			using (var conn = new SqlConnection(GetConnectionString()))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = sqlCommandText;
					foreach (var parameter in parameters)
					{
						cmd.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
					}

					using (var dataReader = cmd.ExecuteReader())
					{
						var dataTable = new DataTable();
						dataTable.Load(dataReader);

						return dataTable;
					}
				}
			}
		}
	}
}
