using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMapper
{
    public static class DataParser
    {
        public static IEnumerable<dynamic> ProcessData(IEnumerable<IEnumerable<object>> values, Type modeltype)
        {
            var headers = values.FirstOrDefault().Select(h => h.ToString()).ToList();
            
            var resultProperties = modeltype.GetProperties();

            foreach (var row in values.Skip(1))
            {
                // skip empty rows in sheet
                if (!row.Any())
                    continue;

                var result = Activator.CreateInstance(modeltype);

                foreach (var resultProperty in resultProperties)
                {
                    if (!headers.Contains(resultProperty.Name))
                    {
                        continue;
                    }
                    var i = headers.IndexOf(resultProperty.Name);
                    if (i < row.Count())
                    {
                        var rowValue = row.Skip(i).FirstOrDefault()?.ToString();
                        if (!string.IsNullOrEmpty(rowValue))
                        {
                            resultProperty.SetValue(result, rowValue);
                        }
                    }
                }

                yield return result;
            }
        }
    }
}