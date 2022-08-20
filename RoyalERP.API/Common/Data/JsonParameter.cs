using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Text.Json;
using static Dapper.SqlMapper;

namespace RoyalERP.API.Common.Data;

/// <summary>
/// Dapper sql map for string to a Npgsql Json parameter
/// </summary>
public class JsonParameter : ICustomQueryParameter {

    private readonly string _value;

    public JsonParameter(string value) {
        _value = value;
    }

    public JsonParameter(object data) {
        _value = JsonSerializer.Serialize(data);
    }

    public void AddParameter(IDbCommand command, string name) {
        var parameter = new NpgsqlParameter(name, NpgsqlDbType.Json) {
            Value = _value
        };
        command.Parameters.Add(parameter);
    }
}