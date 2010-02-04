using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

class Program{
	static void Main(){
		var result = new Dictionary<string, IList<string>>();
		foreach(var file in Directory.GetFiles(Environment.CurrentDirectory, "*.*", SearchOption.AllDirectories)){
			try{
				Console.WriteLine(file);
				byte[] data;
				using(var stream = File.Open(file, FileMode.Open, FileAccess.Read)){
					data = new byte[stream.Length];
					stream.Read(data, 0, (int)stream.Length);
				}
				var enc = EncodingDetector.GetEncoding(data);
				if(!result.ContainsKey(enc.EncodingName)){
					result.Add(enc.EncodingName, new List<string>());
				}
				result[enc.EncodingName].Add(file);
				Console.Write("\n");
			}catch(IOException){
			}
		}
		
		// 結果
		foreach(var item in result){
			Console.WriteLine("{0}", item.Key);
			foreach(var file in item.Value){
				Console.WriteLine(file);
			}
			Console.Write("\n");
		}
	}
}

abstract class EncodingDetector{
	public abstract void Check(byte[] data);
	
	public static Encoding GetEncoding(string str){
		return GetEncoding(Encoding.UTF8.GetBytes(str));
	}
	
	public static Encoding GetEncoding(byte[] data){
		var sevenBit = new SevenBitDetector();
		var iso2022jp = new Iso2022JpDetector();
		var unicodeBom = new UnicodeBomDetector();
		var shiftJis = new ShiftJisDetector();
		var eucJp = new EucJpDetector();
		var utf8 = new Utf8Detector();
		var utf7 = new Utf7Detector();
		sevenBit.Check(data);
		iso2022jp.Check(data);
		unicodeBom.Check(data);
		shiftJis.Check(data);
		eucJp.Check(data);
		utf8.Check(data);
		utf7.Check(data);
		
		Console.WriteLine("sevenBit.IsValid " + sevenBit.IsValid);
		Console.WriteLine("utf8.IsValid " + utf8.IsValid);
		Console.WriteLine("iso2022jp.EscapeSequenceCount " + iso2022jp.EscapeSequenceCount);
		Console.WriteLine("utf7.IsValid " + utf7.IsValid);
		Console.WriteLine("utf7.Base64Count " + utf7.Base64Count);
		Console.WriteLine("unicodeBom.Bom " + unicodeBom.Bom);
		Console.WriteLine("shiftJis.ErrorCount " + shiftJis.ErrorCount);
		Console.WriteLine("eucJp.ErrorCount " + eucJp.ErrorCount);
		Console.WriteLine("utf8.ErrorCount " + utf8.ErrorCount);
		Console.WriteLine("shiftJis.HiraCount " + shiftJis.HiraCount);
		Console.WriteLine("eucJp.HiraCount " + eucJp.HiraCount);
		Console.WriteLine("utf8.HiraCount " + utf8.HiraCount);
		Console.WriteLine("shiftJis.KataCount " + shiftJis.KataCount);
		Console.WriteLine("eucJp.KataCount " + eucJp.KataCount);
		Console.WriteLine("utf8.KataCount " + utf8.KataCount);
		
		if(sevenBit.IsValid){ // 7 bit
			if(iso2022jp.EscapeSequenceCount > 0){
				return Encoding.GetEncoding(50220);
			}else if(utf7.IsValid && (utf7.Base64Count > 0)){
				return Encoding.UTF7;
			}
		}else{
			// BOMに従う
			switch(unicodeBom.Bom){
				case UnicodeBom.UTF8:{
					return Encoding.UTF8;
				}
				case UnicodeBom.UTF7:{
					return Encoding.UTF7;
				}
				case UnicodeBom.UTF16LE:{
					return Encoding.Unicode;
				}
				case UnicodeBom.UTF16BE:{
					return Encoding.GetEncoding(1201);
				}
				case UnicodeBom.UTF32LE:{
					return Encoding.UTF32;
				}
				case UnicodeBom.UTF32BE:{
					return Encoding.GetEncoding(65006);
				}
			}
		}
		var source = (utf8.IsValid) ? new NihongoCountEncodingDetector[]{shiftJis, eucJp, utf8} :
		                              new NihongoCountEncodingDetector[]{shiftJis, eucJp};
		if(sevenBit.IsValid && (source.Where(c => (c.ErrorCount == 0) && (c.HiraCount == 0) && (c.KataCount == 0)).Count() == source.Length)){
			return Encoding.ASCII;
		}else{
			// エラーが一番少ないグループを取得
			var errorGrp = source.OrderBy(c => c.ErrorCount).GroupBy(c => c.ErrorCount).First();
			// 日本語が多いコードを取得
			var code = errorGrp.OrderByDescending(c => (c.HiraCount + c.KataCount)).First();
			if(code is ShiftJisDetector){
				return Encoding.GetEncoding(932);
			}else if(code is EucJpDetector){
				return Encoding.GetEncoding(20932);
			}else if(code is Utf8Detector){
				return Encoding.UTF8;
			}
		}
		return Encoding.ASCII;
	}
}

