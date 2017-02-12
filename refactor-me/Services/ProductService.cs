using System;
using System.Collections.Generic;

using refactor_me.Domain;
using refactor_me.Infrastructure.Interface;

namespace refactor_me.Services
{
	public partial class ProductService : IProductService
	{
		private IRepository<Product> _productDataProvider;
		private IRepository<ProductOption> _productOptionDataProvider;

		public ProductService(IRepository<Product> productDataProvider, IRepository<ProductOption> productOptionDataProvider)
		{
			this._productDataProvider = productDataProvider;
			this._productOptionDataProvider = productOptionDataProvider;
		}

		public List<Product> GetAllProduct()
		{
			return this._productDataProvider.GetAll();
		}

		public List<Product> SearchProductByName(string name)
		{
			return this._productDataProvider.GetAll(new Dictionary<string, object>() { { typeof(Product).GetProperty("Name").Name, name } });
		}

		public Product GetProduct(Guid id)
		{
			return this._productDataProvider.Get(id);
		}

		public void CreateProduct(Product product)
		{
			this._productDataProvider.Save(product, true);
		}

		public void UpdateProduct(Guid id, Product product)
		{
			product.Id = id;

			this._productDataProvider.Save(product, false);
		}

		public void DeleteProduct(Guid id)
		{
			// QUESTION: shoudl this be a transactional operation? And How?
			this._productDataProvider.Delete(id);
			this._productOptionDataProvider.DeleteAll(new Dictionary<string, object>() { { typeof(ProductOption).GetProperty("ProductId").Name, id } });
		}
	}
}