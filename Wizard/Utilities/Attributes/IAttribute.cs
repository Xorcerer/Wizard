using System;
using System.ComponentModel;

namespace Xorcerer.Wizard.Utilities
{
    public interface IAttribute : INotifyPropertyChanged
    {
        string Name { get; }
    }

    public interface IAttribute<T> : IAttribute
    {
        T Value { get; set; }
    }

}

