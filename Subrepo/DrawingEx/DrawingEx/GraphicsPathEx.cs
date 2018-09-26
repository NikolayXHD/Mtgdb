using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace DrawingEx
{
	/// <summary>
	/// encapsulates a GraphicsPath with support
	/// of single-point manipulations
	/// </summary>
	public class GraphicsPathEx
	{
		/// <summary>
		/// PathPoint which can either represent a
		/// line point or a curve point
		/// </summary>
		public struct Point
		{
			/// <summary>
			/// defines the way a figure is closed
			/// </summary>
			public enum CloseType
			{
				/// <summary>
				/// intermidate point
				/// </summary>
				None = 0,
				/// <summary>
				/// figure is closed and
				/// left open
				/// </summary>
				Close = 1,
				/// <summary>
				/// figure is closed and
				/// returned to the starting point
				/// </summary>
				Return = 2
			}
			#region variables
			private CloseType _close;
			private PointF _position,
				_controlPre, _controlPost;
			#endregion
			#region ctors
			/// <summary>
			/// constructs a new pathpoint with
			/// absolute coordinates of the control points
			/// </summary>
			public Point(PointF position,
				PointF controlpre,
				PointF controlpost)
			{
				this._position = position;
				this._controlPre = controlpre;
				this._controlPost = controlpost;
				this._close = CloseType.None;
			}
			/// <summary>
			/// constructs a new pathpoint with
			/// controlpoints set to position
			/// </summary>
			public Point(PointF position)
				: this(position, position, position) { }
			/// <summary>
			/// constructs a new pathpoint with
			/// controlpoints set to position
			/// </summary>
			public Point(float x, float y)
				: this(new PointF(x, y)) { }
			#endregion
			#region properties
			/// <summary>
			/// gets if the point is part of a bezier
			/// or can be written as line point, with
			/// both control points collapsed
			/// </summary>
			public bool isBezier
			{
				get
				{
					return _controlPost != _position ||
					_controlPre != _position;
				}
				set
				{
					if (value) _controlPre = _controlPost = _position;
				}
			}
			/// <summary>
			/// gets or sets the controlpre point in
			/// absolute coordinates
			/// </summary>
			public PointF ControlPre
			{
				get { return _controlPre; }
				set { _controlPre = value; }
			}
			/// <summary>
			/// gets or sets the controlpost point in
			/// absolute coordinates
			/// </summary>
			public PointF ControlPost
			{
				get { return _controlPost; }
				set { _controlPost = value; }
			}
			/// <summary>
			/// gets or sets the controlpost point in
			/// absolute coordinates
			/// </summary>
			public PointF Position
			{
				get { return _position; }
				set { _position = value; }
			}
			/// <summary>
			/// gets the closetype of the figure
			/// </summary>
			public CloseType Close
			{
				get { return _close; }
				set
				{
					_close = value;
				}
			}
			#endregion
		}
		#region variables
		private const byte
			START = 0x0,
			LINE = 0x1,
			BEZIER = 0x3,
			MASK = 0x7,
			MARKER = 0x20,
			RETURN = 0x80;
		private List<Point> _points;
		#endregion
		#region ctor
		/// <summary>
		/// constructs an empty graphicspathex.
		/// use the add() method to add a path.
		/// use the methods of the point array
		/// to manipulate.
		/// </summary>
		public GraphicsPathEx()
		{
			this._points = new List<Point>();
		}
		#endregion
		#region public members
		/// <summary>
		/// adds all figures of the given graphicspath
		/// to this instance
		/// </summary>
		public void Add(GraphicsPath grpath)
		{
			if (grpath == null)
				throw new ArgumentNullException();
			//call overloaded
			this.Add(grpath.PathData);
		}
		/// <summary>
		/// adds all figures of the given graphicspath
		/// data to this instance
		/// </summary>
		public void Add(PathData data)
		{
			if (data == null ||
				data.Points == null || data.Types == null ||
				data.Points.Length != data.Types.Length)
				throw new ArgumentException();
			//loop through points
			for (int i = 0; i < data.Points.Length; i++)
			{
				switch (data.Types[i] & MASK)
				{
					case START:
					case LINE:
						if (data.Types.Length > (i + 1) &&
							(data.Types[i + 1] & MASK) == BEZIER)
						{
							//start new bezier
							_points.Add(new Point
								(data.Points[i], data.Points[i], data.Points[i + 1]));
							i++;
						}
						else
						{
							//line point
							_points.Add(new Point(data.Points[i]));
						}
						break;
					case BEZIER:
						if (data.Types.Length > (i + 1) &&
							(data.Types[i + 1] & MASK) == BEZIER)
						{
							//add a bezier point
							if (data.Types.Length > (i + 2) &&
								(data.Types[i + 2] & MASK) == BEZIER)
							{
								_points.Add(new Point
									(data.Points[i + 1], data.Points[i], data.Points[i + 2]));
								i += 2;
							}
							else //add a bezier endpoint
							{
								_points.Add(new Point
								   (data.Points[i + 1], data.Points[i], data.Points[i + 1]));
								i++;
							}
						}
						//else:skip point
						break;
				}
				//close and return figure
				if (data.Types.Length > (i + 1) &&
					data.Types[i + 1] == START)
				{
					Point tmp = _points[_points.Count - 1];
					if ((data.Types[i] & RETURN) != 0)
						tmp.Close = Point.CloseType.Return;
					else
						tmp.Close = Point.CloseType.Close;
					_points[_points.Count - 1] = tmp;
				}
			}
		}
		/// <summary>
		/// copies all figures of this instance
		/// to a new GraphicsPath
		/// </summary>
		public GraphicsPath GetGraphicsPath()
		{
			PathData data = this.GetPathData();
			return new GraphicsPath(data.Points, data.Types);
		}
		/// <summary>
		/// copies all figures of this instance
		/// to a new GraphicsPathData
		/// </summary>
		public PathData GetPathData()
		{
			List<byte> types = new List<byte>();
			List<PointF> points = new List<PointF>();
			bool figureopen = false,
				beziersession = false;
			//loop through points
			for (int i = 0; i < _points.Count; i++)
			{
				Point pnt = _points[i];
				if (pnt.isBezier)
				{
					byte postype;
					//selection of type for position point
					if (figureopen && beziersession)
					{	/*controlpost*/
						types.Add(BEZIER); points.Add(pnt.ControlPre);
						postype = BEZIER;
					}
					else if (!figureopen)
					{ figureopen = true; postype = START; }
					else
					{ postype = LINE; }
					//position
					types.Add(postype);
					points.Add(pnt.Position);
					//controlpost
					if (i < _points.Count - 1 &&
						pnt.Close == Point.CloseType.None)
					{
						types.Add(BEZIER);
						points.Add(pnt.ControlPost);
					}
					beziersession = true;
				}
				else
				{
					if (!figureopen)
					{
						//open new line segment
						figureopen = true;
						types.Add(START);
					}
					else if (beziersession)
					{
						//close beziers
						types.Add(BEZIER); points.Add(pnt.Position);
						types.Add(BEZIER);
					}
					else
					{
						//add line segment
						types.Add(LINE);
					}
					points.Add(pnt.Position);
					beziersession = false;
				}
				//prepare start of new figure
				if (pnt.Close != Point.CloseType.None)
				{
					figureopen = false;
					if (pnt.Close == Point.CloseType.Return)
						types[i] |= RETURN;
				}
			}
			//return values
			PathData data = new PathData();
			data.Types = types.ToArray();
			data.Points = points.ToArray();
			return data;
		}
		#endregion
		#region properties
		/// <summary>
		/// gets if the instace has no data to draw
		/// </summary>
		public bool IsEmpty
		{
			get { return _points.Count < 2; }
		}
		/// <summary>
		/// gets the list of points which can be manipulated
		/// </summary>
		public List<Point> Points
		{
			get { return this._points; }
		}
		#endregion
	}
}
