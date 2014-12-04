using System;
using System.ComponentModel;

namespace Xorcerer.Wizard.Utilities
{
    [Serializable]
    public abstract class Attribute<T> : IAttribute<T>
    {
        protected Attribute(string name)
        {
            Name = name;
        }

        #region INotifyPropertyChanged implementation
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion
        
        #region IAttribute implementation

        public string Name { get; private set; }

        T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(Name));
            }
        }

        #endregion
    }
}

