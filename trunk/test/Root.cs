using System;

class Program{
	static void Main(){
		double n = Double.Parse(Console.ReadLine());
		double l = 1;
		double r = n;
		while((r - l) > 0.000000000000001d){
			Console.WriteLine("{0} {1}", l, r);
			double t = (l + r) / 2.0d;
			double t2 = t * t;
			if(t2 > n){
				r = t;
			}else{
				l = t;
			}
		}
		Console.WriteLine("Result = {0}", (l + r) / 2.0d);
	}
}