using System;

namespace refactor_me.Infrastructure.Attributes
{
	public class ColumnNameAttribute : Attribute
	{
		public string ColumnName
		{
			get;
			private set;
		}

		public ColumnNameAttribute(string columnName)
		{
			this.ColumnName = columnName;
		}
	}
}
