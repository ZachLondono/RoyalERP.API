using RoyalERP.Sales.Companies.DTO;

namespace RoyalERP.Sales.Contracts;

public static class SalesCompanies {

    public delegate Task<CompanyDTO?> GetCompanyById(Guid Id);

}
