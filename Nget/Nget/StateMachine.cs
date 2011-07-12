using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nget {
	public class StateMachine<T>{
		public State<T> Current{get; private set;}

		public StateMachine(State<T> initial){
			if(initial == null){
				throw new ArgumentNullException("initial");
			}
			this.Current = initial;
		}

		public void NextState(T input){
			this.Current = this.Current.GetNextState(input);
		}
	}

	public abstract class State<T>{
		public abstract State<T> GetNextState(T input);
	}
}
