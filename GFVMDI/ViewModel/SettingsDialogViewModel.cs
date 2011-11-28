using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk.Mvvm;
using GFV.Properties;
using GFV.Messaging;
using CatWalk;
using System.Windows.Input;

namespace GFV.ViewModel {
	[SendMessage(typeof(CloseMessage))]
	public class SettingsDialogViewModel : DataErrorInfoViewModelBase{
		private Settings _SourceSettings;
		public Settings Settings{get; private set;}

		public SettingsDialogViewModel(Settings settings){
			this._SourceSettings = settings;
			this.Settings = new Settings();
			settings.CopyTo(this.Settings);
			this.AdditionalFileFormats = new ObservableCollection<FileFormat>(
				settings.AdditionalFormatExtensions
					.EmptyIfNull()
					.Select(fmt => fmt.Split('|'))
					.Where(elms => elms.Length >= 2)
					.Select(elms => new FileFormat(elms[0], elms[1]))
			);
		}

		public ObservableCollection<FileFormat> AdditionalFileFormats{get; private set;}

		public class FileFormat{
			public string Name{get; set;}
			public string Extensions{get; set;}

			public FileFormat(){}
			public FileFormat(string name, string extensions){
				this.Name = name;
				this.Extensions = extensions;
			}
		}

		#region OKCommand

		private DelegateCommand _SubmitCommand;
		public ICommand SubmitCommand{
			get{
				return this._SubmitCommand ?? (this._SubmitCommand = new DelegateCommand(this.Submit, this.CanSubmit));
			}
		}

		public bool CanSubmit(){
			return !this.HasError;
		}

		public void Submit(){
			this.Settings.CopyTo(this._SourceSettings);
			Messenger.Default.Send(new CloseMessage(this), this);
		}

		#endregion
	}
}
