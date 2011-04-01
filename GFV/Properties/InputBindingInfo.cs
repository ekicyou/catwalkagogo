/*
	$Id$
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Reflection;

namespace GFV.Properties{
	[Serializable]
	public class InputBindingInfo : IXmlSerializable{
		public string CommandPath{get; private set;}
		public IInputGestureInfo GestureInfo{get; private set;}

		public InputBindingInfo(){}
		public InputBindingInfo(string memberPath, IInputGestureInfo gesture){
			this.CommandPath = memberPath;
			this.GestureInfo = gesture;
		}

		#region Apply

		public static void ApplyInputBindings(FrameworkElement self, InputBindingInfo[] infos){
			if(self == null){
				throw new ArgumentNullException("self");
			}
			if(infos == null){
				throw new ArgumentNullException("infos");
			}
			foreach(var binding in self.InputBindings.Cast<InputBinding>()){
				var command = binding.Command;
				var inputGestures = InputBindingInfo.GetInputGestures(command);
				var removeMethod = inputGestures.GetType().GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public);
				if(removeMethod != null){
					removeMethod.Invoke(inputGestures, new object[]{binding.Gesture});
				}
			}
			self.InputBindings.Clear();

			var vm = self.DataContext;
			if(infos != null && vm != null){
				foreach(var info in infos){
					var command = (ICommand)ResolvePath(vm, info.CommandPath);
					if(command != null){
						var inputGestures = GetInputGestures(command);
						if(inputGestures != null){
							var addMethod = inputGestures.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
							if(addMethod != null){
								var gesture = info.GestureInfo.Gesture;
								addMethod.Invoke(inputGestures, new object[]{gesture});
								self.InputBindings.Add(info.GestureInfo.GetBinding(command));
							}
						}
					}
				}
			}
		}

		public static object GetInputGestures(object command){
			var inputGesturesProp = command.GetType().GetProperty("InputGestures");
			if(inputGesturesProp != null){
				return inputGesturesProp.GetValue(command, null);
			}else{
				return null;
			}
		}

		private static void RestoreInputBindings(IDictionary<ICommand, IList<InputGesture>> restoreData){
			foreach(var entry in restoreData){
				var inputGestures = GetInputGestures(entry.Key);
				foreach(var gesture in entry.Value){
					var removeMethod = inputGestures.GetType().GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public);
					if(removeMethod != null){
						removeMethod.Invoke(inputGestures, new object[]{gesture});
					}
				}
			}
		}

		/// <summary>
		/// Static Member:    Type name::Member...Member
		/// Instance Memeber: Member...Member
		/// </summary>
		/// <param name="current"></param>
		/// <param name="path"></param>
		/// <returns>Property Value</returns>
		public static object ResolvePath(object current, string path){
			var idx = path.IndexOf("::");
			if(idx >= 0){
				// Static member
				var typeName = path.Substring(0, idx);
				path = path.Substring(idx + 2);
				var staticType = Assembly.GetExecutingAssembly().GetType(typeName);
				if(staticType != null){
					idx = path.IndexOf('.');
					if(idx < 0){ // No period
						var prop = path;
						var propInfo = staticType.GetProperty(prop, BindingFlags.Static | BindingFlags.Public);
						if(propInfo != null){
							return propInfo.GetValue(null, null);
						}else{
							return null;
						}
					}else{
						var prop = path.Substring(0, idx);
						path = path.Substring(idx + 1);
						var propInfo = staticType.GetProperty(prop, BindingFlags.Static | BindingFlags.Public);
						if(propInfo != null){
							current = propInfo.GetValue(null, null);
						}else{
							return null;
						}
					}
				}else{
					return null;
				}
			}

			foreach(var prop in path.Split('.')){
				var type = current.GetType();
				var propInfo = type.GetProperty(prop, BindingFlags.Instance | BindingFlags.Public);
				if(propInfo != null){
					current = propInfo.GetValue(current, null);
				}else{
					return null;
				}
			}
			return current;
		}

		#endregion

		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader){
			try{
				reader.ReadStartElement();

				reader.ReadStartElement("CommandPath");
				this.CommandPath = reader.ReadString();
				reader.ReadEndElement();

				var typeName = reader["Type"];
				var type = Type.GetType(typeName, true);
				var gesture = (IInputGestureInfo)Activator.CreateInstance(type);
				gesture.ReadXml(reader);
				this.GestureInfo = gesture;

				reader.ReadEndElement();
			}catch(Exception ex){
				System.Windows.MessageBox.Show(ex.ToString());
			}
		}

		public void WriteXml(XmlWriter writer) {
			writer.WriteStartElement("CommandPath");
			writer.WriteValue(this.CommandPath);
			writer.WriteEndElement();

			writer.WriteStartElement("GestureInfo");

			writer.WriteStartAttribute("Type");
			writer.WriteValue(this.GestureInfo.GetType().FullName);
			writer.WriteEndAttribute();

			this.GestureInfo.WriteXml(writer);

			writer.WriteEndElement();
		}

		#endregion
	}

	public interface IInputGestureInfo : IXmlSerializable{
		InputGesture Gesture{get;}
		InputBinding GetBinding(ICommand command);
	}

	[Serializable]
	public class KeyGestureInfo : IInputGestureInfo{
		public Key Key{get; private set;}
		public ModifierKeys Modifiers{get; private set;}

		public KeyGestureInfo(){}
		public KeyGestureInfo(Key key) : this(key, ModifierKeys.None){}
		public KeyGestureInfo(Key key, ModifierKeys modifiers){
			this.Key = key;
			this.Modifiers = modifiers;
		}

		public InputGesture Gesture{
			get{
				return new KeyGesture(this.Key, this.Modifiers);
			}
		}

		public InputBinding GetBinding(ICommand command){
			return new KeyBinding(command, this.Key, this.Modifiers);
		}

		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader){
			reader.ReadStartElement();
			reader.ReadStartElement("Key");
			try{
				var keyName = reader.ReadString();
				this.Key = (Key)Enum.Parse(typeof(Key), keyName);
			}finally{
				reader.ReadEndElement();
			}

			reader.ReadStartElement("Modifiers");
			try{
				var modName = reader.ReadString();
				this.Modifiers = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), modName);
			}finally{
				reader.ReadEndElement();
			}
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer) {
			writer.WriteStartElement("Key");
			try{
				writer.WriteValue(this.Key.ToString());
			}finally{
				writer.WriteEndElement();
			}

			writer.WriteStartElement("Modifiers");
			try{
				writer.WriteValue(this.Modifiers.ToString());
			}finally{
				writer.WriteEndElement();
			}
		}

		#endregion
	}
	
	[Serializable]
	public class MouseGestureInfo : IInputGestureInfo{
		public MouseAction MouseAction{get; private set;}
		public ModifierKeys Modifiers{get; private set;}

		public MouseGestureInfo(){}
		public MouseGestureInfo(MouseAction action) : this(action, ModifierKeys.None){}
		public MouseGestureInfo(MouseAction action, ModifierKeys modifiers){
			this.MouseAction = action;
			this.Modifiers = modifiers;
		}

		public InputGesture Gesture{
			get{
				return new MouseGesture(this.MouseAction, this.Modifiers);
			}
		}

		public InputBinding GetBinding(ICommand command){
			return new MouseBinding(command, new MouseGesture(this.MouseAction, this.Modifiers));
		}

		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader){
			reader.Read();
			reader.ReadStartElement("MouseAction");
			try{
				var actionName = reader.ReadString();
				this.MouseAction = (MouseAction)Enum.Parse(typeof(MouseAction), actionName);
			}finally{
				reader.ReadEndElement();
			}

			reader.ReadStartElement("Modifiers");
			try{
				var modName = reader.ReadString();
				this.Modifiers = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), modName);
			}finally{
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer) {
			writer.WriteStartElement("MouseAction");
			try{
				writer.WriteValue(this.MouseAction.ToString());
			}finally{
				writer.WriteEndElement();
			}

			writer.WriteStartElement("Modifiers");
			try{
				writer.WriteValue(this.Modifiers.ToString());
			}finally{
				writer.WriteEndElement();
			}
		}

		#endregion
	}
	
}
