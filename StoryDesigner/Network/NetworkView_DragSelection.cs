/* Copyright: Ashley Davis (http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a) */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoryDesigner.Network
{
	public partial class NetworkView
	{
		private static readonly double DragThreshold = 5;
		private List<NodeItem> cachedSelectedNodeItems = null;
		private FrameworkElement dragSelectionBorder = null;
		private FrameworkElement dragSelectionCanvas = null;
		private bool isControlAndLeftMouseButtonDown = false;
		private bool isDraggingSelectionRect = false;
		private Point origMouseDownPoint;
		private bool isShiftPressed = false;
		private bool moveCamera = false;

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.LeftShift)
			{
				isShiftPressed = true;
			}
			base.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.Key == Key.LeftShift)
			{
				isShiftPressed = false;
			}
			base.OnKeyUp(e);
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.Focus();
			}
			if (e.ChangedButton == MouseButton.Middle || (isShiftPressed && e.ChangedButton == MouseButton.Left))
			{
				moveCamera = true;
				origMouseDownPoint = e.GetPosition(this);
				this.CaptureMouse();
			}
			base.OnPreviewMouseDown(e);
		}

		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			if (moveCamera)
			{
				var current = e.GetPosition(this);
				Camera += current - origMouseDownPoint;
				origMouseDownPoint = current;
				e.Handled = true;
			}
			base.OnPreviewMouseMove(e);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			moveCamera = false;
			this.ReleaseMouseCapture();
			base.OnMouseUp(e);
		}
	}
}
