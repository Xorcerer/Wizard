using System;
using System.ComponentModel;

namespace Xorcerer.Wizard.Utilities
{

    public sealed class NullAttribute<T> : IAttribute<T>
    {
        public NullAttribute(string name)
        {
            Name = name;
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IAttribute implementation

        public string Name { get; private set; }

        public T Value
        {
            get
            {
                return default(T);
            }
            set { /* do nothing */ }
        }


        #endregion
    }
}
