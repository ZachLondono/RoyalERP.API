namespace RoyalERP.Common.Data;

/// <summary>
/// A wrapper for a type which will be read from a json colum
/// </summary>
public class Json<T> {

    public T? Value { get; init; }

    public Json(T? value) {
        Value = value;
    }

}
