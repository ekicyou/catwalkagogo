/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public partial class Gfl{
		#region enum
		
		[Flags]
		internal enum BitmapType : ushort{
			Binary = 0x0001,
			Grey   = 0x0002,
			Colors = 0x0004,
			Rgb    = 0x0010,
			Rgba   = 0x0020,
			Bgr    = 0x0040,
			Abgr   = 0x0080,
			Bgra   = 0x0100,
			Argb   = 0x0200,
			Cmyk   = 0x0400,
		}
		
		internal enum Origin : ushort{
			TopLeft = 0,
			BottomLeft = 2,
			TopRight = 1,
			BottomRight = 3,
		}
		
		internal enum Error : ushort{
			None = 0,
			FileOpen = 1,
			FileRead = 2,
			FileCreate = 3,
			FileWrite = 4,
			NoMemory = 5,
			UnknownFormat = 6,
			BadBitmap = 7,
			BadFormatIndex = 10,
			BadParameters = 50,
			UnknownError = 255,
		}
		
		internal enum Status : uint{
			Read = 1,
			Write = 2,
		}
		
		[Flags]
		internal enum LoadOptions : uint{
			None                        = 0x00000000,
			SkipAlpha                   = 0x00000001, /* Alpha not loaded (32bits only)                     */
			IgnoreReadError             = 0x00000002,
			RecognizeFormatByExtensionOnly = 0x00000004, /* Use only extension to recognize format. Faster     */
			ReadAllComment              = 0x00000008, /* Read Comment in GFL_FILE_DESCRIPTION               */
			ForceColorModel             = 0x00000010, /* Force to load picture in the ColorModel            */
			PreviewNoCanvasResize       = 0x00000020, /* With gflLoadPreview, width & height are the maximum box */
			BinaryAsGrey                = 0x00000040, /* Load Black&White file in greyscale                 */
			OriginalColorModel          = 0x00000080, /* If the colormodel is CMYK, keep it                 */
			OnlyFirstFrame              = 0x00000100, /* No search to check if file is multi-frame          */
			OriginalDepth               = 0x00000200, /* In the case of 10/16 bits per component            */
			ReadMetadata                = 0x00000400, /* Read all metadata                                  */
			ReadComment                 = 0x00000800, /* Read comment                                       */
			HighQualityThumbnail        = 0x00001000, /* gflLoadThumbnail                                   */
			EmbeddedThumbnail           = 0x00002000, /* gflLoadThumbnail                                   */
			OrientedThumbnail           = 0x00004000, /* gflLoadThumbnail                                   */
			OriginalEmbeddedThumbnail   = 0x00008000, /* gflLoadThumbnail                                   */
			Oriented                    = 0x00008000,
		}
		
		internal enum ChannelOrder : ushort{
			Interleaved = 0,
			Sequential = 1,
			Separate = 2,
		}
		
		internal enum ChannelType : ushort{
			GreyScale  = 0,
			Rgb        = 1,
			Bgr        = 2,
			Rgba       = 3,
			Abgr       = 4,
			Cmy        = 5,
			CMYK       = 6,
		}
		
		internal enum LutType : ushort{
			To8Bits  = 1,
			To10Bits = 2,
			To12Bits = 3,
			To16Bits = 4,
		}
		
		internal enum SaveOptions : uint{
			ReplaceExtension = 0x00000001,
			WantFilename     = 0x00000002,
			SaveAnyway       = 0x00000004,
			SaveIccProfile   = 0x00000008,
		}
		
		internal enum ByteOrder : byte{
			Default = 0,
			LSBF = 1,
			MDBF = 2,
		}
		
		internal enum GetExifOptions : uint{
			None = 0,
			WantMakerNotes = 1,
		}
		
		#endregion
	}
}
