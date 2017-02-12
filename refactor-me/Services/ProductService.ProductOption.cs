using refactor_me.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace refactor_me.Services
{
	public partial class ProductService : IProductService
	{
		public List<ProductOption> GetAllOptions(Guid productId)
		{
			return this._productOptionDataProvider.GetAll(new Dictionary<string, object>() { { typeof(ProductOption).GetProperty("ProductId").Name, productId } });
		}

		public ProductOption GetOption(Guid id)
		{
			return this._productOptionDataProvider.Get(id);
		}

		public void CreateOption(Guid productId, ProductOption option)
		{
			option.ProductId = productId;
			this._productOptionDataProvider.Save(option, true);
		}

		public void UpdateOption(Guid productId, Guid id, ProductOption option)
		{
			option.Id = id;
			option.ProductId = productId;

			this._productOptionDataProvider.Save(option, false);
		}

		public void DeleteOption(Guid id)
		{
			this._productOptionDataProvider.Delete(id);
		}
	}
}