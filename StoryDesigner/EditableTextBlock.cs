/* Copyright: Jöran Malek */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace StoryDesigner
{
	/// <summary>
	/// Editable textblock which displays a textbox on double click or F2.
	/// </summary>
	[TemplatePart(Type = typeof(Grid), Name = EditableTextBlock.GRID_NAME)]
	[TemplatePart(Type = typeof(TextBlock), Name = EditableTextBlock.TEXTBLOCK_DISPLAYTEXT_NAME)]
	[TemplatePart(Type = typeof(TextBox), Name = EditableTextBlock.TEXTBOX_EDITTEXT_NAME)]
	public class EditableTextBlock : Control
	{
		private const string GRID_NAME = "PART_GridContainer";
		private const string TEXTBLOCK_DISPLAYTEXT_NAME = "PART_TbDisplayText";
		private const string TEXTBOX_EDITTEXT_NAME = "PART_TbEditText";
		private Grid m_GridContainer;
		private TextBlock m_TextBlockDisplayText;
		private TextBox m_TextBoxEditText;
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlock), new UIPropertyMetadata(string.Empty));
		public Brush TextBlockForegroundColor
		{
			get { return (Brush)GetValue(TextBlockForegroundColorProperty); }
			set { SetValue(TextBlockForegroundColorProperty, value); }
		}
		public static readonly DependencyProperty TextBlockForegroundColorProperty = DependencyProperty.Register("TextBlockForegroundColor", typeof(Brush), typeof(EditableTextBlock), new UIPropertyMetadata(null));
		public Brush TextBlockBackgroundColor
		{
			get { return (Brush)GetValue(TextBlockBackgroundColorProperty); }
			set { SetValue(TextBlockBackgroundColorProperty, value); }
		}
		public static readonly DependencyProperty TextBlockBackgroundColorProperty = DependencyProperty.Register("TextBlockBackgroundColor", typeof(Brush), typeof(EditableTextBlock), new UIPropertyMetadata(null));
		public Brush TextBoxForegroundColor
		{
			get { return (Brush)GetValue(TextBoxForegroundColorProperty); }
			set { SetValue(TextBoxForegroundColorProperty, value); }
		}
		public static readonly DependencyProperty TextBoxForegroundColorProperty = DependencyProperty.Register("TextBoxForegroundColor", typeof(Brush), typeof(EditableTextBlock), new UIPropertyMetadata(null));
		public Brush TextBoxBackgroundColor
		{
			get { return (Brush)GetValue(TextBoxBackgroundColorProperty); }
			set { SetValue(TextBoxBackgroundColorProperty, value); }
		}
		public static readonly DependencyProperty TextBoxBackgroundColorProperty = DependencyProperty.Register("TextBoxBackgroundColor", typeof(Brush), typeof(EditableTextBlock), new UIPropertyMetadata(null));
		static EditableTextBlock()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableTextBlock), new FrameworkPropertyMetadata(typeof(EditableTextBlock)));
		}


		public bool Multiline
		{
			get { return (bool)GetValue(MultilineProperty); }
			set { SetValue(MultilineProperty, value); }
		}

		public static readonly DependencyProperty MultilineProperty = DependencyProperty.Register("Multiline", typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(false));

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.F2)
			{
				SetFocus();
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.m_GridContainer = this.Template.FindName(GRID_NAME, this) as Grid;
			if (this.m_GridContainer != null)
			{
				this.m_TextBlockDisplayText = this.m_GridContainer.Children[0] as TextBlock;
				this.m_TextBoxEditText = this.m_GridContainer.Children[1] as TextBox;
				this.m_TextBoxEditText.LostFocus += this.OnTextBoxLostFocus;
				this.m_TextBoxEditText.KeyDown += this.OnTextBoxKeyDown;
			}
		}

		private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
		{
			if ((!Multiline || e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)) && e.Key == Key.Return)
			{
				Dispatcher.BeginInvoke(DispatcherPriority.Input,
					new Action(delegate ()
					{
						this.Focus();
					}));
			}
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			SetFocus();
		}
		private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
		{
			this.m_TextBlockDisplayText.Visibility = Visibility.Visible;
			this.m_TextBoxEditText.Visibility = Visibility.Hidden;
		}

		private void SetFocus()
		{
			this.m_TextBlockDisplayText.Visibility = Visibility.Hidden;
			this.m_TextBoxEditText.Visibility = Visibility.Visible;
			Dispatcher.BeginInvoke(DispatcherPriority.Input,
				new Action(delegate ()
				{
					this.m_TextBoxEditText.Focus();
					this.m_TextBoxEditText.SelectAll();
					Keyboard.Focus(this.m_TextBoxEditText);
				}));
		}
	}
}
