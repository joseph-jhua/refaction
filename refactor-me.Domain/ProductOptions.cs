using System.Collections.Generic;

namespace refactor_me.Domain
{
	public class ProductOptions
	{
		public List<ProductOption> Items { get; private set; }

		public ProductOptions(List<ProductOption> productOptions)
		{
			this.Items = productOptions;
		}
	}
}
