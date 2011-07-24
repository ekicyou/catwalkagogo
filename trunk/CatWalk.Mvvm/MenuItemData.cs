using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CatWalk.Windows.ViewModel {
	public class MenuItemData : IMenuItem{
		public MenuItemData(){
		}
		public MenuItemData(string headerText) : this(headerText, null, null){
		}
		public MenuItemData(string headerText, ICommand command) : this(headerText, command, null){
		}
		public MenuItemData(string headerText, ICommand command, object commandParameter){
			this.HeaderText = headerText;
			this.Command = command;
			this.CommandParameter = commandParameter;
		}

		public string HeaderText{get; set;}
		public ICommand Command {get; set;}
		public object CommandParameter {get; set;}
		public string InputGestureText{get; set;}
	}
}
