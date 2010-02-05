using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

static class Program{
	static void Main(){
		const int N = 1000000;
		var sw = new Stopwatch();
		var rnd = new Random();
		var array = new int[N];
		
		for(int i = 0; i < N; i++){
			array[i] = rnd.Next();
		}
		Console.Write("QuickSort: ");
		sw.Reset();
		sw.Start();
		Array.Sort(array);
		sw.Stop();
		Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
		
		for(int i = 0; i < N; i++){
			array[i] = rnd.Next();
		}
		Console.Write("MergeSort: ");
		sw.Reset();
		sw.Start();
		array.MergeSort();
		sw.Stop();
		
		foreach(var n in array){
			//Console.WriteLine(n);
		}
		
		Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
	}
}

static class ArrayExtension{
	public static void MergeSort<T>(this T[] array){
		array.MergeSort(Comparer<T>.Default);
	}
	
	public static void MergeSort<T>(this T[] array, IComparer<T> comparer){
		array.MergeSort(comparer, 0, array.Length, new T[array.Length]);
	}
	
	private static void MergeSort<T>(this T[] array, IComparer<T> comparer, int left, int right, T[] temp){
		if(left >= (right - 1)){
			return;
		}
		int count = right - left;
		if(count <= 12){
			array.InsertSort(comparer, left, right);
		}else{
			int middle = (left + right) >> 1;
			array.MergeSort(comparer, left, middle, temp);
			array.MergeSort(comparer, middle, right, temp);
			array.Merge(comparer, left, middle, right, temp);
		}
	}
	
	private static void Merge<T>(this T[] array, IComparer<T> comparer, int left, int middle, int right, T[] temp){
		Array.Copy(array, left, temp, left, middle - left);
		int i, j;
		for(i = middle, j = right - 1; i < right; i++, j--){
			temp[i] = array[j];
		}
		
		i = left;
		j = right - 1;
		for(int k = left; k < right; k++){
			if(comparer.Compare(temp[i], temp[j]) < 0){
				array[k] = temp[i++];
			}else{
				array[k] = temp[j--];
			}
		}
	}
	
	public static void ParallelMergeSort<T>(this T[] array){
		array.ParallelMergeSort(Comparer<T>.Default);
	}
	
	public static void ParallelMergeSort<T>(this T[] array, IComparer<T> comparer){
		if(array.Length < 1024){
			array.MergeSort(comparer);
		}else{
			var temp = new T[array.Length];
			array.ParallelMergeSort(comparer, 0, array.Length, temp);
		}
	}
	
	private static int threadCount;
	
	private static void ParallelMergeSort<T>(this T[] array, IComparer<T> comparer, int left, int right, T[] temp){
		if(left >= (right - 1)){
			return;
		}
		int count = right - left;
		if((threadCount < 2) && (count > 1024)){
			int middle = (left + right) >> 1;
			Interlocked.Add(ref threadCount, 2);
			var thread1 = new Thread(new ThreadStart(delegate{
				array.ParallelMergeSort(comparer, left, middle, temp);
				Interlocked.Decrement(ref threadCount);
			}));
			var thread2 = new Thread(new ThreadStart(delegate{
				array.ParallelMergeSort(comparer, middle, right, temp);
				Interlocked.Decrement(ref threadCount);
			}));
			thread1.Start();
			thread2.Start();
			thread1.Join();
			thread2.Join();
			array.Merge(comparer, left, middle, right, temp);
		}else if(count <= 16){
			array.InsertSort(comparer, left, right);
		}else{
			int middle = (left + right) >> 1;
			array.ParallelMergeSort(comparer, left, middle, temp);
			array.ParallelMergeSort(comparer, middle, right, temp);
			array.Merge(comparer, left, middle, right, temp);
		}
	}
	
	private static void InsertSort<T>(this T[] array, IComparer<T> comparer, int left, int right){
		for(int i = left + 1; i < right; i++){
			for(int j = i; j >= left + 1 && comparer.Compare(array[j - 1], array[j]) > 0; --j){
				array.Swap(j, j - 1);
			}
		}
	}
	
	private static void Swap<T>(this T[] array, int x, int y){
		T temp = array[x];
		array[x] = array[y];
		array[y] = temp;
	}
}