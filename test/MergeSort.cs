using System;
using System.Threading;
using System.Collections.Generic;

static class Program{
	static void Main(){
		const int N = 100;
		var rnd = new Random();
		var array = new int[N];
		for(int i = 0; i < 100; i++){
			array[i] = rnd.Next();
		}
		array.MergeSort();
		foreach(var n in array){
			Console.WriteLine(n);
		}
	}
}

static class ArrayExtension{
	public static void MergeSort<T>(this T[] array){
		array.MergeSort(Comparer<T>.Default, 0, array.Length);
	}
	
	public static void MergeSort<T>(this T[] array, IComparer<T> comparer){
		array.MergeSort(comparer, 0, array.Length);
	}
	
	private static void MergeSort<T>(this T[] array, IComparer<T> comparer, int left, int right){
		if(left >= (right - 1)){
			return;
		}
		int middle = (left + right) / 2;
		array.MergeSort(comparer, left, middle);
		array.MergeSort(comparer, middle, right);
		array.Merge(comparer, left, middle, right);
	}
	
	private static void Merge<T>(this T[] array, IComparer<T> comparer, int left, int middle, int right){
		T[] temp = new T[array.Length];
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
}