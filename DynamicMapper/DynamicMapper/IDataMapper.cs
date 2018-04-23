using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicMapper
{
    public interface IDataMapper
    {
        object MapToType(IDictionary<string, object> dataRow, Type type);
    }
}
