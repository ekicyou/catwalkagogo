/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
/*
namespace CatWalk.IOSystem {
	public interface ISystemDirectory : ISystemEntry{
		IEnumerable<ISystemEntry> GetChildren();

		/// <summary>
		/// このISystemDirectoryが持つ指定した識別子のISystemDirectoryを返す
		/// </summary>
		/// <param name="name">識別子</param>
		/// <returns>一致したISystemDirectory。見つからない場合はnull</returns>
		ISystemDirectory GetChildDirectory(string name);

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
		ISystemDirectory GetChildDirectory(string name, CancellationToken token);
		bool Contains(string name, CancellationToken token);

		#endregion
	}
}
*/