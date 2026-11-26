using BuildingBlocks.Contracts;
using BuildingBlocks.Domain.Results;
using Catalog.Contracts.Requests;
using Catalog.Contracts.Responses;

namespace Catalog.Contracts;

public interface ICatalogModule : IModule
{
    const string Name = "catalog";

    static string IModule.ModuleName => Name;

    Task<Result<EventPricingResponse>> GetEventPricingAsync(
        long eventId,
        IReadOnlyCollection<SeatPriceRequest>? seatPriceRequests,
        IReadOnlyCollection<GeneralAdmissionPriceRequest>? generalAdmissionPriceRequests,
        CancellationToken cancellationToken = default
    );
}
