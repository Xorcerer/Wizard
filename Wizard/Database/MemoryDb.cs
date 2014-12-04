using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Xorcerer.Wizard.Database
{
    public class MemoryDb : IDb
    {
        #region Logger
        private Castle.Core.Logging.ILogger _logger = Castle.Core.Logging.NullLogger.Instance;

        public Castle.Core.Logging.ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }
        #endregion Logger

        readonly IDictionary<string, string> _jsons = new Dictionary<string, string>(); 

        readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>(),
			TypeNameHandling = TypeNameHandling.Auto,
        };

        #region IDb implementation

        public IList<JsonConverter> Converters { get { return _settings.Converters; } }

        public bool Save(string key, object obj)
        {
            var json = JsonConvert.SerializeObject(obj, _settings);

            Logger.DebugFormat("Saving: {0}\n {1}", obj.GetType(), json);

            _jsons[key] = json;
            return true;
        }

        public object Load(string key, Type type)
        {
            string json;
            if (!_jsons.TryGetValue(key, out json))
                return null;
            var obj = JsonConvert.DeserializeObject(json, type, _settings);
            return obj;
        }

        public T Load<T>(string key) where T: class
        {
            return (T) Load(key, typeof(T));
        }

        #endregion
    }
}

