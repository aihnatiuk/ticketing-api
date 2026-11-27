using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Results;
using BuildingBlocks.Presentation.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Module.Features.Payments.CompletePayment;
using Payment.Module.Features.Payments.FailPayment;

namespace Payment.Module.Features.Payments;

[Route("[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    [HttpPost("{paymentId:long}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Complete(
        [FromRoute] long paymentId,
        [FromServices] ICommandHandler<CompletePaymentCommand> handler,
        CancellationToken cancellationToken
    )
    {
        CompletePaymentCommand command = new(paymentId);
        Result result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess ? NoContent() : this.ToProblem(result.Error);
    }

    [HttpPost("{paymentId:long}/failed")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Fail(
        [FromRoute] long paymentId,
        [FromServices] ICommandHandler<FailPaymentCommand> handler,
        CancellationToken cancellationToken
    )
    {
        FailPaymentCommand command = new(paymentId);
        Result result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess ? NoContent() : this.ToProblem(result.Error);
    }
}
