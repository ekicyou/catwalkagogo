using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IO{
	using IO = System.IO;

	public static partial class Seq{
		#region Directory

		public static IEnumerable<Tuple<IEnumerable<string>, double>> EnumerateFiles(string path, IO::SearchOption option){
			return EnumerateFileSystemEntries(IO::Path.GetFullPath(path), option, true, false, 0, 1);
		}

		public static IEnumerable<Tuple<IEnumerable<string>, double>> EnumerateDirectories(string path, IO::SearchOption option){
			return EnumerateFileSystemEntries(IO::Path.GetFullPath(path), option, false, true, 0, 1);
		}

		public static IEnumerable<Tuple<IEnumerable<string>, double>> EnumerateFileSystemEntries(string path, IO::SearchOption option){
			return EnumerateFileSystemEntries(IO::Path.GetFullPath(path), option, true, true, 0, 1);
		}

		private static IEnumerable<Tuple<IEnumerable<string>, double>> EnumerateFileSystemEntries(string path, IO::SearchOption option, bool isEnumFiles, bool isEnumDirs, double progress, double step){
			if(isEnumFiles){
				IEnumerable<string> files = null;
				try{
					files = IO::Directory.EnumerateFiles(path);
				}catch(IO::IOException){
				}catch(UnauthorizedAccessException){
				}
				if(files != null){
					yield return new Tuple<IEnumerable<string>, double>(files, progress);
				}
			}
			if(option == IO::SearchOption.AllDirectories){
				string[] dirs = null;
				try{
					dirs = IO::Directory.EnumerateDirectories(path).ToArray();
				}catch(IO::IOException){
				}catch(UnauthorizedAccessException){
				}
				if(dirs != null){
					if(isEnumDirs){
						yield return new Tuple<IEnumerable<string>, double>(dirs, progress);
					}
					var stepE = step / dirs.Length;
					for(int i = 0; i < dirs.Length; i++){
						foreach(var fileProg in EnumerateFileSystemEntries(dirs[i], option, isEnumFiles, isEnumDirs, progress + (step * i * stepE), stepE)){
							yield return fileProg;
						}
					}
				}
			}else if(isEnumDirs){
				IEnumerable<string> dirsQ = null;
				try{
					dirsQ = IO::Directory.EnumerateDirectories(path);
				}catch(IO::IOException){
				}catch(UnauthorizedAccessException){
				}
				if(dirsQ != null){
					yield return new Tuple<IEnumerable<string>, double>(dirsQ, progress + step);
				}
			}

		}

		#endregion
	}
}
