using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using refactor_me.Domain;
using refactor_me.Infrastructure.Interface;
using refactor_me.Services;

namespace refectoe_me.Tests.Service
{
	[TestFixture]
	public class ProductServiceTests
	{
		private Mock<IRepository<Product>> _productDataProviderMock;
		private Mock<IRepository<ProductOption>> _productOptionDataProvider;
		private ProductService _productService;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_productDataProviderMock = new Mock<IRepository<Product>>();
			_productOptionDataProvider = new Mock<IRepository<ProductOption>>();
			_productService = new ProductService(_productDataProviderMock.Object, _productOptionDataProvider.Object);
		}

		[SetUp]
		public void SetUp()
		{
			_productDataProviderMock.Reset();
			_productOptionDataProvider.Reset();
		}

		[Test]
		public void Should_call_through_successfully_when_call_GetAllProducts()
		{
			_productService.GetAllProducts();

			_productDataProviderMock.Verify(p => p.GetAll(), Times.Once);
		}

		[Test]
		public void Should_call_through_successfully_when_call_SearchProductByName()
		{
			_productService.SearchProductByName("test");

			_productDataProviderMock.Verify(p => p.GetAll(It.Is<IDictionary<string, object>>(ps => ps.Count == 1 && (string)ps["Name"] == "test")), Times.Once);
		}

		[Test]
		public void Should_call_through_successfully_when_call_GetProduct()
		{
			var id = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");

			_productService.GetProduct(id);

			_productDataProviderMock.Verify(p => p.Get(It.Is<Guid>(guid => guid == id)), Times.Once);
		}

		[Test]
		public void Should_call_through_successfully_when_call_CreateProduct()
		{
			var product = new Product();

			_productService.CreateProduct(product);

			_productDataProviderMock.Verify(p => p.Save(It.Is<Product>(prod => prod.Id == product.Id), It.Is<bool>(b => b)), Times.Once);
		}

		[Test]
		public void Should_call_through_successfully_when_call_UpdateProduct()
		{
			var id = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");
			var product = new Product();

			_productService.UpdateProduct(id, product);

			_productDataProviderMock.Verify(p => p.Save(It.Is<Product>(prod => prod.Id == id), It.Is<bool>(b => !b)), Times.Once);
		}

		[Test]
		public void Should_call_through_successfully_when_call_DeleteProduct()
		{
			var id = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");

			_productService.DeleteProduct(id);

			_productDataProviderMock.Verify(p => p.Delete(It.Is<Guid>(guid => guid == id)), Times.Once);
			_productOptionDataProvider.Verify(po => po.DeleteAll(It.Is<IDictionary<string, object>>(ps => ps.Count == 1 && (Guid)ps["ProductId"] == id)), Times.Once);
		}

		[Test]
		public void Should_call_through_successfully_when_call_GetAllOptions()
		{
			var id = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");

			_productService.GetAllOptions(id);

			_productOptionDataProvider.Verify(po => po.GetAll(It.Is<IDictionary<string, object>>(ps => ps.Count == 1 && (Guid)ps["ProductId"] == id)), Times.Once);
		}

		[Test]
		public void Should_call_through_successfully_when_call_GetOption()
		{
			var id = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");

			_productService.GetOption(id);

			_productOptionDataProvider.Verify(p => p.Get(It.Is<Guid>(guid => guid == id)), Times.Once);
		}

		[Test]
		public void Should_call_through_successfully_when_call_CreateOption()
		{
			var id = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");
			var productOption = new ProductOption();

			_productService.CreateOption(id, productOption);

			_productOptionDataProvider.Verify(p => p.Save(It.Is<ProductOption>(po => po.ProductId == id), It.Is<bool>(b => b)), Times.Once);
		}

		[Test]
		public void Should_call_through_successfully_when_call_UpdateOption()
		{
			var productId = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");
			var id = new Guid("12349176-35ee-4f0a-ae55-83023d2db1a3");
			var productOption = new ProductOption();

			_productService.UpdateOption(productId, id, productOption);

			_productOptionDataProvider.Verify(p => p.Save(It.Is<ProductOption>(po => po.Id == id && po.ProductId == productId), It.Is<bool>(b => !b)), Times.Once);
		}

		[Test]
		public void Should_call_through_successfully_when_call_DeleteOption()
		{
			var id = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");

			_productService.DeleteOption(id);

			_productOptionDataProvider.Verify(p => p.Delete(It.Is<Guid>(guid => guid == id)), Times.Once);
		}
	}
}
