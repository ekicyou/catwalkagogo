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
using System.Threading;
using System.Threading.Tasks;

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

		public static readonly DependencyProperty TaskProperty = DependencyProperty.Register("Task", typeof(Task), typeof(ResultTab));
		public Task Task{
			get{
				return (Task)this.GetValue(TaskProperty);
			}
			set{
				this.SetValue(TaskProperty, value);
			}
		}

		public static readonly DependencyProperty CancellationTokenSourceProperty = DependencyProperty.Register("CancellationTokenSource", typeof(CancellationTokenSource), typeof(ResultTab));
		public CancellationTokenSource CancellationTokenSource{
			get{
				return (CancellationTokenSource)this.GetValue(CancellationTokenSourceProperty);
			}
			set{
				this.SetValue(CancellationTokenSourceProperty, value);
			}
		}

		public ResultTab(){
		}

		public ResultTab(ResultBase result, Task task, CancellationTokenSource tokenSource){
			this.Result = result;
			this.Task = task;
			this.CancellationTokenSource = tokenSource;
		}
	}

	public abstract class ResultBase{
		public SearchCondition SearchCondition{get; private set;}

		public ResultBase(SearchCondition cond){
			this.SearchCondition = cond;
		}
	}
}