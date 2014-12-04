using System;
using System.Collections.Generic;

namespace Xorcerer.Wizard.Utilities
{
    public interface IComponentContainer : IComponent, IEnumerable<IComponent>, IAttributeContainer
	{
        string Name { get; }
        void Add(IComponent component);
        void Update();
	}
}

