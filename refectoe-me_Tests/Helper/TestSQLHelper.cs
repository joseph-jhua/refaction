using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

using refactor_me.Infrastructure.Interface;

namespace refectoe_me.Tests.Helper
{
	internal class TestSQLHelper : ISQLHelper
	{
		private string GetConnectionString()
		{
			return string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0}\Database.mdf;Integrated Security=True", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
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
