/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	public class EnvironmentVariableSystemEntry : SystemEntry{
		public EnvironmentVariableTarget EnvironmentVariableTarget{get; private set;}
		public string VariableName{get; private set;}

		public EnvironmentVariableSystemEntry(ISystemDirectory parent, string name, EnvironmentVariableTarget target, string varName) : base(parent, name){
			this.EnvironmentVariableTarget = target;
			this.VariableName = varName;
		}

		public string Value{
			get{
				return Environment.GetEnvironmentVariable(this.VariableName, this.EnvironmentVariableTarget);
			}
		}
	}
}