class SevenBitDetector : EncodingDetector{
	public bool IsValid{get; private set;}
	
	public SevenBitDetector(){
		this.IsValid = true;
	}
	
	public override void Check(byte[] data){
		for(int i = 0; i < data.Length; i++){
			if(data[i] > 0x7f){
				this.IsValid = false;
				break;
			}
		}
	}
}

class Iso2022JpDetector : EncodingDetector{
	public int EscapeSequenceCount{get; private set;}
	public Iso2022JpDetector(){
		this.EscapeSequenceCount = 0;
	}
	
	public override void Check(byte[] data){
		for(int i = 0; i < data.Length; i++){
			byte c1 = data[i];
			int i2 = i + 1;
			int i3 = i + 2;
			if((c1 == 0x1b) && (i3 < data.Length)){
				byte c2 = data[i2];
				byte c3 = data[i3];
				
				if(c2 == 0x24){
					if((c3 == 0x40) || (c3 == 0x42)){ // 旧JIS漢字開始 / 新JIS漢字開始
						this.EscapeSequenceCount++;
						i += 2;
					}else if(c3 == 0x28){
						int i4 = i + 3;
						if(i4 < data.Length){
							byte c4 = data[i4];
							if((c4 == 0x44) || (c4 == 0x4f) || (c4 == 0x50) || (c4 == 0x51)){
								this.EscapeSequenceCount++;
								i += 3;
							}
						}
					}
				}else if((c2 == 0x26) && (c3 == 0x40)){
					int i4 = i + 3;
					int i5 = i + 4;
					int i6 = i + 5;
					if(i6 < data.Length){
						byte c4 = data[i4];
						byte c5 = data[i5];
						byte c6 = data[i6];
						if((c4 == 0x1b) && (c5 == 0x24) && (c6 == 0x42)){ // JIS X 0208-1990
							this.EscapeSequenceCount++;
							i += 5;
						}
					}
				}else if((c2 == 0x28) && ((c3 == 0x42) || (c3 == 0x4a) || (c3 == 0x49) || (c3 == 0x48))){
					this.EscapeSequenceCount++;
					i += 2;
				}
			}
		}
	}
}

class UnicodeBomDetector : EncodingDetector{
	public UnicodeBom Bom{get; private set;}
	public override void Check(byte[] data){
		// BOMチェック
		if((data.Length > 2) && (data[0] == 0xEF) && (data[1] == 0xBB) && (data[2] == 0xBF)){
			this.Bom = UnicodeBom.UTF8;
		}else if((data.Length > 2) && (data[0] == 0x2B) && (data[1] == 0x2F) && ((data[2] == 0x38) || (data[2] == 0x39) || (data[2] == 0x2B) || (data[2] == 0x2F))){
			this.Bom = UnicodeBom.UTF7;
		}else if((data.Length > 3) && (data[0] == 0x00) && (data[1] == 0x00) && (data[2] == 0xFE) && (data[3] == 0xFF)){
			this.Bom = UnicodeBom.UTF32BE;
		}else if((data.Length > 3) && (data[0] == 0xFF) && (data[1] == 0xFE) && (data[2] == 0x00) && (data[3] == 0x00)){
			this.Bom = UnicodeBom.UTF32LE;
		}else if((data.Length > 1) && (data[0] == 0xFE) && (data[1] == 0xFF)){
			this.Bom = UnicodeBom.UTF16BE;
		}else if((data.Length > 1) && (data[0] == 0xFF) && (data[1] == 0xFE)){
			this.Bom = UnicodeBom.UTF16LE;
		}else{
			this.Bom = UnicodeBom.None;
		}
	}
}

