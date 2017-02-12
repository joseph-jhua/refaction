using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace refactor_me.Infrastructure
{
	public class ExceptionFilter : ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext context)
		{
			// There are exceptions besides HttpResponseException need to be treated
			if (context.Exception != null)
			{
				// Log Error
				throw new HttpResponseException(context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, context.Exception));
			}
		}
	}
}