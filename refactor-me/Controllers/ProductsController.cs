using System;
using System.Net;
using System.Web.Http;

using refactor_me.Domain;
using refactor_me.Services;

namespace refactor_me.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
		private IProductService _productService;

		public ProductsController(IProductService productService)
		{
			this._productService = productService;
		}

        [Route]
        [HttpGet]
        public Products GetAll()
        {
            return new Products(this._productService.GetAllProduct());
        }

        [Route]
        [HttpGet]
        public Products SearchByName(string name)
        {
			return new Products(this._productService.SearchProductByName(name));
        }

        [Route("{id}")]
        [HttpGet]
        public Product GetProduct(Guid id)
        {
            var product = this._productService.GetProduct(id);
            if (null == product)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return product;
        }

        [Route]
        [HttpPost]
        public void Create([FromBody]Product product)
        {
			this._productService.CreateProduct(product);
        }

        [Route("{id}")]
        [HttpPut]
        public void Update(Guid id, Product product)
		{
			this._productService.UpdateProduct(id, product);
        }

        [Route("{id}")]
        [HttpDelete]
        public void Delete(Guid id)
        {
			this._productService.DeleteProduct(id);
        }

        [Route("{productId}/options")]
        [HttpGet]
        public ProductOptions GetOptions(Guid productId)
		{
			return new ProductOptions(this._productService.GetAllOptions(productId));
        }

        [Route("{productId}/options/{id}")]
        [HttpGet]
        public ProductOption GetOption(Guid productId, Guid id)
        {
			// QUESTION: should also check product exists?
            var option = this._productService.GetOption(id);
            if (null == option)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return option;
        }

        [Route("{productId}/options")]
        [HttpPost]
        public void CreateOption(Guid productId, ProductOption option)
        {
			this._productService.CreateOption(productId, option);
        }

        [Route("{productId}/options/{id}")]
        [HttpPut]
        public void UpdateOption(Guid productId, Guid id, ProductOption option)
        {
			this._productService.UpdateOption(productId, id, option);
        }

        [Route("{productId}/options/{id}")]
        [HttpDelete]
        public void DeleteOption(Guid id)
		{
			this._productService.DeleteOption(id);
        }
    }
}
