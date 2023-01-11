using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
// ****************************************************************************
// Original code from Caliburn.Micro
// Small changes to implement interface and collection changed event args
// See: https://caliburnmicro.codeplex.com/SourceControl/latest#src/Caliburn.Micro/BindableCollection.cs
// ****************************************************************************

using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Clarity.Wpf
{
    [Serializable]
    public class BindableCollection<T> : ObservableCollection<T>, IBindableCollection<T>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "BindableCollection{T}" /> class.
        /// </summary>
        public BindableCollection()
        {
            IsNotifying = true;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BindableCollection{T}" /> class.
        /// </summary>
        /// <param name = "collection">The collection from which the elements are copied.</param>
        /// <exception cref = "T:System.ArgumentNullException">
        ///   The <paramref name = "collection" /> parameter cannot be null.
        /// </exception>
        public BindableCollection(IEnumerable<T> collection)
        {
            IsNotifying = true;
            AddRange(collection);
        }

        [field: NonSerialized]
        bool isNotifying;

        /// <summary>
        ///   Enables/Disables property change notification.
        /// </summary>
        public bool IsNotifying
        {
            get { return isNotifying; }
            set { isNotifying = value; }
        }

        /// <summary>
        ///   Notifies subscribers of the property change.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        public void NotifyOfPropertyChange(string propertyName)
        {
            if (IsNotifying)
                RaisePropertyChangedEventImmediately(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///   Raises a change notification indicating that all bindings should be refreshed.
        /// </summary>
        public void Refresh()
        {
            Execute.OnUIThread(() =>
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            });
        }

        /// <summary>
        ///   Inserts the item to the specified position.
        /// </summary>
        /// <param name = "index">The index to insert at.</param>
        /// <param name = "item">The item to be inserted.</param>
        protected override sealed void InsertItem(int index, T item)
        {
            Execute.OnUIThread(() => InsertItemBase(index, item));
        }

        /// <summary>
        ///   Exposes the base implementation of the <see cref = "InsertItem" /> function.
        /// </summary>
        /// <param name = "index">The index.</param>
        /// <param name = "item">The item.</param>
        /// <remarks>
        ///   Used to avoid compiler warning regarding unverifiable code.
        /// </remarks>
        protected virtual void InsertItemBase(int index, T item)
        {
            base.InsertItem(index, item);
        }

#if NET
    /// <summary>
    /// Moves the item within the collection.
    /// </summary>
    /// <param name="oldIndex">The old position of the item.</param>
    /// <param name="newIndex">The new position of the item.</param>
        protected sealed override void MoveItem(int oldIndex, int newIndex) {
            Execute.OnUIThread(() => MoveItemBase(oldIndex, newIndex));
        }

        /// <summary>
        /// Exposes the base implementation fo the <see cref="MoveItem"/> function.
        /// </summary>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        /// <remarks>Used to avoid compiler warning regarding unverificable code.</remarks>
        protected virtual void MoveItemBase(int oldIndex, int newIndex) {
            base.MoveItem(oldIndex, newIndex);
        }
#endif

        /// <summary>
        ///   Sets the item at the specified position.
        /// </summary>
        /// <param name = "index">The index to set the item at.</param>
        /// <param name = "item">The item to set.</param>
        protected override sealed void SetItem(int index, T item)
        {
            Execute.OnUIThread(() => SetItemBase(index, item));
        }

        /// <summary>
        ///   Exposes the base implementation of the <see cref = "SetItem" /> function.
        /// </summary>
        /// <param name = "index">The index.</param>
        /// <param name = "item">The item.</param>
        /// <remarks>
        ///   Used to avoid compiler warning regarding unverifiable code.
        /// </remarks>
        protected virtual void SetItemBase(int index, T item)
        {
            base.SetItem(index, item);
        }

        /// <summary>
        ///   Removes the item at the specified position.
        /// </summary>
        /// <param name = "index">The position used to identify the item to remove.</param>
        protected override sealed void RemoveItem(int index)
        {
            Execute.OnUIThread(() => RemoveItemBase(index));
        }

        /// <summary>
        ///   Exposes the base implementation of the <see cref = "RemoveItem" /> function.
        /// </summary>
        /// <param name = "index">The index.</param>
        /// <remarks>
        ///   Used to avoid compiler warning regarding unverifiable code.
        /// </remarks>
        protected virtual void RemoveItemBase(int index)
        {
            base.RemoveItem(index);
        }

        /// <summary>
        ///   Clears the items contained by the collection.
        /// </summary>
        protected override sealed void ClearItems()
        {
            Execute.OnUIThread(ClearItemsBase);
        }

        /// <summary>
        ///   Exposes the base implementation of the <see cref = "ClearItems" /> function.
        /// </summary>
        /// <remarks>
        ///   Used to avoid compiler warning regarding unverifiable code.
        /// </remarks>
        protected virtual void ClearItemsBase()
        {
            base.ClearItems();
        }

        /// <summary>
        ///   Raises the <see cref = "E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged" /> event with the provided arguments.
        /// </summary>
        /// <param name = "e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (IsNotifying)
            {
#if WinRT
                var args = new VectorChangedEventArgs();
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add:
                        args.CollectionChange = CollectionChange.ItemInserted;
                        args.Index = (uint)e.NewStartingIndex;
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        args.CollectionChange = CollectionChange.ItemRemoved;
                        args.Index = (uint)e.OldStartingIndex;
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        args.CollectionChange = CollectionChange.ItemChanged;
                        args.Index = (uint)e.NewStartingIndex;
                        break;
                    case NotifyCollectionChangedAction.Reset:
                    case NotifyCollectionChangedAction.Move:
                        args.CollectionChange = CollectionChange.Reset;
                        break;
                }

                VectorChanged(this, args);
#endif
                base.OnCollectionChanged(e);
            }
        }

        /// <summary>
        ///   Raises the PropertyChanged event with the provided arguments.
        /// </summary>
        /// <param name = "e">The event data to report in the event.</param>
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (IsNotifying)
            {
#if WinRT
                PropertyChanged(this, new PropertyChangedEventArgs(e.PropertyName));
#endif
                base.OnPropertyChanged(e);
            }
        }

