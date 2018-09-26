using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Reflection;

namespace DrawingEx.Drawing3D
{
	/// <summary>
	/// representing an angle
	/// </summary>
	[StructLayout(LayoutKind.Sequential),
	TypeConverter(typeof(AngleTypeConverter))]
	public struct Angle
	{
		public static readonly Angle Empty=new Angle();
		/// <summary>
		/// two times the value of PI
		/// </summary>
		public const double DoublePI=Math.PI*2.0;
		#region variables
		private double _value;
		#endregion
		#region ctor
		/// <summary>
		/// constructs an angle from a radiant value
		/// </summary>
		public Angle(double radiants)
		{
			_value=ClampAngle(radiants);
		}
		/// <summary>
		/// not needet, use constructor instead
		/// </summary>
		public static Angle FromRadiants(double radiants)
		{
			return new Angle(radiants);
		}
		/// <summary>
		/// constructs an angle from a degree value
		/// </summary>
		public static Angle FromDegree(int degree)
		{
			return new Angle(Math.PI*(double)degree/180f);
		}
		/// <summary>
		/// constructs an angle from a grad value
		/// </summary>
		public static Angle FromGrad(int grad)
		{
			return new Angle(Math.PI*(double)grad/200f);
		}
		#endregion
		#region operators
		//operators
		public static Angle operator+(Angle a, Angle b)
		{
			return new Angle(a._value+b._value);
		}
		public static Angle operator-(Angle a, Angle b)
		{
			return new Angle(a._value-b._value);
		}
		public static bool operator==(Angle a, Angle b)
		{
			return a._value==b._value;
		}
		public static bool operator !=(Angle a, Angle b)
		{
			return !(b==a);
		}
		//override object derived
		public override bool Equals(object obj)
		{
			if(obj is Angle)
			{
				return ((Angle)obj)._value==_value;
			}
			return false;
		}
		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}
		//conversion
		public static implicit operator double (Angle value)
		{
			return value._value;
		}
		public static implicit operator Angle (double value)
		{
			return new Angle(value);
		}
		#endregion
		#region static members
		public static double ClampAngle(double radiants)
		{
			if(double.IsInfinity(radiants) ||
				double.IsNaN(radiants))
				return 0.0;
			radiants=radiants%DoublePI;
			if(radiants<0f)
				return DoublePI+radiants;
			else
				return radiants;
		}
		public static double GetNearestEigth(double value)
		{
			return ClampAngle(value-Math.IEEERemainder(value,Math.PI/4.0));
		}
		#endregion
		#region conversion
		/// <summary>
		/// not needet, use value-propery instead
		/// </summary>
		public double ToRadiants()
		{
			return _value;
		}
		/// <summary>
		/// returns angle value in degree
		/// </summary>
		public int ToDegree()
		{
			return (int)Math.Round(180f*_value/Math.PI);
		}
		/// <summary>
		/// returns angle value in grad
		/// </summary>
		public int ToGrad()
		{
			return (int)Math.Round(200f*_value/Math.PI);
		}
		/// <summary>
		/// returns a human readable string representing the angle
		/// </summary>
		public override string ToString()
		{
			return _value.ToString()+" rad";
		}
		/// <summary>
		/// maps the angle to an ellipse
		/// </summary>
		public PointF MapToEllipse(RectangleF bounds)
		{
			return new PointF(
				bounds.X+bounds.Width*(1f+(float)Math.Cos(_value))/2f,
				bounds.Y+bounds.Height*(1f+(float)Math.Sin(_value))/2f);
		}
		#endregion
	}
	/// <summary>
	/// used to convert an angle value
	/// </summary>
	public class AngleTypeConverter:TypeConverter
	{
		#region convert from
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return (sourceType==typeof(string)) ||
				base.CanConvertFrom (context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			string text=value as string;
			if(text==null)
				return base.ConvertFrom (context, culture, value);
			//remove trailing spaces
			text=text.Trim();
			if(text=="") return null;
			//locate culture
			if(culture==null)
				culture=CultureInfo.CurrentCulture;
			//parse radiants
			if(text.EndsWith("rad"))
			{
				text=text.Remove(text.Length-3,3);
				text=text.Trim();
				if(text=="") return null;
				return Angle.FromRadiants(double.Parse(text,NumberStyles.Float,culture));
			}
			//parse degree
			else if(text.EndsWith("°"))
			{
				text=text.Remove(text.Length-1,1);
				text=text.Trim();
				if(text=="") return null;
				return Angle.FromDegree(int.Parse(text,NumberStyles.Integer,culture));
			}
			return null;
		}
		#endregion
		#region convert to
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return (destinationType==typeof(InstanceDescriptor)) ||
				base.CanConvertTo (context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if(value is Angle)
			{
				Angle angle=(Angle)value;
				if(destinationType==null)
					//destinationtype is needed
					throw new ArgumentNullException("destinationType");
				else if(destinationType==typeof(string))
					//string representation in degree
					return angle.ToDegree().ToString()+"°";
				else if (destinationType==typeof(InstanceDescriptor))
				{
					//get constructor for property grid
					ConstructorInfo member=typeof(Angle).GetConstructor(new Type[]{typeof(double)});
					if(member!=null)
						return new InstanceDescriptor(member,new object[]{angle.ToRadiants()});
				}
			}
			return base.ConvertTo (context, culture, value, destinationType);
		}
		#endregion
	}
}
