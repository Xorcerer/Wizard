using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xorcerer.Wizard.Utilities
{
    [Serializable]
    class AttributeContainer : IAttributeContainer
    {
        IDictionary<string, IAttribute> _attributes = new Dictionary<string, IAttribute>();

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IAttributeContainer implementation

        public void Add(IAttribute attribute)
        {
            var type = attribute.GetType();
            var name = attribute.Name;
            if (name == null && type.IsGenericType)
                name = type.GenericTypeArguments[0].Name;
            _attributes.Add(name, attribute);
        }

        public IAttribute<T> Get<T>(string name = null)
        {
            name = name ?? typeof(T).Name;

            IAttribute attr;
            if (_attributes.TryGetValue(name, out attr))
                return (IAttribute<T>)attr;
            return new NullAttribute<T>(name);
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<IAttribute> GetEnumerator()
        {
            return _attributes.Values.GetEnumerator();
        }
        #endregion

        #region IEnumerable implementation

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _attributes.Values.GetEnumerator();
        }

        #endregion
    }
}
