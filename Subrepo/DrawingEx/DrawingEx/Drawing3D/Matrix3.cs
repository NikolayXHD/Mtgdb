using System;

namespace DrawingEx.Drawing3D
{
	/// <summary>
	/// represents a 4x4 matrix for 3d operations
	/// </summary>
	public class Matrix3
	{
		#region variables
		private double[,] _mat;
		#endregion
		#region constructors
		/// <summary>
		/// constructs a new empty matrix
		/// </summary>
		public Matrix3()
		{
			_mat=new double[4,4];
		}
		/// <summary>
		/// constructs a matrix from a given 4x4 double array
		/// </summary>
		public Matrix3(double[,] members)
		{
			if(members==null ||
				members.GetUpperBound(0)!=3 ||
				members.GetUpperBound(1)!=3)
				throw new ArgumentOutOfRangeException("members");
			_mat=new double[4,4];
			for(int j=0; j<4; j++)
				for(int i=0; i<4; i++)
					_mat[i,j]=members[i,j];
		}	
		#endregion
		#region static constructors
		/// <summary>
		/// returns a matrix that will have no transforming on points
		/// </summary>
		public static Matrix3 Identity
		{
			get
			{
				Matrix3 res=new Matrix3();
				res[0,0]=1.0;
				res[1,1]=1.0;
				res[2,2]=1.0;
				res[3,3]=1.0;
				return res;
			}
		}
		/// <summary>
		/// returns a matrix that will transform points around the X axis with the specified angle
		/// </summary>
		public static Matrix3 RotateX(Angle value)
		{
			Matrix3 res=new Matrix3();
			res[0,0]=1.0;
			res[1,1]=Math.Cos(value); res[2,1]=Math.Sin(value);
			res[1,2]=-res[2,1];res[2,2]=res[1,1];
			res[3,3]=1.0;
			return res;
		}
		/// <summary>
		/// returns a matrix that will transform points around the Y axis with the specified angle
		/// </summary>
		public static Matrix3 RotateY(Angle value)
		{
			Matrix3 res=new Matrix3();
			res[0,0]=Math.Cos(value); res[2,0]=-Math.Sin(value);
			res[1,1]=1.0;
			res[0,2]=-res[2,0]; res[2,2]=res[0,0];
			res[3,3]=1.0;
			return res;
		}
		/// <summary>
		/// returns a matrix that will transform points around the Z axis with the specified angle
		/// </summary>
		public static Matrix3 RotateZ(Angle value)
		{
			Matrix3 res=new Matrix3();
			res[0,0]=Math.Cos(value); res[0,1]=Math.Sin(value);
			res[1,0]=-res[0,1]; res[1,1]=res[0,0];
			res[2,2]=1.0;
			res[3,3]=1.0;
			return res;
		}
		/// <summary>
		/// returns a matrix that will transform points around the X axis with the specified angle
		/// </summary>
		public static Matrix3 Projection(double clipnear, double clipfar)
		{
			double q=clipfar/(clipfar-clipnear);
			if(double.IsInfinity(q))
				throw new ArgumentOutOfRangeException("clipnear");
			//create matrix
			Matrix3 res=new Matrix3();
			res[0,0]=1.0;
			res[1,1]=1.0;
			res[2,2]=q;
			res[3,2]=1.0;
			res[2,3]=-clipnear*q;
			return res;
		}
		/// <summary>
		/// returns a matrix that will scale points by the given factor
		/// </summary>
		public static Matrix3 ScaleAll(double scalar)
		{
			return Matrix3.Scale(new Vector3(scalar,scalar,scalar));
		}
		/// <summary>
		/// returns a matrix that will scale points by the given vector
		/// </summary>
		public static Matrix3 Scale(Vector3 value)
		{
			Matrix3 res=new Matrix3();
			res[0,0]=value.X;
			res[1,1]=value.Y;
			res[2,2]=value.Z;
			res[3,3]=1.0;
			return res;
		}
		/// <summary>
		/// returns a matrix that will transform points by the given values
		/// </summary>
		public static Matrix3 Translate(double x, double y, double z)
		{
			return Matrix3.Translate(new Vector3(x,y,z));
		}
		/// <summary>
		/// returns a matrix that will transform points by the given vector
		/// </summary>
		public static Matrix3 Translate(Vector3 value)
		{
			Matrix3 res=new Matrix3();
			res[0,0]=1.0;
			res[1,1]=1.0;
			res[2,2]=1.0;
			res[0,3]=value.X; res[1,3]=value.X; res[2,3]=value.Z; res[3,3]=1.0;
			return res;
		}
		#endregion
		#region operators
		public static Matrix3 operator*(Matrix3 a, Matrix3 b)
		{
			Matrix3 res=new Matrix3();
			for (int j=0; j<4; j++)
			{
				for(int i=0; i<4; i++)
					for(int k=0; k<4; k++)
						res[i,j]+=a[k,j]*b[i,k];
			}
			return res;
		}
		#endregion
		#region members
		/// <summary>
		/// returns an array with the transformed points
		/// </summary>
		public Vector3[] TransformPoints (Vector3[] ptarray)
		{
			if (ptarray==null || ptarray.Length<1)
				throw new ArgumentNullException("ptarray");
			//copy transformed points
			Vector3[] res=new Vector3[ptarray.Length];
			for (int i=0; i<res.Length; i++)
				res[i]=ptarray[i]*this;
			return res;
		}
		/// <summary>
		/// transforms points in an array
		/// </summary>
		public void TransformPointsInPlace(ref Vector3[] ptarray)
		{
			if (ptarray==null || ptarray.Length<1)
				throw new ArgumentNullException("ptarray");
			//transform points
			for (int i=0; i<ptarray.Length; i++)
				ptarray[i]=ptarray[i]*this;
		}
		/// <summary>
		/// represents the matrix as a human readable string
		/// </summary>
		public override string ToString()
		{
			string ret="Matrix[\n";
			//needet for separator
			System.Globalization.CultureInfo culture=
				System.Globalization.CultureInfo.CurrentCulture;
			//go through the values
			for (int j=0; j<4; j++)
			{
				for (int i=0; i<4; i++)
					ret+=this[i,j].ToString("0.0")+
						culture.TextInfo.ListSeparator+"\t";
				ret+="\n";
			}
			return ret+"]";
		}
		#endregion
		#region properties
		/// <summary>
		/// gets or sets the value at a specific position in the 4x4 matrix
		/// column and row reach from 0-3
		/// </summary>
		public double this[int column, int row]
		{
			get{return _mat[column,row];}
			set{_mat[column,row]=value;}
		}
		#endregion
	}
}

