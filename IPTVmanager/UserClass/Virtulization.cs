﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using IPTVman.Model;
using IPTVman.Helpers;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;


namespace IPTVman.ViewModel
{
    ////////////////////
    //By Paul McClean
    ///////////////////
   
   



    /// <summary>
    /// Demo implementation of IItemsProvider returning dummy customer items after
    /// a pause to simulate network/disk latency.
    /// </summary>
    public class CollectionProvider : IItemsProvider<ParamCanal>
    {

        private readonly int _count;
        private readonly int _fetchDelay;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionProvider"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="fetchDelay">The fetch delay.</param>
        public CollectionProvider(int count, int fetchDelay)
        {
            _count = count;
            _fetchDelay = fetchDelay;
        }

        /// <summary>
        /// Fetches the total number of items available.
        /// </summary>
        /// <returns></returns>
        public int FetchCount()
        {
            //Trace.WriteLine("FetchCount");
            //Thread.Sleep(_fetchDelay);
            //return _count;
            return ViewModelMain.myLISTbase.Count;

        }

        private object threadLock = new object();
        /// <summary>
        /// Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns></returns>
        public IList<ParamCanal> FetchRange(int startIndex, int count)
        {
            lock (threadLock)
            {
                //Trace.WriteLine("СОЗДАНИЕ ДИАПАЗОНА СТРАНИЦЫ: " + startIndex + "," + count);
                // Thread.Sleep(_fetchDelay);

                //создание страницы временной!!!
                ViewModelMain.myLIST = new List<ParamCanal>();

                ParamCanal customer = null;


                for (int i = startIndex; i < startIndex + count; i++)
                {


                    if (i >= ViewModelMain.myLISTbase.Count) break;

                    try
                    {
                        customer = new ParamCanal
                        {
                            name = ViewModelMain.myLISTbase[i].name,
                            ExtFilter = ViewModelMain.myLISTbase[i].ExtFilter,
                            group_title = ViewModelMain.myLISTbase[i].group_title,
                            http = ViewModelMain.myLISTbase[i].http,
                            logo = ViewModelMain.myLISTbase[i].logo,
                            tvg_name = ViewModelMain.myLISTbase[i].tvg_name,
                            ping = ViewModelMain.myLISTbase[i].ping,
                        };
                        ViewModelMain.myLIST.Add(customer);
                    }
                    catch { }
                }
            }
            return ViewModelMain.myLIST;
        }
    }





