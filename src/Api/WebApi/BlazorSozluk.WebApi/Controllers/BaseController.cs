using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlazorSozluk.WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        /// <ÖZET>
        /// Her controllerda mediator ü üretmemek adına bu yapıldı eğer boşsa içini servisten çekip dolduruyor.
        /// </summary>
        protected IMediator? Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        private IMediator? _mediator;

    }
}
