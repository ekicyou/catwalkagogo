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
			//this.worker.ProgressChanged += this.ProgressChangedEventListener;
			this.worker.RunWorkerCompleted += this.RunWorkerCompletedEventListener;
			//this.worker.WorkerReportsProgress = true;
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
		/*
		private void ProgressChangedEventListener(object sender, ProgressChangedEventArgs e){
			var proc = e.UserState as ProcessFileListEventArgs;
			if(proc != null){
				this.OnProcessFileList(proc);
			}else{
				this.OnFileListException((FileListExceptionEventArgs)e.UserState);
			}
		}
		*/
		private void RunWorkerCompletedEventListener(object sender, RunWorkerCompletedEventArgs e){
			if(e.Cancelled){
				this.OnCancelled(EventArgs.Empty);
			}
			this.OnRunWorkerCompleted(e);
		}
		
		public void Start(){
			if(this.IsBusy){
				throw new InvalidOperationException();
			}
			this.worker.RunWorkerAsync();
		}
		
		public void Stop(){
			//if(!this.IsBusy){
			//	throw new InvalidOperationException();
			//}
			this.worker.CancelAsync();
			this.OnCancelling(EventArgs.Empty);
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
				var files = IO.Directory.EnumerateFiles(path)
					.Where(file => IO.Path.GetFileName(file)
					.Let(name => 
						(masks.Where(mask => name.IsMatchWildCard(mask)).FirstOrDefault() != null) &&
						(exMasks.Where(mask => name.IsMatchWildCard(mask)).FirstOrDefault() == null)));
				this.OnProcessFileList(new ProcessFileListEventArgs(files, (progress + step) / 100));
				var dirsQ = IO.Directory.EnumerateDirectories(path);
				if(this.isEnumDirectories){
					this.OnProcessFileList(new ProcessFileListEventArgs(dirsQ, (progress + step) / 100));
				}
				if(this.option == SearchOption.AllDirectories){
					var dirs = dirsQ.ToArray();
					for(int i = 0; i < dirs.Length; i++){
						if(this.worker.CancellationPending){
							e.Cancel = true;
							return;
						}
						this.List(dirs[i], progress + (step / dirs.Length) * i, step / dirs.Length, e, masks, exMasks);
					}
				}
			}catch(IO.IOException ex){
				this.OnFileListException(new FileListExceptionEventArgs(ex, path, (progress + step) / 100));
			}catch(UnauthorizedAccessException ex){
				this.OnFileListException(new FileListExceptionEventArgs(ex, path, (progress + step) / 100));
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
		
		public event ProcessFileListEventHandler ProcessFileList;
		protected virtual void OnProcessFileList(ProcessFileListEventArgs e){
			if(this.ProcessFileList != null){
				this.ProcessFileList(this, e);
			}
		}
		
		public event RunWorkerCompletedEventHandler RunWorkerCompleted;
		protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e){
			if(this.RunWorkerCompleted != null){
				this.RunWorkerCompleted(this, e);
			}
		}
		
		public event FileListExceptionEventHandler FileListException;
		protected virtual void OnFileListException(FileListExceptionEventArgs e){
			if(this.FileListException != null){
				this.FileListException(this, e);
			}
		}

		public event EventHandler Cancelled;
		protected virtual void OnCancelled(EventArgs e){
			if(this.Cancelled != null){
				this.Cancelled(this, e);
			}
		}

		public event EventHandler Cancelling;
		protected virtual void OnCancelling(EventArgs e){
			if(this.Cancelling != null){
				this.Cancelling(this, e);
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

	public delegate void ProcessFileListEventHandler(object sender, ProcessFileListEventArgs e);
	public class ProcessFileListEventArgs : EventArgs{
		public IEnumerable<string> Files{get; private set;}
		public double ProgressPercentage{get; private set;}

		public ProcessFileListEventArgs(IEnumerable<string> files, double progress){
			this.Files = files;
			this.ProgressPercentage = progress;
		}
	}

	public delegate void FileListExceptionEventHandler(object sender, FileListExceptionEventArgs e);
	public class FileListExceptionEventArgs : EventArgs{
		public Exception Exception{get; private set;}
		public string Path{get; private set;}
		public double ProgressPercentage{get; private set;}

		public FileListExceptionEventArgs(Exception ex, string path, double progress){
			this.Exception = ex;
		}
	}
}