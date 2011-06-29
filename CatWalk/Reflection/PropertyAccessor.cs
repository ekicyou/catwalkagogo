using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Reflection {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	public class PropertyAccesor {
		private Func<object, object> _Getter;
		private Action<object, object> _Setter;

		public PropertyAccesor(PropertyInfo property) {
			_Getter = InitializeGet(property);
			_Setter = InitializeSet(property, false);
		}

		public PropertyAccesor(PropertyInfo property, BindingFlags bindingFlags) {
			_Getter = InitializeGet(property);
			_Setter = InitializeSet(property, (bindingFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
		}


		public object Get(object instance) {
			return _Getter(instance);
		}

		public void Set(object instance, object value) {
			_Setter(instance, value);
		}

		private static Action<object, object> InitializeSet(PropertyInfo property, bool includeNonPublic) {
			var instance = Expression.Parameter(typeof(object), "instance");
			var value = Expression.Parameter(typeof(object), "value");

			// value as T is slightly faster than (T)value, so if it's not a value type, use that
			UnaryExpression instanceCast;
			if (property.DeclaringType.IsValueType)
				instanceCast = Expression.Convert(instance, property.DeclaringType);
			else
				instanceCast = Expression.TypeAs(instance, property.DeclaringType);

			UnaryExpression valueCast;
			if (property.PropertyType.IsValueType)
				valueCast = Expression.Convert(value, property.PropertyType);
			else
				valueCast = Expression.TypeAs(value, property.PropertyType);

			var call = Expression.Call(instanceCast, property.GetSetMethod(includeNonPublic), valueCast);

			return Expression.Lambda<Action<object, object>>(call, new[] { instance, value }).Compile();
		}

		private static Func<object, object> InitializeGet(PropertyInfo property) {
			var instance = Expression.Parameter(typeof(object), "instance");
			UnaryExpression instanceCast;
			if (property.DeclaringType.IsValueType)
				instanceCast = Expression.Convert(instance, property.DeclaringType);
			else
				instanceCast = Expression.TypeAs(instance, property.DeclaringType);

			var call = Expression.Call(instanceCast, property.GetGetMethod());
			var typeAs = Expression.TypeAs(call, typeof(object));

			return Expression.Lambda<Func<object, object>>(typeAs, instance).Compile();
		}
	}

	public class PropertyAccesor<TInstance> {
		public Func<TInstance, object> _Getter;
		public Action<TInstance, object> _Setter;

		public PropertyAccesor(PropertyInfo property) {
			_Getter = InitializeGet(property);
			_Setter = InitializeSet(property, false);
		}

		public PropertyAccesor(PropertyInfo property, BindingFlags bindingFlags) {
			_Getter = InitializeGet(property);
			_Setter = InitializeSet(property, (bindingFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
		}

		public object Get(TInstance instance) {
			return _Getter(instance);
		}

		public void Set(TInstance instance, object value) {
			_Setter(instance, value);
		}

		private static Action<TInstance, object> InitializeSet(PropertyInfo property, bool includeNonPublic) {
			var instance = Expression.Parameter(typeof(TInstance), "instance");
			var value = Expression.Parameter(typeof(object), "value");
			UnaryExpression valueCast;
			if (property.PropertyType.IsValueType)
				valueCast = Expression.Convert(value, property.PropertyType);
			else
				valueCast = Expression.TypeAs(value, property.PropertyType);

			var call = Expression.Call(instance, property.GetSetMethod(includeNonPublic), valueCast);

			return Expression.Lambda<Action<TInstance, object>>(call, new[] { instance, value }).Compile();
		}

		private static Func<TInstance, object> InitializeGet(PropertyInfo property) {
			var instance = Expression.Parameter(typeof(TInstance), "instance");
			var call = Expression.Call(instance, property.GetGetMethod());
			var typeAs = Expression.TypeAs(call, typeof(object));
			return Expression.Lambda<Func<TInstance, object>>(typeAs, instance).Compile();
		}
	}

	public class PropertyAccesor<TInstance, P> {
		public Func<TInstance, P> _Getter;
		public Action<TInstance, P> _Setter;

		public PropertyAccesor(PropertyInfo property) {
			_Getter = InitializeGet(property);
			_Setter = InitializeSet(property, false);
		}

		public PropertyAccesor(PropertyInfo property, BindingFlags bindingFlags) {
			_Getter = InitializeGet(property);
			_Setter = InitializeSet(property, (bindingFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
		}

		public P Get(TInstance instance) {
			return _Getter(instance);
		}

		public void Set(TInstance instance, P value) {
			_Setter(instance, value);
		}

		private static Action<TInstance, P> InitializeSet(PropertyInfo property, bool includeNonPublic) {
			var instance = Expression.Parameter(typeof(TInstance), "instance");
			var value = Expression.Parameter(typeof(P), "value");
			var call = Expression.Call(instance, property.GetSetMethod(includeNonPublic), value);

			return Expression.Lambda<Action<TInstance, P>>(call, new[] { instance, value }).Compile();

			// roughly looks like Action<T,P> a = new Action<T,P>((instance,value) => instance.set_Property(value));
		}

		private static Func<TInstance, P> InitializeGet(PropertyInfo property) {
			var instance = Expression.Parameter(typeof(TInstance), "instance");
			return Expression.Lambda<Func<TInstance, P>>(Expression.Call(instance, property.GetGetMethod()), instance).Compile();

			// roughly looks like Func<T,P> getter = instance => return instance.get_Property();
		}
	}
}
