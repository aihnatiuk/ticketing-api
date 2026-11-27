namespace BuildingBlocks.Domain.Exceptions;

public class DomainInvariantException(string message) : Exception(message) { }
