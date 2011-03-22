/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFV.ViewModel {
	public class ProgressManager : ViewModelBase{
		private IDictionary<object, double> jobs = new Dictionary<object, double>();
		
		#region 関数
		
		public object AddJob(){
			var id = new object();
			this.AddJob(id, 0);
			return id;
		}

		public void AddJob(object id){
			this.AddJob(id, 0);
		}

		public void AddJob(object id, double progress){
			if(this.jobs.ContainsKey(id)){
				throw new InvalidOperationException();
			}
			this.jobs.Add(id, progress);
			this.OnPropertyChanged("JobCount", "IsBusy");
			this.CalculateProgressPercentage();
		}
		
		public void Complete(object id){
			if(!this.jobs.Remove(id)){
				throw new InvalidOperationException();
			}
			this.OnPropertyChanged("JobCount", "IsBusy");
			this.CalculateProgressPercentage();
		}
		
		public void ReportProgress(object id, double progress){
			if(!this.jobs.ContainsKey(id)){
				throw new InvalidOperationException();
			}
			if((progress < 0) || (1 < progress)){
				throw new ArgumentOutOfRangeException();
			}
			this.jobs[id] = progress;
			this.CalculateProgressPercentage();
		}
		
		private void CalculateProgressPercentage(){
			if(this.jobs.Count > 0){
				this._TotalProgress = this.jobs.Sum(job => job.Value) / this.jobs.Count;
			}else{
				this._TotalProgress = 0;
			}
			this.OnPropertyChanged("TotalProgress");
		}
		
		#endregion
		
		#region プロパティ
		
		public bool IsBusy{
			get{
				return (this.jobs.Count > 0);
			}
		}

		private double _TotalProgress;
		public double TotalProgress{
			get{
				return this._TotalProgress;
			}
		}

		public int JobCount{
			get{
				return this.jobs.Count;
			}
		}

		#endregion
	}
}
