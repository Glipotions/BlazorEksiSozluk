using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorSozluk.WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        /// <ÖZET>
        /// Her controllerda mediator ü üretmemek adına bu yapıldı eğer boşsa içini servisten çekip dolduruyor.
        /// </summary>
        //protected IMediator? Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        //private IMediator? _mediator;


        public Guid? UserId
        {
            get
            {
                var val = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return val is null ? null : new Guid(val);
            }
        }

    }
}
