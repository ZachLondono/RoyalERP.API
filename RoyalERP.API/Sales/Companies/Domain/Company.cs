using RoyalERP.API.Common.Domain;
using RoyalERP.API.Contracts.Companies;

namespace RoyalERP.API.Sales.Companies.Domain;

public class Company : AggregateRoot {

    public string Name { get; private set; }

    public string Contact { get; private set; }

    public string Email { get; private set; }

    public Address Address { get; private set; }

    private readonly List<DefaultConfiguration> _defaultConfigurations;
    public IReadOnlyCollection<DefaultConfiguration> DefaultConfigurations => _defaultConfigurations.AsReadOnly();

    private readonly Dictionary<string, string> _info;
    public IReadOnlyDictionary<string, string> Info => _info;

    public Company(Guid id, int version, string name, string contact, string email, Address address, List<DefaultConfiguration> defaultConfigurations, Dictionary<string, string> info) : base(id, version) {
        Name = name;
        Contact = contact;
        Email = email;
        Address = address;
        _defaultConfigurations = defaultConfigurations;
        _info = info;
    }

    private Company(string name) : this(Guid.NewGuid(), 0, name, "", "", new(), new(), new()) {
        AddEvent(new Events.CompanyCreatedEvent(Id, name));
    }

    public static Company Create(string name) => new(name);

    public void Update(string name, string contact, string email) {
        Name = name;
        Contact = contact;
        Email = email;
        AddEvent(new Events.CompanyUpdatedEvent(Id, name, contact, email));
    }

    public void SetAddress(string line1, string line2, string line3, string city, string state, string zip) {
        Address.Line1 = line1;
        Address.Line2 = line2;
        Address.Line3 = line3;
        Address.City = city;
        Address.State = state;
        Address.Zip = zip;
        AddEvent(new Events.CompanyAddressUpdatedEvent(Id, line1, line2, line3, city, state, zip));
    }

    public DefaultConfiguration SetDefault(Guid productId, Guid attributeId, string value) {
        var config = _defaultConfigurations.Where(c => c.ProductId.Equals(productId) && c.AttributeId.Equals(attributeId))
                                            .FirstOrDefault();
        if (config is null) {
            config = DefaultConfiguration.Create(Id, productId, attributeId, value);
            _defaultConfigurations.Add(config);
        } else config.SetValue(value);

        return config;
    }

    public bool RemoveDefault(DefaultConfiguration config) {
        var result = _defaultConfigurations.Remove(config);
        if (result) AddEvent(new Events.CompanyDefaultRemovedEvent(Id, config.Id));
        return result;
    }

    public void SetInfo(string field, string value) {
        _info[field] = value;
        AddEvent(new Events.CompanyInfoFieldSet(Id, field, value));
    }

    public bool RemoveInfo(string field) {
        var result = _info.Remove(field);
        if (result) AddEvent(new Events.CompanyInfoFieldRemoved(Id, field));
        return result;
    }

    public CompanyDTO AsDTO() {

        List<DefaultConfigurationDTO> defaults = new();

        foreach (var config in _defaultConfigurations) {

            defaults.Add(new() {
                ProductId = config.ProductId,
                AttributeId = config.AttributeId,
                Value = config.Value
            });

        }

        return new() {
            Id = Id,
            Name = Name,
            Contact = Contact,
            Email = Email,
            Defaults = defaults,
            Info = _info,
            Address = new() {
                Line1 = Address.Line1,
                Line2 = Address.Line2,
                Line3 = Address.Line3,
                City = Address.City,
                State = Address.State,
                Zip = Address.Zip,
            }
        };

    }

}
