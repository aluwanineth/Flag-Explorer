using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlagExplorer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IMediator Mediator { get; }

        protected BaseApiController(IMediator mediator)
        {
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
    }
}
