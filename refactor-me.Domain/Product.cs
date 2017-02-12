using System;

using refactor_me.Infrastructure.Attributes;
using refactor_me.Infrastructure.DataModel;

namespace refactor_me.Domain
{
	[TableName("product")]
	public class Product : BaseDataModelWithId
	{
		[ColumnName("Name")]
		public string Name { get; set; }

		[ColumnName("Description")]
		public string Description { get; set; }

		[ColumnName("Price")]
		public decimal Price { get; set; }

		[ColumnName("DeliveryPrice")]
		public decimal DeliveryPrice { get; set; }

		public Product()
		{
			this.Id = Guid.NewGuid();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || typeof(Product) != obj.GetType())
			{
				return false;
			}

			var product = obj as Product;
			return product.Id == Id
				&& product.Name == Name
				&& product.Description == Description
				&& product.Price == Price
				&& product.DeliveryPrice == DeliveryPrice;
		}
    }
}
