using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPF.MDI {
	public class MdiArranger : Arranger{
		public Arranger BaseArranger{get; private set;}
		public MdiArranger(Arranger baseArranger){
			if(baseArranger == null){
				throw new ArgumentNullException("baseArranger");
			}
			this.BaseArranger = baseArranger;
		}

		public override IEnumerable<Rect> Arrange(Size containerSize, int count) {
			return this.BaseArranger.Arrange(containerSize, count);
		}

		public void Arrange(MdiContainer mdiContainer){
			this.Arrange(new Size(mdiContainer.ContainerWidth, mdiContainer.ContainerHeight), mdiContainer.Children);
		}

		public virtual void Arrange(Size containerSize, IEnumerable<MdiChild> children){
			List<MdiChild> minimizedWindows = new List<MdiChild>(),
				normalWindows = new List<MdiChild>();
			foreach (MdiChild mdiChild in children){
				switch (mdiChild.WindowState){
					case WindowState.Minimized:
						minimizedWindows.Add(mdiChild);
						break;
					case WindowState.Maximized:
						mdiChild.WindowState = WindowState.Normal;
						normalWindows.Add(mdiChild);
						break;
					default:
						normalWindows.Add(mdiChild);
						break;
				}
			}

			var comp = new MdiContainer.MdiChildComparer();
			minimizedWindows.Sort(comp);
			normalWindows.Sort(comp);

			var containerHeight = this.ArrangeMinimizedWindows(containerSize, minimizedWindows);
			var i = 0;
			foreach(var rect in this.BaseArranger.Arrange(new Size(containerSize.Width, containerHeight), normalWindows.Count)){
				var mdiChild = normalWindows[i];
				if(mdiChild.IsResizable){
					mdiChild.Width = rect.Width;
					mdiChild.Height = rect.Height;
				}
				mdiChild.Top = rect.Y;
				mdiChild.Left = rect.X;
				i++;
			}
		}

		public virtual double ArrangeMinimizedWindows(Size containerSize, IEnumerable<MdiChild> minimizedWindows){
			double containerHeight = containerSize.Height;
			var i = 0;
			foreach(var mdiChild in minimizedWindows){
				int capacity = Convert.ToInt32(containerSize.Width) / MdiChild.MinimizedWidth,
					row = i / capacity + 1,
					col = i % capacity;
				containerHeight = containerSize.Height - MdiChild.MinimizedHeight * row;
				double newLeft = MdiChild.MinimizedWidth * col;
				mdiChild.Left = newLeft;
				mdiChild.Top = containerHeight;
				i++;
			}
			return containerHeight;
		}

		private static WeakReference _CascadeMdiArranger;
		public static MdiArranger CascadeMdiArranger{
			get{
				MdiArranger arranger = (_CascadeMdiArranger != null) ? (MdiArranger)_CascadeMdiArranger.Target : null;
				if(arranger == null){
					arranger = new MdiArranger(new CascadeArranger());
					_CascadeMdiArranger = new WeakReference(arranger);
					return arranger;
				}else{
					return arranger;
				}
			}
		}

		private static WeakReference _TileHorizontalMdiArranger;
		public static MdiArranger TileHorizontalMdiArranger{
			get{
				MdiArranger arranger = (_TileHorizontalMdiArranger != null) ? (MdiArranger)_TileHorizontalMdiArranger.Target : null;
				if(arranger == null){
					arranger = new MdiArranger(new TileHorizontalArranger());
					_TileHorizontalMdiArranger = new WeakReference(arranger);
					return arranger;
				}else{
					return arranger;
				}
			}
		}

		private static WeakReference _TileVerticalMdiArranger;
		public static MdiArranger TileVerticalMdiArranger{
			get{
				MdiArranger arranger = (_TileVerticalMdiArranger != null) ? (MdiArranger)_TileVerticalMdiArranger.Target : null;
				if(arranger == null){
					arranger = new MdiArranger(new TileVerticalArranger());
					_TileVerticalMdiArranger = new WeakReference(arranger);
					return arranger;
				}else{
					return arranger;
				}
			}
		}

		private static WeakReference _StackHorizontalMdiArranger;
		public static MdiArranger StackHorizontalMdiArranger{
			get{
				MdiArranger arranger = (_StackHorizontalMdiArranger != null) ? (MdiArranger)_StackHorizontalMdiArranger.Target : null;
				if(arranger == null){
					arranger = new MdiArranger(new StackHorizontalArranger());
					_StackHorizontalMdiArranger = new WeakReference(arranger);
					return arranger;
				}else{
					return arranger;
				}
			}
		}

		private static WeakReference _StackVerticalMdiArranger;
		public static MdiArranger StackVerticalMdiArranger{
			get{
				MdiArranger arranger = (_StackVerticalMdiArranger != null) ? (MdiArranger)_StackVerticalMdiArranger.Target : null;
				if(arranger == null){
					arranger = new MdiArranger(new StackVerticalArranger());
					_StackVerticalMdiArranger = new WeakReference(arranger);
					return arranger;
				}else{
					return arranger;
				}
			}
		}
	}
}
