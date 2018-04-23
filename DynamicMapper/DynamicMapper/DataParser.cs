using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMapper
{
    public class DataParser
    {
        private readonly IParsableDataProvider _dataProvider;
        private readonly IDataMapper _dataMapper;

        public DataParser(IParsableDataProvider dataprovider, IDataMapper dataMapper)
        {
            _dataProvider = dataprovider;
            _dataMapper = dataMapper;
        }

        public IEnumerable<dynamic> ProcessData(Type modeltype)
        {
            var values = _dataProvider.Get();
            var headers = values.FirstOrDefault().Select(h => h.ToString()).ToList();
            
            foreach (var row in values.Skip(1))
            {
                // skip empty rows in sheet
                if (!row.Any())
                    continue;
                var dictionary = GetDictionaryFromData(headers, row.ToList());
                
                yield return _dataMapper.MapToType(dictionary, modeltype);
            }
        }

        private IDictionary<string, object> GetDictionaryFromData(IList<string> headers, IList<object> objects)
        {
            var dictionary = new Dictionary<string, object>();
            for (var i = 0; i < headers.Count; i++)
            {
                dictionary.Add(headers[i], objects.Count > i ? objects[i] : null);
            }
            return dictionary;
        }
    }
}