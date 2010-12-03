/*
	$Id$
*/
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;
using CatWalk;

namespace Nekome.Search{
	using IO = System.IO;
	
	public class FileListWorker : IDisposable{
		#region フィールド
		
		private bool disposed = false;
		private SearchOption option = SearchOption.AllDirectories;
		private string path = null;
		private string mask = "*.*";
		public string ExcludingMask{get; set;}
		private bool isEnumDirectories = true;
		
		private BackgroundWorker worker = new BackgroundWorker();
		private ProgressChangedEventHandler progressChanged = null;
		private RunWorkerCompletedEventHandler runWorkerCompleted = null;
		private object resumeObject = new object();
		private volatile bool isSuspended = false;
		
		#endregion
		
		#region コンストラクタ
		
		public FileListWorker()                                              : this(null, "*.*", SearchOption.AllDirectories, true){}
		public FileListWorker(string path)                                   : this(path, "*.*", SearchOption.AllDirectories, true){}
		public FileListWorker(string path, string mask)                      : this(path, mask,  SearchOption.AllDirectories, true){}
		public FileListWorker(string path, string mask, SearchOption option) : this(path, mask, option, true){}
		public FileListWorker(string path, string mask, SearchOption option, bool isEnumDirectories){
			this.path = path;
			this.mask = mask;
			this.option = option;
			this.isEnumDirectories = isEnumDirectories;
			
			this.worker.DoWork += this.DoWorkEventListener;
			this.worker.ProgressChanged += this.ProgressChangedEventListener;
			this.worker.RunWorkerCompleted += this.RunWorkerCompletedEventListener;
			this.worker.WorkerReportsProgress = true;
			this.worker.WorkerSupportsCancellation = true;
		}
		
		#endregion
		
		#region 関数
		
		private void DoWorkEventListener(object sender, DoWorkEventArgs e){
			DateTime t = DateTime.Now;
			string[] masks = this.mask.Split(';');
			string[] exMasks = (this.ExcludingMask.IsNullOrEmpty()) ? new string[0] : this.ExcludingMask.Split(';');
			this.List(this.path, 0, 100, e, masks, exMasks);
			e.Result = this.ElapsedTime = DateTime.Now - t;
		}
		
		private void ProgressChangedEventListener(object sender, ProgressChangedEventArgs e){
			this.OnProgressChanged(e);
		}
		
		private void RunWorkerCompletedEventListener(object sender, RunWorkerCompletedEventArgs e){
			this.OnRunWorkerCompleted(e);
		}
		
		public void Start(){
			if(this.IsBusy){
				throw new InvalidOperationException();
			}
			this.worker.RunWorkerAsync();
		}
		
		public void Stop(){
			this.worker.CancelAsync();
			if(this.isSuspended){
				this.isSuspended = false;
				Monitor.Enter(this.resumeObject);
				try{
					Monitor.Pulse(this.resumeObject);
				}finally{
					Monitor.Exit(this.resumeObject);
				}
			}
		}
		
		public void Suspend(){
			this.isSuspended = true;
		}
		
		public void Resume(){
			if(!this.isSuspended){
				throw new InvalidOperationException();
			}
			this.isSuspended = false;
			Monitor.Enter(this.resumeObject);
			try{
				Monitor.Pulse(this.resumeObject);
			}finally{
				Monitor.Exit(this.resumeObject);
			}
		}
		
		private void List(string path, double progress, double step, DoWorkEventArgs e, string[] masks, string[] exMasks){
			if(this.isSuspended){
				lock(this.resumeObject){
					Monitor.Wait(this.resumeObject);
				}
			}
			if(this.worker.CancellationPending){
				e.Cancel = true;
				return;
			}
			try{
				var files = IO.Directory.GetFiles(path)
					.Where(file =>
						IO.Path.GetFileName(file).Let(name => (masks.Where(mask => name.IsMatchWildCard(mask)).FirstOrDefault() != null) &&
						                                      (exMasks.Where(mask => name.IsMatchWildCard(mask)).FirstOrDefault() == null)));
				this.worker.ReportProgress((int)(progress + step), files);
				var dirs = IO.Directory.GetDirectories(path);
				if(this.isEnumDirectories){
					this.worker.ReportProgress((int)(progress + step), dirs);
				}
				if(this.option == SearchOption.AllDirectories){
					for(int i = 0; i < dirs.Length; i++){
						if(this.worker.CancellationPending){
							e.Cancel = true;
							return;
						}
						this.List(dirs[i], progress + (step / dirs.Length) * i, step / dirs.Length, e, masks, exMasks);
					}
				}
			}catch(IO.IOException ex){
				this.worker.ReportProgress((int)(progress + step), ex);
			}catch(UnauthorizedAccessException ex){
				this.worker.ReportProgress((int)(progress + step), ex);
			}
		}
		
