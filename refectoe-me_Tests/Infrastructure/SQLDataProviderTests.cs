using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

using Moq;
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
		private Mock<ISQLHelper> _sqlHelperMock;

		private List<Product> _defaultProducts;
		private Product _validProduct;

		private readonly string _getSqlCommand = @"SELECT * FROM product WHERE Id = @Id";
		private readonly string _saveSqlCommand = @"IF NOT EXISTS(SELECT 1 FROM product WHERE Id = @Id)" +
			@" INSERT INTO product (Name,Description,Price,DeliveryPrice,Id) VALUES (@Name,@Description,@Price,@DeliveryPrice,@Id)" +
			@" ELSE" +
			@" UPDATE product SET Name=@Name,Description=@Description,Price=@Price,DeliveryPrice=@DeliveryPrice,Id=@Id WHERE Id = @Id";
		private readonly string _updateSqlCommand = @"UPDATE product SET Name=@Name,Description=@Description,Price=@Price,DeliveryPrice=@DeliveryPrice,Id=@Id WHERE Id = @Id";
		private readonly string _deleteSqlCommand = @"DELETE FROM product WHERE Id = @Id";
		private readonly string _getAllSqlCommand = @"SELECT * FROM product ";
		private readonly string _getAllWithWhereClauseSqlCommand = @"SELECT * FROM product WHERE Name = @Name";
		private readonly string _deleteAllWithWhereClauseSqlCommand = @"DELETE FROM product WHERE Name = @Name";

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_sqlHelperMock = new Mock<ISQLHelper>();
			_sqlDataProvider = new SQLRepository<Product>(_sqlHelperMock.Object);
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

			_sqlHelperMock.Reset();
		}

		[TestCase]
		public void Should_return_data_if_exists_when_call_Get()
		{
			var id = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");

			_sqlHelperMock.Setup(s => s.ExcuteQuery(It.IsAny<string>(), It.Is<IDictionary<string, object>>(ps => ps.Count == 1 && (Guid)ps["@Id"] == id)))
				.Returns(GenerateDataTable(new List<Product>() { _validProduct }));

			var result = _sqlDataProvider.Get(id);

			Assert.AreEqual(_validProduct, result);
			_sqlHelperMock.Verify(s => s.ExcuteQuery(
				It.Is<string>(txt => txt == _getSqlCommand),
				It.Is<IDictionary<string, object>>(ps => ps.Count == 1 && (Guid)ps["@Id"] == id)),
				Times.Once());
		}

		[TestCase]
		public void Should_return_null_if_exists_when_call_Get()
		{
			var validId = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");
			var invalidId = new Guid("1f2e9176-35ee-4f0a-ae55-83023d2db1a3");

			_sqlHelperMock.Setup(s => s.ExcuteQuery(It.IsAny<string>(), It.Is<IDictionary<string, object>>(ps => ps.Count == 1 && (Guid)ps["@Id"] == validId)))
				.Returns(GenerateDataTable(new List<Product>() { _validProduct }));

			var result = _sqlDataProvider.Get(invalidId);

			Assert.Null(result);
			_sqlHelperMock.Verify(s => s.ExcuteQuery(
				It.Is<string>(txt => txt == _getSqlCommand),
				It.Is<IDictionary<string, object>>(ps => ps.Count == 1 && (Guid)ps["@Id"] == invalidId)),
				Times.Once());
		}

		[TestCase]
		public void Should_update_data_if_exists_when_call_Save()
		{
			var validId = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");

			var oldProduct = new Product
			{
				Id = validId,
				Name = "Samsung Galaxy S6",
				Description = "Old mobile product from Samsung.",
				Price = 724.99m,
				DeliveryPrice = 12.99m
			};

			_sqlHelperMock.Setup(s => s.ExcuteNonQueryInTransaction(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()));

			_sqlDataProvider.Save(oldProduct, true);

			_sqlHelperMock.Verify(s => s.ExcuteNonQueryInTransaction(
				It.Is<string>(txt => txt == _saveSqlCommand),
				It.Is<IDictionary<string, object>>(ps => ps.Count == 5 &&
					(Guid)ps["@Id"] == validId &&
					(string)ps["@Name"] == oldProduct.Name &&
					(string)ps["@Description"] == oldProduct.Description &&
					(decimal)ps["@Price"] == oldProduct.Price &&
					(decimal)ps["@DeliveryPrice"] == oldProduct.DeliveryPrice)
				), Times.Once);
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

			_sqlHelperMock.Setup(s => s.ExcuteNonQueryInTransaction(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()));

			_sqlDataProvider.Save(newProduct, true);

			_sqlHelperMock.Verify(s => s.ExcuteNonQueryInTransaction(
				It.Is<string>(txt => txt == _saveSqlCommand),
				It.Is<IDictionary<string, object>>(ps => ps.Count == 5 &&
					(Guid)ps["@Id"] == newProduct.Id &&
					(string)ps["@Name"] == newProduct.Name &&
					(string)ps["@Description"] == newProduct.Description &&
					(decimal)ps["@Price"] == newProduct.Price &&
					(decimal)ps["@DeliveryPrice"] == newProduct.DeliveryPrice)
				), Times.Once);
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
			_sqlHelperMock.Setup(s => s.ExcuteNonQueryInTransaction(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()));
			
			_sqlDataProvider.Save(newProduct, false);

			_sqlHelperMock.Verify(s => s.ExcuteNonQueryInTransaction(
				It.Is<string>(txt => txt == _updateSqlCommand),
				It.Is<IDictionary<string, object>>(ps => ps.Count == 5 &&
					(Guid)ps["@Id"] == newProduct.Id &&
					(string)ps["@Name"] == newProduct.Name &&
					(string)ps["@Description"] == newProduct.Description &&
					(decimal)ps["@Price"] == newProduct.Price &&
					(decimal)ps["@DeliveryPrice"] == newProduct.DeliveryPrice)
				), Times.Once);
		}

		[TestCase]
		public void Should_delete_data_when_call_Delete()
		{
			var validId = new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3");

			_sqlHelperMock.Setup(s => s.ExcuteNonQuery(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()));

			_sqlDataProvider.Delete(validId);

			_sqlHelperMock.Verify(s => s.ExcuteNonQuery(
				It.Is<string>(txt => txt == _deleteSqlCommand),
				It.Is<IDictionary<string, object>>(ps => ps.Count == 1 && (Guid)ps["@Id"] == validId)),
				Times.Once());
		}

		[TestCase]
		public void Should_return_all_data_when_call_GetAll()
		{
			_sqlHelperMock.Setup(s => s.ExcuteQuery(It.IsAny<string>(), It.Is<IDictionary<string, object>>(ps => ps.Count == 0)))
				.Returns(GenerateDataTable(_defaultProducts));
			var result = _sqlDataProvider.GetAll();

			Assert.AreEqual(2, result.Count);
			_sqlHelperMock.Verify(s => s.ExcuteQuery(
				It.Is<string>(txt => txt == _getAllSqlCommand),
				It.Is<IDictionary<string, object>>(ps => ps.Count == 0)),
				Times.Once());
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
			_sqlHelperMock.Setup(s => s.ExcuteQuery(It.IsAny<string>(), It.Is<IDictionary<string, object>>(ps => ps.Count == 1 && (string)ps["@Name"] == _validProduct.Name)))
				.Returns(GenerateDataTable(new List<Product>() { _validProduct, anotherProduct }));

			var result = _sqlDataProvider.GetAll(new Dictionary<string, object>() { { "Name", "Samsung Galaxy S7" } });

			Assert.AreEqual(2, result.Count);

			_sqlHelperMock.Verify(s => s.ExcuteQuery(
				It.Is<string>(txt => txt == _getAllWithWhereClauseSqlCommand),
				It.Is<IDictionary<string, object>>(ps => ps.Count == 1 && (string)ps["@Name"] == _validProduct.Name)),
				Times.Once());
		}

		[TestCase]
		public void Should_delete_all_data_match_search_condition_when_call_DeleteAll()
		{
			_sqlHelperMock.Setup(s => s.ExcuteNonQuery(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()));

			_sqlDataProvider.DeleteAll(new Dictionary<string, object>() { { "Name", "Samsung Galaxy S7" } });

			_sqlHelperMock.Verify(s => s.ExcuteNonQuery(
				It.Is<string>(txt => txt == _deleteAllWithWhereClauseSqlCommand),
				It.Is<IDictionary<string, object>>(ps => ps.Count == 1 && (string)ps["@Name"] == _validProduct.Name)),
				Times.Once());
		}

		private DataTable GenerateDataTable(List<Product> products)
		{
			var result = new DataTable("product");

			result.Columns.Add(new DataColumn("Id", System.Type.GetType("System.Guid")));
			result.Columns.Add(new DataColumn("Name", System.Type.GetType("System.String")));
			result.Columns.Add(new DataColumn("Description", System.Type.GetType("System.String")));
			result.Columns.Add(new DataColumn("Price", System.Type.GetType("System.Decimal")));
			result.Columns.Add(new DataColumn("DeliveryPrice", System.Type.GetType("System.Decimal")));

			if (products == null)
			{
				return result;
			}

			foreach (var product in products)
			{
				var newRow = result.NewRow();
				newRow["Id"] = product.Id;
				newRow["Name"] = product.Name;
				newRow["Description"] = product.Description;
				newRow["Price"] = product.Price;
				newRow["DeliveryPrice"] = product.DeliveryPrice;
				result.Rows.Add(newRow);
			}

			return result;
		}
	}
}
