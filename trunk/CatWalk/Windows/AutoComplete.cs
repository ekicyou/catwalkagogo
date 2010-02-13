/*
	$Id$
*/
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Text.RegularExpressions;
using CatWalk.Collections;

namespace CatWalk.Forms{
	public static class AutoComplete{
		#region 添付プロパティ
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(AutoComplete), new UIPropertyMetadata(false, IsEnabledChanged));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static bool GetIsEnabled(DependencyObject obj){
			return (bool)obj.GetValue(IsEnabledProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetIsEnabled(DependencyObject obj, bool value){
			obj.SetValue(IsEnabledProperty, value);
		}
		
		public static readonly DependencyProperty CandidatesListBoxProperty =
			DependencyProperty.RegisterAttached("CandidatesListBox", typeof(ListBox), typeof(AutoComplete));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static ListBox GetCandidatesListBox(DependencyObject obj){
			return (ListBox)obj.GetValue(CandidatesListBoxProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetCandidatesListBox(DependencyObject obj, ListBox value){
			obj.SetValue(CandidatesListBoxProperty, value);
		}
		
		public static readonly DependencyProperty PopupProperty =
			DependencyProperty.RegisterAttached("Popup", typeof(Popup), typeof(AutoComplete));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static Popup GetPopup(DependencyObject obj){
			return (Popup)obj.GetValue(PopupProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetPopup(DependencyObject obj, Popup value){
			obj.SetValue(PopupProperty, value);
		}
		
		public static readonly DependencyProperty PopupOffsetProperty =
			DependencyProperty.RegisterAttached("PopupOffset", typeof(Vector), typeof(AutoComplete));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static Vector GetPopupOffset(DependencyObject obj){
			return (Vector)obj.GetValue(PopupOffsetProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetPopupOffset(DependencyObject obj, Vector value){
			obj.SetValue(PopupOffsetProperty, value);
		}
		
		public static readonly DependencyProperty CandidatesProperty =
			DependencyProperty.RegisterAttached("Candidates", typeof(IDictionary<string, object>), typeof(AutoComplete));
		
		public static IDictionary<string, object> GetCandidates(DependencyObject obj){
			return (IDictionary<string, object>)obj.GetValue(CandidatesProperty);
		}
		
		public static readonly DependencyProperty TokenPatternProperty =
			DependencyProperty.RegisterAttached("TokenPattern", typeof(string), typeof(AutoComplete));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static string GetTokenPattern(DependencyObject obj){
			return (string)obj.GetValue(TokenPatternProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetTokenPattern(DependencyObject obj, string value){
			obj.SetValue(TokenPatternProperty, value);
		}
		
		private static readonly DependencyProperty AutoCompleteStateProperty =
			DependencyProperty.RegisterAttached("States", typeof(AutoCompleteState), typeof(AutoComplete));
		
		private static AutoCompleteState GetState(DependencyObject obj){
			return (AutoCompleteState)obj.GetValue(AutoCompleteStateProperty);
		}
		
		private static void SetState(DependencyObject obj, AutoCompleteState value){
			obj.SetValue(AutoCompleteStateProperty, value);
		}
		
		private class AutoCompleteState{
			public string ProcessingWord{get; set;}
		}
		
		public static readonly DependencyProperty InsertWordTypesProperty =
			DependencyProperty.RegisterAttached("InsertWordTypes", typeof(Type[]), typeof(AutoComplete));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static Type[] GetInsertWordTypes(DependencyObject obj){
			return (Type[])obj.GetValue(InsertWordTypesProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetInsertWordTypes(DependencyObject obj, Type[] value){
			obj.SetValue(InsertWordTypesProperty, value);
		}
		
		public static readonly DependencyProperty QueryCandidatesProperty =
			DependencyProperty.RegisterAttached("QueryCandidates", typeof(QueryCandidatesEventHandler), typeof(AutoComplete));
		public static void AddQueryCandidates(DependencyObject obj, QueryCandidatesEventHandler handler){
			var ev = (QueryCandidatesEventHandler)obj.GetValue(QueryCandidatesProperty);
			if(ev != null){
				ev += handler;
			}else{
				ev = handler;
			}
			obj.SetValue(QueryCandidatesProperty, ev);
		}
		
		public static void RemoveQueryCandidates(DependencyObject obj, QueryCandidatesEventHandler handler){
			var ev = (QueryCandidatesEventHandler)obj.GetValue(QueryCandidatesProperty);
			if(ev != null){
				ev -= handler;
			}
			obj.SetValue(QueryCandidatesProperty, ev);
		}
		
		#endregion
		
		#region イベント処理
		
		private static void IsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e){
			TextBox textBox = (TextBox)sender;
			
			bool newValue = (bool)e.NewValue;
			bool oldValue = (bool)e.OldValue;
			if(newValue != oldValue){
				if(oldValue){
					textBox.TextChanged -= TextBox_TextChanged;
					textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
					textBox.SetValue(CandidatesProperty, null);
					SetState(textBox, null);
				}else{
					textBox.TextChanged += TextBox_TextChanged;
					textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
					textBox.SetValue(CandidatesProperty, new PrefixDictionary<object>(new CharIgnoreCaseComparer()));
					SetState(textBox, new AutoCompleteState());
					if(String.IsNullOrEmpty(GetTokenPattern(textBox))){
						SetTokenPattern(textBox, " ");
					}
				}
			}
		}
		
		private static void TextBox_TextChanged(object sender, TextChangedEventArgs e){
			var textBox = (TextBox)sender;
			var listBox = GetCandidatesListBox(textBox);
			var state = GetState(textBox);
			if(listBox != null){
				var popup = GetPopup(textBox);
				var tokenPattern = GetTokenPattern(textBox);
				var dict = (PrefixDictionary<object>)GetCandidates(textBox);
				var text = textBox.Text;
				var caretIndex = textBox.CaretIndex;
				
				state.ProcessingWord = text;
				if(!String.IsNullOrEmpty(text)){
					ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
						var startIndex = GetStartIndex(text, caretIndex, tokenPattern);
						var queryWord = text.Substring(startIndex, caretIndex - startIndex);
						
						if(!String.IsNullOrEmpty(queryWord) && (queryWord.Length < 32)){
							RefreshListAsync(textBox, listBox, queryWord, dict, delegate{
								if(popup != null){
									if(state.ProcessingWord == text){
										if((listBox.Items.Count > 1) || 
										   ((listBox.Items.Count == 1) && 
											 !queryWord.Equals(((KeyValuePair<string, object>)listBox.Items[0]).Key,
												StringComparison.OrdinalIgnoreCase))){
											OpenPopup(textBox, popup, startIndex);
										}else{
											popup.IsOpen = false;
											listBox.ItemsSource = null;
										}
									}
								}
							});
						}else{
							textBox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate{
								if(state.ProcessingWord == text){
									if(popup != null){
										popup.IsOpen = false;
									}
									listBox.ItemsSource = null;
								}
							}));
						}
					}));
				}else{
					if(popup != null){
						popup.IsOpen = false;
					}
					listBox.ItemsSource = null;
				}
			}
		}
		
		private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e){
			var textBox = (TextBox)sender;
			var listBox = GetCandidatesListBox(textBox);
			var popup = GetPopup(textBox);
			if(listBox.Items.Count > 0){
				switch(e.Key){
					case Key.Up:{
						if(listBox.SelectedIndex > 0){
							listBox.SelectedIndex--;
							listBox.ScrollIntoView(listBox.SelectedItem);
						}
						e.Handled = true;
						break;
					}
					case Key.Down:{
						if(listBox.SelectedIndex < listBox.Items.Count){
							listBox.SelectedIndex++;
							listBox.ScrollIntoView(listBox.SelectedItem);
						}
						e.Handled = true;
						break;
					}
					case Key.Enter:
					case Key.Tab:{
						string text = textBox.Text;
						if(!String.IsNullOrEmpty(text)){
							int caretIndex = textBox.CaretIndex;
							int startIndex = GetStartIndex(text, caretIndex, GetTokenPattern(textBox));
							string left = text.Substring(0, startIndex);
							string right = text.Substring(caretIndex);
							var value = (KeyValuePair<string, object>)listBox.SelectedValue;
							string word = value.Key;
							
							var types = GetInsertWordTypes(textBox);
							// 候補ワードに置き換える
							if((types == null) || (Array.IndexOf(types, value.Value.GetType()) == -1)){
								textBox.Text = left + word + right;
							}else{ // 挿入する。
								string inputed = text.Substring(startIndex, caretIndex - startIndex);
								string insert = word.Substring(inputed.Length);
								textBox.Text = left + inputed + insert + right;
							}
							int inputedLength = caretIndex - startIndex;
							textBox.CaretIndex = caretIndex + (word.Length - inputedLength);
							listBox.ItemsSource = null;
							if(popup != null){
								popup.IsOpen = false;
							}
							e.Handled = true;
						}
						break;
					}
					case Key.Left:
					case Key.Right:
					case Key.PageUp:
					case Key.PageDown:{
						listBox.ItemsSource = null;
						if(popup != null){
							popup.IsOpen = false;
						}
						break;
					}
				}
			}
		}
		
		#endregion
		
		#region 関数
		
		private static int GetStartIndex(string text, int index, string tokenPattern){
			if((index < 0) || (text.Length < index)){
				throw new ArgumentOutOfRangeException();
			}
			var regex = new Regex(tokenPattern, RegexOptions.RightToLeft);
			var match = regex.Match(text, index);
			if(match.Success){
				return match.Index + match.Length;
			}else{
				return 0;
			}
		}
		
		public static void RefreshList(TextBox textBox){
			string text = textBox.Text;
			if(text == null){
				text = "";
			}
			int caretIndex = textBox.CaretIndex;
			int startIndex = GetStartIndex(text, caretIndex, GetTokenPattern(textBox));
			var listBox = GetCandidatesListBox(textBox);
			string word = text.Substring(startIndex, caretIndex);
			RefreshList(textBox, listBox, word);
		}
		
		private static void RefreshList(TextBox textBox, ListBox listBox, string word){
			ManualResetEvent wait = new ManualResetEvent(false);
			wait.Reset();
			RefreshListAsync(textBox, listBox, word, delegate{
				wait.Set();
			});
			wait.WaitOne();
		}
		
		private static void RefreshListAsync(TextBox textBox, ListBox listBox, string word, Action callback){
			var dict = (PrefixDictionary<object>)textBox.GetValue(CandidatesProperty);
			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
				RefreshListAsync(textBox, listBox, word, dict, callback);
			}));
		}
		
		private static void RefreshListAsync(TextBox textBox, ListBox listBox, string word, PrefixDictionary<object> dict, Action callback){
			var matches = dict.Search(word).ToList();
			listBox.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate{
				var ev = (QueryCandidatesEventHandler)textBox.GetValue(QueryCandidatesProperty);
				var e = new QueryCandidatesEventArgs();
				foreach(var del in ev.GetInvocationList()){
					del.DynamicInvoke(new object[]{textBox, e});
					if(e.Candidates != null){
						matches.AddRange(e.Candidates);
					}
				}
			}));
			listBox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate{
				listBox.ItemsSource = matches.ToArray();
				if(listBox.Items.Count > 0){
					listBox.SelectedIndex = 0;
					listBox.ScrollIntoView(listBox.SelectedItem);
				}
				if(callback != null){
					callback();
				}
			}));
		}
		
		private static void OpenPopup(TextBox textBox, Popup popup, int index){
			popup.PlacementTarget = textBox;
			var rect = textBox.GetRectFromCharacterIndex(Math.Max(0, index));
			rect.Offset(GetPopupOffset(textBox));
			popup.PlacementRectangle = rect;
			popup.IsOpen = true;
		}
		#endregion 
	}
	
	public delegate void QueryCandidatesEventHandler(object sender, QueryCandidatesEventArgs e);
	
	public class QueryCandidatesEventArgs : EventArgs{
		public KeyValuePair<string, object>[] Candidates{get; set;}
		
		public QueryCandidatesEventArgs(){}
	}
}