    /// <summary>
    /// Specialized list implementation that provides data virtualization. The collection is divided up into pages,
    /// and pages are dynamically fetched from the IItemsProvider when required. Stale pages are removed after a
    /// configurable period of time.
    /// Intended for use with large collections on a network or disk resource that cannot be instantiated locally
    /// due to memory consumption or fetch latency.
    /// </summary>
    /// <remarks>
    /// The IList implmentation is not fully complete, but should be sufficient for use as read only collection 
    /// data bound to a suitable ItemsControl.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class VirtualizingCollection<T> : IList<T>, IList
    {
        private object threadLock = new object();

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageTimeout">The page timeout.</param>
        public VirtualizingCollection(IItemsProvider<T> itemsProvider, int pageSize, int pageTimeout)
        {
            _itemsProvider = itemsProvider;
            _pageSize = pageSize;
            _pageTimeout = pageTimeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
        public VirtualizingCollection(IItemsProvider<T> itemsProvider, int pageSize)
        {
            _itemsProvider = itemsProvider;
            _pageSize = pageSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        public VirtualizingCollection(IItemsProvider<T> itemsProvider)
        {
            _itemsProvider = itemsProvider;
        }

        #endregion

        #region ItemsProvider

        private readonly IItemsProvider<T> _itemsProvider;

        /// <summary>
        /// Gets the items provider.
        /// </summary>
        /// <value>The items provider.</value>
        public IItemsProvider<T> ItemsProvider
        {
            get { return _itemsProvider; }
        }

        #endregion

        #region PageSize

        private readonly int _pageSize = 100;

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize
        {
            get { return _pageSize; }
        }

        #endregion

        #region PageTimeout

        private readonly long _pageTimeout = 10000;

        /// <summary>
        /// Gets the page timeout.
        /// </summary>
        /// <value>The page timeout.</value>
        public long PageTimeout
        {
            get { return _pageTimeout; }
        }

        #endregion

        #region IList<T>, IList

        #region Count

        private int _count = -1;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// The first time this property is accessed, it will fetch the count from the IItemsProvider.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public virtual int Count
        {
            get
            {
                //if (_count == -1)
                // {
                //     LoadCount();
                //}
                //return _count;a
                return ViewModelMain.myLISTbase.Count;//размер
            }
            protected set
            {
                _count = value;
            }
        }

        #endregion

        #region Indexer

        /// <summary>
        /// Gets the item at the specified index. This property will fetch
        /// the corresponding page from the IItemsProvider if required.
        /// </summary>
        /// <value></value>
        public T this[int index]
        {
            get 
            {
                lock (threadLock)
                {
                    // determine which page and offset within page
                    int pageIndex = index / PageSize;
                    int pageOffset = index % PageSize;

                    // request primary page
                    RequestPage(pageIndex);

                    // if accessing upper 50% then request next page
                    if (pageOffset > PageSize / 2 && pageIndex < Count / PageSize)
                    { RequestPage(pageIndex + 1); }

                    // if accessing lower 50% then request prev page
                    if (pageOffset < PageSize / 2 && pageIndex > 0)
                    { RequestPage(pageIndex - 1); }

                    // remove stale pages
                    CleanUpPages();

                    //defensive check in case of async load
                    //if (_pages[pageIndex] == null)
                    //{
                    //    //Trace.WriteLine(" return default");
                    //    return default(T);
                    //}


                    //Trace.WriteLine( "ALLPAGES=" + _pages.Count);
                    // Trace.WriteLine("main " +" index=" + pageIndex + " pageOffset =" + pageOffset +"  cou="+ Count );

                    /////fix crash
                    T retPAGE = default(T);

                    bool ok = false;
                    try
                    {
                        // Trace.WriteLine($"[1]index={pageIndex}  offser={pageOffset} size={_pages.Count} i={index} ps={PageSize} ");
                        if (_pages[pageIndex] != null) retPAGE = _pages[pageIndex][pageOffset];

                    }
                    catch(Exception ex)
                    {
                        Trace.WriteLine($"my crash {ex.Message} size={_pages.Count} i={index} ps={PageSize} ");
                        LoadPage(pageIndex);
                    }
                    finally { ok = true; }


                    if (ok) return retPAGE;
                    else return  //_pages[pageIndex][0];
                    default(T);

                    // return requested item
                }
            }
            set { throw new NotSupportedException(); }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region IEnumerator<T>, IEnumerator

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <remarks>
        /// This method should be avoided on large collections due to poor performance.
        /// </remarks>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Add

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Contains

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// Always false.
        /// </returns>
        public bool Contains(T item)
        {
            return false;
        }

        #endregion

        #region Clear

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IndexOf

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// Always -1.
        /// </returns>
        public int IndexOf(T item)
        {
            return -1;
        }

        #endregion

        #region Insert

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region CopyTo

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// -or-
        /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Misc

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>Always false.
        /// </returns>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>Always true.
        /// </returns>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>Always false.
        /// </returns>
        public bool IsFixedSize
        {
            get { return false; }
        }

        #endregion

        #endregion

        #region Paging

        private readonly Dictionary<int, IList<T>> _pages = new Dictionary<int, IList<T>>();
        private readonly Dictionary<int, DateTime> _pageTouchTimes = new Dictionary<int, DateTime>();

        /// <summary>
        /// Cleans up any stale pages that have not been accessed in the period dictated by PageTimeout.
        /// </summary>
        public void CleanUpPages()
        {
            List<int> keys = new List<int>(_pageTouchTimes.Keys);
            foreach (int key in keys)
            {
                // page 0 is a special case, since WPF ItemsControl access the first item frequently
                if (key != 0 && (DateTime.Now - _pageTouchTimes[key]).TotalMilliseconds > PageTimeout)
                {
                    _pages.Remove(key);
                    _pageTouchTimes.Remove(key);
                   // Trace.WriteLine("Removed Page: " + key);
                }
            }
        }

        /// <summary>
        /// Populates the page within the dictionary.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="page">The page.</param>
        protected virtual void PopulatePage(int pageIndex, IList<T> page)
        {
            
            if (_pages.ContainsKey(pageIndex)) _pages[pageIndex] = page;
        }

        /// <summary>
        /// Makes a request for the specified page, creating the necessary slots in the dictionary,
        /// and updating the page touch time.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        protected virtual void RequestPage(int pageIndex)
        {
          
                if (!_pages.ContainsKey(pageIndex))
                {
                    _pages.Add(pageIndex, null);
                    _pageTouchTimes.Add(pageIndex, DateTime.Now);
                    //Trace.WriteLine("Added page: " + pageIndex + "  pages count="+ _pages.Count);
                    LoadPage(pageIndex);
                }
                else
                {
                   _pageTouchTimes[pageIndex] = DateTime.Now;
                }
        
        }

        #endregion

        #region Load methods

        /// <summary>
        /// Loads the count of items.
        /// </summary>
        protected virtual void LoadCount()
        {
            Count = FetchCount();
        }

        /// <summary>
        /// Loads the page of items.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        protected virtual void LoadPage(int pageIndex)
        {
            //Trace.WriteLine("ЗАПОЛНЕНИЕ СТРАНИЦЫ [2] " + pageIndex);
            PopulatePage(pageIndex, FetchPage(pageIndex));
        }

        #endregion

        #region Fetch methods

        /// <summary>
        /// Fetches the requested page from the IItemsProvider.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns></returns>
        protected IList<T> FetchPage(int pageIndex)
        {
            //Trace.WriteLine("ВОЗВРАТ СТРАНИЦЫ:  pageIndex=" + pageIndex);
            return ItemsProvider.FetchRange(pageIndex * PageSize, PageSize);
        }

        /// <summary>
        /// Fetches the count of itmes from the IItemsProvider.
        /// </summary>
        /// <returns></returns>
        protected int FetchCount()
        {
            return ItemsProvider.FetchCount();
        }

        #endregion
    }



 
    /// <summary>
    /// Derived VirtualizatingCollection, performing loading asychronously.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    public class AsyncVirtualizingCollection<T> : VirtualizingCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private object threadLock = new object();
        public void UPDATE()
        {
            lock (threadLock)
            {
                LoadPage(0);
            }
        }
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncVirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        public AsyncVirtualizingCollection(IItemsProvider<T> itemsProvider)
            : base(itemsProvider)
        {
            _synchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncVirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
        public AsyncVirtualizingCollection(IItemsProvider<T> itemsProvider, int pageSize)
            : base(itemsProvider, pageSize)
        {
            _synchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncVirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageTimeout">The page timeout.</param>
        public AsyncVirtualizingCollection(IItemsProvider<T> itemsProvider, int pageSize, int pageTimeout)
            : base(itemsProvider, pageSize, pageTimeout)
        {
            _synchronizationContext = SynchronizationContext.Current;
        }

        #endregion

        #region SynchronizationContext

        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>
        /// Gets the synchronization context used for UI-related operations. This is obtained as
        /// the current SynchronizationContext when the AsyncVirtualizingCollection is created.
        /// </summary>
        /// <value>The synchronization context.</value>
        protected SynchronizationContext SynchronizationContext
        {
            get { return _synchronizationContext; }
        }

        #endregion

        #region INotifyCollectionChanged

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler h = CollectionChanged;
            if (h != null)
                h(this, e);
        }

        /// <summary>
        /// Fires the collection reset event.
        /// </summary>
        private void FireCollectionReset()
        {
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(e);
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler h = PropertyChanged;
            if (h != null)
                h(this, e);
        }

        /// <summary>
        /// Fires the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void FirePropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
            OnPropertyChanged(e);
        }

        #endregion

        #region IsLoading

        private bool _isLoading;

        /// <summary>
        /// Gets or sets a value indicating whether the collection is loading.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this collection is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                if (value != _isLoading)
                {
                    _isLoading = value;
                }
                FirePropertyChanged("IsLoading");
            }
        }

        #endregion

        #region Load overrides

        /// <summary>
        /// Asynchronously loads the count of items.
        /// </summary>
        protected override void LoadCount()
        {
            Count = 0;
            IsLoading = true;
            ThreadPool.QueueUserWorkItem(LoadCountWork);
        }

        /// <summary>
        /// Performed on background thread.
        /// </summary>
        /// <param name="args">None required.</param>
        private void LoadCountWork(object args)
        {
            int count = FetchCount();
            SynchronizationContext.Send(LoadCountCompleted, count);
        }

        /// <summary>
        /// Performed on UI-thread after LoadCountWork.
        /// </summary>
        /// <param name="args">Number of items returned.</param>
        private void LoadCountCompleted(object args)
        {
            Count = (int)args;
            IsLoading = false;
            FireCollectionReset();
        }

        /// <summary>
        /// Asynchronously loads the page.
        /// </summary>
        /// <param name="index">The index.</param>
        protected override void LoadPage(int index)
        {
           
            IsLoading = true;
            ThreadPool.QueueUserWorkItem(LoadPageWork, index);
        }

        /// <summary>
        /// Performed on background thread.
        /// </summary>
        /// <param name="args">Index of the page to load.</param>
        private void LoadPageWork(object args)
        {
           
            int pageIndex = (int)args;
            //Trace.WriteLine("ЗАПРОС ");
            IList<T> page = FetchPage(pageIndex);
            SynchronizationContext.Send(LoadPageCompleted, new object[] { pageIndex, page });
        }

        /// <summary>
        /// Performed on UI-thread after LoadPageWork.
        /// </summary>
        /// <param name="args">object[] { int pageIndex, IList(T) page }</param>
        private void LoadPageCompleted(object args)
        {
            int pageIndex = (int)((object[])args)[0];
            IList<T> page = (IList<T>)((object[])args)[1];

            //Trace.WriteLine("ЗАПОЛНЕНИЕ СТРАНИЦЫ [1] " + pageIndex);
            PopulatePage(pageIndex, page);
            IsLoading = false;
            FireCollectionReset();
        }

        #endregion
    }











    /// <summary>
    /// Represents a provider of collection details.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public interface IItemsProvider<T>
    {
        /// <summary>
        /// Fetches the total number of items available.
        /// </summary>
        /// <returns></returns>
        int FetchCount();

        /// <summary>
        /// Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns></returns>
        IList<T> FetchRange(int startIndex, int count);
    }


}