#if WinRT
        /// <summary>
        ///   Raises the PropertyChanged event with the provided arguments.
        /// </summary>
        /// <param name = "e">The event data to report in the event.</param>
        protected void OnPropertyChanged(PropertyChangedEventArgs e) {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(e.PropertyName));
        }
#endif

        void RaisePropertyChangedEventImmediately(PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        /// <summary>
        ///   Adds the range.
        /// </summary>
        /// <param name = "items">The items.</param>
        public virtual void AddRange(System.Collections.IEnumerable items)
        {
            Execute.OnUIThread(() =>
            {
                var previousNotificationSetting = IsNotifying;
                IsNotifying = false;
                var index = Count;
                var changedItems = new List<T>();
                foreach (var item in items)
                {
                    if (item is T)
                    {
                        changedItems.Add((T)item);
                        InsertItemBase(index, (T)item);
                        index++;
                    }
                }
                IsNotifying = previousNotificationSetting;

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems));
                OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            });
        }

        /// <summary>
        ///   Adds the range.
        /// </summary>
        /// <param name = "items">The items.</param>
        public virtual void AddRange(IEnumerable<T> items)
        {
            Execute.OnUIThread(() =>
            {
                var previousNotificationSetting = IsNotifying;
                IsNotifying = false;
                var index = Count;
                var changedItems = new List<T>();
                foreach (var item in items)
                {
                    changedItems.Add((T)item);
                    InsertItemBase(index, item);
                    index++;
                }
                IsNotifying = previousNotificationSetting;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems));
                OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            });
        }

        /// <summary>
        ///   Removes the range.
        /// </summary>
        /// <param name = "items">The items.</param>
        public virtual void RemoveRange(IEnumerable<T> items)
        {
            Execute.OnUIThread(() =>
            {
                var previousNotificationSetting = IsNotifying;
                IsNotifying = false;
                foreach (var item in items)
                {
                    var index = IndexOf(item);
                    RemoveItemBase(index);
                }
                IsNotifying = previousNotificationSetting;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            });
        }

        /// <summary>
        /// Called when the object is deserialized.
        /// </summary>
        /// <param name="c">The streaming context.</param>
        [OnDeserialized]
        public void OnDeserialized(StreamingContext c)
        {
            IsNotifying = true;
        }

        /// <summary>
        /// Used to indicate whether or not the IsNotifying property is serialized to Xml.
        /// </summary>
        /// <returns>Whether or not to serialize the IsNotifying property. The default is false.</returns>
        public virtual bool ShouldSerializeIsNotifying()
        {
            return false;
        }
    }
}
