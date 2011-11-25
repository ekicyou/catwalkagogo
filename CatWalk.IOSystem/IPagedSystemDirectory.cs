/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	public interface IPagedSystemDirectory : ISystemDirectory{
		void MoveNextPage();
		void MovePreviousPage();
		void ResetPage();
		int Page{get;set;}
		int PageCount{get;}
	}
}
