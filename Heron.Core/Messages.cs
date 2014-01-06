﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;

namespace CatWalk.Heron {
	public abstract class MessageBase {
		public object Sender {
			get;
			private set;
		}
		public MessageBase(object sender) {
			this.Sender = sender;
		}
	}

	public static class Messages {
		public class RequestPropertyMessage<T> : MessageBase{
			private static Dictionary<Tuple<Type, string>, Func<object, T>> _Cache = new Dictionary<Tuple<Type,string>, Func<object, T>>();

			public T Value{get; set;}
			public string PropertyName{get; private set;}

			public RequestPropertyMessage(object sender, string propName) : base(sender){
				propName.ThrowIfNullOrEmpty("propName");
			}

			public void AssignToMessage(object obj) {
				obj.ThrowIfNull("obj");
				var type = obj.GetType();
				Func<object, T> call;
				if(!_Cache.TryGetValue(Tuple.Create(type, this.PropertyName), out call)) {
					var expObj = Expression.Parameter(type);
					var expProp = Expression.Property(expObj, this.PropertyName);
					var expGet = Expression.Lambda<Func<object, T>>(
						expProp,
						expObj
					);
					call = expGet.Compile();
					_Cache[Tuple.Create(type, this.PropertyName)] = call;
				}
				this.Value = call(obj);
			}
		}

		public class SetPropertyMessage<T> : MessageBase {
			private static Dictionary<Tuple<Type, string>, Action<object, T>> _Cache = new Dictionary<Tuple<Type, string>, Action<object, T>>();

			public T Value {
				get;
				set;
			}
			public string PropertyName {
				get;
				private set;
			}

			public SetPropertyMessage(object sender, string propName)
				: base(sender) {
				propName.ThrowIfNullOrEmpty("propName");
			}

			public void AssignToObject(object obj) {
				obj.ThrowIfNull("obj");
				var type = obj.GetType();
				Action<object, T> call;
				if(!_Cache.TryGetValue(Tuple.Create(type, this.PropertyName), out call)) {
					var expObj = Expression.Parameter(type);
					var expProp = Expression.Property(expObj, this.PropertyName);
					var expValue = Expression.Parameter(typeof(T), this.PropertyName);
					var expSet = Expression.Lambda<Action<object, T>>(
						Expression.Assign(expProp, expValue),
						expObj,
						expValue
					);
					call = expSet.Compile();
					_Cache[Tuple.Create(type, this.PropertyName)] = call;
				}
				call(obj, this.Value);
			}
		}
	}
}