abstract class ErrorCountEncodingDetector : EncodingDetector{
	public int ErrorCount{get; protected set;}
	
	public ErrorCountEncodingDetector(){
		this.ErrorCount = 0;
	}
}

abstract class NihongoCountEncodingDetector : ErrorCountEncodingDetector{
	public int HiraCount{get; protected set;}
	public int KataCount{get; protected set;}
	
	public NihongoCountEncodingDetector(){
		this.HiraCount = this.KataCount = 0;
	}
}

class ShiftJisDetector : NihongoCountEncodingDetector{
	public override void Check(byte[] data){
		for(int i = 0; i < data.Length; i++){
			byte c1 = data[i];
			if((0x00 <= c1) && (c1 <= 0x7f)){ // ASCII
				continue;
			}else if((0xa1 <= c1) && (c1 <= 0xdf)){ // 半角
				continue;
			}else{
				int i2 = i + 1;
				if(i2 < data.Length){
					byte c2 = data[i2];
					if((c1 == 0x82) && (0x9f <= c2) && (c2 <= 0xf1)){ // ひらがな
						this.HiraCount++;
						i++;
						continue;
					}else if((c1 == 0x83) && (0x40 <= c2) && (c2 <= 0x96)){ // かたかな
						this.KataCount++;
						i++;
						continue;
					}else if(((0x85 <= c1) && (c1 <= 0x87)) || ((0xeb <= c1) && (c1 <= 0xef))){ // 未使用領域(だいたい)
						this.ErrorCount++;
						i++;
						continue;
					}else if((((0x81 <= c1) && (c1 <= 0x9f)) || ((0xe0 <= c1) && (c1 <= 0xef))) &&
					   (((0x40 <= c2) && (c2 <= 0x7e)) || ((0x80 <= c2) && (c2 <= 0xfc)))){ // 全角
						i++;
						continue;
					}
				}
			}
			this.ErrorCount++;
		}
	}
}

class EucJpDetector : NihongoCountEncodingDetector{
	public override void Check(byte[] data){
		for(int i = 0; i < data.Length; i++){
			byte c1 = data[i];
			if((0x00 <= c1) && (c1 <= 0x7f)){ // ASCII
				continue;
			}else{
				int i2 = i + 1;
				if(i2 < data.Length){
					byte c2 = data[i2];
					if((0xa4 == c1) && (0xa1 <= c2) && (c2 <= 0xf3)){ // ひらがな
						i++;
						this.HiraCount++;
						continue;
					}else if((0xa5 == c1) && (0xa1 <= c2) && (c2 <= 0xf6)){ // カタカナ
						i++;
						this.KataCount++;
						continue;
					}else if(((0xa9 <= c1) && (c1 <= 0xaf)) || ((0xf5 <= c1) && (c1 <= 0xfe))){ // 未使用領域(だいたい)
						this.ErrorCount++;
						i++;
						continue;
					}else if((0xa1 <= c1) && (c1 <= 0xfe) && (0xa1 <= c1) && (c1 <= 0xfe)){ // 全角
						i++;
						continue;
					}else if((c1 == 0x8e) && (0xa1 <= c2) && (c2 <= 0xfe)){ // 半角
						i++;
						continue;
					}
				}
			}
			this.ErrorCount++;
		}
	}
}

class Utf8Detector : NihongoCountEncodingDetector{
	public bool IsValid{get; private set;}
	
	public Utf8Detector(){
		this.IsValid = true;
	}
	
