using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using DrawingEx.ColorManagement.ColorModels;
namespace DrawingEx.ColorManagement
{
	[DefaultEvent("PositionChanged")]
	public class ColorFader:Control
	{
		#region variables
		//painting
		private HatchBrush _checker=new HatchBrush(HatchStyle.LargeCheckerBoard, Color.Silver,Color.White);
		private LinearGradientBrush _grd=new LinearGradientBrush(new Point(0,0),new Point(1,0),Color.Black,Color.Transparent);
		//control
		private Orientation _orientation=Orientation.Horizontal;
		private int _position=50;
		private bool _highlighted=false;
		private FadeType _fadetype=FadeType.Internal;
		#endregion
		/// <summary>
		/// ctor
		/// </summary>
		public ColorFader()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.ResizeRedraw |
				ControlStyles.UserPaint, true);
		}
		#region controller
		//paint slider
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Rectangle bandrct=this.GetBandRectangle();
			if(bandrct.Width<1|| bandrct.Height<1) return;
			//forecolor adjust
			Color forecolor=
				this._fadetype==FadeType.Internal?
				ColorUtility.AoverB(this.GetSelectedColor(),Color.Silver)://blended with checker
				this.BackColor;
			if(_highlighted)
				forecolor=ColorUtility.MaxContrastRBTo(forecolor);//color
			else
				forecolor=ColorUtility.MaxContrastTo(forecolor);//blackwhite
			int position;
			//start painting
			switch(_orientation)
			{
				case Orientation.Horizontal:
					#region horizontal
					//horizontal stretching
					_grd.Transform=new Matrix(
						(float)bandrct.Width+2f,0f,0f,1f,(float)bandrct.X-1f,0f);
					//fill band
					e.Graphics.FillRectangle(_checker,bandrct);
					e.Graphics.FillRectangle(_grd,bandrct);
					//draw position
					e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
					position=bandrct.X+(bandrct.Width*_position)/100;
					if(_fadetype==FadeType.External)//triangle select
					{
						using (SolidBrush sld=new SolidBrush(forecolor))
						{
							e.Graphics.FillPolygon(sld,new Point[]{//upper triangle
																	  new Point(position+3,0),
																	  new Point(position,5),
																	  new Point(position-3,0)
																  });
							e.Graphics.FillPolygon(sld,new Point[]{//lower triangle
																	  new Point(position+3,this.Height-1),
																	  new Point(position,this.Height-6),
																	  new Point(position-3,this.Height-1)
																  });
						}
					}
					else//bracket select
					{
						using (Pen pn=new Pen(forecolor))
						{
							e.Graphics.DrawLines(pn,new Point[]{//left bracket
																   new Point(position-1,0),
																   new Point(position-3,0),
																   new Point(position-3,this.Height-1),
																   new Point(position-1,this.Height-1)
															   });
							e.Graphics.DrawLines(pn,new Point[]{//right bracket
																   new Point(position+1,0),
																   new Point(position+3,0),
																   new Point(position+3,this.Height-1),
																   new Point(position+1,this.Height-1)
															   });

						}
					}

					#endregion
					break;
				case Orientation.Vertical:
					#region vertical
					//horizontal stretching
					//vertical stretching and rotating
					_grd.Transform=new Matrix(
						0f,(float)bandrct.Width+2f,1f,0f,0f,(float)bandrct.Y+1f);
					//fill band
					e.Graphics.FillRectangle(_checker,bandrct);
					e.Graphics.FillRectangle(_grd,bandrct);
					//draw position
					e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
					position=bandrct.Y+(bandrct.Height*_position)/100;
					if(_fadetype==FadeType.External)//triangle select
					{
						using (SolidBrush sld=new SolidBrush(forecolor))
						{
							e.Graphics.FillPolygon(sld,new Point[]{//left triangle
																	  new Point(0,position+3),
																	  new Point(5,position),
																	  new Point(0,position-3)
																  });
							e.Graphics.FillPolygon(sld,new Point[]{//lower triangle
																	  new Point(this.Width-1,position+3),
																	  new Point(this.Width-6,position),
																	  new Point(this.Width-1,position-3)
																  });
						}
					}
					else//bracket select
					{
						using (Pen pn=new Pen(forecolor))
						{
							e.Graphics.DrawLines(pn,new Point[]{//left bracket
																   new Point(0,position-1),
																   new Point(0,position-3),
																   new Point(this.Width-1,position-3),
																   new Point(this.Width-1,position-1)
															   });
							e.Graphics.DrawLines(pn,new Point[]{//right bracket
																   new Point(0,position+1),
																   new Point(0,position+3),
																   new Point(this.Width-1,position+3),
																   new Point(this.Width-1,position+1)
															   });

						}
					}
					#endregion
					break;
			}
		}
		//stick to mouse
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);
			SetPositionToPoint(new Point(e.X,e.Y));
		}
		//move slider
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove (e);
			if(e.Button!=MouseButtons.None)
			{
				if(SetPositionToPoint(new Point(e.X,e.Y)))
					RaiseScroll();
			}
		}
		//raise positionchanged
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			if(SetPositionToPoint(new Point(e.X,e.Y)))
				RaisePositionChanged();
		}
		//set higlight
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter (e);
			this._highlighted=true;
			this.Refresh();
		}
		//reset higlight
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave (e);
			this._highlighted=false;
			this.Refresh();
		}
		#endregion
		#region members
		/// <summary>
		/// gets the rectangle of the slider
		/// </summary>
		public Rectangle GetBandRectangle()
		{
			Rectangle ret=this.ClientRectangle;
			if(_fadetype==FadeType.External)
			{
				if(_orientation==Orientation.Horizontal)
					ret.Inflate(-4,-5);
				else
					ret.Inflate(-5,-4);
			}
			return ret;
		}
		/// <summary>
		/// sets the slider position to the specified point and returns TRUE if successful
		/// </summary>
		public bool SetPositionToPoint(Point clientpos)
		{
			Rectangle bandrct=GetBandRectangle();
			int pos;
			if(_orientation==Orientation.Horizontal)
			{
				if(bandrct.Width<2) return false;

				pos=Math.Max(0,Math.Min(100,
					((clientpos.X-bandrct.X)*100)/bandrct.Width));
			}
			else
			{
				if(bandrct.Height<2) return false;

				pos=Math.Max(0,Math.Min(100,
					((clientpos.Y-bandrct.Y)*100)/bandrct.Height));
			}
			if(pos!=_position)
			{
				_position=pos;
				this.Refresh();
				return true;
			}
			return false;
		}
		/// <summary>
		/// gets the color the slider is at
		/// </summary>
		public Color GetSelectedColor()
		{
			return ColorUtility.BlendOver(
				this.FirstColor,
				this.SecondColor,
				(float)_position/100f);
		}
		#endregion
		#region properties
		/// <summary>
		/// gets or sets the orientation of the fader
		/// </summary>
		[Category("ColorFader properties"),
		Description("gets or sets the orientation of the fader"),
		DefaultValue(typeof(Orientation),"Horizontal")]
		public Orientation Orientation
		{
			get{return _orientation;}
			set
			{
				if(_orientation==value) return;
				_orientation=value;
				this.Refresh();
			}
		}
		/// <summary>
		/// gets or sets the fader type
		/// </summary>
		[Category("ColorFader properties"),
		Description("gets or sets the fader type"),
		DefaultValue(typeof(FadeType),"Internal")]
		public FadeType FadeType
		{
			get{return _fadetype;}
			set
			{
				if(_fadetype==value) return;
				_fadetype=value;
				this.Refresh();
			}
		}
		/// <summary>
		/// gets or sets the position of the fader (0-100)
		/// </summary>
		[Category("ColorFader properties"),
		Description("gets or sets the position of the fader (0-100)"),
		DefaultValue(50)]
		public int Position
		{
			get{return _position;}
			set
			{
				value=Math.Max(0,Math.Min(100,value));
				if(_position==value) return;
				_position=value;
				this.Refresh();
			}
		}
		/// <summary>
		/// gets or sets the color that is displayed at top or respectively at left end
		/// </summary>
		[Category("ColorFader properties"),
		Description("gets or sets the color that is displayed at top or respectively at left end"),
		DefaultValue(typeof(Color),"0,0,0")]

		public Color FirstColor
		{
			get{return _grd.LinearColors[0];}
			set
			{
				if(_grd.LinearColors[0]==value) return;
				_grd.LinearColors=new Color[]{
													  value,
													  _grd.LinearColors[1]
												  };
				this.Refresh();
			}
		}
		/// <summary>
		/// gets or sets the color that is displayed at bottom or respectively at right end
		/// </summary>
		[Category("ColorFader properties"),
		Description("gets or sets the color that is displayed at bottom or respectively at right end"),
		DefaultValue(typeof(Color),"0,255,255,255")]
		public Color SecondColor
		{
			get{return _grd.LinearColors[1];}
			set
			{
				if(_grd.LinearColors[1]==value) return;
				_grd.LinearColors=new Color[]{
													  _grd.LinearColors[0],
													  value
												  };
				this.Refresh();
			}
		}
		#endregion
		#region events
		//scrolling is while the mouse is moving
		private void RaiseScroll()
		{
			if(this.Scroll!=null)
				this.Scroll(this,EventArgs.Empty);
		}
		public event EventHandler Scroll;
		//positionchanged is when the position is dropped
		private void RaisePositionChanged()
		{
			if(this.PositionChanged!=null)
				this.PositionChanged(this,EventArgs.Empty);
		}
		public event EventHandler PositionChanged;
		#endregion
	}
	/// <summary>
	/// the display type of colorfader control
	/// </summary>
	public enum FadeType
	{
		/// <summary>
		/// fade position is displayed through two brackets
		/// </summary>
		Internal=0,
		/// <summary>
		/// fade band is smaller and fade position is displayed
		/// trough two triangles
		/// </summary>
		External=1
	}
}
