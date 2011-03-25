/*
	$Id$
*/
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace CatWalk.Collections {
	[Serializable]
	public sealed class ReadOnlyObservableList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged{
		private IList<T> _Items;
		
		public ReadOnlyObservableList() : this(new ObservableCollection<T>()){
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="list">list must impliment INotifyCollectionChanged and INotifyPropertyChanged</param>
		public ReadOnlyObservableList(IList<T> list){
			if(!(list is INotifyCollectionChanged)){
				throw new InvalidCastException();
			}
			if(!(list is INotifyPropertyChanged)){
				throw new InvalidCastException();
			}
			this._Items = list;
		}
		
		#region IList
		
		public void Add(T item){
			throw new NotSupportedException();
		}
		
		public void Insert(int index, T item){
			throw new NotSupportedException();
		}
		
		public void RemoveAt(int index){
			throw new NotSupportedException();
		}
		
		public void Clear(){
			throw new NotSupportedException();
		}
		
		public bool Remove(T item){
			throw new NotSupportedException();
		}
		
		public T this[int index]{
			get{
				return this._Items[index];
			}
			set{
				throw new NotSupportedException();
			}
		}
		
		public bool Contains(T item){
			return this._Items.Contains(item);
		}
		
		public int IndexOf(T item){
			return this._Items.IndexOf(item);
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return this.GetEnumerator();
		}
		
		public IEnumerator<T> GetEnumerator(){
			return this._Items.GetEnumerator();
		}
		
		public void CopyTo(T[] array, int index){
			this._Items.CopyTo(array, index);
		}
		
		public int Count{
			get{
				return this._Items.Count;
			}
		}
		
		public bool IsReadOnly{
			get{
				return this._Items.IsReadOnly;
			}
		}
		
		#endregion
		
		#region INotifyCollectionChanged

		public event NotifyCollectionChangedEventHandler CollectionChanged{
			add{
				((INotifyCollectionChanged)this._Items).CollectionChanged += value;
			}
			remove{
				((INotifyCollectionChanged)this._Items).CollectionChanged -= value;
			}
		}
		
		#endregion
		
		#region INotifyPropertyChanged
		
		public event PropertyChangedEventHandler PropertyChanged{
			add{
				((INotifyPropertyChanged)this._Items).PropertyChanged += value;
			}
			remove{
				((INotifyPropertyChanged)this._Items).PropertyChanged -= value;
			}
		}
		
		#endregion
	}
}
