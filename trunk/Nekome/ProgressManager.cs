/*
	$Id$
*/
using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;

namespace Nekome{
	public class ProgressManager : DependencyObject{
		private IDictionary<object, double> jobs = new Dictionary<object, double>();
		
		#region 関数
		
		public void Start(object id){
			this.Start(id, 0);
		}
		public void Start(object id, int progress){
			if(this.jobs.ContainsKey(id)){
				throw new InvalidOperationException();
			}
			this.jobs.Add(id, progress);
			this.CalculateProgressPercentage();
		}
		
		public void Complete(object id){
			if(!this.jobs.Remove(id)){
				throw new InvalidOperationException();
			}
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
				double sum = 0;
				foreach(var p in this.jobs.Values){
					sum += p;
				}
				this.ProgressPercentage = sum / this.jobs.Count;
				if(!this.IsBusy){
					this.IsBusy = true;
				}
			}else{
				this.ProgressPercentage = 0;
				if(this.IsBusy){
					this.IsBusy = false;
				}
			}
			this.OnProgressChanged(this.ProgressPercentage, this.IsBusy);
		}
		
		#endregion
		
		#region プロパティ
		
		public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(ProgressManager));
		public bool IsBusy{
			get{
				return (bool)this.GetValue(IsBusyProperty);
			}
			private set{
				this.SetValue(IsBusyProperty, value);
			}
		}
		
		public static readonly DependencyProperty ProgressPercentageProperty = DependencyProperty.Register("ProgressPercentage", typeof(double), typeof(ProgressManager));
		public double ProgressPercentage{
			get{
				return (double)this.GetValue(ProgressPercentageProperty);
			}
			private set{
				this.SetValue(ProgressPercentageProperty, value);
			}
		}

		public static readonly DependencyProperty ProgressMessageProperty = DependencyProperty.Register("ProgressMessage", typeof(string), typeof(ProgressManager));
		public string ProgressMessage{
			get {
				return (string)this.GetValue(ProgressMessageProperty);
			}
			set{
				this.SetValue(ProgressMessageProperty, value);
			}
		}
		
		#endregion
		
		#region イベント
		
		
		private void OnProgressChanged(double progress, bool isBusy){
			if(this.ProgressChanged != null){
				this.OnProgressChanged(new ProgressPercentageChangedEventArgs(progress, isBusy));
			}
		}
		
		protected virtual void OnProgressChanged(ProgressPercentageChangedEventArgs e){
			if(this.ProgressChanged != null){
				this.ProgressChanged(this, e);
			}
		}
		
		public event ProgressPercentageChangedEventHandler ProgressChanged;
		
		#endregion
	}

	public delegate void ProgressPercentageChangedEventHandler(object sender, ProgressPercentageChangedEventArgs e);

	public class ProgressPercentageChangedEventArgs : EventArgs{
		public double ProgressPercentage{get; private set;}
		public bool IsBusy{get; private set;}

		public ProgressPercentageChangedEventArgs(double progress, bool isBusy){
			this.ProgressPercentage = progress;
			this.IsBusy = isBusy;
		}
	}
}