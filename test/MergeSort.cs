using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

static class Program{
	static void Main(){
		const int N = 10000000;
		var sw = new Stopwatch();
		var rnd = new Random();
		var array = new int[N];
		
		for(int i = 0; i < N; i++){
			array[i] = rnd.Next();
		}
		Console.Write("ShellSort: ");
		sw.Reset();
		sw.Start();
		array.ShellSort();
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
		Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
		
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
		Console.Write("HeapSort: ");
		sw.Reset();
		sw.Start();
		array.HeapSort();
		sw.Stop();
		Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
		
		foreach(var n in array){
		//	Console.WriteLine(n);
		}
		Console.WriteLine(array.Check());
	}
	
	private static bool Check<T>(this T[] array){
		var comparer = Comparer<T>.Default;
		int max = array.Length - 1;
		bool check = true;
		for(int i = 0; i < max; i++){
			check = check && (comparer.Compare(array[i], array[i + 1]) < 0);
		}
		return check;
	}
}

static class ArrayExtension{
	public static void StableSort<T>(this T[] array){
		array.MergeSort(Comparer<T>.Default);
	}
	
	public static void StableSort<T>(this T[] array, IComparer<T> comparer){
		array.MergeSort(comparer);
	}
	
	public static void MergeSort<T>(this T[] array){
		array.MergeSort(Comparer<T>.Default);
	}
	
	public static void MergeSort<T>(this T[] array, IComparer<T> comparer){
		array.MergeSort(comparer, 0, array.Length, new T[array.Length]);
	}
	
	private const int MergeSortThreshold = 16;
	
	private static void MergeSort<T>(this T[] array, IComparer<T> comparer, int left, int right, T[] temp){
		if(left >= (right - 1)){
			return;
		}
		int count = right - left;
		if(count <= MergeSortThreshold){
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
	
	private static int threadCount = 1;
	
	private static void ParallelMergeSort<T>(this T[] array, IComparer<T> comparer, int left, int right, T[] temp){
		if(left >= (right - 1)){
			return;
		}
		int count = right - left;
		if(count <= MergeSortThreshold){
			array.InsertSort(comparer, left, right);
		}else if(threadCount < 2){
			int middle = (left + right) >> 1;
			Interlocked.Increment(ref threadCount);
			var thread1 = new Thread(new ThreadStart(delegate{
				array.ParallelMergeSort(comparer, left, middle, temp);
				Interlocked.Decrement(ref threadCount);
			}));
			thread1.Start();
			array.ParallelMergeSort(comparer, middle, right, temp);
			thread1.Join();
			array.Merge(comparer, left, middle, right, temp);
		}else{
			int middle = (left + right) >> 1;
			array.ParallelMergeSort(comparer, left, middle, temp);
			array.ParallelMergeSort(comparer, middle, right, temp);
			array.Merge(comparer, left, middle, right, temp);
		}
	}
	
	public static void InsertSort<T>(this T[] array){
		array.InsertSort(Comparer<T>.Default, 0, array.Length);
	}
	
	private static void InsertSort<T>(this T[] array, IComparer<T> comparer, int left, int right){
		for(int i = left + 1; i < right; i++){
			for(int j = i; j >= left + 1 && comparer.Compare(array[j - 1], array[j]) > 0; --j){
				Swap(ref array[j], ref array[j - 1]);
			}
		}
	}
	
	public static void ShellSort<T>(this T[] array){
		array.ShellSort(Comparer<T>.Default, 0, array.Length);
	}
	
	private static void ShellSort<T>(this T[] array, IComparer<T> comparer, int left, int right){
		int j, h;
		T temp;
		h = left + 1;
		while (h < right){
		    h = (h * 3) + 1;
		}
		int left2 = left + 1;
		while(h > left2){
			h = h / 3;
			for(int i = h; i < right; i++){
				temp = array[i];
				j = i - h;
				while(comparer.Compare(temp, array[j]) < 0){
					array[j + h] = array[j];
					j = j - h;
					if(j < left){
						break;
					}
				}
				array[j + h] = temp;
			}
		}
    }
	
	private static void Swap<T>(ref T x, ref T y){
		T temp = x;
		x = y;
		y = temp;
	}
	
	public static void HeapSort<T>(this T[] array){
		array.HeapSort(Comparer<T>.Default, 0, array.Length);
	}
	/*
	private static void HeapSort<T>(this T[] array, IComparer<T> comparer, int left, int right){
		for(int i = right - 1; i > left; i--){
			array.MakeHeap(comparer, left, i + 1);
			Swap(ref array[left], ref array[i]);
		}
	}
	
	private static void MakeHeap<T>(this T[] array, IComparer<T> comparer, int i, int right){
		int max = right - 1;
		int j = (i << 1) + 1; // leaf
		int k = j + 1; // leaf
		if(max < j){
			return;
		}
		if(j == max){
			if(comparer.Compare(array[i], array[j]) < 0){
				Swap(ref array[i], ref array[j]);
			}
			return;
		}
		array.MakeHeap(comparer, j, right);
		array.MakeHeap(comparer, k, right);
		int d = (comparer.Compare(array[j], array[k]) > 0) ? j : k;
		if(comparer.Compare(array[i], array[d]) < 0){
			Swap(ref array[i], ref array[d]);
			//array.MakeHeap(comparer, d, right);
		}
	}
    void sort() {					// ヒープソート(昇順)
	for (int i = (length - 2) / 2; i >= 0; i--) {
	    downheap(i, length - 1);
	}
	for (int i = length - 1; i > 0; i--) {
	    swap(0, i);
	    downheap(0, i - 1);
	}
    }
    void downheap(int k, int r) {
	int j, v;
	v = a[k];
	while (true) {
	    j = 2 * k + 1;
	    if (j > r) break;
	    if (j != r) {
		if (a[j + 1] > a[j]) {
		    j = j + 1;
		}
	    }
	    if (v >= a[j]) break;
	    a[k] = a[j];
	    k = j;
	}
	a[k] = v;
    }

	*/
	private static void HeapSort<T>(this T[] array, IComparer<T> comparer, int left, int right){
		int max = right - 1;
		for(int i = (max - 1) >> 1; i >= left; i--){
			array.MakeHeap(comparer, i, max);
		}
		for(int i = max; i > left; i--){
			Swap(ref array[left], ref array[i]);
			array.MakeHeap(comparer, left, i - 1);
		}
	}
	
	private static void MakeHeap<T>(this T[] array, IComparer<T> comparer, int i, int right){
		T v = array[i];
		while(true){
			int j = (i << 1) + 1;
			if(j > right){
				break;
			}
			if(j != right){
				if(comparer.Compare(array[j + 1], array[j]) > 0){
					j = j + 1;
				}
			}
			if(comparer.Compare(v, array[j]) >= 0){
				break;
			}
			array[i] = array[j];
			i = j;
		}
		array[i] = v;
	}
}