using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GflNet {
	public partial class GflExtended : IDisposable{
		private T LoadMethod<T>(string name) where T : class{
			return LoadMethod<T>(name, this.Handle);
		}

		protected static T LoadMethod<T>(string name, IntPtr hModule) where T : class{
			return Marshal.GetDelegateForFunctionPointer(NativeMethods.GetProcAddress(hModule, name), typeof(T)) as T;
		}

		#region Colors

		private GflBitmapFuncInt32 _SepiaDelegate;
		internal Gfl.Error Sepia(Bitmap src, ref IntPtr dst, int percentage){
			this.ThrowIfDisposed();
			if(this._SepiaDelegate == null){
				this._SepiaDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflSepia");
			}
			return this._SepiaDelegate(src.Handle, ref dst, percentage);
		}

		private GflBitmapFuncInt32GflColor _SepiaExDelegate;
		internal Gfl.Error Sepia(Bitmap src, ref IntPtr dst, int percentage, ref Gfl.GflColor color){
			this.ThrowIfDisposed();
			if(this._SepiaExDelegate == null){
				this._SepiaExDelegate = this.LoadMethod<GflBitmapFuncInt32GflColor>("gflSepia");
			}
			return this._SepiaExDelegate(src.Handle, ref dst, percentage, ref color);
		}

		private GflBitmapFuncSwapColors _SwapColorsDelegate;
		internal Gfl.Error SwapColors(Bitmap src, ref IntPtr dst, SwapColorsFilter filter){
			this.ThrowIfDisposed();
			if(this._SwapColorsDelegate == null){
				this._SwapColorsDelegate = this.LoadMethod<GflBitmapFuncSwapColors>("gflSwapColors");
			}
			return this._SwapColorsDelegate(src.Handle, ref dst, filter);
		}

		#endregion

		#region Colors Destructive

		private GflBitmapFuncDestSwapColors _SwapColorsDestDelegate;
		internal Gfl.Error SwapColors(Bitmap src, SwapColorsFilter filter){
			this.ThrowIfDisposed();
			if(this._SwapColorsDestDelegate == null){
				this._SwapColorsDestDelegate = this.LoadMethod<GflBitmapFuncDestSwapColors>("gflSwapColors");
			}
			return this._SwapColorsDestDelegate(src.Handle, IntPtr.Zero, filter);
		}

		private GflBitmapFuncDestInt32 _SepiaDestDelegate;
		internal Gfl.Error Sepia(Bitmap src, int percentage){
			this.ThrowIfDisposed();
			if(this._SepiaDestDelegate == null){
				this._SepiaDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflSepia");
			}
			return this._SepiaDestDelegate(src.Handle, IntPtr.Zero, percentage);
		}

		private GflBitmapFuncDestInt32GflColor _SepiaExDestDelegate;
		internal Gfl.Error Sepia(Bitmap src, int percentage, ref Gfl.GflColor color){
			this.ThrowIfDisposed();
			if(this._SepiaExDestDelegate == null){
				this._SepiaExDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32GflColor>("gflSepia");
			}
			return this._SepiaExDestDelegate(src.Handle, IntPtr.Zero, percentage, ref color);
		}

		#endregion

		#region Filters

		private GflBitmapFuncInt32 _AverageDelegate;
		internal Gfl.Error Average(Bitmap src, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._AverageDelegate == null){
				this._AverageDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflAverage");
			}
			return this._AverageDelegate(src.Handle, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _SoftenDelegate;
		internal Gfl.Error Soften(Bitmap src, ref IntPtr dst, int percentage){
			this.ThrowIfDisposed();
			if(this._SoftenDelegate == null){
				this._SoftenDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflSoften");
			}
			return this._SoftenDelegate(src.Handle, ref dst, percentage);
		}

		private GflBitmapFuncInt32 _BlurDelegate;
		internal Gfl.Error Blur(Bitmap src, ref IntPtr dst, int percentage){
			this.ThrowIfDisposed();
			if(this._BlurDelegate == null){
				this._BlurDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflBlur");
			}
			return this._BlurDelegate(src.Handle, ref dst, percentage);
		}

		private GflBitmapFuncInt32 _GaussianBlurDelegate;
		internal Gfl.Error GaussianBlur(Bitmap src, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._GaussianBlurDelegate == null){
				this._GaussianBlurDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflGaussianBlur");
			}
			return this._GaussianBlurDelegate(src.Handle, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _MaximumDelegate;
		internal Gfl.Error Maximum(Bitmap src, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._MaximumDelegate == null){
				this._MaximumDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflMaximum");
			}
			return this._MaximumDelegate(src.Handle, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _MinimumDelegate;
		internal Gfl.Error Minimum(Bitmap src, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._MinimumDelegate == null){
				this._MinimumDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflMinimum");
			}
			return this._MinimumDelegate(src.Handle, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _MedianBoxDelegate;
		internal Gfl.Error MedianBox(Bitmap src, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._MedianBoxDelegate == null){
				this._MedianBoxDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflMedianBox");
			}
			return this._MedianBoxDelegate(src.Handle, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _MedianCrossDelegate;
		internal Gfl.Error MedianCross(Bitmap src, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._MedianCrossDelegate == null){
				this._MedianCrossDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflMedianCross");
			}
			return this._MedianCrossDelegate(src.Handle, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _SharpenDelegate;
		internal Gfl.Error Sharpen(Bitmap src, ref IntPtr dst, int percentage){
			this.ThrowIfDisposed();
			if(this._SharpenDelegate == null){
				this._SharpenDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflSharpen");
			}
			return this._SharpenDelegate(src.Handle, ref dst, percentage);
		}

		private GflBitmapFunc _EnhanceDetailDelegate;
		internal Gfl.Error EnhanceDetail(Bitmap src, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EnhanceDetailDelegate == null){
				this._EnhanceDetailDelegate = this.LoadMethod<GflBitmapFunc>("gflEnhanceDetail");
			}
			return this._EnhanceDetailDelegate(src.Handle, ref dst);
		}

		private GflBitmapFunc _EnhanceFocusDelegate;
		internal Gfl.Error EnhanceFocus(Bitmap src, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EnhanceFocusDelegate == null){
				this._EnhanceFocusDelegate = this.LoadMethod<GflBitmapFunc>("gflEnhanceFocus");
			}
			return this._EnhanceFocusDelegate(src.Handle, ref dst);
		}

		private GflBitmapFunc _FocusRestorationDelegate;
		internal Gfl.Error FocusRestoration(Bitmap src, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._FocusRestorationDelegate == null){
				this._FocusRestorationDelegate = this.LoadMethod<GflBitmapFunc>("gflFocusRestoration");
			}
			return this._FocusRestorationDelegate(src.Handle, ref dst);
		}

		private GflBitmapFunc _EdgeDetectLightDelegate;
		internal Gfl.Error EdgeDetectLight(Bitmap src, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EdgeDetectLightDelegate == null){
				this._EdgeDetectLightDelegate = this.LoadMethod<GflBitmapFunc>("gflEdgeDetectLight");
			}
			return this._EdgeDetectLightDelegate(src.Handle, ref dst);
		}

		private GflBitmapFunc _EdgeDetectMediumDelegate;
		internal Gfl.Error EdgeDetectMedium(Bitmap src, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EdgeDetectMediumDelegate == null){
				this._EdgeDetectMediumDelegate = this.LoadMethod<GflBitmapFunc>("gflEdgeDetectMedium");
			}
			return this._EdgeDetectMediumDelegate(src.Handle, ref dst);
		}

		private GflBitmapFunc _EdgeDetectHeavyDelegate;
		internal Gfl.Error EdgeDetectHeavy(Bitmap src, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EdgeDetectHeavyDelegate == null){
				this._EdgeDetectHeavyDelegate = this.LoadMethod<GflBitmapFunc>("gflEdgeDetectHeavy");
			}
			return this._EdgeDetectHeavyDelegate(src.Handle, ref dst);
		}

		private GflBitmapFunc _EmbossDelegate;
		internal Gfl.Error Emboss(Bitmap src, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EmbossDelegate == null){
				this._EmbossDelegate = this.LoadMethod<GflBitmapFunc>("gflEmboss");
			}
			return this._EmbossDelegate(src.Handle, ref dst);
		}

		private GflBitmapFunc _EmbossMoreDelegate;
		internal Gfl.Error EmbossMore(Bitmap src, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EmbossMoreDelegate == null){
				this._EmbossMoreDelegate = this.LoadMethod<GflBitmapFunc>("gflEmbossMore");
			}
			return this._EmbossMoreDelegate(src.Handle, ref dst);
		}

		private GflBitmapFunc _ReduceNoiseDelegate;
		internal Gfl.Error Sepia(Bitmap src, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._ReduceNoiseDelegate == null){
				this._ReduceNoiseDelegate = this.LoadMethod<GflBitmapFunc>("gflReduceNoise");
			}
			return this._ReduceNoiseDelegate(src.Handle, ref dst);
		}

		private GflBitmapFuncInt32Int32Bool _DropShadowDelegate;
		internal Gfl.Error Sepia(Bitmap src, ref IntPtr dst, int size, int depth, bool keepSize){
			this.ThrowIfDisposed();
			if(this._DropShadowDelegate == null){
				this._DropShadowDelegate = this.LoadMethod<GflBitmapFuncInt32Int32Bool>("gflDropShadow");
			}
			return this._DropShadowDelegate(src.Handle, ref dst, size, depth, keepSize);
		}

		private GflBitmapFuncFilter _ConvolveDelegate;
		internal Gfl.Error Convolve(Bitmap src, ref IntPtr dst, ref GflFilter filter){
			this.ThrowIfDisposed();
			if(this._ConvolveDelegate == null){
				this._ConvolveDelegate = this.LoadMethod<GflBitmapFuncFilter>("gflConvolve");
			}
			return this._ConvolveDelegate(src.Handle, ref dst, ref filter);
		}

		#endregion

		#region Filters Destructive

		private GflBitmapFuncDestInt32 _AverageDestDelegate;
		internal Gfl.Error Average(Bitmap src, int filterSize){
			this.ThrowIfDisposed();
			if(this._AverageDestDelegate == null){
				this._AverageDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflAverage");
			}
			return this._AverageDestDelegate(src.Handle, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _SoftenDestDelegate;
		internal Gfl.Error Soften(Bitmap src, int percentage){
			this.ThrowIfDisposed();
			if(this._SoftenDestDelegate == null){
				this._SoftenDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflSoften");
			}
			return this._SoftenDestDelegate(src.Handle, IntPtr.Zero, percentage);
		}

		private GflBitmapFuncDestInt32 _BlurDestDelegate;
		internal Gfl.Error Blur(Bitmap src, int percentage){
			this.ThrowIfDisposed();
			if(this._BlurDestDelegate == null){
				this._BlurDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflBlur");
			}
			return this._BlurDestDelegate(src.Handle, IntPtr.Zero, percentage);
		}

		private GflBitmapFuncDestInt32 _GaussianBlurDestDelegate;
		internal Gfl.Error GaussianBlur(Bitmap src, int filterSize){
			this.ThrowIfDisposed();
			if(this._GaussianBlurDestDelegate == null){
				this._GaussianBlurDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflGaussianBlur");
			}
			return this._GaussianBlurDestDelegate(src.Handle, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _MaximumDestDelegate;
		internal Gfl.Error Maximum(Bitmap src, int filterSize){
			this.ThrowIfDisposed();
			if(this._MaximumDestDelegate == null){
				this._MaximumDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflMaximum");
			}
			return this._MaximumDestDelegate(src.Handle, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _MinimumDestDelegate;
		internal Gfl.Error Minimum(Bitmap src, int filterSize){
			this.ThrowIfDisposed();
			if(this._MinimumDestDelegate == null){
				this._MinimumDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflMinimum");
			}
			return this._MinimumDestDelegate(src.Handle, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _MedianBoxDestDelegate;
		internal Gfl.Error MedianBox(Bitmap src, int filterSize){
			this.ThrowIfDisposed();
			if(this._MedianBoxDestDelegate == null){
				this._MedianBoxDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflMedianBox");
			}
			return this._MedianBoxDestDelegate(src.Handle, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _MedianCrossDestDelegate;
		internal Gfl.Error MedianCross(Bitmap src, int filterSize){
			this.ThrowIfDisposed();
			if(this._MedianCrossDestDelegate == null){
				this._MedianCrossDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflMedianCross");
			}
			return this._MedianCrossDestDelegate(src.Handle, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _SharpenDestDelegate;
		internal Gfl.Error Sharpen(Bitmap src, int percentage){
			this.ThrowIfDisposed();
			if(this._SharpenDestDelegate == null){
				this._SharpenDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflSharpen");
			}
			return this._SharpenDestDelegate(src.Handle, IntPtr.Zero, percentage);
		}

		private GflBitmapFuncDest _EnhanceDetailDestDelegate;
		internal Gfl.Error EnhanceDetail(Bitmap src){
			this.ThrowIfDisposed();
			if(this._EnhanceDetailDestDelegate == null){
				this._EnhanceDetailDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEnhanceDetail");
			}
			return this._EnhanceDetailDestDelegate(src.Handle, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EnhanceFocusDestDelegate;
		internal Gfl.Error EnhanceFocus(Bitmap src){
			this.ThrowIfDisposed();
			if(this._EnhanceFocusDestDelegate == null){
				this._EnhanceFocusDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEnhanceFocus");
			}
			return this._EnhanceFocusDestDelegate(src.Handle, IntPtr.Zero);
		}

		private GflBitmapFuncDest _FocusRestorationDestDelegate;
		internal Gfl.Error FocusRestoration(Bitmap src){
			this.ThrowIfDisposed();
			if(this._FocusRestorationDestDelegate == null){
				this._FocusRestorationDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflFocusRestoration");
			}
			return this._FocusRestorationDestDelegate(src.Handle, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EdgeDetectLightDestDelegate;
		internal Gfl.Error EdgeDetectLight(Bitmap src){
			this.ThrowIfDisposed();
			if(this._EdgeDetectLightDestDelegate == null){
				this._EdgeDetectLightDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEdgeDetectLight");
			}
			return this._EdgeDetectLightDestDelegate(src.Handle, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EdgeDetectMediumDestDelegate;
		internal Gfl.Error EdgeDetectMedium(Bitmap src){
			this.ThrowIfDisposed();
			if(this._EdgeDetectMediumDestDelegate == null){
				this._EdgeDetectMediumDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEdgeDetectMedium");
			}
			return this._EdgeDetectMediumDestDelegate(src.Handle, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EdgeDetectHeavyDestDelegate;
		internal Gfl.Error EdgeDetectHeavy(Bitmap src){
			this.ThrowIfDisposed();
			if(this._EdgeDetectHeavyDestDelegate == null){
				this._EdgeDetectHeavyDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEdgeDetectHeavy");
			}
			return this._EdgeDetectHeavyDestDelegate(src.Handle, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EmbossDestDelegate;
		internal Gfl.Error Emboss(Bitmap src){
			this.ThrowIfDisposed();
			if(this._EmbossDestDelegate == null){
				this._EmbossDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEmboss");
			}
			return this._EmbossDestDelegate(src.Handle, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EmbossMoreDestDelegate;
		internal Gfl.Error EmbossMore(Bitmap src){
			this.ThrowIfDisposed();
			if(this._EmbossMoreDestDelegate == null){
				this._EmbossMoreDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEmbossMore");
			}
			return this._EmbossMoreDestDelegate(src.Handle, IntPtr.Zero);
		}

		private GflBitmapFuncDest _ReduceNoiseDestDelegate;
		internal Gfl.Error Sepia(Bitmap src){
			this.ThrowIfDisposed();
			if(this._ReduceNoiseDestDelegate == null){
				this._ReduceNoiseDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflReduceNoise");
			}
			return this._ReduceNoiseDestDelegate(src.Handle, IntPtr.Zero);
		}

		private GflBitmapFuncDestInt32Int32Bool _DropShadowDestDelegate;
		internal Gfl.Error Sepia(Bitmap src, int size, int depth, bool keepSize){
			this.ThrowIfDisposed();
			if(this._DropShadowDestDelegate == null){
				this._DropShadowDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32Int32Bool>("gflDropShadow");
			}
			return this._DropShadowDestDelegate(src.Handle, IntPtr.Zero, size, depth, keepSize);
		}

		private GflBitmapFuncDestFilter _ConvolveDestDelegate;
		internal Gfl.Error Convolve(Bitmap src, ref GflFilter filter){
			this.ThrowIfDisposed();
			if(this._ConvolveDestDelegate == null){
				this._ConvolveDestDelegate = this.LoadMethod<GflBitmapFuncDestFilter>("gflConvolve");
			}
			return this._ConvolveDestDelegate(src.Handle, IntPtr.Zero, ref filter);
		}

		#endregion

		#region Misc

		private delegate Gfl.Error GflJpegLosslessTransformDelegate(string path, JpegLosslessTransform transform);
		private GflJpegLosslessTransformDelegate _JpegLosslessTransformDelegate;
		internal Gfl.Error JpegLosslessTransformInternal(string path, JpegLosslessTransform filter){
			this.ThrowIfDisposed();
			if(this._JpegLosslessTransformDelegate == null){
				this._JpegLosslessTransformDelegate = this.LoadMethod<GflJpegLosslessTransformDelegate>("gflJpegLosslessTransform");
			}
			return this._JpegLosslessTransformDelegate(path, filter);
		}

		#endregion

		#region Windows

		private delegate Gfl.Error ExportIntoClipboardDelegate(IntPtr src);
		private ExportIntoClipboardDelegate _ExportIntoClipboardDelegate;
		internal Gfl.Error ExportIntoClipboard(Bitmap src){
			this.ThrowIfDisposed();
			if(this._ExportIntoClipboardDelegate == null){
				this._ExportIntoClipboardDelegate = this.LoadMethod<ExportIntoClipboardDelegate>("gflExportIntoClipboard");
			}
			return this._ExportIntoClipboardDelegate(src.Handle);
		}

		private delegate Gfl.Error ImportFromClipboardDelegate(ref IntPtr dst);
		private ImportFromClipboardDelegate _ImportFromClipboardDelegate;
		internal Gfl.Error ImportFromClipboard(ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._ImportFromClipboardDelegate == null){
				this._ImportFromClipboardDelegate = this.LoadMethod<ImportFromClipboardDelegate>("gflImportFromClipboard");
			}
			return this._ImportFromClipboardDelegate(ref dst);
		}

		#endregion

		#region Delegates

		private delegate Gfl.Error GflBitmapFunc(IntPtr src, ref IntPtr dst);
		private delegate Gfl.Error GflBitmapFuncInt32(IntPtr src, ref IntPtr dst, int prm);
		private delegate Gfl.Error GflBitmapFuncInt32GflColor(IntPtr src, ref IntPtr dst, int prm, ref Gfl.GflColor color);
		private delegate Gfl.Error GflBitmapFuncInt32Int32Bool(IntPtr src, ref IntPtr dst, int prm, int prm2, bool prm3);
		private delegate Gfl.Error GflBitmapFuncFilter(IntPtr src, ref IntPtr dst, ref GflFilter filter);
		private delegate Gfl.Error GflBitmapFuncSwapColors(IntPtr src, ref IntPtr dst, SwapColorsFilter filter);

		private delegate Gfl.Error GflBitmapFuncDest(IntPtr src, IntPtr dst);
		private delegate Gfl.Error GflBitmapFuncDestInt32(IntPtr src, IntPtr dst, int prm);
		private delegate Gfl.Error GflBitmapFuncDestInt32GflColor(IntPtr src, IntPtr dst, int prm, ref Gfl.GflColor color);
		private delegate Gfl.Error GflBitmapFuncDestInt32Int32Bool(IntPtr src, IntPtr dst, int prm, int prm2, bool prm3);
		private delegate Gfl.Error GflBitmapFuncDestFilter(IntPtr src, IntPtr dst, ref GflFilter filter);
		private delegate Gfl.Error GflBitmapFuncDestSwapColors(IntPtr src, IntPtr dst, SwapColorsFilter filter);

		#endregion

		#region Structs

		internal struct GflFilter{
			public uint Size;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=7*7)]
			public UInt16[] Matrix;
			public UInt16 Divisor;
			public UInt16 Bias;
		}

		#endregion

	}
}
