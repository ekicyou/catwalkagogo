using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Xml.Linq;

namespace CatWalk.Net{
	/*
	<packages>
		<package>
			<version></version>
			<infoversion></infoversion>
			<installeruri></installeruri>
			<changelog></changelog>
			<state></state>
			<date></date>
		</package>
		...
	</packages>
	*/
	public class AutoUpdater{
		public Uri[] CheckUris{get; private set;}
		public IWebProxy Proxy{get; set;}
		public int Timeout{get; set;}
		
		public AutoUpdater(params Uri[] checkUris){
			this.CheckUris = checkUris;
			this.Timeout = 15000;
		}
		
		public IEnumerable<WebRequest> RequestUpdates(){
			foreach(var uri in this.CheckUris){
				WebRequest req = WebRequest.Create(uri);
				req.Proxy = this.Proxy;
				req.Timeout = 15000;
				yield return req;
			}
		}
		
		public IEnumerable<UpdatePackage> CheckUpdates(IEnumerable<WebRequest> requests){
			foreach(var req in requests){
				XDocument doc;
				using(WebResponse res = req.GetResponse())
				using(Stream stream = res.GetResponseStream())
				using(StreamReader reader = new StreamReader(stream, Encoding.UTF8)){
					doc = XDocument.Parse(reader.ReadToEnd());
				}
				foreach(var package in doc.Element("packages").Elements("package")){
					UpdatePackage updatePackage = null;
					try{
						updatePackage = new UpdatePackage(package);
					}catch{
					}
					if(updatePackage != null){
						yield return updatePackage;
					}
				}
			}
		}
		
		public IEnumerable<UpdatePackage> CheckUpdates(){
			return this.CheckUpdates(this.RequestUpdates());
		}
	}
	
	public class UpdatePackage{
		public Version Version{get; private set;}
		public Version InformationalVersion{get; private set;}
		public Uri InstallerUri{get; private set;}
		public string ChangeLog{get; private set;}
		public PackageState State{get; private set;}
		public DateTime Date{get; private set;}
		
		public UpdatePackage(XElement elm){
			this.Version = new Version((string)elm.Element("version"));
			this.InformationalVersion = new Version((string)elm.Element("infoversion"));
			this.InstallerUri = new Uri((string)elm.Element("installeruri"));
			this.ChangeLog = (string)elm.Element("changelog");
			this.State = (PackageState)Enum.Parse(typeof(PackageState), (string)elm.Element("state"), false);
			this.Date = DateTime.ParseExact(
				(string)elm.Element("date"),
				"yyyy-MM-dd",
				System.Globalization.DateTimeFormatInfo.InvariantInfo,
				System.Globalization.DateTimeStyles.AllowWhiteSpaces);
		}
		
		public string DownloadInstaller(){
			var client = new WebClient();
			string file = Path.GetTempPath() + Path.GetFileName(this.InstallerUri.AbsolutePath);
			client.DownloadFile(this.InstallerUri, file);
			return file;
		}
		
		public void DownloadInstallerAsync(DownloadProgressChangedEventHandler progress, AsyncCompletedEventHandler completed){
			var client = new WebClient();
			if(progress != null){
				client.DownloadProgressChanged += progress;
			}
			if(completed != null){
				client.DownloadFileCompleted += completed;
			}
			
			string file = Path.GetTempPath() + Path.GetFileName(this.InstallerUri.AbsolutePath);
			client.DownloadFileAsync(this.InstallerUri, file, file);
		}
	}
	
	public enum PackageState{
		Stable,
		Beta,
		Alpha,
		ReleaseCandidate,
	}
}