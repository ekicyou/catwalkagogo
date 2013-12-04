/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace CatWalk.IOSystem {
	public interface ISystemEntry : IEquatable<ISystemEntry>{
		string Name{get;}
		string Path{get;}
		string DisplayName{get;}
		ISystemEntry Parent{get;}
		string DisplayPath{get;}
		bool IsExists();
		bool IsExists(CancellationToken token);

		bool IsDirectory { get; }

		IEnumerable<ISystemEntry> GetChildren();

		/// <summary>
		/// このISystemDirectoryが持つ指定した識別子のISystemDirectoryを返す
		/// </summary>
		/// <param name="name">識別子</param>
		/// <returns>一致したISystemDirectory。見つからない場合はnull</returns>
		ISystemEntry GetChildDirectory(string name);

		/// <summary>
		/// 指定した識別子のSystemEntryを含むかどうか
		/// </summary>
		/// <returns></returns>
		bool Contains(string name);

		/// <summary>
		/// 表示パスを連結する
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string ConcatDisplayPath(string name);

		/// <summary>
		/// パスを連結する。
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string ConcatPath(string name);

		#region Async

		IEnumerable<ISystemEntry> GetChildren(CancellationToken token);
		ISystemEntry GetChildDirectory(string name, CancellationToken token);
		bool Contains(string name, CancellationToken token);

		#endregion
	}
}
