using System.Collections.Generic;

namespace refactor_me.Domain
{
	public class Products
	{
		public List<Product> Items { get; private set; }

		public Products(List<Product> products)
		{
			this.Items = products;
		}
	}
}
