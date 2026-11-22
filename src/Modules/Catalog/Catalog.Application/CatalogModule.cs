using Catalog.Contracts;

namespace Catalog.Application;

/// <summary>
/// Represents the Catalog module in the application.
/// </summary>
public sealed class CatalogModule : ICatalogModule
{
    public static string ModuleName => ICatalogModule.ModuleName;
}
