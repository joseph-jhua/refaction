using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

using refactor_me.Domain;

namespace refactor_me.Services
{
	public partial class ProductService : IProductService
	{
		public List<ProductOption> GetAllOptions(Guid productId)
		{
			return _productOptionDataProvider.GetAll(new Dictionary<string, object>() { { typeof(ProductOption).GetProperty("ProductId").Name, productId } });
		}

		public ProductOption GetOption(Guid id)
		{
			return _productOptionDataProvider.Get(id);
		}

		public void CreateOption(Guid productId, ProductOption option)
		{
			var product = _productDataProvider.Get(productId);

			if (product == null)
			{
				// log
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}

			option.ProductId = productId;
			_productOptionDataProvider.Save(option, true);
		}

		public void UpdateOption(Guid productId, Guid id, ProductOption option)
		{
			var product = _productDataProvider.Get(productId);

			if (product == null)
			{
				// log
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}

			option.Id = id;
			option.ProductId = productId;

			_productOptionDataProvider.Save(option, false);
		}

		public void DeleteOption(Guid id)
		{
			_productOptionDataProvider.Delete(id);
		}
	}
}