using Ninject.Modules;

using refactor_me.Domain;
using refactor_me.Infrastructure.DataProvider;
using refactor_me.Infrastructure.Interface;
using refactor_me.Services;

namespace refactor_me
{
	public class RefactorMeWebModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IProductService>().To<ProductService>();
			Bind<IRepository<Product>>().To<SQLRepository<Product>>();
			Bind<IRepository<ProductOption>>().To<SQLRepository<ProductOption>>();
			Bind<ISQLHelper>().To<WebServerLocalSQLHelper>();
		}
	}
}