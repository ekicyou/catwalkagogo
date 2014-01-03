using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CatWalk.Mvvm;
using CatWalk.Windows.Threading;

namespace CatWalk.Heron.ViewModel {
	public class AppViewModelBase : SynchronizeViewModel{
		private static ISynchronizeInvoke _SychronizeInvoke = new DefaultSynchronizeInvoke();
		public static ISynchronizeInvoke AppSynchronizeInvoke{
			get{
				return _SychronizeInvoke;
			}
			set{
				value.ThrowIfNull("value");
				_SychronizeInvoke = value;
			}
		}

		public AppViewModelBase() : base(_SychronizeInvoke){}
	}
}
