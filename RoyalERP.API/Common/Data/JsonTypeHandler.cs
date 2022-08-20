using System.Data;
using System.Text.Json;
using static Dapper.SqlMapper;

namespace RoyalERP.API.Common.Data;

public class JsonTypeHandler<T> : TypeHandler<Json<T>> {

    public override void SetValue(IDbDataParameter parameter, Json<T> value) {
        parameter.Value = JsonSerializer.Serialize(value.Value);
    }

    public override Json<T> Parse(object value) {
        if (value is string json) {
            return new Json<T>(JsonSerializer.Deserialize<T>(json));
        }

        return new Json<T>(default);
    }

}