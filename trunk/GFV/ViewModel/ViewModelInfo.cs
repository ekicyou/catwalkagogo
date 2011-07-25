using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Xml.Linq;
using GFV.ViewModel;

namespace GFV.Properties {
	public class ViewModelInfo : Dictionary<string, object> {
		public static ViewModelInfo Load(string path){
			using(var stream = File.OpenRead(path)){
				return Load(stream);
			}
		}
		public static ViewModelInfo Load(Stream stream){
			throw new NotSupportedException();
		}
		public void Save(string path){
			using(var stream = File.OpenWrite(path)){
				Save(stream);
			}
		}
		public void Save(Stream stream){
			this.GetXml().Save(stream);
		}
		public XElement GetElement(){
			var document = new XElement("ViewModelInfo");
			foreach(var pair in this.Where(pair => pair.Value != null)){
				var conv = TypeDescriptor.GetConverter(pair.Value.GetType());
				var elm = new XElement(pair.Key, conv.ConvertToString(pair.Value));
			}
			return document;
		}
	}

	public class ViewModelInfoCollection : Collection<ViewModelInfo>{
		public void Save(string path){
			using(var stream = File.OpenWrite(path)){
				Save(stream);
			}
		}
		public void Save(Stream stream){
			this.GetXml().Save(stream);
		}
		public XElement GetXml(){
			var document = new XElement("ViewModelInfoCollection");
			foreach(var info in this){
				document.Add(info.GetElement());
			}
			return document;
		}
	}
}
