using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using refactor_me.Infrastructure.Attributes;
using refactor_me.Infrastructure.Interface;

namespace refactor_me.Infrastructure.DataModel
{
	public abstract class BaseDataModelWithId
	{
		[ColumnName("Id")]
		public Guid Id
		{
			get;
			set;
		}
	}
}
