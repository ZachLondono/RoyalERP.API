using RoyalERP.API.Common.Domain;

namespace RoyalERP.API.Sales.Companies.Domain;

public interface ICompanyRepository : IRepository<Company> {

    /// <summary>
    /// Find all companies with the given name (not case sensitive).
    /// </summary>
    public Task<IEnumerable<Guid>> GetCompanyIdsWithName(string name);

}
