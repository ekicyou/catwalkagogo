/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk{
	using IO = System.IO;

	public struct FilePath : IEquatable<FilePath>{
		public FilePathKind PathKind{get; private set;}
		public string Path{get; private set;}
		public bool IsValid{get; private set;}

		#region Constructor

		public FilePath(string path) : this(){
			if(path == null){
				throw new ArgumentNullException();
			}

			if(IsAbsolute(path)){
				this.PathKind = FilePathKind.Absolute;
			}else{
				this.PathKind = FilePathKind.Relative;
			}

			this.IsValid = PathIsValid(path, this.PathKind);
			this.Path = (this.IsValid) ? Normalize(path, this.PathKind) : path;
		}

		public FilePath(string path, FilePathKind pathKind) : this(){
			if(path == null){
				throw new ArgumentNullException("path");
			}
			if(!Enum.IsDefined(typeof(FilePathKind), pathKind)){
				throw new ArgumentException("pathKind");
			}
			if(IsAbsolute(path)){
				if(pathKind != FilePathKind.Absolute){
					throw new ArgumentException("path");
				}
			}else{
				if(pathKind != FilePathKind.Relative){
					throw new ArgumentException("path");
				}
			}
			this.PathKind = pathKind;
			this.IsValid = PathIsValid(path, pathKind);
			this.Path = (this.IsValid) ? Normalize(path, this.PathKind) : path;
		}

		public static string Normalize(string path, FilePathKind pathKind){
			if(pathKind == FilePathKind.Absolute && path.Length >= 1){
				// ドライブ文字を大文字に
				path = path.Substring(0, 1).ToUpper() + path.Substring(1);
			}
			// /を\に
			path = path.Replace(IO.Path.AltDirectorySeparatorChar, IO.Path.DirectorySeparatorChar);
			return PackRelativePathInternal(path.TrimEnd(IO.Path.DirectorySeparatorChar));
		}

		private static bool IsAbsolute(string path){
			return (path.Length >= 1 && path[1] == ':');
		}

		private static bool PathIsValid(string path, FilePathKind pathKind){
			// 絶対パスの場合、ドライブ名をチェック
			if(pathKind ==FilePathKind.Absolute){
				if(!(path.Length >= 1 && (('A' <= path[0] && path[0] <= 'Z') || ('a' <= path[0] && path[0] <= 'z')))){
					return false;
				}
				if(path.Length >= 2 && (path[2] != IO.Path.DirectorySeparatorChar && path[2] != IO.Path.AltDirectorySeparatorChar)){
					return false;
				}
			}

			var nameInvChars = IO.Path.GetInvalidFileNameChars();
			var names = path.Split(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar);
			// 絶対パスの場合はドライブ名をスキップ
			return !((pathKind == FilePathKind.Absolute) ? names.Skip(1) : names)
				.Any(name => name.Any(c => nameInvChars.Contains(c)));
		}

		#endregion

		#region Properties

		/// <summary>
		/// フルパスを取得する。相対パスの場合はカレントディレクトリを基点としたフルパスを取得する。
		/// </summary>
		public string FullPath{
			get{
				this.ThrowIfInvalid();

				if(this.PathKind == FilePathKind.Absolute){
					return this.Path;
				}else{
					return this.GetFullPath(Environment.CurrentDirectory).Path;
				}
			}
		}

		/// <summary>
		/// ファイル名を取得する。パスがドライブパスや、ディレクトリパス(区切り文字で終わっている)の場合は空文字を返す。
		/// </summary>
		public string Name{
			get{
				this.ThrowIfInvalid();

				// 絶対パスでドライブパスの場合
				if(this.PathKind == FilePathKind.Absolute && this.Path.Length <= 3){
					return String.Empty;
				}
				var idx = this.Path.LastIndexOf(IO.Path.DirectorySeparatorChar);
				if(idx >= 0){
					return this.Path.Substring(idx + 1);
				}else{
					return this.Path;
				}
			}
		}

		public string Extension{
			get{
				this.ThrowIfInvalid();

				var name = this.Name;
				var idx = name.LastIndexOf('.');
				if(idx >= 0){
					return name.Substring(idx + 1);
				}else{
					return String.Empty;
				}
			}
		}

		public string Directory{
			get{
				this.ThrowIfInvalid();

				// 絶対パスでドライブパスの場合
				if(this.PathKind == FilePathKind.Absolute && this.Path.Length <= 3){
					return String.Empty;
				}
				var idx = this.Path.LastIndexOf(IO.Path.DirectorySeparatorChar);
				if(idx >= 0){
					return this.Path.Substring(0, idx);
				}else{
					return this.Path;
				}
			}
		}

		#endregion

		#region Functions

		private void ThrowIfInvalid(){
			if(!this.IsValid){
				throw new InvalidOperationException("This path is invalid");
			}
		}

		public FilePath GetFullPath(string basePath){
			return this.GetFullPath(new FilePath(basePath, FilePathKind.Absolute));
		}

		/// <summary>
		/// Get full path from base path.
		/// </summary>
		/// <param name="basePath">Base absolute path</param>
		/// <exception cref="System.InvalidOperationException">this path kind is not relative</exception>
		/// <exception cref="System.UriFormatException">The base path is not absolute.</exception>
		/// <returns>full path</returns>
		public FilePath GetFullPath(FilePath basePath){
			this.ThrowIfInvalid();
			if(this.PathKind != FilePathKind.Relative){
				throw new InvalidOperationException("Path kind is not relative");
			}
			if(basePath.PathKind != FilePathKind.Absolute){
				throw new ArgumentException("Given base path is not abusolute", "basePath");
			}
			if(!basePath.IsValid){
				throw new ArgumentException("Given path is invalid.", "basePath");
			}

			return basePath.Join(this);
		}

		public FilePath Join(string relativePath){
			return this.Join(new FilePath(relativePath, FilePathKind.Relative));
		}

		/// <summary>
		/// 指定した相対パスを結合する
		/// </summary>
		/// <param name="relativePath"></param>
		/// <exception cref="System.InvalidOperation">this path is not valid.</exception>
		/// <exception cref="System.ArgumentException">given path is not relative</exception>
		/// <returns>二つのパスを結合したパス。</returns>
		public FilePath Join(FilePath relativePath){
			this.ThrowIfInvalid();
			if(relativePath.PathKind != FilePathKind.Relative){
				throw new ArgumentException("Given path is not relative.", "relativePath");
			}
			if(!relativePath.IsValid){
				throw new ArgumentException("Given path is invalid.", "relativePath");
			}
			
			var baseNames = this.Path.Split(IO.Path.DirectorySeparatorChar);
			var destNames = relativePath.Path.Split(IO.Path.DirectorySeparatorChar);
			var outNames = new List<string>(baseNames.Length + destNames.Length);
			foreach(var names in new string[][]{baseNames, destNames}){
				PackRelativePathInternal(names, outNames);
			}
			return new FilePath(String.Join(IO.Path.DirectorySeparatorChar.ToString(), outNames));
		}

		public static FilePath GetCommonRoot(params string[] paths){
			return GetCommonRoot(paths.Select(path => new FilePath(path)));
		}

		public static FilePath GetCommonRoot(IEnumerable<string> paths){
			return GetCommonRoot(paths.Select(path => new FilePath(path)));
		}

		/// <summary>
		/// 指定したパスの配列に共通するパスを取得する。
		/// </summary>
		/// <param name="paths"></param>
		/// <exception cref="System.ArgumentException">指定したパスの配列の要素に相対パスが含まれているか、配列が空の時。</exception>
		/// <returns></returns>
		public static FilePath GetCommonRoot(IEnumerable<FilePath> paths){
			return GetCommonRoot(paths.ToArray());
		}
	
		/// <summary>
		/// 指定したパスの配列に共通するパスを取得する。
		/// </summary>
		/// <param name="paths"></param>
		/// <exception cref="System.ArgumentException">指定したパスの配列の要素に相対パスが含まれているか、配列が空の時。</exception>
		/// <returns></returns>
		public static FilePath GetCommonRoot(params FilePath[] paths){
			if(paths.Any(path => path.PathKind != FilePathKind.Absolute || !path.IsValid)){
				throw new ArgumentException("One or more paths are relative or invalid.", "paths");
			}
			if(paths.Length == 0){
				throw new ArgumentException("paths paremeter is empty", "paths");
			}

			if(paths.Length == 1){
				return paths[0];
			}
			var namesList = paths.Select(path => path.FullPath.Split(IO.Path.DirectorySeparatorChar));
			return new FilePath(String.Join(IO.Path.DirectorySeparatorChar.ToString(), GetCommonRootInternal(namesList)));
		}

		private static IEnumerable<string> GetCommonRootInternal(IEnumerable<string[]> namesList){
			namesList = namesList.OrderBy(names => names.Length).ToArray();
			var source = namesList.First();
			var target = namesList.Skip(1).ToArray();
			var common = source.TakeWhile((name, idx) => target.All(names => name.Equals(names[idx], StringComparison.OrdinalIgnoreCase)));
			return common;
		}

		public FilePath GetRelativePathTo(string dest){
			return this.GetRelativePathTo(new FilePath(dest, FilePathKind.Absolute));
		}

		/// <summary>
		/// 指定した絶対パスへの相対パスを取得する
		/// </summary>
		/// <param name="dest"></param>
		/// <exception cref="System.InvalidOperationException">this path kind is not absolute</exception>
		/// <exception cref="System.ArgumentException">dest path kind is not absolute.</exception>
		/// <returns>相対パス</returns>
		public FilePath GetRelativePathTo(FilePath dest){
			this.ThrowIfInvalid();
			if(this.PathKind != FilePathKind.Absolute){
				throw new InvalidOperationException("This path kind is not absolute");
			}
			if(dest.PathKind != FilePathKind.Absolute){
				throw new ArgumentException("Given base path is not abusolute", "dest");
			}
			if(!dest.IsValid){
				throw new ArgumentException("Given path is invalid.", "dest");
			}

			var fromNames = this.FullPath.Split(IO.Path.DirectorySeparatorChar);
			var destNames = dest.FullPath.Split(IO.Path.DirectorySeparatorChar);
			var common = GetCommonRootInternal(new string[][]{fromNames, destNames}).ToArray();
			var fromRoute = fromNames.Skip(common.Length);
			var destRoute = destNames.Skip(common.Length);
			return new FilePath(String.Join(IO.Path.DirectorySeparatorChar.ToString(), Enumerable.Repeat(@"..", fromRoute.Count()).Concat(destRoute)));
		}

		public static FilePath PackRelativePath(FilePath relativePath){
			if(!relativePath.IsValid){
				throw new ArgumentException("relativePath");
			}
			if(relativePath.PathKind != FilePathKind.Relative){
				throw new ArgumentException("relativePath");
			}

			return new FilePath(PackRelativePathInternal(relativePath.Path));
		}

		private static string PackRelativePathInternal(string relativePath){
			return String.Join(IO.Path.DirectorySeparatorChar.ToString(), PackRelativePathInternal(relativePath.Split(IO.Path.DirectorySeparatorChar)));
		}
		private static IEnumerable<string> PackRelativePathInternal(IEnumerable<string> names){
			var outNames = new List<string>();
			PackRelativePathInternal(names, outNames);
			return outNames;
		}
		private static void PackRelativePathInternal(IEnumerable<string> names, List<string> outNames){
			var names2 = names.ToArray();
			foreach(var name in names2){
				if(name == ".."){
					if(outNames.Count > 0 && outNames[outNames.Count - 1] != ".."){
						outNames.RemoveAt(outNames.Count - 1);
					}else{
						outNames.Add("..");
					}
				}else if(name != "."){
					outNames.Add(name);
				}
			}
		}

		#endregion

		#region IEquatable<FilePath> Members

		public bool Equals(FilePath other) {
			return
				this.Path.Equals(other.Path, StringComparison.OrdinalIgnoreCase) &&
				this.PathKind.Equals(other.PathKind) &&
				this.IsValid.Equals(other.IsValid);
		}

		public override int GetHashCode() {
			return this.Path.ToUpper().GetHashCode() ^ this.PathKind.GetHashCode() ^ this.IsValid.GetHashCode();
		}

		public override bool Equals(object obj) {
			if(!(obj is FilePath)) {
				return false;
			}
			return this.Equals((FilePath)obj);
		}

		#endregion

		#region Operators

		public static bool operator ==(FilePath a, FilePath b){
			return a.Equals(b);
		}

		public static bool operator !=(FilePath a, FilePath b){
			return !a.Equals(b);
		}

		#endregion
	}

	public enum FilePathKind{
		Absolute,
		Relative,
	}
}
