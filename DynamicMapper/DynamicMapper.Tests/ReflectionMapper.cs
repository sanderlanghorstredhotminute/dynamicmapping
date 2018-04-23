using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicMapper.Tests
{
    public class ReflectionMapper : IDataMapper
    {
        public object MapToType(IDictionary<string, object> dataRow, Type type)
        {
            var resultProperties = type.GetProperties();
            var result = Activator.CreateInstance(type);

            foreach (var resultProperty in resultProperties)
            {
                if (!dataRow.ContainsKey(resultProperty.Name))
                {
                    continue;
                }
                
                var rowValue = dataRow[resultProperty.Name]?.ToString();
                if (!string.IsNullOrEmpty(rowValue))
                {
                    resultProperty.SetValue(result, rowValue);
                }
            }
            return result;
        }
    }
}
