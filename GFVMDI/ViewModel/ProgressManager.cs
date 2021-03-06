﻿/*
	$Id: ProgressManager.cs 278 2011-08-03 09:06:12Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.Mvvm;

namespace GFV.ViewModel {
	public class ProgressManager : ViewModelBase{
		private IDictionary<object, double> jobs = new Dictionary<object, double>();
		
		#region 関数
		
		public object AddJob(){
			var id = new object();
			this.Start(id, 0);
			return id;
		}

		public void Start(object id){
			this.Start(id, 0);
		}

		public void Start(object id, double progress){
			if(this.jobs.ContainsKey(id)){
				throw new InvalidOperationException();
			}
			lock(this.jobs){
				this.jobs.Add(id, progress);
				this.OnPropertyChanged("JobCount", "IsBusy");
				this.CalculateProgressPercentage();
			}
		}
		
		public void Complete(object id){
			lock(this.jobs){
				if(!this.jobs.Remove(id)){
					throw new InvalidOperationException();
				}
				this.OnPropertyChanged("JobCount", "IsBusy");
				this.CalculateProgressPercentage();
			}
		}
		
		public void ReportProgress(object id, double progress){
			if(!this.jobs.ContainsKey(id)){
				throw new InvalidOperationException();
			}
			if((progress < 0) || (1 < progress)){
				throw new ArgumentOutOfRangeException();
			}
			lock(this.jobs){
				this.jobs[id] = progress;
				this.CalculateProgressPercentage();
			}
		}
		
		private void CalculateProgressPercentage(){
			lock(this.jobs){
				if(this.jobs.Count > 0){
					foreach(var job in this.jobs){
						if(Double.IsNaN(job.Value)){
							this._TotalProgress = Double.NaN;
							goto end;
						}else{
							this._TotalProgress += job.Value;
						}
					}
					this._TotalProgress /= this.jobs.Count;
				}else{
					this._TotalProgress = 0;
				}
			end:
				this.OnPropertyChanged("TotalProgress");
			}
		}
		
		public bool Contains(object id){
			lock(this.jobs){
				return this.jobs.ContainsKey(id);
			}
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
