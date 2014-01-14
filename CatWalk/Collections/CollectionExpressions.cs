using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace CatWalk.Collections {
	public static class CollectionExpressions {
		public static Action<object, object> GetAddFunction(Type type) {
			var method = type.GetMethod("Add");
			var instance = Expression.Parameter(type.GetType(), "instance");
			var paramV = Expression.Parameter(method.GetParameters()[0].ParameterType, "item");
			var call = Expression.Call(
				instance,
				method,
				paramV);
			var lambda = Expression.Lambda<Action<object, object>>(
				call, instance, paramV).Compile();
			return lambda;
		}

		public static Action<object, object> GetRemoveFunction(Type type) {
			var method = type.GetMethod("Remove");
			var instance = Expression.Parameter(type.GetType(), "instance");
			var paramV = Expression.Parameter(method.GetParameters()[0].ParameterType, "item");
			var call = Expression.Call(
				instance,
				method,
				paramV);
			var lambda = Expression.Lambda<Action<object, object>>(
				call, instance, paramV).Compile();
			return lambda;
		}

		public static Action<object> GetClearFunction(Type type) {
			var method = type.GetMethod("Clear");
			var instance = Expression.Parameter(type.GetType(), "instance");
			var call = Expression.Call(
				instance,
				method);
			var lambda = Expression.Lambda<Action<object>>(
				call, instance).Compile();
			return lambda;
		}

	}
}
