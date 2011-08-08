using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk.Mvvm;
using GFV.Properties;

namespace GFV.ViewModel {
	public class SettingDialogViewModel : ViewModelBase{
		public Settings Settings{get; private set;}

		public SettingDialogViewModel(Settings settings){
			this.Settings = new Settings();
			settings.CopyTo(this.Settings);
		}
	}
}