		#endregion
		
		#region プロパティ
		
		public string Path{
			get{
				return this.path;
			}
			set{
				if(this.IsBusy){
					throw new InvalidOperationException();
				}
				string path = IO.Path.GetFullPath(value);
			}
		}
		
		public string Mask{
			get{
				return this.mask;
			}
			set{
				if(this.IsBusy){
					throw new InvalidOperationException();
				}
				string mask = value;
			}
		}
		
		public SearchOption SearchOption{
			get{
				return this.option;
			}
			set{
				if(this.IsBusy){
					throw new InvalidOperationException();
				}
				this.option = value;
			}
		}
		
		public bool IsEnumDirectories{
			get{
				return this.isEnumDirectories;
			}
			set{
				if(this.IsBusy){
					throw new InvalidOperationException();
				}
				this.isEnumDirectories = value;
			}
		}
		
		public bool IsBusy{
			get{
				return this.worker.IsBusy;
			}
		}
		
		public bool IsSuspended{
			get{
				return this.isSuspended;
			}
		}

		public TimeSpan ElapsedTime{get; private set;}
		
		#endregion
		
		#region イベント
		
		protected virtual void OnProgressChanged(ProgressChangedEventArgs e){
			if(this.progressChanged != null){
				this.progressChanged(this, e);
			}
		}
		
		protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e){
			if(this.runWorkerCompleted != null){
				this.runWorkerCompleted(this, e);
			}
		}
		
		/// <remarks>
		/// ProgressChangedEventArgs.UserStateには、ディレクトリのファイル一覧を列挙したときはstring[]、
		/// 例外が発生したときはExceptionの派生クラス(IOException・UnauthorizedAccessException)が入ります。
		/// 
		/// ProgressParcentageは100をディレクトリの数だけ再帰的に割った数で計算されています。
		/// </remarks>
		public event ProgressChangedEventHandler ProgressChanged{
			add{
				this.progressChanged += value;
			}
			remove{
				this.progressChanged -= value;
			}
		}
		
		/// <remarks>
		/// RunWorkerCompletedEventArgs.Resultには処理にかかった時間(TimeSpan)が入ります。
		/// </remarks>
		public event RunWorkerCompletedEventHandler RunWorkerCompleted{
			add{
				this.runWorkerCompleted += value;
			}
			remove{
				this.runWorkerCompleted -= value;
			}
		}
		
		#endregion
		
		#region IDisposable
		
		public void Dispose(){
			try{
				this.Dispose(true);
			}finally{
				if(!this.disposed){
					this.disposed = true;
					if(this.IsBusy){
						this.Stop();
					}
					this.worker.Dispose();
					GC.SuppressFinalize(this);
				}
			}
		}
		
		protected virtual void Dispose(bool disposing){
		}
		
		~FileListWorker(){
			this.Dispose(false);
		}
		
		#endregion
		
		#region テスト
		
		/*
		static void Main(){
			var worker = new FileListWorker("c:\\", "*.txt", SearchOption.AllDirectories, false);
			worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e){
				var files = e.UserState as string[];
				if(files != null){
					foreach(var file in files){
						Console.WriteLine(file);
					}
				}else{
					Console.WriteLine(e.UserState.ToString());
				}
			};
			worker.Start();
			Console.Write("Press any key to suspend.");
			Console.ReadLine();
			worker.Suspend();
			Console.Write("Press any key to resume.");
			Console.ReadLine();
			worker.Resume();
			Console.Write("Press any key to stop.");
			Console.ReadLine();
			worker.Dispose();
			Console.ReadLine();
		}
		*/
		
		#endregion
	}
}