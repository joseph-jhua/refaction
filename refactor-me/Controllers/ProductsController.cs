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
		private readonly IProductService _productService;

		public ProductsController(IProductService productService)
		{
			_productService = productService;
		}

        [Route]
        [HttpGet]
        public Products GetAll()
        {
            return new Products(_productService.GetAllProducts());
        }

        [Route]
        [HttpGet]
        public Products SearchByName(string name)
        {
			return new Products(_productService.SearchProductByName(name));
        }

        [Route("{id}")]
        [HttpGet]
        public Product GetProduct(Guid id)
        {
            var product = _productService.GetProduct(id);
            if (null == product)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return product;
        }

        [Route]
        [HttpPost]
        public void Create([FromBody]Product product)
        {
			_productService.CreateProduct(product);
        }

        [Route("{id}")]
        [HttpPut]
        public void Update(Guid id, Product product)
		{
			_productService.UpdateProduct(id, product);
        }

        [Route("{id}")]
        [HttpDelete]
        public void Delete(Guid id)
        {
			_productService.DeleteProduct(id);
        }

        [Route("{productId}/options")]
        [HttpGet]
        public ProductOptions GetOptions(Guid productId)
		{
			return new ProductOptions(_productService.GetAllOptions(productId));
        }

        [Route("{productId}/options/{id}")]
        [HttpGet]
        public ProductOption GetOption(Guid productId, Guid id)
        {
			// QUESTION: should also check product exists?
            var option = _productService.GetOption(id);
            if (null == option)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return option;
        }

        [Route("{productId}/options")]
        [HttpPost]
        public void CreateOption(Guid productId, ProductOption option)
        {
			_productService.CreateOption(productId, option);
        }

        [Route("{productId}/options/{id}")]
        [HttpPut]
        public void UpdateOption(Guid productId, Guid id, ProductOption option)
        {
			_productService.UpdateOption(productId, id, option);
        }

        [Route("{productId}/options/{id}")]
        [HttpDelete]
        public void DeleteOption(Guid id)
		{
			_productService.DeleteOption(id);
        }
    }
}
