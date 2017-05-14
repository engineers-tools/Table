using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using BYOS.Interfaces;

namespace BYOS.Table
{
    public abstract class AbstractVectorCollection : ObservableCollection<IVector>
    {
        private string _DefaultPrefix = " ";
        public string DefaultPrefix 
        { 
            get { return _DefaultPrefix; }
            set { _DefaultPrefix = value; }
        }

        private void _InitialiseEvents()
        {
            CollectionChanged += _CollectionChanged;
        }

        public AbstractVectorCollection() : base()
        {
            _InitialiseEvents();
        }

        public AbstractVectorCollection(IEnumerable<IVector> collection) : base(collection)
        {
            _InitialiseEvents();
        }

        private void _CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IVector item = (IVector)e.NewItems[0];

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    item.Position = Count;
                    if (string.IsNullOrEmpty(item.Header)) item.Header = DefaultPrefix + Count;
                    ItemsAdded?.Invoke(this, e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ItemsRemoved?.Invoke(this, e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        public delegate void ItemsAddedHandler(object sender, NotifyCollectionChangedEventArgs e);
        public event ItemsAddedHandler ItemsAdded;

        public delegate void ItemsRemovedHandler(object sender, NotifyCollectionChangedEventArgs e);
        public event ItemsRemovedHandler ItemsRemoved;
    }
}