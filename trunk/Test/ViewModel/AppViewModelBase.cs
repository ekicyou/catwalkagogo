using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk;
using CatWalk.Mvvm;

namespace Test.ViewModel {
	public class AppViewModelBase : SynchronizeViewModel{
		public AppViewModelBase()
			: base(AppSynchronizeInvoke) {

		}

		private static ISynchronizeInvoke _SynchronizeInvoke = new DefaultSynchronizeInvoke();
		private static Lazy<ISynchronizeInvoke> _SynchronizeInvokeFactory = new Lazy<ISynchronizeInvoke>(() => _SynchronizeInvoke);
		public static ISynchronizeInvoke AppSynchronizeInvoke{
			get {
				return _SynchronizeInvokeFactory.Value;
			}
			set {
				value.ThrowIfNull("value");
				if(_SynchronizeInvokeFactory.IsValueCreated) {
					throw new InvalidOperationException("View models has already been created.");
				}
				_SynchronizeInvoke = value;
			}
		}
	}
}
