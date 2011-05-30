using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	public interface ISystemDirectory : ISystemEntry{
		/// <summary>
		/// 子のSystemEntry。
		/// 常に最新の状態を返すこと。
		/// </summary>
		IEnumerable<ISystemEntry> Children{get;}

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

		string ConcatPath(string name);
	}
}
