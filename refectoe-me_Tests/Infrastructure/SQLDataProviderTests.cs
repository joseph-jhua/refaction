using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

using NUnit.Framework;

using refactor_me.Domain;
using refactor_me.Infrastructure.DataProvider;
using refactor_me.Infrastructure.Interface;
using refectoe_me.Tests.Helper;

namespace refectoe_me.Tests
{
	[TestFixture]
	public class SQLDataProviderTests
	{
		private SQLRepository<Product> _sqlDataProvider;

		private List<Product> _defaultProducts;
		private Product _validProduct;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_sqlDataProvider = new SQLRepository<Product>(new TestSQLHelper());
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
		}

		[SetUp]
		public void SetUp()
		{
			_sqlDataProvider.DeleteAll(null);
			foreach (var product in _defaultProducts)
			{
				_sqlDataProvider.Save(product, true);
			}
		}

		[TestCase]
		public void Should_return_data_if_exists_when_call_Get()
		{
			var result = _sqlDataProvider.Get(new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"));

			Assert.AreEqual(_validProduct, result);
		}

		[TestCase]
		public void Should_return_null_if_exists_when_call_Get()
		{
			var result = _sqlDataProvider.Get(new Guid("1f2e9176-35ee-4f0a-ae55-83023d2db1a3"));

			Assert.Null(result);
		}

		[TestCase]
		public void Should_update_data_if_exists_when_call_Save()
		{
			var oldProduct = new Product
			{
				Id = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"),
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				Price = 724.99m,
				DeliveryPrice = 12.99m
			};

			_sqlDataProvider.Save(oldProduct, true);

			var result = _sqlDataProvider.Get(new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"));

			Assert.AreEqual(oldProduct, result);
		}

		[TestCase]
		public void Should_insert_data_if_not_exists_when_set_true_for_insertWhenNotExists_when_call_Save()
		{
			var newProduct = new Product
			{
				Id = Guid.NewGuid(),
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				Price = 724.99m,
				DeliveryPrice = 12.99m
			};

			var nullResult = _sqlDataProvider.Get(newProduct.Id);

			_sqlDataProvider.Save(newProduct, true);

			var result = _sqlDataProvider.Get(newProduct.Id);

			Assert.Null(nullResult);
			Assert.AreEqual(newProduct, result);
		}

		[TestCase]
		public void Should_do_nothing_if_not_exists_when_set_false_for_insertWhenNotExists_when_call_Save()
		{
			var newProduct = new Product
			{
				Id = Guid.NewGuid(),
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				Price = 724.99m,
				DeliveryPrice = 12.99m
			};

			var nullResult = _sqlDataProvider.Get(newProduct.Id);

			_sqlDataProvider.Save(newProduct, false);

			var result = _sqlDataProvider.Get(newProduct.Id);

			Assert.Null(nullResult);
			Assert.Null(result);
		}

		[TestCase]
		public void Should_delete_data_when_call_Delete()
		{
			var resultBeforeDelete = _sqlDataProvider.Get(new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"));

			_sqlDataProvider.Delete(new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"));

			var resultAfterDelete = _sqlDataProvider.Get(new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"));

			Assert.NotNull(resultBeforeDelete);
			Assert.Null(resultAfterDelete);
		}

		[TestCase]
		public void Should_return_all_data_when_call_GetAll()
		{
			var result = _sqlDataProvider.GetAll();

			Assert.AreEqual(2, result.Count);
		}

		[TestCase]
		public void Should_return_all_data_match_search_condition_when_call_GetAll()
		{
			var anotherProduct = new Product
			{
				Id = Guid.NewGuid(),
				Name = "Samsung Galaxy S7",
				Description = "Broken mobile product from Samsung.",
				Price = 724.99m,
				DeliveryPrice = 12.99m
			};

			_sqlDataProvider.Save(anotherProduct, true);

			var result = _sqlDataProvider.GetAll(new Dictionary<string, object>() { { "Name", "Samsung Galaxy S7" } });

			Assert.AreEqual(2, result.Count);
		}

		[TestCase]
		public void Should_delete_all_data_match_search_condition_when_call_GetAll()
		{
			var anotherProduct = new Product
			{
				Id = Guid.NewGuid(),
				Name = "Samsung Galaxy S7",
				Description = "Broken mobile product from Samsung.",
				Price = 724.99m,
				DeliveryPrice = 12.99m
			};

			_sqlDataProvider.Save(anotherProduct, true);

			var resultBeforeDelete = _sqlDataProvider.GetAll(new Dictionary<string, object>() { { "Name", "Samsung Galaxy S7" } });
			_sqlDataProvider.DeleteAll(new Dictionary<string, object>() { { "Name", "Samsung Galaxy S7" } });
			var resultAfterDelete = _sqlDataProvider.GetAll(new Dictionary<string, object>() { { "Name", "Samsung Galaxy S7" } });

			Assert.AreEqual(2, resultBeforeDelete.Count);
			Assert.AreEqual(0, resultAfterDelete.Count);
		}
	}
}
