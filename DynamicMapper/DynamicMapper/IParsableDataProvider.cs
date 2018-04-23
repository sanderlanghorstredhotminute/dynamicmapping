using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicMapper
{
    public interface IParsableDataProvider
    {
        IEnumerable<IEnumerable<object>> Get();
    }
}
