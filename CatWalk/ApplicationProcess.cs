/*
	$Id: ApplicationProcess.cs 69 2010-02-06 06:03:13Z catwalk $
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
		private static IDictionary<string, Delegate> actions = new Dictionary<string, Delegate>();
		
		private const string RemoteControlerUri = "controler";
		
		#region コンストラクタ
		
		static ApplicationProcess(){
			id = Environment.UserName + ":" + System.Windows.Forms.Application.ExecutablePath.GetHashCode();
			mutex = new Mutex(false, id);
			isStarted = !(mutex.WaitOne(0, false));
			
			if(isStarted){
				controler = GetRemoteControler();
				mutex.Close();
			}else{
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
		
		public static void InvokeRemote(string name){
			if(controler == null){
				throw new InvalidOperationException();
			}
			controler.Invoke(name);
		}
		
		public static void InvokeRemote(string name, object[] args){
			if(controler == null){
				throw new InvalidOperationException();
			}
			controler.Invoke(name, args);
		}
		
		#endregion
		
		#region プロパティ
		
		public static bool IsFirst{
			get{
				return !isStarted;
			}
		}
		
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