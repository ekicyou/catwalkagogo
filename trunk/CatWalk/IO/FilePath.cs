/*
	$Id: FilePath.cs 315 2013-12-11 07:59:06Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IO{
	using IO = System.IO;

	public struct FilePath : IEquatable<FilePath>{
		public FilePathKind PathKind{get; private set;}
		/// <summary>
		/// ボリューム名
		/// PathFormatがWindowsの場合はドライブレター
		/// </summary>
		public string VolumeName { get; private set; }
		public string RawPath { get; private set; }
		private readonly string[] _Fragments;
		public IEnumerable<string> Fragments {
			get {
				return Array.AsReadOnly(this._Fragments);
			}
		}
		public bool IsValid{get; private set;}
		public FilePathFormat PathFormat { get; private set; }

		#region Constructor

		public static FilePathFormat PlatformPathFormat {
			get {
				switch(Environment.OSVersion.Platform) {
					case PlatformID.Win32NT:
					case PlatformID.Win32S:
					case PlatformID.Win32Windows:
					case PlatformID.WinCE:
					case PlatformID.Xbox:
						return FilePathFormat.Windows;
					default:
						return FilePathFormat.Unix;
				}
			}
		}

		public FilePath(string path) : this(path, PlatformPathFormat){
		}

		public FilePath(string path, FilePathFormat format) : this() {
			if(path == null) {
				throw new ArgumentNullException();
			}

			if(IsAbsolute(path)) {
				this.PathKind = FilePathKind.Absolute;
			} else {
				this.PathKind = FilePathKind.Relative;
			}

			string[] fragments;
			this.IsValid = PathIsValid(path, this.PathKind, format, out fragments);
			this.RawPath = path;
			if(this.IsValid) {
				Normalize(fragments, this.PathKind, format);
				if(this.PathKind == FilePathKind.Relative) {
					this.VolumeName = "";
				} else {
					if(format == FilePathFormat.Windows) {
						this.VolumeName = fragments[0].Substring(0, 1);
						var frag2 = new string[0];
						Array.Copy(fragments,1, frag2, 0, fragments.Length - 1);
						this._Fragments = frag2;
					} else {
						this.VolumeName = "";
						this._Fragments = fragments;
					}
				}
			}
		}

		public FilePath(string path, FilePathKind pathKind) : this(path, pathKind, PlatformPathFormat){
		}

		public FilePath(string path, FilePathKind pathKind, FilePathFormat format) : this(){
			if(path == null) {
				throw new ArgumentNullException("path");
			}
			if(!Enum.IsDefined(typeof(FilePathKind), pathKind)) {
				throw new ArgumentException("pathKind");
			}
			if(!Enum.IsDefined(typeof(FilePathFormat), format)) {
				throw new ArgumentException("format");
			}
			if(IsAbsolute(path)) {
				if(pathKind != FilePathKind.Absolute) {
					throw new ArgumentException("path");
				}
			} else {
				if(pathKind != FilePathKind.Relative) {
					throw new ArgumentException("path");
				}
			}
			this.PathKind = pathKind;
			this.PathFormat = format;
			string[] fragments;
			this.RawPath = path;
			this.IsValid = PathIsValid(path, pathKind, format, out fragments);
			if(this.IsValid) {
				Normalize(fragments, pathKind, format);
				if(format == FilePathFormat.Windows) {
					this.VolumeName = fragments[0].Substring(0, 1);
					var frag2 = new string[0];
					Array.Copy(fragments, 1, frag2, 0, fragments.Length - 1);
					this._Fragments = frag2;
				} else {
					this.VolumeName = "";
					this._Fragments = fragments;
				}
			}
		}

		#endregion

		#region Normalize

		public static string Normalize(string path, FilePathKind pathKind) {
			return Normalize(path, pathKind, PlatformPathFormat);
		}

		/// <summary>
		/// パス文字列を正規化する
		/// 区切り文字をformatに合わせ、末尾の区切り文字を削除する
		/// Windowsの場合はドライブ文字を大文字にする
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pathKind"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Normalize(string path, FilePathKind pathKind, FilePathFormat format){
			if(format == FilePathFormat.Windows) {
				if(pathKind == FilePathKind.Absolute && path.Length >= 1) {
					// ドライブ文字を大文字に
					path = path.Substring(0, 1).ToUpper() + path.Substring(1);
				}
			}

			path = path.Replace(GetAltDirectorySeparatorChar(format), GetDirectorySeparatorChar(format));
			if(pathKind == FilePathKind.Relative) {
				return PackRelativePathInternal(path.TrimEnd(GetDirectorySeparatorChar(format)), format);
			} else {
				return path.TrimEnd(GetDirectorySeparatorChar(format));
			}
		}

		private static void Normalize(string[] fragments, FilePathKind pathKind, FilePathFormat format) {
			if(format == FilePathFormat.Windows) {
				if(pathKind == FilePathKind.Absolute && fragments.Length > 0 && fragments[0].Length > 1) {
					// ドライブ文字を大文字に
					fragments[0] = fragments[0].Substring(0, 1).ToUpper() + GetVolumeSeparatorChar(format);
				}
			}

			if(pathKind == FilePathKind.Relative) {
				PackRelativePathInternal(fragments);
			}
		}

		#endregion

		#region Chars

		public static char GetDirectorySeparatorChar(FilePathFormat format) {
			switch(format) {
				case FilePathFormat.Windows:
					return '\\';
				case FilePathFormat.Unix:
					return '/';
			}

			throw new ArgumentException("format");
		}

		public static char GetAltDirectorySeparatorChar(FilePathFormat format) {
			switch(format) {
				case FilePathFormat.Windows:
					return '/';
				case FilePathFormat.Unix:
					return '\\';
			}

			throw new ArgumentException("format");
		}

		public static IReadOnlyCollection<char> GetInvalidFileNameChars() {
			return GetInvalidFileNameChars(PlatformPathFormat);
		}

		private static readonly IReadOnlyCollection<char> _InvalidFileNameChars = Array.AsReadOnly<char>(
			new char[]{ '\"', '<', '>', '|', '\0', (Char)1, (Char)2, (Char)3, (Char)4, (Char)5, (Char)6, (Char)7, (Char)8, (Char)9, (Char)10, (Char)11, (Char)12, (Char)13, (Char)14, (Char)15, (Char)16, (Char)17, (Char)18, (Char)19, (Char)20, (Char)21, (Char)22, (Char)23, (Char)24, (Char)25, (Char)26, (Char)27, (Char)28, (Char)29, (Char)30, (Char)31, ':', '*', '?', '\\', '/' });

		public static IReadOnlyCollection<char> GetInvalidFileNameChars(FilePathFormat format) {
			return _InvalidFileNameChars;
		}

		public static char GetVolumeSeparatorChar(FilePathFormat format) {
			switch(format) {
				case FilePathFormat.Windows:
					return ':';
				case FilePathFormat.Unix:
					return '/';
				default:
					throw new ArgumentException("format");
			}

		}


		#endregion

		#region Check

		private static bool IsAbsolute(string path){
			return (path.Length >= 1 && path[1] == ':');
		}

		private static bool PathIsValid(string path, FilePathKind pathKind, FilePathFormat format, out string[] fragments){
			if(!Enum.IsDefined(typeof(FilePathKind), pathKind)) {
				throw new ArgumentException("pathKind");
			}
			fragments = null;

			switch(format) {
				case FilePathFormat.Windows: {
						// 絶対パスの場合、ドライブ名をチェック
						if(pathKind == FilePathKind.Absolute) {
							if(!(path.Length >= 1 && (('A' <= path[0] && path[0] <= 'Z') || ('a' <= path[0] && path[0] <= 'z')))) {
								return false;
							}
							if(path.Length >= 2 && (path[2] != GetDirectorySeparatorChar(format) && path[2] != IO.Path.AltDirectorySeparatorChar)) {
								return false;
							}
						}
						break;
					}
				case FilePathFormat.Unix: {
						break;
					}
				default:
					throw new ArgumentException("format");
			}
			var nameInvChars = GetInvalidFileNameChars(format);
			fragments = path.Split(GetDirectorySeparatorChar(format), GetDirectorySeparatorChar(format));
			// 絶対パスでWindowsの場合はドライブ名をスキップ
			return !((pathKind == FilePathKind.Absolute && format == FilePathFormat.Windows) ? fragments.Skip(1) : fragments)
				.Any(name => name.Any(c => nameInvChars.Contains(c)));

		}

		#endregion

		#region Properties
		/// <summary>
		/// パス
		/// 絶対パスでWindowsの場合はボリューム区切り文字以降(C:\path→\path)
		/// </summary>
		public string Path {
			get {
				return this._Fragments != null ? String.Join(GetDirectorySeparatorChar(this.PathFormat).ToString(), this._Fragments) : null;
			}
		}

		/// <summary>
		/// フルパスを取得する。相対パスの場合はカレントディレクトリを基点としたフルパスを取得する。
		/// </summary>
		public string FullPath{
			get{
				this.ThrowIfInvalid();

				if(this.PathKind == FilePathKind.Absolute){
					if(this.PathFormat == FilePathFormat.Windows) {
						return this.VolumeName + GetVolumeSeparatorChar(this.PathFormat) + this.Path;
					} else {
						return this.Path;
					}
				}else{
					return this.GetFullPath(Environment.CurrentDirectory).Path;
				}
			}
		}

		/// <summary>
		/// ファイル名を取得する。パスがドライブパスや、ディレクトリパス(区切り文字で終わっている)の場合は空文字を返す。
		/// </summary>
		public string FileName{
			get{
				this.ThrowIfInvalid();

				if(this._Fragments.Length > 0) {
					return this._Fragments[this._Fragments.Length - 1];
				} else {
					return "";
				}
			}
		}

		public string Extension{
			get{
				this.ThrowIfInvalid();

				var name = this.FileName;
				var idx = name.LastIndexOf('.');
				if(idx >= 0){
					return name.Substring(idx + 1);
				}else{
					return String.Empty;
				}
			}
		}

		public string FileNameWithoutExtension {
			get {
				var name = this.FileName;
				return name.Substring(0, name.Length - this.Extension.Length);
			}
		}

		public string Directory{
			get{
				this.ThrowIfInvalid();

				if(this.PathKind == FilePathKind.Absolute) {
					return String.Join(GetDirectorySeparatorChar(this.PathFormat).ToString(), this._Fragments);
				} else {
					if(this._Fragments.Length > 0) {
						return String.Join(GetDirectorySeparatorChar(this.PathFormat).ToString(), this._Fragments.Skip(1));
					} else {
						return "";
					}
				}
			}
		}

		#endregion

		#region private Functions

		private void ThrowIfInvalid(){
			if(!this.IsValid){
				throw new InvalidOperationException("This path is invalid");
			}
		}

		#endregion

		#region GetFullPath

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
				throw new ArgumentException("Given base path is not absolute", "basePath");
			}
			if(!basePath.IsValid){
				throw new ArgumentException("Given path is invalid.", "basePath");
			}

			return basePath.Concat(this);
		}

		#endregion

		#region Concat

		public FilePath Concat(string relativePath){
			return this.Concat(new FilePath(relativePath, FilePathKind.Relative));
		}

		/// <summary>
		/// 指定した相対パスを結合する
		/// </summary>
		/// <param name="relativePath"></param>
		/// <exception cref="System.InvalidOperationException">this path is not valid.</exception>
		/// <exception cref="System.ArgumentException">given path is not relative</exception>
		/// <returns>二つのパスを結合したパス。</returns>
		public FilePath Concat(FilePath relativePath){
			this.ThrowIfInvalid();
			if(relativePath.PathKind != FilePathKind.Relative){
				throw new ArgumentException("Given path is not relative.", "relativePath");
			}
			if(!relativePath.IsValid){
				throw new ArgumentException("Given path is invalid.", "relativePath");
			}

			return this.Concat(relativePath._Fragments);
		}

		private FilePath Concat(string[] fragments) {
			var baseNames = this._Fragments;
			var destNames = fragments;
			var outNames = new List<string>(baseNames.Length + destNames.Length);
			foreach(var names in new string[][] { baseNames, destNames }) {
				PackRelativePathInternal(names, outNames);
			}
			return new FilePath(this, outNames);
		}

		private FilePath(FilePath basePath, IEnumerable<string> fragments) : this() {
			this.RawPath = null;
			this.VolumeName = basePath.VolumeName;
			this.PathFormat = basePath.PathFormat;
			this.PathKind = basePath.PathKind;
			this._Fragments = fragments.ToArray();
		}

		#endregion

		#region GetCommonRoot

		public static FilePath GetCommonRoot(params string[] paths){
			return GetCommonRoot(PlatformPathFormat, paths);
		}

		public static FilePath GetCommonRoot(IEnumerable<string> paths){
			return GetCommonRoot(PlatformPathFormat, paths);
		}

		public static FilePath GetCommonRoot(FilePathFormat format, params string[] paths) {
			return GetCommonRoot(format, paths.Select(path => new FilePath(path)));
		}

		public static FilePath GetCommonRoot(FilePathFormat format, IEnumerable<string> paths) {
			return GetCommonRoot(format, paths.Select(path => new FilePath(path)));
		}

		public static FilePath GetCommonRoot(FilePathFormat format, IEnumerable<FilePath> paths) {
			return GetCommonRoot(format, paths.ToArray());
		}

		/// <summary>
		/// 指定したパスの配列に共通するパスを取得する。
		/// </summary>
		/// <param name="paths"></param>
		/// <exception cref="System.ArgumentException">指定したパスの配列の要素に相対パスが含まれているか、配列が空の時。</exception>
		/// <returns></returns>
		public static FilePath GetCommonRoot(IEnumerable<FilePath> paths){
			return GetCommonRoot(PlatformPathFormat, paths.ToArray());
		}

		public static FilePath GetCommonRoot(params FilePath[] paths) {
			return GetCommonRoot(PlatformPathFormat, paths);
		}

		/// <summary>
		/// 指定したパスの配列に共通するパスを取得する。
		/// </summary>
		/// <param name="paths"></param>
		/// <exception cref="System.ArgumentException">指定したパスの配列の要素に相対パスが含まれているか、配列が空の時。</exception>
		/// <returns></returns>
		public static FilePath GetCommonRoot(FilePathFormat format, params FilePath[] paths){
			if(paths.Any(path => path.PathKind != FilePathKind.Absolute || !path.IsValid)){
				throw new ArgumentException("One or more paths are relative or invalid.", "paths");
			}
			if(paths.Length == 0){
				throw new ArgumentException("paths parameter is empty", "paths");
			}

			if(paths.Length == 1){
				return paths[0];
			}
			var namesList = paths.Select(path => path.FullPath.Split(GetDirectorySeparatorChar(format)));
			return new FilePath(String.Join(GetDirectorySeparatorChar(format).ToString(), GetCommonRootInternal(namesList)));
		}

		private static IEnumerable<string> GetCommonRootInternal(IEnumerable<string[]> namesList){
			namesList = namesList.OrderBy(names => names.Length).ToArray();
			var source = namesList.First();
			var target = namesList.Skip(1).ToArray();
			var common = source.TakeWhile((name, idx) => target.All(names => name.Equals(names[idx], StringComparison.OrdinalIgnoreCase)));
			return common;
		}

		#endregion

		#region GetRelativePathTo

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
				throw new ArgumentException("Given base path is not absolute", "dest");
			}
			if(!dest.IsValid){
				throw new ArgumentException("Given path is invalid.", "dest");
			}

			var fromNames = this.FullPath.Split(GetDirectorySeparatorChar(this.PathFormat));
			var destNames = dest.FullPath.Split(GetDirectorySeparatorChar(this.PathFormat));
			var common = GetCommonRootInternal(new string[][]{fromNames, destNames}).ToArray();
			var fromRoute = fromNames.Skip(common.Length);
			var destRoute = destNames.Skip(common.Length);
			return new FilePath(String.Join(GetDirectorySeparatorChar(this.PathFormat).ToString(), Enumerable.Repeat(@"..", fromRoute.Count()).Concat(destRoute)));
		}

		#endregion

		#region PackRelativePath

		public static FilePath PackRelativePath(FilePath relativePath) {
			return PackRelativePath(relativePath, PlatformPathFormat);
		}

		/// <summary>
		/// 相対パスの余分を削除します
		/// </summary>
		/// <param name="relativePath"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static FilePath PackRelativePath(FilePath relativePath, FilePathFormat format){
			if(!relativePath.IsValid){
				throw new ArgumentException("relativePath");
			}
			if(relativePath.PathKind != FilePathKind.Relative){
				throw new ArgumentException("relativePath");
			}

			return new FilePath(PackRelativePathInternal(relativePath.Path, format));
		}

		private static string PackRelativePathInternal(string relativePath, FilePathFormat format){
			return String.Join(GetDirectorySeparatorChar(format).ToString(), PackRelativePathInternal(relativePath.Split(GetDirectorySeparatorChar(format))));
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
			if(this.PathFormat == FilePathFormat.Windows) {
				return
					this.PathKind.Equals(other.PathKind) &&
					this.PathFormat.Equals(other.PathFormat) &&
					this.Path.Equals(other.Path, StringComparison.OrdinalIgnoreCase) &&
					this.VolumeName.Equals(other.VolumeName, StringComparison.OrdinalIgnoreCase) &&
					this.IsValid.Equals(other.IsValid);
			} else {
				return
					this.PathKind.Equals(other.PathKind) &&
					this.PathFormat.Equals(other.PathFormat) &&
					this.Path.Equals(other.Path) &&
					this.VolumeName.Equals(other.VolumeName) &&
					this.IsValid.Equals(other.IsValid);
			}
		}

		public override int GetHashCode() {
			if(this.PathFormat == FilePathFormat.Windows) {
				return this.Path.ToUpper().GetHashCode() ^
					this.PathKind.GetHashCode() ^
					this.IsValid.GetHashCode() ^
					this.VolumeName.ToUpper().GetHashCode() ^
					this.PathFormat.GetHashCode();
			} else {
				return this.Path.GetHashCode() ^
					this.PathKind.GetHashCode() ^
					this.IsValid.GetHashCode() ^
					this.VolumeName.GetHashCode() ^
					this.PathFormat.GetHashCode();

			}
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
		Absolute = 0,
		Relative = 1,
	}

	public enum FilePathFormat {
		Windows = 0,
		Unix = 1,
	}
}
