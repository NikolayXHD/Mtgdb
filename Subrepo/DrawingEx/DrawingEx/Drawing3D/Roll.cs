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
	/// parameters for a view-to matrix
	/// </summary>
	[StructLayout(LayoutKind.Sequential),
	TypeConverter(typeof(RollTypeConverter))]
	public struct Roll
	{
		public static readonly Roll Empty=new Roll();
		#region variables
		private Angle _rolldirection,
			_rollamount;
		#endregion
		/// <summary>
		/// ctor
		/// </summary>
		public Roll(Angle direction, Angle amount)
		{
			_rolldirection=direction;
			_rollamount=Math.Min(Math.PI/2.0,amount);
		}
		#region operators
		public static bool operator==(Roll a, Roll b)
		{
			return a._rollamount==b._rollamount &&
				a._rolldirection==b._rolldirection;
		}
		public static bool operator!=(Roll a, Roll b)
		{
			return !(a==b);
		}
		public override bool Equals(object obj)
		{
			if(obj is Roll)
			{
				return ((Roll)obj)==this;
			}
			return base.Equals (obj);
		}
		public override int GetHashCode()
		{
			string representation=_rollamount.ToString()+":"+
				_rolldirection.ToString();
			return representation.GetHashCode();
		}
		#endregion
		#region conversion
		public Matrix3 GetMatrix()
		{
			return
				Matrix3.RotateZ(RollDirection)*Matrix3.RotateY(RollAmount)*
				Matrix3.RotateZ(-RollDirection);
		}
		public Point MapToCircle(double radius)
		{
			double amount=radius*2.0*_rollamount/Math.PI;
			return new Point(
				(int)Math.Round(radius+amount*Math.Cos(_rolldirection)),
				(int)Math.Round(radius+amount*Math.Sin(_rolldirection)));
		}
		public override string ToString()
		{
			return string.Format("Roll[\nDirection={0};\tAmount={1};\n]",
				RollDirection,RollAmount);
		}
		#endregion
		#region properties
		/// <summary>
		/// gets or sets the rolldirection
		/// </summary>
		[Description("gets or sets the rolldirection")]
		public Angle RollDirection
		{
			get{return _rolldirection;}
			set{_rolldirection=value;}
		}
		/// <summary>
		/// gets or sets the rollamount
		/// </summary>
		[Description("gets or sets the rollamount")]
		public Angle RollAmount
		{
			get{return _rollamount;}
			set{_rollamount=Math.Max(0.0,Math.Min(Math.PI/2.0,value));}
		}
		#endregion
		#region static functions
		public static double Square(double value)
		{
			return value*value;
		}
		#endregion
	}
	/// <summary>
	/// used for converting a roll struct
	/// </summary>
	public class RollTypeConverter:TypeConverter
	{
		#region canconvert
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return (sourceType==typeof(string)) ||
				base.CanConvertFrom (context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return (destinationType==typeof(InstanceDescriptor)) ||
				base.CanConvertTo (context, destinationType);
		}
		#endregion
		#region convert
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
			string[] parts=text.Split(
				culture.TextInfo.ListSeparator.ToCharArray());
			if(parts.Length!=2) return null;
			TypeConverter converter=
				TypeDescriptor.GetConverter(typeof(Angle));
			return new Roll(
				(Angle)converter.ConvertFromString(parts[0]),
				(Angle)converter.ConvertFromString(parts[1]));
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if(value is Roll)
			{
				//locate culture
				if(culture==null)
					culture=CultureInfo.CurrentCulture;

				Roll roll=(Roll)value;
				if(destinationType==null)
					//destinationtype is needed
					throw new ArgumentNullException("destinationType");
				else if(destinationType==typeof(string))
				{
					TypeConverter converter=
						TypeDescriptor.GetConverter(typeof(Angle));
					return string.Join(culture.TextInfo.ListSeparator+" ",
						new string[]
								{
									converter.ConvertToString(context,culture,roll.RollDirection),
									converter.ConvertToString(context,culture,roll.RollAmount)
								});
				}
				else if (destinationType==typeof(InstanceDescriptor))
				{
					//get constructor for property grid
					ConstructorInfo member=typeof(Roll).GetConstructor(new Type[]{typeof(Angle),typeof(Angle)});
					if(member!=null)
						return new InstanceDescriptor(member,new object[]{roll.RollDirection,roll.RollAmount});
				}
			}
			return base.ConvertTo (context, culture, value, destinationType);
		}
		#endregion
		#region getproperties
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(Roll),attributes).Sort(new string[]{"RollDirection","RollAmount"});
		}
		#endregion
		#region createinstance
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			if(propertyValues==null)
				throw new ArgumentNullException("propertyValues");
			//check for right properties
			object rolldir=propertyValues["RollDirection"],
				rollam=propertyValues["RollAmount"];
			if(rolldir==null || !(rolldir is Angle) ||
				rollam==null || !(rollam is Angle))
				throw new ArgumentException("propertyValues is invalid");
			//create struct
			return new Roll((Angle)rolldir,(Angle)rollam);
		}
		#endregion
	}
}
