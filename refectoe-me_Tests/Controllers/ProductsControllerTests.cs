using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using NUnit.Framework;

using refactor_me.Controllers;
using refactor_me.Domain;
using refactor_me.Services;
using refactor_me.Infrastructure.DataProvider;
using refectoe_me.Tests.Helper;

namespace refectoe_me.Tests.Controllers
{
	// This is actually not unit test now, more like functional test to make sure the refactor is working the same as legacy code
	// TODO: refactor to real UT
	[TestFixture]
	public class ProductsControllerTests
	{
		private ProductsController _productsController;

		private List<Product> _defaultProducts;
		private Product _validProduct;
		private List<ProductOption> _defaultProductOptions;
		private ProductOption _validProductOption;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_productsController = new ProductsController(new ProductService(new SQLRepository<Product>(new TestSQLHelper()), new SQLRepository<ProductOption>(new TestSQLHelper())));
			_defaultProducts = new List<Product>();
			_validProduct = new Product
			{
				Id = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"),
				Name = "Samsung Galaxy S7",
				Description = "Newest mobile product from Samsung.",
				Price = 1024.99m,
				DeliveryPrice = 16.99m
			};
			_defaultProducts.Add(_validProduct);
			_defaultProducts.Add
			(
				new Product
				{
					Id = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3"),
					Name = "Apple iPhone 6S",
					Description = "Newest mobile product from Apple.",
					Price = 1299.99m,
					DeliveryPrice = 15.99m
				}
			);

			_defaultProductOptions = new List<ProductOption>();
			_validProductOption = new ProductOption {
				Id = new Guid("0643ccf0-ab00-4862-b3c5-40e2731abcc9"),
				ProductId = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"),
				Name = "White",
				Description = "White Samsung Galaxy S7"
			};
			_defaultProductOptions.Add(_validProductOption);
			_defaultProductOptions.Add(new ProductOption
			{
				Id = new Guid("a21d5777-a655-4020-b431-624bb331e9a2"),
				ProductId = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"),
				Name = "Black",
				Description = "Black Samsung Galaxy S7"
			});
			_defaultProductOptions.Add(new ProductOption
			{
				Id = new Guid("5c2996ab-54ad-4999-92d2-89245682d534"),
				ProductId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3"),
				Name = "Rose Gold",
				Description = "Gold Apple iPhone 6S"
			});
			_defaultProductOptions.Add(new ProductOption
			{
				Id = new Guid("9ae6f477-a010-4ec9-b6a8-92a85d6c5f03"),
				ProductId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3"),
				Name = "White",
				Description = "White Apple iPhone 6S"
			});
			_defaultProductOptions.Add(new ProductOption
			{
				Id = new Guid("4e2bc5f2-699a-4c42-802e-ce4b4d2ac0ef"),
				ProductId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3"),
				Name = "Black",
				Description = "Black Apple iPhone 6S"
			});
		}

		[SetUp]
		public void SetUp()
		{
			var allProducts = _productsController.GetAll();
			foreach(var product in allProducts.Items)
			{
				_productsController.Delete(product.Id);
			}

			foreach(var product in _defaultProducts)
			{
				_productsController.Create(product);
			}
			foreach (var productOptions in _defaultProductOptions)
			{
				_productsController.CreateOption(productOptions.ProductId, productOptions);
			}
		}

		[TestCase]
		public void Should_get_all_products_successfully_when_call_GetAll()
		{
			var result = _productsController.GetAll();

			Assert.AreEqual(_defaultProducts.Count, result.Items.Count);

			foreach (var product in result.Items)
			{
				var expect = _defaultProducts.AsQueryable().FirstOrDefault(p => p.Id == product.Id);
				Assert.AreEqual(expect, product);
			}
		}

		[TestCase]
		public void Should_get_all_products_with_input_name_exists_successfully_when_call_SearchByName()
		{
			var result = _productsController.SearchByName("Samsung Galaxy S7");

			Assert.AreEqual(1, result.Items.Count);

			Assert.AreEqual(_validProduct, result.Items.ElementAt(0));
		}

