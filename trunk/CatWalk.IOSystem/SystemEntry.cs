/*
	$Id$
*/
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
		public SystemEntry(ISystemDirectory parent, string name){
			if(name == null){
				throw new ArgumentNullException("name");
			}
			this.Parent = parent;
			this.Name = name;
			this.DisplayName = name;
		}

		#region Implemented

		/// <summary>
		/// 同階層内で一意な識別子
		/// </summary>
		public string Name{get; private set;}

		public string Path{
			get{
				if(this.Parent == null){
					return this.Parent.ConcatPath(this.Name);
				}else{
					return this.Name;
				}
			}
		}

		/// <summary>
		/// 表示名
		/// </summary>
		public virtual string DisplayName{get; private set;}

		/// <summary>
		/// 親のISystemDirectory
		/// </summary>
		public ISystemDirectory Parent{get; private set;}

		/// <summary>
		/// 表示パス。
		/// 親のISystemDirectoryのConcatDisplayPath関数によりDisplayNameを連結してDisplayPathに設定します。
		/// </summary>
		public string DisplayPath{
			get{
				if(this.Parent == null){
					return this.DisplayName;
				}else{
					return this.Parent.ConcatDisplayPath(this.DisplayName);
				}
			}
		}

		/// <summary>
		/// このエントリの実体が存在するかどうか。
		/// 既定では親のISystemDirectoryのContains関数を呼び出します。
		/// </summary>
		public virtual bool Exists {
			get {
				return (this.Parent != null) ? this.Parent.Contains(this.Name) : true;
			}
		}

		#endregion
	}
}
