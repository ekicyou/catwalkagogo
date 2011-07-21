using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twitman.Controls;
using CatWalk;
using Twitman.IOSystem;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Twitman.Screens {
	public class AccountScreen : DirectoryScreen{
		private CancellationTokenSource _VerifyTokenSource = new CancellationTokenSource();
		private Task _VerifyTask;

		public AccountSystemDirectory AccountDirectory{
			get{
				return (AccountSystemDirectory)this.Directory;
			}
		}

		public AccountScreen(AccountSystemDirectory dir) : base(dir){
			this.MessageLabel.Text = new ConsoleRun(new []{
				new ConsoleText(dir.DisplayPath, ConsoleColor.Magenta)
			});
		}

		protected override void OpenDirectory(CatWalk.IOSystem.ISystemDirectory dir) {
			var tlDir = dir as TimelineSystemDirectory;
			if(tlDir != null){
				ConsoleApplication.SetScreen(new TimelineScreen(tlDir), true);
			}else{
				base.OpenDirectory(dir);
			}
		}

		protected override void OnCancelKeyPress(ConsoleCancelEventArgs e) {
			if(this._VerifyTask.Status == TaskStatus.Running || this._VerifyTask.Status == TaskStatus.WaitingToRun){
				this._VerifyTokenSource.Cancel();
			}
			e.Cancel = true;
		}

		private void VerifyCredential(){
			var ui = TaskScheduler.Current;

			this.PromptBox.Text = new ConsoleRun("Verifying account credential...", ConsoleColor.Cyan);
			this._VerifyTask = new Task(new Action(delegate{
				this.AccountDirectory.Account.VerifyCredential(this._VerifyTokenSource.Token);
				this.AccountDirectory.AccountInfo.Name = this.AccountDirectory.Account.User.Name;
			}), this._VerifyTokenSource.Token);
			this._VerifyTask.ContinueWith(task => {
				this.PromptBox.Text = new ConsoleRun(task.Exception.InnerExceptions[0].Message, ConsoleColor.Red);
				Console.ReadKey();
				ConsoleApplication.RestoreScreen();
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);
			this._VerifyTask.ContinueWith(task => {
				this.PromptBox.Text = new ConsoleRun("Cancelled", ConsoleColor.Yellow);
				Console.ReadKey();
				ConsoleApplication.RestoreScreen();
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnCanceled, ui);
			this._VerifyTask.ContinueWith(task => {
				this.PromptBox.Text = new ConsoleRun("Ready!", ConsoleColor.Green);
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, ui);

			this._VerifyTask.Start();
		}

		protected override void OnAttach(EventArgs e) {
			if(!this.AccountDirectory.Account.IsVerified){
				this.VerifyCredential();
			}

			base.OnAttach(e);
		}

		protected override void OpenMenuItem(ConsoleMenuItem item) {
			if(this.AccountDirectory.Account.IsVerified){
				base.OpenMenuItem(item);
			}
		}
	}
}
