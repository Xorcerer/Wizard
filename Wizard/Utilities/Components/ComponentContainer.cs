using System;
using System.Collections.Generic;
using System.Linq;


namespace Xorcerer.Wizard.Utilities
{
    public class ComponentContainer : ComponentBase, IComponentContainer, IAttributeContainer
    {
        public string Name { get; set; }

        protected ISet<IComponent> Components { get; private set; }

        public ComponentContainer()
        {
            Components = new SortedSet<IComponent>();
        }

        /// <summary>
        /// Accept and deliver the message to all components.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <exception cref="AggregateException">LogicExceptions from sub components' OnMessage.</exception>
        public override void OnMessage(object message)
        {
            var exceptions = new List<Exception>();
            foreach (var c in Components)
            {
                try { c.OnMessage(message); }
                catch (LogicException ex) { exceptions.Add(ex); }
            }
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        #region IComponentContainer implementation

        public virtual void Add(IComponent component)
        {
            Components.Add(component);
        }

        public void Update()
        {
            foreach (dynamic attr in _updatableAttributes)
                attr.Value.Update(this);
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<IComponent> GetEnumerator()
        {
            return Components.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Components.GetEnumerator();
        }

        #endregion

        #region --- IAttributeContainer ----

        #region INotifyPropertyChanged implementation

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        IAttributeContainer _attributeContainer = new AttributeContainer();
        IList<IAttribute> _updatableAttributes = new List<IAttribute>();

        #region IAttributeContainer implementation
        
        public void Add(IAttribute attribute)
        {
            _attributeContainer.Add(attribute);

            var type = attribute.GetType();
            if (type.GenericTypeArguments[0].GetInterfaces().Contains(typeof(IUpdatable)))
                _updatableAttributes.Add(attribute);
        }

        public IAttribute<T> Get<T>(string name)
        {
            return _attributeContainer.Get<T>(name);
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator<IAttribute> IEnumerable<IAttribute>.GetEnumerator()
        {
            return _attributeContainer.GetEnumerator();
        }

        #endregion

        #endregion ---- IAttributeContainer ----
    }
}

