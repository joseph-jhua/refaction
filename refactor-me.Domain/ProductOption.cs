using System;

using refactor_me.Infrastructure.Attributes;
using refactor_me.Infrastructure.DataModel;

namespace refactor_me.Domain
{
	[TableName("ProductOption")]
	public class ProductOption : BaseDataModelWithId
	{
		[ColumnName("ProductId")]
        public Guid ProductId { get; set; }

		[ColumnName("Name")]
        public string Name { get; set; }

		[ColumnName("Description")]
		public string Description { get; set; }

		public ProductOption()
		{
			this.Id = Guid.NewGuid();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || typeof(ProductOption) != obj.GetType())
			{
				return false;
			}

			var productOption = obj as ProductOption;
			return productOption.Id == Id
				&& productOption.Name == Name
				&& productOption.Description == Description
				&& productOption.ProductId == ProductId;
		}
	}
}
