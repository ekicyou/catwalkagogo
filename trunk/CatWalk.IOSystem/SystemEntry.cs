using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace CatWalk.IOSystem {
	public abstract class SystemEntry : ISystemEntry{
		/// <summary>
		/// コンストラクタ。parentがnullの場合はルートフォルダになります。
		/// </summary>
		/// <param name="parent">親のISystemDirectory</param>
		/// <param name="id">同階層内で一意な識別子</param>
		public SystemEntry(ISystemDirectory parent, object id){
			this.Parent = parent;
			this.Id = id;
			this.DisplayName = (id != null) ? id.ToString() : "";
			if(parent == null){
				this._DisplayPath = new RefreshableLazy<string>(() => this.DisplayName);
			}else{
				this._DisplayPath = new RefreshableLazy<string>(() => this.Parent.ConcatDisplayPath(this.DisplayName));
			}
			this._Exists = new RefreshableLazy<bool>(() => (this.Parent != null) ? this.Parent.Contains(this.Id) : true);
		}

		#region Implemented

		/// <summary>
		/// 同階層内で一意な識別子
		/// </summary>
		public virtual object Id{get; private set;}

		/// <summary>
		/// 表示名
		/// </summary>
		public virtual string DisplayName{get; private set;}

		/// <summary>
		/// 親のISystemDirectory
		/// </summary>
		public virtual ISystemDirectory Parent{get; private set;}

		private RefreshableLazy<string> _DisplayPath;
		/// <summary>
		/// 表示パス。
		/// 既定では親のISystemDirectoryのConcatDisplayPath関数によりDisplayNameを連結してDisplayPathに設定します。
		/// </summary>
		public virtual string DisplayPath{
			get{
				return this._DisplayPath.Value;
			}
		}

		private RefreshableLazy<bool> _Exists;
		/// <summary>
		/// このエントリの実体が存在するかどうか。
		/// 既定では親のISystemDirectoryのContains関数を呼び出します。
		/// </summary>
		public virtual bool Exists {
			get {
				return this._Exists.Value;
			}
		}

		public virtual void Refresh(){
			this._Exists.Refresh();
			this._DisplayPath.Refresh();
			this.OnPropertyChanged("Exists", "DisplayPath");
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(params string[] names){
			CheckPropertyName(names);
			foreach(var name in names){
				this.OnPropertyChanged(new PropertyChangedEventArgs(name));
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e){
			var eh = this.PropertyChanged;
			if(eh != null){
				eh(this, e);
			}
		}

		[Conditional("DEBUG")]
		private void CheckPropertyName(params string[] names){
			var props = GetType().GetProperties();
			foreach(var name in names){
				var prop = props.Where(p => p.Name == name).SingleOrDefault();
				if(prop == null){
					throw new ArgumentException(name);
				}
			}
		}

		#endregion
	}
}
