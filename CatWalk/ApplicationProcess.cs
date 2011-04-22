/*
	$Id$
*/
using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Runtime.Serialization;

namespace CatWalk{
	/// <summary>
	/// 二重起動防止・プロセス間通信クラス
	/// </summary>
	public static class ApplicationProcess{
		private static Mutex mutex;
		private static bool isStarted;
		private static IRemoteControler controler = null;
		private static IChannel serverChannel = null;
		private static string id;
		private static IDictionary<string, Delegate> actions;
		
		private const string RemoteControlerUri = "controler";
		
		#region コンストラクタ
		
		static ApplicationProcess(){
			id = Environment.UserName + "@" + Assembly.GetEntryAssembly().Location.ToLower().GetHashCode();
			mutex = new Mutex(false, id);
			isStarted = !(mutex.WaitOne(0, false));
			
			if(isStarted){
				controler = GetRemoteControler();
				mutex.Close();
			}else{
				actions = new Dictionary<string, Delegate>();
				RegisterRemoteControler(typeof(RemoteControler));
			}
		}
		
		#endregion
		
		#region 関数
		
		private static IRemoteControler GetRemoteControler(){
			IpcClientChannel clientChannel = new IpcClientChannel();
			ChannelServices.RegisterChannel(clientChannel, true);
			return (IRemoteControler)Activator.GetObject(typeof(IRemoteControler), "ipc://" + id + "/" + RemoteControlerUri);
		}
		
		private static void RegisterRemoteControler(Type type){
			if(serverChannel == null && !(isStarted)){
				// IServerChannelSinkProvider初期化
				BinaryServerFormatterSinkProvider sinkProvider = new BinaryServerFormatterSinkProvider();
				sinkProvider.TypeFilterLevel = TypeFilterLevel.Full;
				
				// IpcServerChannel初期化
				serverChannel = new IpcServerChannel("ipc", id, sinkProvider);
				ChannelServices.RegisterChannel(serverChannel, true);
				
				// リモートオブジェクト登録
				RemotingConfiguration.RegisterWellKnownServiceType(type, RemoteControlerUri, WellKnownObjectMode.Singleton);
			}else{
				throw new InvalidOperationException();
			}
		}
		
		/// <summary>
		/// プロセス間通信で<see cref="Actions"/>に登録した関数を実行。
		/// </summary>
		/// <param name="name">関数名</param>
		/// <seealso cref="Actions"/>
		public static void InvokeRemote(string name){
			if(controler == null){
				throw new InvalidOperationException();
			}
			controler.Invoke(name);
		}
		
		/// <summary>
		/// プロセス間通信で<see cref="Actions"/>に登録した関数を実行。
		/// </summary>
		/// <param name="name">関数名</param>
		/// <param name="args">引数</param>
		/// <seealso cref="Actions"/>
		public static void InvokeRemote(string name, params object[] args){
			if(controler == null){
				throw new InvalidOperationException();
			}
			controler.Invoke(name, args);
		}
		
		#endregion
		
		#region プロパティ
		
		/// <summary>
		/// 現在のプロセスが一つ目かどうかを取得。
		/// </summary>
		public static bool IsFirst{
			get{
				return !isStarted;
			}
		}
		
		/// <summary>
		/// プロセス間通信で実行する関数。
		/// キーに呼び出しに使用する関数名、値に<see cref="System.Delegate"/>を指定する。
		/// </summary>
		public static IDictionary<string, Delegate> Actions{
			get{
				return actions;
			}
		}
		
		#endregion
		
		#region クラス
		
		private interface IRemoteControler{
			void Invoke(string name);
			void Invoke(string name, object[] args);
		}
		
		private class RemoteControler : MarshalByRefObject, IRemoteControler{
			public void Invoke(string name){
				ApplicationProcess.actions[name].DynamicInvoke(null);
			}
			
			public void Invoke(string name, object[] args){
				ApplicationProcess.actions[name].DynamicInvoke(args);
			}
		}
		
		#endregion
	}
}