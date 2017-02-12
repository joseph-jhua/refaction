using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using refactor_me.Infrastructure.Attributes;
using refactor_me.Infrastructure.DataModel;
using refactor_me.Infrastructure.Interface;

namespace refactor_me.Infrastructure.DataProvider
{
	/// <summary>
	/// Data provider using Sql
	/// </summary>
	/// <typeparam name="T">Data model which is Id based</typeparam>
	public class SQLRepository<T> : IRepository<T>
		where T : BaseDataModelWithId, new()
	{
		private ISQLHelper _sqlHelper;
		private string _tableName;
		private IDictionary<string, string> _columnInfo;

		private string _getSqlCommand = @"SELECT * FROM {0} WHERE Id = @Id";
		private string _saveSqlCommand = @"IF NOT EXISTS(SELECT 1 FROM {0} WHERE Id = @Id)" +
                @" INSERT INTO {0} ({1}) VALUES ({2})" +
                @" ELSE" +
                @" UPDATE {0} SET {3} WHERE Id = @Id";
		private string _updateSqlCommand = @" UPDATE {0} SET {1} WHERE Id = @Id";
		private string _deleteSqlCommand = @"DELETE FROM {0} WHERE Id = @Id";
		private string _getAllSqlCommand = @"SELECT * FROM {0} {1}";
		private string _deleteAllSqlCommand = @"DELETE FROM {0} {1}";
		private string _whereClause = @"WHERE {0}";

		private string _notSupprtTypeExceptionString = @"Property with type: {0} is not supported.";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlHelper">SQL Helper for execute sql commands</param>
		public SQLRepository(ISQLHelper sqlHelper)
		{
			this._sqlHelper = sqlHelper;

			ExtractTableColumnInfo();
		}

		private void ExtractTableColumnInfo()
		{
			var tableNameAttributes = typeof(T).GetCustomAttributes(typeof(TableNameAttribute), true);
			if (tableNameAttributes.Length == 0 || string.IsNullOrEmpty(((TableNameAttribute)tableNameAttributes[0]).TableName))
			{
				this._tableName = typeof(T).Name;
			}
			else
			{
				this._tableName = ((TableNameAttribute)tableNameAttributes[0]).TableName;
			}

			this._columnInfo = new Dictionary<string, string>();
			foreach (var property in typeof(T).GetProperties())
			{
				var columnNameAttributes = property.GetCustomAttributes(typeof(ColumnNameAttribute), true);
				if (columnNameAttributes.Length == 0 || string.IsNullOrEmpty(((ColumnNameAttribute)columnNameAttributes[0]).ColumnName))
				{
					this._columnInfo.Add(property.Name, property.Name);
				}
				else
				{
					this._columnInfo.Add(property.Name, ((ColumnNameAttribute)columnNameAttributes[0]).ColumnName);
				}
			}
		}

		/// <summary>
		/// Get entity using Id
		/// </summary>
		/// <param name="id">the unique Id to search for entity</param>
		/// <returns>entity corrspond to input Id</returns>
		public T Get(Guid id)
		{
			var cmdText = string.Format(this._getSqlCommand, this._tableName);
			var parameters = new Dictionary<string, object>()
			{
				{"@Id", id}
			};
			var dataTable = this._sqlHelper.ExcuteQuery(cmdText, parameters);

			if (dataTable.Rows == null || dataTable.Rows.Count == 0)
			{
				return null;
			}
			
			return Parse<T>(dataTable.Rows[0]);
		}

		private object GetValue<T1>(string valueStr)
		{
			if (typeof(T1) == typeof(String))
			{
				return valueStr;
			}

			if (typeof(T1) == typeof(Decimal))
			{
				return Decimal.Parse(valueStr);
			}

			if (typeof(T1) == typeof(Guid))
			{
				return Guid.Parse(valueStr);
			}

			throw new NotSupportedException(string.Format(_notSupprtTypeExceptionString, typeof(T).ToString()));
		}

		private T Parse<T>(DataRow dataRow)
			where T : BaseDataModelWithId, new()
		{
			var result = new T();

			foreach (var propertyMap in this._columnInfo)
			{
				var columnValue = (null == dataRow[propertyMap.Value] || DBNull.Value == dataRow[propertyMap.Value]) ? null : dataRow[propertyMap.Value].ToString();
				var columnPropertyInfo = result.GetType().GetProperty(propertyMap.Key);
				columnPropertyInfo.GetSetMethod().Invoke(result, new object[]
				{
					this.GetType().GetMethod("GetValue", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(columnPropertyInfo.PropertyType).Invoke(this, new object[] { columnValue })
				});
			}

			return result;
		}

		public void Save(T data, bool insertWhenNotExists)
		{
			var insertColumnList = new List<string>();
			var insertValueList = new List<string>();
			var updateList = new List<string>();
			var parameters = new Dictionary<string, object>();

			foreach (var property in this._columnInfo)
			{
				insertColumnList.Add(property.Value);
				insertValueList.Add(string.Format("@{0}", property.Value));
				updateList.Add(string.Format("{0}=@{0}", property.Value));
				parameters.Add(string.Format("@{0}", property.Value), typeof(T).GetProperty(property.Key).GetGetMethod().Invoke(data, null));
			}

			var sqlCommandText = insertWhenNotExists ?
				string.Format(this._saveSqlCommand,
					this._tableName,
					string.Join(",", insertColumnList.ToArray()),
					string.Join(",", insertValueList.ToArray()),
					string.Join(",", updateList.ToArray())) :
				string.Format(this._updateSqlCommand,
					this._tableName,
					string.Join(",", updateList.ToArray()));

			this._sqlHelper.ExcuteNonQueryInTransaction(sqlCommandText, parameters);
		}

		public void Delete(Guid id)
		{
			var cmdText = string.Format(this._deleteSqlCommand, this._tableName);
			var parameters = new Dictionary<string, object>()
			{
				{"@Id", id}
			};
			this._sqlHelper.ExcuteNonQuery(cmdText, parameters);
		}

		public List<T> GetAll()
		{
			return GetAll(null);
		}

		public List<T> GetAll(IDictionary<string, object> searchParameters)
		{
			List<string> conditionList;
			IDictionary<string, object> sqlParameters;
			ConvertToSqlParameters(searchParameters, out conditionList, out sqlParameters);

			var cmdText = string.Format(this._getAllSqlCommand, this._tableName,
				conditionList.Count > 0 ?
				string.Format(this._whereClause, string.Join(" AND ", conditionList)) :
				string.Empty);

			var dataTable = this._sqlHelper.ExcuteQuery(cmdText, sqlParameters);

			var result = new List<T>();

			if (dataTable.Rows == null || dataTable.Rows.Count == 0)
			{
				return result;
			}

			foreach (DataRow row in dataTable.Rows)
			{
				result.Add(Parse<T>(row));
			}

			return result;
		}

		public void DeleteAll(IDictionary<string, object> searchParameters)
		{
			List<string> conditionList;
			IDictionary<string, object> sqlParameters;
			ConvertToSqlParameters(searchParameters, out conditionList, out sqlParameters);

			var cmdText = string.Format(this._deleteAllSqlCommand, this._tableName,
				conditionList.Count > 0 ?
				string.Format(this._whereClause, string.Join(" AND ", conditionList)) :
				string.Empty);

			this._sqlHelper.ExcuteNonQuery(cmdText, sqlParameters);
		}

		private void ConvertToSqlParameters(IDictionary<string, object> searchParameters, out List<string> conditionList, out IDictionary<string, object> sqlParameters)
		{
			conditionList = new List<string>();
			sqlParameters = new Dictionary<string, object>();

			if (searchParameters == null || searchParameters.Count == 0)
			{
				return;
			}

			foreach (var searchParameter in searchParameters)
			{
				conditionList.Add(string.Format("{0} = @{0}", this._columnInfo[searchParameter.Key]));
				sqlParameters.Add(string.Format("@{0}", this._columnInfo[searchParameter.Key]), searchParameter.Value);
			}
		}
	}
}
