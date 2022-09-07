using MediatR;
using RoyalERP.API.Common.Data;
using RoyalERP.API.Sales.Companies.Data;
using System.Data;
using System.Diagnostics;

namespace RoyalERP.API.Sales.Companies.Domain;

public class CompanyRepository : ICompanyRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    private readonly List<Company> _activeEntities = new();

    public CompanyRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task AddAsync(Company entity) {

        _activeEntities.Add(entity);

        const string command = @"INSERT INTO sales.companies (id, name, contact, email) values (@Id, @Name, @Contact, @Email);";

        await _connection.ExecuteAsync(sql: command, transaction: _transaction, param: entity);

        const string addressCommand = @"INSERT INTO sales.addresses (id, companyId, line1, line2, line3, city, state, zip) values (@Id, @CompanyId, @Line1, @Line2, @Line3, @City, @State, @Zip);";

        await _connection.ExecuteAsync(sql: addressCommand, transaction: _transaction, param: new {
            Id = Guid.NewGuid(),
            CompanyId = entity.Id,
            entity.Address.Line1,
            entity.Address.Line2,
            entity.Address.Line3,
            entity.Address.City,
            entity.Address.State,
            entity.Address.Zip
        });

    }

    public async Task<IEnumerable<Company>> GetAllAsync() {

        const string query = @"SELECT sales.companies.id as id, version, name, contact, email, sales.addresses.id as addressid, line1, line2, city, state, zip
                                FROM sales.companies
                                LEFT JOIN sales.addresses
                                ON sales.companies.id = sales.addresses.companyid;";

        var companiesData =  await _connection.QueryAsync<CompanyData>(query, transaction: _transaction);

        var companies = new List<Company>();
        foreach (var data in companiesData) {

            const string defaultsQuery = @"SELECT id, companyid, productid, attributeid, value FROM sales.companydefaults WHERE companyid = @CompanyId;";

            var defaultsData = await _connection.QueryAsync<DefaultConfigurationData>(defaultsQuery, transaction: _transaction, param: new { CompanyId = data });
            var defaults = new List<DefaultConfiguration>();
            foreach (var defaultData in defaultsData) {
                defaults.Add(new(defaultData.Id, defaultData.CompanyId, defaultData.ProductId, defaultData.AttributeId, defaultData.Value));
            }

            var info = data.Info?.Value ?? new();

            companies.Add(new Company(data.Id, data.Version, data.Name, data.Contact, data.Email, new() {
                Line1 = data.Line1,
                Line2 = data.Line2,
                Line3 = data.Line3,
                City = data.City,
                State = data.State,
                Zip = data.Zip,
            }, defaults, info));

        }

        return companies;

    }

    public async Task<Company?> GetAsync(Guid id) {

        const string query = @"SELECT sales.companies.id as id, version, name, contact, email, sales.addresses.id as addressid, line1, line2, city, state, zip
                                FROM sales.companies
                                LEFT JOIN sales.addresses
                                ON sales.companies.id = sales.addresses.companyid
                                WHERE sales.companies.id = @Id;";

        var data = await _connection.QuerySingleOrDefaultAsync<CompanyData?>(query, transaction: _transaction, param: new { Id = id });
        if (data is null) return null;

        const string defaultsQuery = @"SELECT id, companyid, productid, attributeid, value FROM sales.companydefaults WHERE companyid = @CompanyId;";

        var defaultsData = await _connection.QueryAsync<DefaultConfigurationData>(defaultsQuery, transaction: _transaction, param: new { CompanyId = id });
        var defaults = new List<DefaultConfiguration>();
        foreach (var defaultData in defaultsData) {
            defaults.Add(new(defaultData.Id, defaultData.CompanyId, defaultData.ProductId, defaultData.AttributeId, defaultData.Value));
        }

        var info = data.Info?.Value ?? new();

        return new Company(data.Id, data.Version, data.Name, data.Contact, data.Email, new() {
            Line1 = data.Line1,
            Line2 = data.Line2,
            Line3 = data.Line3,
            City = data.City,
            State = data.State,
            Zip = data.Zip,
        }, defaults, info);

    }

    public async Task RemoveAsync(Company entity) {

        const string query = "DELETE FROM sales.companies WHERE id = @Id;";

        var rows = await _connection.ExecuteAsync(query, transaction: _transaction, param: new { entity.Id });

        if (rows != 1) {

            Debug.WriteLine($"No rows where affected while trying to delete company {entity.Id}");
            
        }

    }

    public async Task UpdateAsync(Company entity) {

        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            int result = 0;

            if (domainEvent is Events.CompanyUpdatedEvent update) {

                const string command = "UPDATE sales.companies SET name = @Name, email = @Email, contact = @Contact WHERE id = @CompanyId;";

                result = await _connection.ExecuteAsync(command, param: new {
                    update.CompanyId,
                    update.Name,
                    update.Email,
                    update.Contact
                }, _transaction);

            } else if (domainEvent is Events.CompanyAddressUpdatedEvent addressUpdate) {

                const string command = "UPDATE sales.addresses SET line1 = @Line1, line2 = @Line2, line3 = @Line3, city = @City, state = @state, zip = @Zip WHERE companyid = @CompanyId;";

                result = await _connection.ExecuteAsync(command, param: new {
                    addressUpdate.CompanyId,
                    addressUpdate.Line1,
                    addressUpdate.Line2,
                    addressUpdate.Line3,
                    addressUpdate.City,
                    addressUpdate.State,
                    addressUpdate.Zip,
                }, _transaction);

            } else if (domainEvent is Events.CompanyInfoFieldSetEvent infoSet) {

                var info = await GetInfoColumn(infoSet.CompanyId);
                if (info is null) {
                    // TODO: log attempt to update value that does not exist and/or create new value in column
                    continue;
                }
                info[infoSet.Field] = infoSet.Value;

                result = await SetInfoColumn(infoSet.CompanyId, info);

            } else if (domainEvent is Events.CompanyInfoFieldRemovedEvent infoRemoved) {

                var info = await GetInfoColumn(infoRemoved.CompanyId);
                if (info is null) {
                    // TODO: log attempt to update value that does not exist
                    continue;
                }
                info.Remove(infoRemoved.Field);

                result = await SetInfoColumn(infoRemoved.CompanyId, info);

            }

            if (result < 1) {
                // TODO: log no update was preformed
            }

        }

        var existing = _activeEntities.FirstOrDefault(o => o.Id == entity.Id);
        if (existing is not null) _activeEntities.Remove(existing);
        _activeEntities.Add(entity);

    }
    
    private async Task<Dictionary<string,string>?> GetInfoColumn(Guid companyId) {
        const string query = "SELECT info FROM sales.companies WHERE id = @CompanyId";
        var infoJS = await _connection.QuerySingleOrDefaultAsync<Json<Dictionary<string, string>>>(query, param: new {
            CompanyId = companyId
        }, _transaction);

        if (infoJS is null) return null;
        return infoJS.Value;
    }

    private Task<int> SetInfoColumn(Guid companyId, Dictionary<string,string> info) {
        const string command = "UPDATE sales.companies SET info = @Info WHERE id = @CompanyId";
        var infoJS = new JsonParameter(info);
        return _connection.ExecuteAsync(command, param: new { CompanyId = companyId, Info = infoJS}, _transaction);
    }

    public async Task PublishEvents(IPublisher publisher) {
        foreach (var entity in _activeEntities) {
            await entity.PublishEvents(publisher);
        }
        _activeEntities.Clear();
    }

    public Task<IEnumerable<Guid>> GetCompanyIdsWithName(string name) {
        
        const string query = "SELECT id FROM sales.companies WHERE LOWER(name) = @Name;";

        return _connection.QueryAsync<Guid>(query, transaction: _transaction, param: new { Name = name.ToLower() });

    }

}