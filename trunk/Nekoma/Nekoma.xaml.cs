using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using CatWalk;

namespace Nekoma{
	public partial class Program : Application{
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
var seq =
    from _ in Enumerable.Repeat(1, 1)
    let x = 1000
    let xs = new List<int>{x}
    let y = Math.PI / 2.0
    let z = "hauhau"
    select _;
var seq2 = Enumerable.Repeat(1, 1).Let(_ =>
                 1000.Let(x =>
     new List<int>{x}.Let(xs =>
      (Math.PI / 2.0).Let(y =>
             "hauhau".Let(z => _)))));
		}
	}
}