		[TestCase]
		public void Should_get_no_product_with_input_name_not_exists_successfully_when_call_SearchByName()
		{
			var result = _productsController.SearchByName("Samsung Galaxy S6");

			Assert.AreEqual(0, result.Items.Count);
		}

		[TestCase]
		public void Should_get_product_with_input_id_exists_successfully_when_call_GetProduct()
		{
			var result = _productsController.GetProduct(_validProduct.Id);

			Assert.AreEqual(_validProduct, result);
		}

		[TestCase]
		public void Should_throw_exception_with_input_id_not_exists_when_call_GetProduct()
		{
			Assert.Throws<HttpResponseException>(() => _productsController.GetProduct(Guid.NewGuid()));
		}

		[TestCase]
		public void Should_create_product_when_is_new_successfully_when_call_Create()
		{
			var newProduct = new Product
			{
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				Price = 724.99m,
				DeliveryPrice = 16.99m
			};

			_productsController.Create(newProduct);

			var result = _productsController.GetProduct(newProduct.Id);

			Assert.AreEqual(newProduct, result);
		}

		[TestCase]
		public void Should_update_product_when_is_not_new_successfully_when_call_Create()
		{
			var oldProduct = _productsController.GetProduct(_validProduct.Id);
			oldProduct.Name = "test";

			_productsController.Create(oldProduct);

			var result = _productsController.GetProduct(_validProduct.Id);

			Assert.AreEqual(oldProduct, result);
		}

		[TestCase]
		public void Should_update_product_when_is_not_new_successfully_when_call_Update()
		{
			var oldProduct = _productsController.GetProduct(_validProduct.Id);
			var newProduct = new Product
			{
				Id = oldProduct.Id,
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				Price = 724.99m,
				DeliveryPrice = 16.99m
			};

			_productsController.Update(oldProduct.Id, newProduct);

			var result = _productsController.GetProduct(_validProduct.Id);

			Assert.AreEqual(newProduct, result);
		}

		[TestCase]
		public void Should_do_nothing_when_product_is_new_successfully_when_call_Update()
		{
			var newProduct = new Product
			{
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				Price = 724.99m,
				DeliveryPrice = 16.99m
			};

			_productsController.Update(newProduct.Id, newProduct);

			var result = _productsController.GetAll();

			Assert.AreEqual(_defaultProducts.Count, result.Items.Count);
		}

		[TestCase]
		public void Should_do_nothing_when_product_not_exists_successfully_when_call_Delete()
		{
			var newProduct = new Product
			{
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				Price = 724.99m,
				DeliveryPrice = 16.99m
			};

			_productsController.Delete(newProduct.Id);

			var result = _productsController.GetAll();

			Assert.AreEqual(_defaultProducts.Count, result.Items.Count);
		}

		[TestCase]
		public void Should_delete_product_and_all_its_options_when_product_exists_successfully_when_call_Delete()
		{
			_productsController.Delete(_validProduct.Id);

			var resultOptions = _productsController.GetOptions(_validProduct.Id);

			Assert.Throws<HttpResponseException>(() => _productsController.GetProduct(_validProduct.Id));
			Assert.AreEqual(0, resultOptions.Items.Count);
		}

		[TestCase]
		public void Should_get_all_product_options_when_product_exists_successfully_when_call_GetOptions()
		{
			var result = _productsController.GetOptions(_validProduct.Id);

			Assert.AreEqual(_defaultProductOptions.AsQueryable().Count(po => po.ProductId == _validProduct.Id), result.Items.Count);

			foreach (var productOptions in result.Items)
			{
				var expect = _defaultProductOptions.AsQueryable().FirstOrDefault(po => po.Id == productOptions.Id);
				Assert.AreEqual(expect, productOptions);
			}
		}

		[TestCase]
		public void Should_get_0_product_options_when_product_not_exists_successfully_when_call_GetOptions()
		{
			var result = _productsController.GetOptions(Guid.NewGuid());

			Assert.AreEqual(0, result.Items.Count);
		}

