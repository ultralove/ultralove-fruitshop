using System.ComponentModel;
using System.Data;

namespace Fruitshop;

public static class IEnumerableExtensions
{
  public static DataTable AsDataTable<T>(this IEnumerable<T> data)
  {
    var properties = TypeDescriptor.GetProperties(typeof(T));
    var table = new DataTable();
    foreach (PropertyDescriptor prop in properties) {
      table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
    }
    foreach (var item in data) {
      var row = table.NewRow();
      foreach (PropertyDescriptor prop in properties) {
        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
      }
      table.Rows.Add(row);
    }
    return table;
  }
}
