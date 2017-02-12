using System;
using System.Collections.Generic;

using refactor_me.Domain;
using refactor_me.Infrastructure.Interface;

namespace refactor_me.Services
{
	public partial class ProductService : IProductService
	{
		private readonly IRepository<Product> _productDataProvider;
		private readonly IRepository<ProductOption> _productOptionDataProvider;

		public ProductService(IRepository<Product> productDataProvider, IRepository<ProductOption> productOptionDataProvider)
		{
			_productDataProvider = productDataProvider;
			_productOptionDataProvider = productOptionDataProvider;
		}

		public List<Product> GetAllProducts()
		{
			return _productDataProvider.GetAll();
		}

		public List<Product> SearchProductByName(string name)
		{
			return _productDataProvider.GetAll(new Dictionary<string, object>() { { typeof(Product).GetProperty("Name").Name, name } });
		}

		public Product GetProduct(Guid id)
		{
			return _productDataProvider.Get(id);
		}

		public void CreateProduct(Product product)
		{
			_productDataProvider.Save(product, true);
		}

		public void UpdateProduct(Guid id, Product product)
		{
			product.Id = id;

			_productDataProvider.Save(product, false);
		}

		public void DeleteProduct(Guid id)
		{
			// QUESTION: shoudl this be a transactional operation? And How?
			_productDataProvider.Delete(id);
			_productOptionDataProvider.DeleteAll(new Dictionary<string, object>() { { typeof(ProductOption).GetProperty("ProductId").Name, id } });
		}
	}
}