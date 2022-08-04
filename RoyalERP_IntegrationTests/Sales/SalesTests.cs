﻿using Npgsql;
using RoyalERP.Common.Data;
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
        return new SalesUnitOfWork(factory, new FakePublisher(), companyRepoFactory, orderRepoFactory);
    }

    protected class SalesConnFactory : ISalesConnectionFactory {

        private readonly string _connString;
        public SalesConnFactory(string connString) {
            _connString = connString;
        }

        public IDbConnection CreateConnection() {
            return new NpgsqlConnection(_connString);
        }

    }

}