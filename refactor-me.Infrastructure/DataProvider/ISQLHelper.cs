using System.Collections.Generic;
using System.Data;

namespace refactor_me.Infrastructure.Interface
{
	public interface ISQLHelper
	{
		void ExcuteNonQuery(string sqlCommandText, IDictionary<string, object> parameters);
		void ExcuteNonQueryInTransaction(string sqlCommandText, IDictionary<string, object> parameters);
		DataTable ExcuteQuery(string sqlCommandText, IDictionary<string, object> parameters);
	}
}
