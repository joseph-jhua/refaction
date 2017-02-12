using System;

namespace refactor_me.Services
{
	public interface IProductService
	{
		void CreateOption(Guid productId, refactor_me.Domain.ProductOption option);
		void CreateProduct(refactor_me.Domain.Product product);
		void DeleteOption(Guid id);
		void DeleteProduct(Guid id);
		System.Collections.Generic.List<refactor_me.Domain.ProductOption> GetAllOptions(Guid productId);
		System.Collections.Generic.List<refactor_me.Domain.Product> GetAllProducts();
		refactor_me.Domain.ProductOption GetOption(Guid id);
		refactor_me.Domain.Product GetProduct(Guid id);
		System.Collections.Generic.List<refactor_me.Domain.Product> SearchProductByName(string name);
		void UpdateOption(Guid productId, Guid id, refactor_me.Domain.ProductOption option);
		void UpdateProduct(Guid id, refactor_me.Domain.Product product);
	}
}
