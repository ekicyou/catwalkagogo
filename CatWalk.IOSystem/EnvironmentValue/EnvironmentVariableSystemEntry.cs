using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	public class EnvironmentVariableSystemEntry : SystemEntry{
		public EnvironmentVariableTarget EnvironmentVariableTarget{get; private set;}

		public EnvironmentVariableSystemEntry(ISystemDirectory parent, string name, EnvironmentVariableTarget target) : base(parent, name){
			this.EnvironmentVariableTarget = target;
			this._Value = new RefreshableLazy<string>(() => Environment.GetEnvironmentVariable(this.Name, this.EnvironmentVariableTarget));
		}

		public string Name{
			get{
				return (string)this.Id;
			}
		}

		private RefreshableLazy<string> _Value;
		public string Value{
			get{
				return this._Value.Value;
			}
		}

		public override void Refresh() {
			base.Refresh();
			this._Value.Refresh();
			this.OnPropertyChanged("Value");
		}
	}
}
