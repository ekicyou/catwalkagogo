﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk {
	public class WeakReference<T> : WeakReference, IEquatable<WeakReference<T>> where T : class {
		public WeakReference(T target) : base(target) { }

		public new T Target {
			get {
				return (T)this.Target;
			}
		}

		public bool Equals(WeakReference<T> reference) {
			return this.Target.Equals(reference.Target);
		}

		public override bool Equals(object obj) {
			var reference = obj as WeakReference<T>;
			if(reference == null) {
				return false;
			}
			return this.Target.Equals(reference.Target);
		}

		public override int GetHashCode() {
			return this.Target.GetHashCode();
		}

		// Casting
		public static implicit operator T(WeakReference<T> reference) {
			return reference.Target;
		}

		public static bool operator ==(WeakReference<T> a, WeakReference<T> b){
			// If both are null, or both are same instance, return true.
			if(System.Object.ReferenceEquals(a, b)){
				return true;
			}

			// If one is null, but not both, return false.
			if(((object)a == null) || ((object)b == null)){
				return false;
			}

			// Return true if the fields match:
			return (a.Target == b.Target);
		}

		public static bool operator !=(WeakReference<T> a, WeakReference<T> b){
			return !(a == b);
		}
	}
}
