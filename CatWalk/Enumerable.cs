/*
	$Id$
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatWalk{
	public static class EnumerableExtension{
		/// <summary>
		/// �V�[�P���X�̐擪����ׂ荇����̃A�C�e�������ɕԂ��܂��B
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns>��̃A�C�e����������<see cref="Tuple"/></returns>
		public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> source){
			source.ThrowIfNull("source");
			var enumerator = source.GetEnumerator();
			if(enumerator.MoveNext()){
				var item1 = enumerator.Current;
				while(enumerator.MoveNext()){
					yield return new Tuple<T, T>(item1, enumerator.Current);
					item1 = enumerator.Current;
				}
			}else{
				yield break;
			}
		}
		
		/// <summary>
		/// let��̊֐���
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="R"></typeparam>
		/// <param name="var">�ϐ�</param>
		/// <param name="func">�֐�</param>
		/// <returns>�֐��̖߂�l</returns>
		/// <example>
		/// <code>
		/// var req = Enumerable.Range(1, 5).Let(e => {
		/// 	...
		/// });
		/// </code>
		/// </example>
		public static R Let<T, R>(this T var, Func<T, R> func){
			func.ThrowIfNull("func");
			return func(var);
		}

		/// <summary>
		/// let��̊֐���
		/// </summary>
		/// <typeparam name="T">�V�[�P���X�̗v�f�̌^</typeparam>
		/// <typeparam name="R1">�������ϐ��̌^</typeparam>
		/// <typeparam name="R2">�Ԃ��V�[�P���X�̗v�f�̌^</typeparam>
		/// <param name="source">���̓V�[�P���X</param>
		/// <param name="selector">���̓V�[�P���X�ɓK�p���ĕϐ��ɑ������ʑ��֐�</param>
		/// <param name="func">���̓V�[�P���X�̗v�f�Ƒ�����ꂽ�ϐ�����o�̓V�[�P���X�ւ̎ʑ��֐�</param>
		/// <returns>�o�̓V�[�P���X</returns>
		/// <example>
		/// <code>
		/// var req = Enumerable.Range(1, 5).Let(n => (char)n, (n, c) => String.Format("{0} => {1}", n, c));
		/// </code>
		/// </example>
		public static IEnumerable<R2> Let<T, R1, R2>(this IEnumerable<T> source, Func<T, R1> selector, Func<T, R1, R2> func){
			source.ThrowIfNull("source");
			selector.ThrowIfNull("selector");
			func.ThrowIfNull("func");
			foreach(var item in source){
				yield return func(item, selector(item));
			}
		}
		
		/// <summary>
		/// �^����ꂽ�V�[�P���X�𖳌��ɌJ��Ԃ��B
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T> Cycle<T>(params T[] source){
			source.ThrowIfNull("source");
			while(true){
				foreach(var item in source) {
					yield return item;
				}
			}
		}

		/// <summary>
		/// �S�Ă̑g�ݍ��킹��񋓂���
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns>�e�g�ݍ��킹</returns>
		public static IEnumerable<T[]> Combination<T>(this IEnumerable<T> source){
			source.ThrowIfNull("source");
			var array = source.ToArray();
			return CombinationInternal(array, new List<T>(), 0, array.Length);
		}

		/// <summary>
		/// �g�ݍ��킹��񋓂���
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="n">�I�Ԍ�</param>
		/// <returns>�e�g�ݍ��킹</returns>
		public static IEnumerable<T[]> Combination<T>(this IEnumerable<T> source, int n){
			source.ThrowIfNull("source");
			if(n < 0){
				throw new ArgumentOutOfRangeException();
			}
			var array = source.ToArray();
			return CombinationInternal(array, new List<T>(array.Length), 0, n);
		}

		private static IEnumerable<T[]> CombinationInternal<T>(T[] source, List<T> part, int index, int n){
			if(source.Length > index){
				if(part.Count < n){
					part.Add(source[index]);
					foreach(var comb in CombinationInternal(source, part, index + 1, n)){
						yield return comb;
					}
					part.RemoveAt(part.Count - 1);
				}
				foreach(var comb in CombinationInternal(source, part, index + 1, n)){
					yield return comb;
				}
			}else if(source.Length == index){
				yield return part.ToArray();
			}
		}

		/// <summary>
		/// �����񋓂���
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="n">���ׂ��</param>
		/// <returns></returns>
		public static IEnumerable<T[]> Permutation<T>(this IEnumerable<T> source, int n){
			source.ThrowIfNull("source");
			foreach(var comb in source.Combination(n)){
				foreach(var perm in PermutationAllInternal(new LinkedList<T>(comb), new T[comb.Length], 0)){
					yield return perm;
				}
			}
		}

		/// <summary>
		/// �����񋓂���
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T[]> Permutation<T>(this IEnumerable<T> source){
			source.ThrowIfNull("source");
			var list = new LinkedList<T>(source);
			return PermutationAllInternal(list, new T[list.Count], 0);
		}

		private static IEnumerable<T[]> PermutationAllInternal<T>(LinkedList<T> source, T[] part, int index){
			if(source.Count > 0){
				var node = source.First;
				LinkedListNode<T> last = null;
				while(node != null){
					part[index] = node.Value;
					source.Remove(node);

					foreach(var perm in PermutationAllInternal(source, part, index + 1)){
						yield return perm;
					}
					if(last != null){
						source.AddAfter(last, node);
					}else{
						source.AddFirst(node);
					}

					last = node;
					node = node.Next;
				}
			}else{
				yield return part;
			}
		}
		/*
		 * use Aggregate
		public static T Sum<T>(this IEnumerable<T> source, Func<T, T, T> func){
			source.ThrowIfNull("source");
			func.ThrowIfNull("func");
			T sum = default(T);
			foreach(var v in source){
				sum = func(sum, v);
			}
			return sum;
		}
		*/
		/// <summary>
		/// �V�[�P���X��null�Ȃ��̃V�[�P���X��Ԃ��B
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source){
			if(source == null){
				return new T[0];
			}else{
				return source;
			}
		}

		/// <summary>
		/// ForEach for IEnumerable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action){
			source.ThrowIfNull("source");
			action.ThrowIfNull("action");
			foreach(T value in source){
				action(value);
			}
		}

		/// <summary>
		/// ForEach for IEnumerable with index
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action){
			source.ThrowIfNull("source");
			action.ThrowIfNull("action");
			var index = 0;
			foreach(T value in source){
				action(value, index++);
			}
		}

		/// <summary>
		/// isSuccess��true��Ԃ��܂�funcs�����s��������B
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="funcs"></param>
		/// <param name="isSuccess"></param>
		/// <returns></returns>
		public static T TryThese<T>(this IEnumerable<Func<T>> funcs, Predicate<T> isSuccess){
			return TryThese(funcs, isSuccess, default(T));
		}

		/// <summary>
		/// isSuccess��true��Ԃ��܂�funcs�����s��������B�S�Ď��s�����ꍇ�ɕԂ��l��defValue�ɐݒ�ł���B
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="funcs"></param>
		/// <param name="isSuccess"></param>
		/// <param name="defValue"></param>
		/// <returns></returns>
		public static T TryThese<T>(this IEnumerable<Func<T>> funcs, Predicate<T> isSuccess, T defValue){
			isSuccess.ThrowIfNull("isSuccess");
			funcs.ThrowIfNull("funcs");
			foreach(var func in funcs){
				var result = func();
				if(isSuccess(result)){
					return result;
				}
			}
			return defValue;
		}

		public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector){
			source.ThrowIfNull("source");
			keySelector.ThrowIfNull("keySelector");
			var hash = new HashSet<TKey>();
			foreach(var item in source){
				if(hash.Add(keySelector(item))){
					yield return item;
				}
			}
		}

		/// <summary>
		/// �l�X�g���ꂽ�V�[�P���X����i���R������B
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source){
			source.ThrowIfNull("source");
			foreach(var sub in source){
				foreach(var item in sub){
					yield return item;
				}
			}
		}

		/// <summary>
		/// �l�X�g���ꂽ�V�[�P���X��S�ĕ��R������B
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable FlattenAll(this IEnumerable source){
			source.ThrowIfNull("source");
			return FlattenAllInternal(source);
		}

		private static IEnumerable FlattenAllInternal(IEnumerable source){
			foreach(var item in source){
				var sub = source as IEnumerable;
				if(sub != null){
					foreach(var item2 in FlattenAllInternal(sub)){
						yield return item;
					}
				}else{
					yield return item;
				}
			}
		}

		/// <summary>
		/// �l�X�g���ꂽ�V�[�P���X���w�肳�ꂽ�K�w�����R������B
		/// </summary>
		/// <param name="source"></param>
		/// <param name="depth"></param>
		/// <returns></returns>
		public static IEnumerable FlattenSome(this IEnumerable source, int depth){
			source.ThrowIfNull("source");
			depth.ThrowIfOutOfRange(0, "depth");
			return FlattenSomeInternal(source, depth);
		}

		private static IEnumerable FlattenSomeInternal(IEnumerable source, int depth){
			foreach(var item in source){
				if(depth > 0){
					var sub = item as IEnumerable;
					if(sub != null){
						foreach(var item2 in FlattenAllInternal(sub)){
							yield return item;
						}
					}else{
						yield return item;
					}
				}else{
					yield return item;
				}
			}
		}
	}
}