using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xorcerer.Wizard.Utilities
{
    public interface IAttributeContainer : IEnumerable<IAttribute>, INotifyPropertyChanged
    {
        void Add(IAttribute attribute);

        IAttribute<T> Get<T>(string name = null);
    }

}

