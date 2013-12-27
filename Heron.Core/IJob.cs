using System;
namespace CatWalk.Heron {
	public interface IJob : IProgress<double> {
		double Progress { get; set; }
		void ReportCancelled();
		JobStatus Status { get; }
	}

	public enum JobStatus {
		Pending,
		Running,
		Cancelled,
		Completed,
		Failed,
	}
}
