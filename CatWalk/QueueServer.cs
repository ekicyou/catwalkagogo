/*
	$Id$
*/
using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.ComponentModel;

namespace CatWalk{
	/// <summary>
	/// Itemをキューイングして遅延実行するクラス
	/// </summary>
	/// <typeparam name="T">アイテムの型</typeparam>
	public class QueueServer<T> : DisposableObject{
		private Thread thread = null;
		private Queue<T> queue = new Queue<T>();
		private bool isBackground = false;
		
		public QueueServer(){
		}
		
		private bool disposed = false;
		protected override void Dispose(bool disposing){
			try{
				if(!this.disposed){
					this.ClearItems();
				}
			}finally{
				base.Dispose(disposing);
			}
		}
		
		#region アイテム
		
		/// <summary>
		/// Itemをキューに入れる
		/// </summary>
		/// <param name="item"></param>
		public void EnqueueItem(T item){
			lock(this.queue){
				this.queue.Enqueue(item);
			}
			if((this.thread == null) || !(this.thread.IsAlive)){
				this.CreateThread();
				this.thread.Start();
			}
		}
		
		/// <summary>
		/// キューをクリアする。
		/// </summary>
		public void ClearItems(){
			lock(this.queue){
				this.queue.Clear();
			}
		}
		
		#endregion
		
		#region Thread
		
		private void CreateThread(){
			this.thread = new Thread(new ThreadStart(this.ThreadProc));
			this.thread.IsBackground = this.isBackground;
		}
		
		private void ThreadProc(){
			T item = default(T);
			while(true){
				lock(this.queue){
					if(this.queue.Count > 0){
						item = this.queue.Dequeue();
					}else{
						break;
					}
				}
				try{
					this.OnProcessItem(item);
				}catch{
				}
			}
		}
		
		protected virtual void OnProcessItem(T item){
			if(ProcessItem != null){
				ProcessItem(item);
			}
		}
		
		/// <summary>
		/// アイテムを処理する関数
		/// </summary>
		public event Action<T> ProcessItem;
		
		#endregion
		
		#region プロパティ
		
		/// <summary>
		/// ThreadをBackgroundにするかどうか。
		/// </summary>
		public bool IsBackground{
			get{
				return this.isBackground;
			}
			set{
				this.isBackground = true;
				if((this.thread != null) && (this.thread.IsAlive)){
					this.thread.IsBackground = this.isBackground;
				}
			}
		}
		
		/// <summary>
		/// キューの現在の中身。
		/// </summary>
		public T[] Items{
			get{
				lock(this.queue){
					return this.queue.ToArray();
				}
			}
		}
		
		/// <summary>
		/// キューイングされたItemの数
		/// </summary>
		public int QueueCount{
			get{
				lock(this.queue){
					return this.queue.Count;
				}
			}
		}
		
		#endregion
	}
}