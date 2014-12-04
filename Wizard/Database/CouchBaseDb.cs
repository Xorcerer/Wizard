using Couchbase;
using Couchbase.Extensions;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Xorcerer.Wizard.Database
{
    public class CouchBaseDb : IDb
    {
        private static readonly CouchbaseClient _instance;

        static CouchBaseDb()
        {
            _instance = new CouchbaseClient();
        }

        static CouchbaseClient Instance { get { return _instance; } }

        readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Converters = new List<JsonConverter>(),
            //TraceWriter = new MemoryTraceWriter(),
        };

        #region IDb implementation

        public IList<JsonConverter> Converters { get { return _settings.Converters; } }

        public bool Save(string key, object obj)
        {
            return Instance.StoreJson(StoreMode.Set, key, obj);
        }

        public object Load(string key, Type type)
        {
            string json = Instance.Get<string>(key);
            Debug.WriteLine(json);

            object obj = JsonConvert.DeserializeObject(json, type);
            Debug.Assert(obj.GetType() == type);
            return obj;
        }

        public T Load<T>(string key) where T: class
        {
            return Instance.GetJson<T>(key);
        }

        #endregion
    }
}

