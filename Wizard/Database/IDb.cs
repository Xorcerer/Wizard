using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Xorcerer.Wizard.Database
{
    /// <summary>
    /// Interface inspired by Couchbase .Net client.
    /// </summary>
    public interface IDb
    {
        bool Save(string key, object obj);
        object Load(string key, Type type);
        T Load<T>(string key) where T: class;

        IList<JsonConverter> Converters { get; }
        // TODO: Lock
    }
}

