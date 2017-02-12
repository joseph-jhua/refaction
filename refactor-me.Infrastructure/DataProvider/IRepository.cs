using System;
using System.Collections.Generic;

using refactor_me.Infrastructure.DataModel;

namespace refactor_me.Infrastructure.Interface
{
	public interface IRepository<T> where T : BaseDataModelWithId
	{
		T Get(Guid id);
		void Save(T data, bool insertWhenNotExists);
		void Delete(Guid id);
		List<T> GetAll();
		List<T> GetAll(IDictionary<string, object> searchParameters);
		void DeleteAll(IDictionary<string, object> searchParameters);
	}
}
