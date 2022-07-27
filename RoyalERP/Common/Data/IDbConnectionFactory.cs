using System.Data;

namespace RoyalERP.Common.Data;

public interface IDbConnectionFactory {

    IDbConnection CreateConnection();

}