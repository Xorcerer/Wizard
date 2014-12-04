using System;
using System.Collections.Generic;

namespace Xorcerer.Wizard.Utilities
{
    public class TypeDict: IReadOnlyDictionary<int, Type>
    {
        IDictionary<int, Type> _idTypeDict = new Dictionary<int, Type>();
        IDictionary<Type, int> _typeIdDict = new Dictionary<Type, int>();

        public void Add(int id, Type type)
        {
            _idTypeDict.Add(id, type);
            _typeIdDict.Add(type, id);
        }

        public Type this[int id]
        {
            get
            {
                return _idTypeDict[id];
            }
        }

        public int this[Type type]
        {
            get
            {
                return _typeIdDict[type];
            }
        }

        #region IReadOnlyDictionary implementation

        public bool ContainsKey(int key)
        {
            return _idTypeDict.ContainsKey(key);
        }

        public bool TryGetValue(int key, out Type value)
        {
            return _idTypeDict.TryGetValue(key, out value);
        }

        public IEnumerable<int> Keys
        {
            get
            {
                return _idTypeDict.Keys;
            }
        }

        public IEnumerable<Type> Values
        {
            get
            {
                return _idTypeDict.Values;
            }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<KeyValuePair<int, Type>> GetEnumerator()
        {
            return _idTypeDict.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _idTypeDict.GetEnumerator();
        }

        #endregion

        #region IReadOnlyCollection implementation

        public int Count
        {
            get
            {
                return _idTypeDict.Count;
            }
        }

        #endregion
    }
}

