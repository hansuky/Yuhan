using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

//  http://www.codeproject.com/KB/WPF/ScrollSynchronization.aspx

namespace Yuhan.WPF.DsxGridCtrl
{
	public class ScrollSynchronizer : DependencyObject
	{
		public static readonly DependencyProperty       HScrollGroupProperty = DependencyProperty.RegisterAttached("HScrollGroup", typeof(string), typeof(ScrollSynchronizer), new PropertyMetadata(new PropertyChangedCallback(OnHScrollGroupChanged)) );
		public static readonly DependencyProperty       VScrollGroupProperty = DependencyProperty.RegisterAttached("VScrollGroup", typeof(string), typeof(ScrollSynchronizer), new PropertyMetadata(new PropertyChangedCallback(OnVScrollGroupChanged)) );

		private static Dictionary<ScrollViewer, string> m_hScrollViewers     = new Dictionary<ScrollViewer, string>();
		private static Dictionary<ScrollViewer, string> m_vScrollViewers     = new Dictionary<ScrollViewer, string>();

		private static Dictionary<string, double> horizontalScrollOffsets    = new Dictionary<string, double>();
		private static Dictionary<string, double> verticalScrollOffsets      = new Dictionary<string, double>();

        #region DP Accessor - SetHScrollGroup

		public static void SetHScrollGroup(DependencyObject obj, string scrollGroup)
		{
			obj.SetValue(HScrollGroupProperty, scrollGroup);
		}
        #endregion

        #region DP Accessor - GetHScrollGroup

        public static string GetHScrollGroup(DependencyObject obj)
		{
			return (string)obj.GetValue(HScrollGroupProperty);
		}
        #endregion

        #region DP Accessor - SetVScrollGroup

		public static void SetVScrollGroup(DependencyObject obj, string scrollGroup)
		{
			obj.SetValue(VScrollGroupProperty, scrollGroup);
		}
        #endregion

        #region DP Accessor - GetVScrollGroup

		public static string GetVScrollGroup(DependencyObject obj)
		{
			return (string)obj.GetValue(VScrollGroupProperty);
		}
        #endregion

        #region DP EventConsumer - OnHScrollGroupChanged

        private static void OnHScrollGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var scrollViewer = d as ScrollViewer;
			if (scrollViewer != null)
			{
				if (!string.IsNullOrEmpty((string)e.OldValue))
				{
					if (m_hScrollViewers.ContainsKey(scrollViewer))
					{
						scrollViewer.ScrollChanged -= OnHScrollChanged;
						m_hScrollViewers.Remove(scrollViewer);
					}
				}

				if (!string.IsNullOrEmpty((string)e.NewValue))
				{
					if (horizontalScrollOffsets.Keys.Contains((string)e.NewValue))
					{
						scrollViewer.ScrollToHorizontalOffset(horizontalScrollOffsets[(string)e.NewValue]);
					}
					else
					{
						horizontalScrollOffsets.Add((string)e.NewValue, scrollViewer.HorizontalOffset);
					}

					m_hScrollViewers.Add(scrollViewer, (string)e.NewValue);
					scrollViewer.ScrollChanged += OnHScrollChanged;
				}
			}
		}
        #endregion

        #region DP EventConsumer - OnVScrollGroupChanged

        private static void OnVScrollGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var scrollViewer = d as ScrollViewer;
			if (scrollViewer != null)
			{
				if (!string.IsNullOrEmpty((string)e.OldValue))
				{
					if (m_vScrollViewers.ContainsKey(scrollViewer))
					{
						scrollViewer.ScrollChanged -= OnVScrollChanged;
						m_vScrollViewers.Remove(scrollViewer);
					}
				}

				if (!string.IsNullOrEmpty((string)e.NewValue))
				{
					if (verticalScrollOffsets.Keys.Contains((string)e.NewValue))
					{
						scrollViewer.ScrollToHorizontalOffset(verticalScrollOffsets[(string)e.NewValue]);
					}
					else
					{
						verticalScrollOffsets.Add((string)e.NewValue, scrollViewer.VerticalOffset);
					}

					m_vScrollViewers.Add(scrollViewer, (string)e.NewValue);
					scrollViewer.ScrollChanged += OnVScrollChanged;
				}
			}
		}
        #endregion


        #region EventConsumer - OnHScrollChanged

		private static void OnHScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.HorizontalChange != 0)
			{
				var changedScrollViewer = sender as ScrollViewer;
				ApplyHScroll(changedScrollViewer);
			}
		}
        #endregion

        #region EventConsumer - OnVScrollChanged

		private static void OnVScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.VerticalChange != 0)
			{
				var changedScrollViewer = sender as ScrollViewer;
				ApplyVScroll(changedScrollViewer);
			}
		}
        #endregion

        #region Method - ApplyHScroll

		private static void ApplyHScroll(ScrollViewer changedScrollViewer)
		{
			var group = m_hScrollViewers[changedScrollViewer];
			horizontalScrollOffsets[group] = changedScrollViewer.HorizontalOffset;

			foreach (var scrollViewer in m_hScrollViewers.Where((s) => s.Value == group && s.Key != changedScrollViewer))
			{
				if (scrollViewer.Key.HorizontalOffset != changedScrollViewer.HorizontalOffset)
				{
					scrollViewer.Key.ScrollToHorizontalOffset(changedScrollViewer.HorizontalOffset);
				}
			}
		}
        #endregion

        #region Method - ApplyVScroll

        private static void ApplyVScroll(ScrollViewer changedScrollViewer)
		{
			var group = m_vScrollViewers[changedScrollViewer];
			verticalScrollOffsets[group] = changedScrollViewer.VerticalOffset;

			foreach (var scrollViewer in m_vScrollViewers.Where((s) => s.Value == group && s.Key != changedScrollViewer))
			{
				if (scrollViewer.Key.VerticalOffset != changedScrollViewer.VerticalOffset)
				{
					scrollViewer.Key.ScrollToVerticalOffset(changedScrollViewer.VerticalOffset);
				}
			}
		}
        #endregion
	}
}
