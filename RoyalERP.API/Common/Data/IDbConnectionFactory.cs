using System.Data;

namespace RoyalERP.API.Common.Data;

public interface IDbConnectionFactory {

    IDbConnection CreateConnection();

}