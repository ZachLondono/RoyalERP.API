using RoyalERP.Common.Data;
using RoyalERP.Common.Domain;
using RoyalERP.Sales;
using RoyalERP.Sales.Companies.Domain;
using RoyalERP.Sales.Orders.Domain;
using RoyalERP_IntegrationTests.Infrastructure;
using System;
using System.Data;
using System.Threading;

namespace RoyalERP_IntegrationTests.Sales;

public abstract class SalesTests : DbTests {

    protected readonly CancellationToken _token;

    public SalesTests() {
        CancellationTokenSource source = new();
        _token = source.Token;
    }

    protected ISalesUnitOfWork CreateUOW() {
        var factory = new SalesConnFactory(dbcontainer.ConnectionString);
        Func<IDbConnection, IDbTransaction, IOrderRepository> orderRepoFactory = (conn, trx) => new OrderRepository(new DapperConnection(conn), trx);
        Func<IDbConnection, IDbTransaction, ICompanyRepository> companyRepoFactory = (conn, trx) => new CompanyRepository(new DapperConnection(conn), trx);
        return new SalesUnitOfWork(factory, new FakeLogger<UnitOfWork>(), new FakePublisher(), companyRepoFactory, orderRepoFactory);
    }

}