using BuildingBlocks.Contracts;

namespace Catalog.Contracts;

public interface ICatalogModule : IModule
{
    new const string ModuleName = "catalog";
}
