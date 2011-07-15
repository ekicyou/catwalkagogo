using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CatWalk.Windows.ViewModel {
	public interface IMenuItem{
		string HeaderText{get;}
		ICommand Command{get;}
		object CommandParameter{get;}
		string InputGestureText{get;}
	}
}