	public override void Check(byte[] data){
		for(int i = 0; i < data.Length; i++){
			byte c1 = data[i];
			if((0x00 <= c1) && (c1 <= 0x7f)){ // ASCII
				continue;
			}else if((c1 == 0xfe) || (c1 == 0xff)){ // BOMコード
				this.IsValid = false;
				continue;
			}else{
				int i2 = i + 1;
				if(i2 < data.Length){
					byte c2 = data[i2];
					if((0xc0 <= c1) && (c1 <= 0xdf) && (0x80 <= c2) && (c2 <= 0xbf)){ // U+0080...U+07FF
						i++;
						continue;
					}else{
						int i3 = i2 + 1;
						if(i3 < data.Length){
							byte c3 = data[i3];
							if((0xe0 <= c1) && (c1 <= 0xef) && (0x80 <= c2) && (c2 <= 0xbf) &&
							   (0x80 <= c3) && (c3 <= 0xbf)){ // U+0800...U+FFFF
								int code = ((c1 & 0xf) << 12) + ((c2 & 0x3f) << 6) + (c3 & 0x3f);
								if((0x3040 <= code) && (code <= 0x309f)){ // ひらがな
									this.HiraCount++;
								}else if((0x30a1 <= code) && (code <= 0x30fa)){ // カタカナ
									this.KataCount++;
								}
								i += 2;
								continue;
							}else{
								int i4 = i3 + 1;
								if(i4 < data.Length){
									byte c4 = data[i4];
									if((0xf0 <= c1) && (c1 <= 0xf7) && (0x80 <= c2) && (c2 <= 0xbf) &&
									   (0x80 <= c3) && (c3 <= 0xbf) && (0x80 <= c4) && (c4 <= 0xbf)){ // U+10000...U+1FFFF
										i += 3;
										continue;
									}else{
										int i5 = i4 + 1;
										if(i5 < data.Length){
											byte c5 = data[i5];
											if((0xf8 <= c1) && (c1 <= 0xfb) && (0x80 <= c2) && (c2 <= 0xbf) &&
											   (0x80 <= c3) && (c3 <= 0xbf) && (0x80 <= c4) && (c4 <= 0xbf) &&
											   (0x80 <= c5) && (c5 <= 0xbf)){ // U+200000...U+3FFFFF
												i += 4;
												continue;
											}else{
												int i6 = i5 + 1;
												if(i6 < data.Length){
													byte c6 = data[i6];
													if((0xfc <= c1) && (c1 <= 0xfd) && (0x80 <= c2) && (c2 <= 0xbf) &&
													   (0x80 <= c3) && (c3 <= 0xbf) && (0x80 <= c4) && (c4 <= 0xbf) &&
													   (0x80 <= c5) && (c5 <= 0xbf) && (0x80 <= c6) && (c6 <= 0xbf)){ // U+400000...U+7FFFFFFF
														i += 4;
														continue;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			this.ErrorCount++;
		}
	}
}

class Utf7Detector : EncodingDetector{
	public bool IsValid{get; private set;}
	public int Base64Count{get; private set;}
	private bool isInBase64 = false;
	
	public Utf7Detector(){
		this.IsValid = true;
		this.Base64Count = 0;
	}
	
	public override void Check(byte[] data){
		for(int i = 0; i < data.Length; i++){
			byte c1 = data[i];
			if((c1 == 0x3d) || (c1 == 0x5c) || (c1 == 0x7e)){ // = / ~
				this.IsValid = false;
				break;
			}else if(this.isInBase64){
				if(c1 == 0x2d){ // -
					this.isInBase64 = false;
					this.Base64Count++;
				}else if(!(((0x30 <= c1) && (c1 <= 0x39)) ||
					       ((0x41 <= c1) && (c1 <= 0x5a)) ||
					       ((0x61 <= c1) && (c1 <= 0x7a)) ||
					       (c1 == 0x2b) || (c1 == 0x2f))){
					this.IsValid = false;
					break;
				}
			}else{
				if(c1 == 0x2b){ // +
					this.isInBase64 = true;
				}
			}
		}
	}
}

enum UnicodeBom{
	None,
	UTF7,
	UTF8,
	UTF16BE,
	UTF16LE,
	UTF32BE,
	UTF32LE,
}
                      