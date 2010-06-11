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

namespace CatWalk.Collections{
	[Serializable]
	public class ObservableList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged{
		private IList<T> list;
		
		public ObservableList() : this(new List<T>()){
		}
		
		public ObservableList(IList<T> list){
			this.list = list;
		}
		
		#region Reentrancy
		
		private SimpleMonitor monitor = new SimpleMonitor();
		
		protected IDisposable BlockReentrancy(){
			this.monitor.Enter();
			return this.monitor;
		}
		
		protected void CheckReentrancy(){
			if((this.monitor.Busy && (this.CollectionChanged != null)) && (this.CollectionChanged.GetInvocationList().Length > 1)){
				throw new InvalidOperationException();
			}
		}
		
		[Serializable]
		private class SimpleMonitor : IDisposable{
			private int busyCount = 0;
			
			public bool Busy{
				get{
					return this.busyCount > 0;
				}
			}
			
			public void Enter(){
				this.busyCount++;
			}
			
			public void Dispose(){
				this.busyCount--;
			}
		}
		
		#endregion
		
		#region IList
		
		public void Add(T item){
			this.CheckReentrancy();
			this.list.Add(item);
			this.OnPropertyChanged("Count");
			this.OnPropertyChanged("Item[]");
			this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
		}
		
		public void Insert(int index, T item){
			this.CheckReentrancy();
			this.list.Insert(index, item);
			this.OnPropertyChanged("Count");
			this.OnPropertyChanged("Item[]");
			this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}
		
		public void RemoveAt(int index){
			this.CheckReentrancy();
			T item = this.list[index];
			this.list.RemoveAt(index);
			this.OnPropertyChanged("Count");
			this.OnPropertyChanged("Item[]");
			this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
		}
		
		public void Clear(){
			if(this.Count > 0){
				this.CheckReentrancy();
				this.list.Clear();
				this.OnPropertyChanged("Count");
				this.OnPropertyChanged("Item[]");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Reset);
			}
		}
		
		public bool Remove(T item){
			int index = this.list.IndexOf(item);
			if(index >= 0){
				this.CheckReentrancy();
				this.list.RemoveAt(index);
				this.OnPropertyChanged("Count");
				this.OnPropertyChanged("Item[]");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
				return true;
			}else{
				return false;
			}
		}
		
		public T this[int index]{
			get{
				return this.list[index];
			}
			set{
				T item = this.list[index];
				this.list[index] = value;
				this.OnPropertyChanged("Item[]");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, value, item, index);
			}
		}
		
		public bool Contains(T item){
			return this.list.Contains(item);
		}
		
		public int IndexOf(T item){
			return this.list.IndexOf(item);
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return this.GetEnumerator();
		}
		
		public IEnumerator<T> GetEnumerator(){
			return this.list.GetEnumerator();
		}
		
		public void CopyTo(T[] array, int index){
			this.list.CopyTo(array, index);
		}
		
		public int Count{
			get{
				return this.list.Count;
			}
		}
		
		public bool IsReadOnly{
			get{
				return this.list.IsReadOnly;
			}
		}
		
		#endregion
		
		#region INotifyCollectionChanged
		
		private void OnCollectionChanged(NotifyCollectionChangedAction action, T item){
			if(this.CollectionChanged != null){
				using(this.BlockReentrancy()){
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item));
				}
			}
		}
		
		private void OnCollectionChanged(NotifyCollectionChangedAction action, IList<T> list){
			if(this.CollectionChanged != null){
				using(this.BlockReentrancy()){
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, list));
				}
			}
		}
		
		private void OnCollectionChanged(NotifyCollectionChangedAction action, T newItem, T oldItem, int index){
			if(this.CollectionChanged != null){
				using(this.BlockReentrancy()){
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
				}
			}
		}
		
		private void OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index){
			if(this.CollectionChanged != null){
				using(this.BlockReentrancy()){
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
				}
			}
		}
		
		private void OnCollectionChanged(NotifyCollectionChangedAction action){
			if(this.CollectionChanged != null){
				using(this.BlockReentrancy()){
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
				}
			}
		}
		
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e){
			if(this.CollectionChanged != null){
				using(this.BlockReentrancy()){
					this.CollectionChanged(this, e);
				}
			}
		}
		
		#endregion
		
		#region INotifyPropertyChanged
		
		private void OnPropertyChanged(string prop){
			if(this.PropertyChanged != null){
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e){
			if(this.PropertyChanged != null){
				this.PropertyChanged(this, e);
			}
		}
		
		#endregion
	}
}