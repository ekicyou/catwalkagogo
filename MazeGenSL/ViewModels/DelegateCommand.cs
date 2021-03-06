﻿/*
	$Id: Account.cs 51 2010-01-28 21:55:33Z catwalk $
*/
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MazeGenSL.ViewModels{
	/// <summary>
	/// <para>
	///     This class allows delegating the commanding logic to methods passed as parameters,
	///     and enables a View to bind commands to objects that are not part of the element tree.
	/// </para>
	/// <para>
	///     このクラスはパラメータとして渡されたメソッドへのコマンドのロジックの委譲を実現します。
	///     また、Viewが要素ツリーに含まれないオブジェクトにコマンドをバインドすることを可能にします。
	/// </para>
	/// </summary>
	public class DelegateCommand : ICommand{
		#region Data

		private readonly Action _executeMethod = null;
		private readonly Func<bool> _canExecuteMethod = null;
		private bool _isAutomaticRequeryDisabled = false;
		private LinkedList<WeakReference> _canExecuteChangedHandlers;

		#endregion
	
		#region Constructors

		public DelegateCommand(Action executeMethod) : this(executeMethod, null, false){}
		public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod) : this(executeMethod, canExecuteMethod, false){}
		public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod, bool isAutomaticRequeryDisabled){
			if(executeMethod == null) {
				throw new ArgumentNullException("executeMethod");
			}

			_executeMethod = executeMethod;
			_canExecuteMethod = canExecuteMethod;
			_isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// <para>
		///     Method to determine if the command can be executed
		/// </para>
		/// <para>
		///     コマンドが実行可能かを返す
		/// </para>
		/// </summary>
		public bool CanExecute() {
			if(_canExecuteMethod != null) {
				return _canExecuteMethod();
			}
			return true;
		}

		/// <summary>
		/// <para>
		///     Execution of the command
		/// </para>
		/// <para>
		///     コマンドを実行する
		/// </para>
		/// </summary>
		public void Execute() {
			if(_executeMethod != null) {
				_executeMethod();
			}
		}

		/// <summary>
		/// <para>
		///     Property to enable or disable CommandManager's automatic requery on this command
		/// </para>
		/// <para>
		///     CommandManagerのこのコマンドに対する自動再要求の有効/無効を設定する
		/// </para>
		/// </summary>
		public bool IsAutomaticRequeryDisabled {
			get {
				return _isAutomaticRequeryDisabled;
			}
			set {
				if(_isAutomaticRequeryDisabled != value) {
					if(value) {
						CommandManagerHelper.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
					} else {
						CommandManagerHelper.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);
					}
					_isAutomaticRequeryDisabled = value;
				}
			}
		}

		/// <summary>
		/// <para>
		///     Raises the CanExecuteChaged event
		/// </para>
		/// <para>
		///     CanExecuteChangedイベントを発生させる
		/// </para>
		/// </summary>
		public void RaiseCanExecuteChanged() {
			OnCanExecuteChanged();
		}

		/// <summary>
		/// <para>
		///     Protected virtual method to raise CanExecuteChanged event
		/// </para>
		/// <para>
		///     CanExecuteChangedイベントを発生させるprotected virtualメソッド
		/// </para>
		/// </summary>
		protected virtual void OnCanExecuteChanged() {
			CommandManagerHelper.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
		}

		#endregion

		#region ICommand Members

		/// <summary>
		/// <para>
		///     ICommand.CanExecuteChanged implementation
		/// </para>
		/// <para>
		///     ICommand.CanExecuteChangedの実装
		/// </para>
		/// </summary>
		public event EventHandler CanExecuteChanged {
			add {
				if(!_isAutomaticRequeryDisabled) {
					CommandManager.RequerySuggested += value;
				}
				CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value);
			}
			remove {
				if(!_isAutomaticRequeryDisabled) {
					CommandManager.RequerySuggested -= value;
				}
				CommandManagerHelper.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
			}
		}

		public bool CanExecute(object parameter){
			return this.CanExecute();
		}

		public void Execute(object parameter){
			this.Execute();
		}

		#endregion
	}

	/// <summary>
	/// <para>
	///     This class allows delegating the commanding logic to methods passed as parameters,
	///     and enables a View to bind commands to objects that are not part of the element tree.
	/// </para>
	/// <para>
	///     このクラスはパラメータとして渡されたメソッドへのコマンドのロジックの委譲を実現します。
	///     また、Viewが要素ツリーに含まれないオブジェクトにコマンドをバインドすることを可能にします。
	/// </para>
	/// </summary>
	/// <typeparam name="T">Type of the parameter passed to the delegates</typeparam>
	public class DelegateCommand<T> : ICommand {
		
		#region Data

		private readonly Action<T> _executeMethod = null;
		private readonly Func<T, bool> _canExecuteMethod = null;
		private bool _isAutomaticRequeryDisabled = false;
		private LinkedList<WeakReference> _canExecuteChangedHandlers;

		#endregion

		#region Constructors

		/// <summary>
		///     Constructor
		/// </summary>
		public DelegateCommand(Action<T> executeMethod)
			: this(executeMethod, null, false) {
		}

		/// <summary>
		///     Constructor
		/// </summary>
		public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
			: this(executeMethod, canExecuteMethod, false) {
		}

		/// <summary>
		///     Constructor
		/// </summary>
		public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod, bool isAutomaticRequeryDisabled) {
			if(executeMethod == null) {
				throw new ArgumentNullException("executeMethod");
			}
			_executeMethod = executeMethod;
			_canExecuteMethod = canExecuteMethod;
			_isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// <para>
		///     Method to determine if the command can be executed
		/// </para>
		/// <para>
		///     コマンドが実行可能かを返す
		/// </para>
		/// </summary>
		public bool CanExecute(T parameter) {
			if(_canExecuteMethod != null) {
				return _canExecuteMethod(parameter);
			}
			return true;
		}

		/// <summary>
		/// <para>
		///     Execution of the command
		/// </para>
		/// <para>
		///     コマンドを実行する
		/// </para>
		/// </summary>
		public void Execute(T parameter) {
			if(_executeMethod != null) {
				_executeMethod(parameter);
			}
		}

		/// <summary>
		/// <para>
		///     Raises the CanExecuteChaged event
		/// </para>
		/// <para>
		///     CanExecuteChangedイベントを発生させる
		/// </para>
		/// </summary>
		public void RaiseCanExecuteChanged() {
			OnCanExecuteChanged();
		}

		/// <summary>
		/// <para>
		///     Protected virtual method to raise CanExecuteChanged event
		/// </para>
		/// <para>
		///     CanExecuteChangedイベントを発生させるprotected virtualメソッド
		/// </para>
		/// </summary>
		protected virtual void OnCanExecuteChanged() {
			CommandManagerHelper.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
		}

		/// <summary>
		/// <para>
		///     Property to enable or disable CommandManager's automatic requery on this command
		/// </para>
		/// <para>
		///     CommandManagerのこのコマンドに対する自動再要求の有効/無効を設定する
		/// </para>
		/// </summary>
		public bool IsAutomaticRequeryDisabled {
			get {
				return _isAutomaticRequeryDisabled;
			}
			set {
				if(_isAutomaticRequeryDisabled != value) {
					if(value) {
						CommandManagerHelper.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
					} else {
						CommandManagerHelper.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);
					}
					_isAutomaticRequeryDisabled = value;
				}
			}
		}

		#endregion

		#region ICommand Members

		/// <summary>
		/// <para>
		///     ICommand.CanExecuteChanged implementation
		/// </para>
		/// <para>
		///     ICommand.CanExecuteChangedの実装
		/// </para>
		/// </summary>
		public event EventHandler CanExecuteChanged {
			add {
				if(!_isAutomaticRequeryDisabled) {
					CommandManager.RequerySuggested += value;
				}
				CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value);
			}
			remove {
				if(!_isAutomaticRequeryDisabled) {
					CommandManager.RequerySuggested -= value;
				}
				CommandManagerHelper.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
			}
		}

		bool ICommand.CanExecute(object parameter) {
			// if T is of value type and the parameter is not
			// set yet, then return false if CanExecute delegate
			// exists, else return true

			// Tが値型でかつ、parameterがセットされていない場合、
			// CanExecuteデリゲートが存在するならfalseを返します。
			// そうでなければtrueを返します。
			if(parameter == null &&
				typeof(T).IsValueType) {
				return (_canExecuteMethod == null);
			}
			return CanExecute((T)parameter);
		}

		void ICommand.Execute(object parameter) {
			Execute((T)parameter);
		}

		#endregion
	}

	#region CommandMannagerHelper

	/// <summary>
	/// <para>
	///     This class contains methods for the CommandManager that help avoid memory leaks by
	///     using weak references.
	/// </para>
	/// <para>
	///     このクラスはCommandManagerに対する、弱参照を使うことでメモリリークを防ぐ助けとなるメソッドを含んでいます。
	/// </para>
	/// </summary>
	internal class CommandManagerHelper {
		internal static void CallWeakReferenceHandlers(LinkedList<WeakReference> handlers) {
			if(handlers != null) {
				// Take a snapshot of the handlers before we call out to them since the handlers
				// could cause the array to me modified while we are reading it.

				// 我々が私への配列を読んでいる間にハンドラはそれの変更を引き起こしうるので、
				// それらを呼び出す前にハンドラのスナップショットをとります。

				EventHandler[] callees = new EventHandler[handlers.Count];
				int count = 0;

				for(var node = handlers.First; node != null;) {
					var next = node.Next;
					WeakReference reference = node.Value;
					EventHandler handler = reference.Target as EventHandler;
					if(handler == null) {
						// Clean up old handlers that have been collected
						// 収集されたハンドラを削除します
						handlers.Remove(node);
					} else {
						callees[count] = handler;
						count++;
					}
					node = next;
				}

				// Call the handlers that we snapshotted

				// スナップショットをとったハンドラを呼びます
				for(int i = 0; i < count; i++) {
					EventHandler handler = callees[i];
					handler(null, EventArgs.Empty);
				}
			}
		}

		internal static void AddHandlersToRequerySuggested(LinkedList<WeakReference> handlers) {
			if(handlers != null) {
				foreach(WeakReference handlerRef in handlers) {
					EventHandler handler = handlerRef.Target as EventHandler;
					if(handler != null) {
						CommandManager.RequerySuggested += handler;
					}
				}
			}
		}

		internal static void RemoveHandlersFromRequerySuggested(LinkedList<WeakReference> handlers) {
			if(handlers != null) {
				foreach(WeakReference handlerRef in handlers) {
					EventHandler handler = handlerRef.Target as EventHandler;
					if(handler != null) {
						CommandManager.RequerySuggested -= handler;
					}
				}
			}
		}

		internal static void AddWeakReferenceHandler(ref LinkedList<WeakReference> handlers, EventHandler handler) {
			if(handlers == null) {
				handlers = new LinkedList<WeakReference>();
			}

			handlers.AddLast(new WeakReference(handler));
		}

		internal static void RemoveWeakReferenceHandler(LinkedList<WeakReference> handlers, EventHandler handler) {
			if(handlers != null){
				for(var node = handlers.First; node != null;){
					var next = node.Next;
					WeakReference reference = node.Value;
					EventHandler existingHandler = reference.Target as EventHandler;
					if((existingHandler == null) || (existingHandler == handler)) {
						// Clean up old handlers that have been collected
						// in addition to the handler that is to be removed.

						// 削除されるハンドラに加えて、収集された古いハンドラ
						// を削除します。
						handlers.Remove(node);
					}
					node = node.Next;
				}
			}
		}
	}

	#endregion

	public static class CommandManager {
		public static event EventHandler RequerySuggested;

		/// <summary>
		/// <see cref="RequerySuggested"/> イベントを発生させます。
		/// </summary>
		public static void FireRequerySuggested() {
			var handler = RequerySuggested;
			if(handler != null) {
				handler(null, EventArgs.Empty);
			}
		}
	}

}