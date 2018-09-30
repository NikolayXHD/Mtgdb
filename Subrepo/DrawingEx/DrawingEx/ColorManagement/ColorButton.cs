using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Resources;
using System.Runtime.InteropServices;

namespace DrawingEx.ColorManagement
{
	/// <summary>
	/// Zusammenfassung für ColorButton.
	/// </summary>
	public class ColorButton:Button
	{
		#region variables
		//painting
		private SolidBrush _colbrs=new SolidBrush(Color.Transparent);
		//control
		private Color _color=Color.Black;
		private byte _alpha=255;
		private bool _tracking=false,
			_hexdisplay=false,
			_screen=false;
		private Bitmap _bmp;
		#endregion
		/// <summary>
		/// ctor
		/// </summary>
		public ColorButton()
		{
			base.FlatStyle=FlatStyle.System;
			this.UpdateText();
			ResourceManager man=new ResourceManager(typeof(ColorButton));
			_bmp=(Bitmap)man.GetObject("tl_picker.png");

			this.SetStyle(ControlStyles.DoubleBuffer |
				ControlStyles.AllPaintingInWmPaint,true);
			this.UpdateText();
			this.UpdateColor();
		}
		#region helper
		/// <summary>
		/// updates the text
		/// </summary>
		private void UpdateText()
		{
			base.Text="    "+this.ColorToString(_colbrs.Color);
		}
		/// <summary>
		/// updates the colorbrush
		/// </summary>
		private void UpdateColor()
		{
			_colbrs.Color=Color.FromArgb(_alpha,_color);
		}
		/// <summary>
		/// returns the string according to the current format
		/// </summary>
		private string ColorToString(Color col)
		{
			if (_hexdisplay)
			{
				return string.Format("{0:X6}",
					col.ToArgb()&0xFFFFFF,16);
			}
			else
			{
				return col.R.ToString()+","+
					col.G.ToString()+","+
					col.B.ToString();
			}
		}
		#endregion
		#region controller
		protected override void WndProc(ref Message m)
		{
			base.WndProc (ref m);
			if (m.Msg==0xF)
			{
				using(Graphics gr=this.CreateGraphics())
				{
					if(_alpha<255)
					{
						using(HatchBrush hbrs=new HatchBrush(HatchStyle.SmallCheckerBoard,
							Color.Silver,Color.White))
						{
							gr.FillRectangle(hbrs,//draw checker
								5,5,this.Height-10,this.Height-10);
						}
					}
					gr.FillRectangle(_colbrs,//draw color
						5,5,this.Height-10,this.Height-10);
					if (_screen)//draw picker
						gr.DrawImageUnscaled(_bmp,
							5+(this.Height/2)-13,
							5+(this.Height/2)-13);
				}
			}
		}
		#region mouse
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);
			_tracking=true;
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove (e);
			if (_tracking)
			{
				_screen=!this.ClientRectangle.Contains(
					this.PointToClient(Control.MousePosition));
				Color c=_colbrs.Color;
				if (_screen)//pick color from screen
				{
					_colbrs.Color=GDI32.GetScreenPixel(Control.MousePosition.X,
						Control.MousePosition.Y);
				}
				this.Cursor=_screen?Cursors.Cross:Cursors.Default;
				UpdateText();
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			this.Cursor=Cursors.Default;
			if (_screen)//update color
				this.Color=_colbrs.Color;
			else if((ModifierKeys&Keys.Control)!=0)//copy to clipboard
			{
				Clipboard.SetDataObject(this.ColorToString(_colbrs.Color),true);
			}
			//else use click-event
			_tracking=_screen=false;
		}
		#endregion
		#endregion
		#region properties
		/// <summary>
		/// specifies if the colors are displayed in hexcode
		/// </summary>
		[Description("specifies if the colors are displayed in hexcode"),
		DefaultValue(false)]
		public bool DisplayHex
		{
			get{return _hexdisplay;}
			set{_hexdisplay=value; UpdateText();}
		}
		/// <summary>
		/// specifies the color without alpha
		/// </summary>
		[Description("specifies the color without alpha"),
		DefaultValue(typeof(Color),"0,0,0")]
		public Color Color
		{
			get{return _color;}
			set
			{
				value=Color.FromArgb(255,value);
				if (value==_color) return;
				_color=value;
				this.UpdateColor();
				this.UpdateText();
				this.Refresh();
				this.RaiseColorChanged();
			}
		}
		/// <summary>
		/// sets the color with alpha
		/// </summary>
		public void SetColorAlpha(Color value)
		{
			if (value==_colbrs.Color) return;
			_color=Color.FromArgb(255,value);
			_alpha=value.A;

			this.UpdateColor();
			this.UpdateText();
			this.Refresh();
			this.RaiseColorChanged();
		}
		/// <summary>
		/// specifies the alpha value of the color
		/// </summary>
		[Description("specifies the alpha value of the color"),
		DefaultValue(255)]
		public byte Alpha
		{
			get{return _alpha;}
			set
			{
				if (value==_alpha) return;
				_alpha=value;
				this.Refresh();
				this.RaiseColorChanged();
			}
		}
		//protect
		[Browsable(false)]
		public new FlatStyle FlatStyle
		{
			get{return FlatStyle.System;}
			set{return;}
		}
		[Browsable(false)]
		public override string Text
		{
			get{return "";}
			set{return;}
		}
		#endregion
		#region events
		//color changed
		public void RaiseColorChanged()
		{
			if (ColorChanged!=null)
				ColorChanged(this,new EventArgs());
		}
		public event EventHandler ColorChanged;
		#endregion
	}
}