		[TestCase]
		public void Should_get_product_option_with_input_ids_exist_successfully_when_call_GetOption()
		{
			var result = _productsController.GetOption(_validProductOption.ProductId, _validProductOption.Id);

			Assert.AreEqual(_validProductOption, result);
		}

		[TestCase]
		public void Should_throw_exception_with_input_option_id_not_exists_when_call_GetOption()
		{
			Assert.Throws<HttpResponseException>(() => _productsController.GetOption(_validProductOption.ProductId, Guid.NewGuid()));
		}

		[TestCase]
		public void Should_throw_exception_with_input_product_id_not_exists_and_option_id_exists_when_call_GetOption()
		{
			var result = _productsController.GetOption(Guid.NewGuid(), _validProductOption.Id);

			Assert.AreEqual(_validProductOption, result);
		}

		[TestCase]
		public void Should_create_option_when_is_new_successfully_when_call_CreateOption()
		{
			var newProductOption = new ProductOption
			{
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				ProductId = Guid.NewGuid()
			};

			_productsController.CreateOption(newProductOption.ProductId, newProductOption);

			var result = _productsController.GetOption(newProductOption.ProductId, newProductOption.Id);

			Assert.AreEqual(newProductOption, result);
		}

		[TestCase]
		public void Should_update_option_when_is_not_new_successfully_when_call_CreateOption()
		{
			var oldProductOption = _productsController.GetOption(_validProductOption.ProductId, _validProductOption.Id);
			oldProductOption.Name = "test";

			_productsController.CreateOption(oldProductOption.ProductId, oldProductOption);

			var result = _productsController.GetOption(_validProductOption.ProductId, _validProductOption.Id);

			Assert.AreEqual(oldProductOption, result);
		}

		[TestCase]
		public void Should_update_option_when_is_not_new_successfully_when_call_UpdateOption()
		{
			var oldProductOption = _productsController.GetOption(_validProductOption.ProductId, _validProductOption.Id);
			var newProductOption = new ProductOption
			{
				Id = oldProductOption.Id,
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				ProductId = oldProductOption.ProductId
			};

			_productsController.UpdateOption(oldProductOption.ProductId, oldProductOption.Id, newProductOption);

			var result = _productsController.GetOption(_validProductOption.ProductId, _validProductOption.Id);

			Assert.AreEqual(newProductOption, result);
		}

		[TestCase]
		public void Should_do_nothing_when_option_is_new_successfully_when_call_Update()
		{
			var newProductOption = new ProductOption
			{
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				ProductId = _validProductOption.ProductId
			};

			_productsController.UpdateOption(_validProductOption.ProductId, newProductOption.Id, newProductOption);

			var result = _productsController.GetOptions(_validProductOption.ProductId);

			Assert.AreEqual(_defaultProductOptions.AsQueryable().Count(po => po.ProductId == _validProductOption.ProductId), result.Items.Count);
		}

		[TestCase]
		public void Should_do_nothing_when_option_not_exists_successfully_when_call_DeleteOption()
		{
			var newProductOption = new ProductOption
			{
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				ProductId = _validProductOption.ProductId
			};

			_productsController.DeleteOption(newProductOption.Id);

			var result = _productsController.GetOptions(_validProductOption.ProductId);

			Assert.AreEqual(_defaultProductOptions.AsQueryable().Count(po => po.ProductId == _validProductOption.ProductId), result.Items.Count);
		}

		[TestCase]
		public void Should_delete_option_when_option_exists_successfully_when_call_DeleteOption()
		{
			_productsController.DeleteOption(_validProductOption.Id);

			var resultOptions = _productsController.GetOptions(_validProductOption.ProductId);

			Assert.AreEqual(_defaultProductOptions.AsQueryable().Count(po => po.ProductId == _validProductOption.ProductId) - 1, resultOptions.Items.Count);
		}
	}
}
