using Booking.Domain.Entities;
using BuildingBlocks.Application.Abstractions;

namespace Booking.Application.Abstractions;

public interface IOrderRepository : IRepository<Order, long> { }
