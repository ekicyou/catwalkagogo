using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using CatWalk;

namespace TwEditman {
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application {
		private class CommandLineOption{
			[CommandLineOptionName("replyToId")]
			public ulong ReplyToId{get; set;}
		}

		protected override void OnStartup(StartupEventArgs e) {
			var initialStatus = Console.In.ReadToEnd();
			var cmdParser = new CommandLineParser("-", " ", StringComparer.Ordinal);
			var option = cmdParser.Parse<CommandLineOption>();
			base.OnStartup(e);
		}
	}
}
