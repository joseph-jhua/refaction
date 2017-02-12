using System;

namespace refactor_me.Infrastructure.Attributes
{
	public class TableNameAttribute : Attribute
	{
		public string TableName
		{
			get;
			private set;
		}

		public TableNameAttribute(string tableName)
		{
			this.TableName = tableName;
		}
	}
}
