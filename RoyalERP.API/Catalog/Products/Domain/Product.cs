﻿using RoyalERP.Common.Domain;

namespace RoyalERP.API.Catalog.Products.Domain;

public class Product : AggregateRoot {

    public string Name { get; private set; }

    private readonly List<Guid> _attributeIds;
    public IReadOnlyCollection<Guid> AttributeIds => _attributeIds.AsReadOnly();

    public Product(Guid id, int version, string name, List<Guid> attributes) : base(id, version) {
        Name = name;
        _attributeIds = attributes;
    }

    public void SetName(string name) {
        Name = name;
        throw new NotImplementedException();
    }

    public void AddAttribute(Guid attributeId) {
        _attributeIds.Add(attributeId);
        throw new NotImplementedException();
    }

    public void RemoveAttribute(Guid attributeId) {
        _attributeIds.Remove(attributeId);
        throw new NotImplementedException();
    }

}
