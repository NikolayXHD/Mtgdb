using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DrawingEx.Drawing3D
{
	public struct Vector3
	{
		public static readonly Vector3 Empty=new Vector3();
		#region variables
		private double _x, _y, _z, _w;
		#endregion
		#region ctor
		/// <summary>
		/// constructs a new vector from the given coordinates divided by w
		/// </summary>
		public Vector3(double x,double y,double z,double w)
		{
//			if(w==0.0)
//				throw new ArgumentOutOfRangeException("w");
			_x=x/w;
			_y=y/w;
			_z=z/w;
			_w=1.0;
		}
		/// <summary>
		/// constructs a new vector from the given coordinates
		/// </summary>
		public Vector3(double x,double y,double z)
		{
			_x=x;
			_y=y;
			_z=z;
			_w=1.0;
		}
		#endregion
		#region static functions
		/// <summary>
		/// returns a new vector that keeps the direction of the old one,
		/// but its length restricted to 1 (0 respectively)
		/// </summary>
		public static Vector3 GetNormalizedVector(Vector3 value)
		{
			double length=value.GetLength();
			if (length==0.0){return Vector3.Empty;}
			return value/length;
		}
		/// <summary>
		/// calculates the angle between two vectors
		/// </summary>
		public static Angle GetAngleBetween(Vector3 a, Vector3 b)
		{
			double divisor=a.GetLength()*b.GetLength();
			if(divisor==0.0) return Angle.Empty;
			return Angle.FromRadiants(Math.Acos(
				(a._x*b._x+a._y*b._y+a._z*b._z)/divisor));
		}
		/// <summary>
		/// cross-multiplicates two vectors
		/// </summary>
		public static Vector3 Cross(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a._y*b._z-a._z*b._y,
				a._z*b._x-a._x*b._z,
				a._x*b._y-a._y*b._x);
		}
		/// <summary>
		/// gets a normal from a plane that is constructed by three points
		/// </summary>
		public static Vector3 GetNormalFromPoints(Vector3 a, Vector3 b, Vector3 c)
		{
			return Vector3.GetNormalizedVector(Vector3.Cross(a-b,b-c));
		}
		/// <summary>
		/// gets a point that is element of the line constructed by a and b,
		/// scalar representing the distance from point a
		/// </summary>
		public static Vector3 GetPointOnLine(Vector3 a, Vector3 b, float scalar)
		{
			return new Vector3(
				a._x+scalar*(b._x-a._x),
				a._y+scalar*(b._y-a._y),
				a._z+scalar*(b._z-a._z));
		}
		#endregion
		#region operators
		//calculations
		public static Vector3 operator+(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a._x+b._x,
				a._y+b._y,
				a._z+b._z);
		}
		public static Vector3 operator-(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a._x-b._x,
				a._y-b._y,
				a._z-b._z);
		}
		public static Vector3 operator*(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a._x*b._x,
				a._y*b._y,
				a._z*b._z);
		}
		public static Vector3 operator*(Vector3 value, Matrix3 mat)
		{
			return new Vector3(value._x*mat[0,0]+value._y*mat[0,1]+value._z*mat[0,2]+value._w*mat[0,3],
				value._x*mat[1,0]+value._y*mat[1,1]+value._z*mat[1,2]+value._w*mat[1,3],
				value._x*mat[2,0]+value._y*mat[2,1]+value._z*mat[2,2]+value._w*mat[2,3],
				value._x*mat[3,0]+value._y*mat[3,1]+value._z*mat[3,2]+value._w*mat[3,3]);
		}
		public static Vector3 operator*(Vector3 value, double scalar)
		{
			return new Vector3(
				value._x*scalar,
				value._x*scalar,
				value._z*scalar);
		}
		public static Vector3 operator/(Vector3 value, double scalar)
		{
			return new Vector3(
				value._x/scalar,
				value._x/scalar,
				value._z/scalar);
		}
		//equality
		public static bool operator==(Vector3 a, Vector3 b)
		{
			return a._x==b._x &&
				a._y==b._y &&
				a._z==b._z &&
				a._w==b._w;
		}
		public static bool operator!=(Vector3 a, Vector3 b)
		{
			return !(a==b);
		}
		public override bool Equals(object obj)
		{
			if(obj is Vector3)
			{
				Vector3 vct=(Vector3)obj;
				return vct==this;
			}
			return false;
		}
		public override int GetHashCode()
		{
			string representation=
				_x.ToString()+":"+
				_y.ToString()+":"+
				_z.ToString()+":"+
				_w.ToString();
			return representation.GetHashCode();
		}
		#endregion
		#region members
		/// <summary>
		/// gets the length of the vector
		/// </summary>
		public double GetLength()
		{
			return Math.Sqrt(
				_x*_x+
				_y*_y+
				_z*_z);
		}
		/// <summary>
		/// rounds the x and y coordinates tho the next integer value
		/// </summary>
		public void RoundXYInPlace()
		{
			_x=Math.Round(_x);
			_y=Math.Round(_y);
		}
		#endregion
		#region conversion
		/// <summary>
		/// represents the vector in human readable appearance
		/// </summary>
		public override string ToString()
		{
			return string.Format("Vector[\nX={0};\tY={1};\tZ={2};\tW={3}\n]",
				_x,_y,_z,_w);
		}
		/// <summary>
		/// returns a point by clamping z and w values
		/// </summary>
		public PointF ToPoint()
		{
			return new PointF(
				(float)_x,
				(float)_y);
		}
		/// <summary>
		/// converts an array of vectors to 2d points
		/// </summary>
		public static PointF[] ToPointArray(Vector3[] ptarray)
		{
			if (ptarray==null || ptarray.Length<1)
				throw new ArgumentNullException("ptarray");
			//copy points
			PointF[] ret=new PointF[ptarray.Length];
			for (int i=0; i<ptarray.Length; i++)
			{
				ret[i]=ptarray[i].ToPoint();
			}
			return ret;
		}
		#endregion
		#region properties
		/// <summary>
		/// gets or sets the x-coordinate
		/// </summary>
		public double X
		{
			get{return _x;}
			set{_x=value;}
		}
		/// <summary>
		/// gets or sets the y-coordinate
		/// </summary>
		public double Y
		{
			get{return _y;}
			set{_y=value;}
		}
		/// <summary>
		/// gets or sets the z-coordinate
		/// </summary>
		public double Z
		{
			get{return _z;}
			set{_z=value;}
		}
		/// <summary>
		/// gets or sets the scaling data
		/// </summary>
		public double W
		{
			get{return _w;}
			set{_w=value;}
		}
		#endregion
	}
}
