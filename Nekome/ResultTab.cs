/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using Nekome.Search;
using System.Windows;

namespace Nekome{
	public class ResultTab : DependencyObject{
		public static readonly DependencyProperty ResultProperty = DependencyProperty.Register("Result", typeof(ResultBase), typeof(ResultTab));
		public ResultBase Result{
			get{
				return (ResultBase)this.GetValue(ResultProperty);
			}
			set{
				this.SetValue(ResultProperty, value);
			}
		}

		public static readonly DependencyProperty WorkerProperty = DependencyProperty.Register("Worker", typeof(FileListWorker), typeof(ResultTab));
		public FileListWorker Worker{
			get{
				return (FileListWorker)this.GetValue(WorkerProperty);
			}
			set{
				this.SetValue(WorkerProperty, value);
			}
		}
		
		public ResultTab(){
		}

		public ResultTab(ResultBase result, FileListWorker worker){
			this.Result = result;
			this.Worker = worker;
		}
	}

	public abstract class ResultBase{
		public SearchCondition SearchCondition{get; private set;}

		public ResultBase(SearchCondition cond){
			this.SearchCondition = cond;
		}
	}
}