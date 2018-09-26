using System;
using System.Collections.Generic;
using System.Text;

namespace DrawingEx
{
	/// <summary>
	/// class for transforming a 2D set of numbers.
	/// the transformation is performed trough a slow
	/// algorithm unless block size is 8x8, then
	/// the fast transformation of AAN is used
	/// </summary>
	public class DCT2D
	{
		#region variables
		private double[,] _data;
		private int _width, _height;
		#endregion
		public DCT2D(int width, int height)
		{
			if (width < 1 || height < 1)
				throw new ArgumentException("size");
			_data = new double[width, height];
			_width = width;
			_height = height;
			init();
		}
		#region public members
		public void Transform()
		{
			if (_width == 8 && _height == 8)
				transformAAN();
			else
				transform(false);
		}
		public void InverseTransform()
		{
			transform(true);
		}
		#endregion
		#region private functions
		double[,] rowmat, colmat, revrowmat, revcolmat, buffer;
		double wx, wy, u0, u, v0, v;
		private void init()
		{
			//prepare constants
			wx = Math.PI / (2.0 * (double)_width);
			wy = Math.PI / (2.0 * (double)_height);
			u0 = Math.Sqrt(1.0 / (double)_width);
			u = Math.Sqrt(2.0 / (double)_width);
			v0 = Math.Sqrt(1.0 / (double)_height);
			v = Math.Sqrt(2.0 / (double)_height);
			//prepare arrays
			buffer = new double[_width, _height];
			//row matrix
			rowmat = new double[_height, _height];
			revrowmat = new double[_height, _height];
			for (int x = 0; x < _height; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					revrowmat[x, y] = Math.Cos(((y << 1) + 1) * x * wy);
					rowmat[x, y] = Math.Cos(((x << 1) + 1) * y * wy);
				}
			}
			//column matrix
			colmat = new double[_width, _width];
			revcolmat = new double[_width, _width];
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _width; y++)
				{
					revcolmat[x, y] = Math.Cos(((x << 1) + 1) * y * wx);
					colmat[x, y] = Math.Cos(((y << 1) + 1) * x * wx);
				}
			}
		}
		private void transform(bool inverse)
		{
			double w;
			//transform cols
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					buffer[x, y] = 0.0;
					for (int k = 0; k < _width; k++)
					{
						if (inverse)
						{
							if (k != 0) w = u;
							else w = u0;
							buffer[x, y] += w * revcolmat[x, k] * _data[k, y];
						}
						else
							buffer[x, y] += colmat[x, k] * _data[k, y];
					}
				}
			}
			//transform rows
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					_data[x, y] = 0;
					for (int k = 0; k < _height; k++)
					{
						if (inverse)
						{
							if (k != 0) w = v;
							else w = v0;
							_data[x, y] += w * buffer[x, k] * revrowmat[k, y];
						}
						else
							_data[x, y] += buffer[x, k] * rowmat[k, y];
					}
				}
			}
			//output stage
			if (!inverse)
			{
				_data[0, 0] *= u0 * v0;
				for (int x = 1; x < _width; x++)
					_data[x, 0] *= u * v0;

				for (int y = 1; y < _height; y++)
					_data[0, y] *= u0 * v;

				for (int x = 1; x < _width; x++)
				{
					for (int y = 1; y < _height; y++)
						_data[x,y] *= u * v;
				}
			}
		}
		private void transformAAN()
		{
			int[,] rows = new int[8, 8];

			const int c1 = 1004 /* cos(pi/16) << 10 */,
							s1 = 200 /* sin(pi/16) */,
							c3 = 851 /* cos(3pi/16) << 10 */,
							s3 = 569 /* sin(3pi/16) << 10 */,
							r2c6 = 554 /* sqrt(2)*cos(6pi/16) << 10 */,
							r2s6 = 1337 /* sqrt(2)*sin(6pi/16) << 10 */,
							r2 = 181; /* sqrt(2) << 7*/

			int x0, x1, x2, x3, x4, x5, x6, x7, x8;

			/* transform rows */
			for (int i = 0; i < 8; i++)
			{
				x0 = (int)_data[0, i];
				x1 = (int)_data[1, i];
				x2 = (int)_data[2, i];
				x3 = (int)_data[3, i];
				x4 = (int)_data[4, i];
				x5 = (int)_data[5, i];
				x6 = (int)_data[6, i];
				x7 = (int)_data[7, i];

				/* Stage 1 */
				x8 = x7 + x0;
				x0 -= x7;
				x7 = x1 + x6;
				x1 -= x6;
				x6 = x2 + x5;
				x2 -= x5;
				x5 = x3 + x4;
				x3 -= x4;

				/* Stage 2 */
				x4 = x8 + x5;
				x8 -= x5;
				x5 = x7 + x6;
				x7 -= x6;
				x6 = c1 * (x1 + x2);
				x2 = (-s1 - c1) * x2 + x6;
				x1 = (s1 - c1) * x1 + x6;
				x6 = c3 * (x0 + x3);
				x3 = (-s3 - c3) * x3 + x6;
				x0 = (s3 - c3) * x0 + x6;

				/* Stage 3 */
				x6 = x4 + x5;
				x4 -= x5;
				x5 = r2c6 * (x7 + x8);
				x7 = (-r2s6 - r2c6) * x7 + x5;
				x8 = (r2s6 - r2c6) * x8 + x5;
				x5 = x0 + x2;
				x0 -= x2;
				x2 = x3 + x1;
				x3 -= x1;

				/* Stage 4 and output */
				rows[0, i] = x6;
				rows[4, i] = x4;
				rows[2, i] = x8 >> 10;
				rows[6, i] = x7 >> 10;
				rows[7, i] = (x2 - x5) >> 10;
				rows[1, i] = (x2 + x5) >> 10;
				rows[3, i] = (x3 * r2) >> 17;
				rows[5, i] = (x0 * r2) >> 17;
			}

			/* transform columns */
			for (int i = 0; i < 8; i++)
			{
				x0 = rows[i, 0];
				x1 = rows[i, 1];
				x2 = rows[i, 2];
				x3 = rows[i, 3];
				x4 = rows[i, 4];
				x5 = rows[i, 5];
				x6 = rows[i, 6];
				x7 = rows[i, 7];

				/* Stage 1 */
				x8 = x7 + x0;
				x0 -= x7;
				x7 = x1 + x6;
				x1 -= x6;
				x6 = x2 + x5;
				x2 -= x5;
				x5 = x3 + x4;
				x3 -= x4;

				/* Stage 2 */
				x4 = x8 + x5;
				x8 -= x5;
				x5 = x7 + x6;
				x7 -= x6;
				x6 = c1 * (x1 + x2);
				x2 = (-s1 - c1) * x2 + x6;
				x1 = (s1 - c1) * x1 + x6;
				x6 = c3 * (x0 + x3);
				x3 = (-s3 - c3) * x3 + x6;
				x0 = (s3 - c3) * x0 + x6;

				/* Stage 3 */
				x6 = x4 + x5;
				x4 -= x5;
				x5 = r2c6 * (x7 + x8);
				x7 = (-r2s6 - r2c6) * x7 + x5;
				x8 = (r2s6 - r2c6) * x8 + x5;
				x5 = x0 + x2;
				x0 -= x2;
				x2 = x3 + x1;
				x3 -= x1;

				/* Stage 4 and output */
				_data[i, 0] = (double)((x6 + 16) >> 3);
				_data[i, 4] = (double)((x4 + 16) >> 3);
				_data[i, 2] = (double)((x8 + 16384) >> 13);
				_data[i, 6] = (double)((x7 + 16384) >> 13);
				_data[i, 7] = (double)((x2 - x5 + 16384) >> 13);
				_data[i, 1] = (double)((x2 + x5 + 16384) >> 13);
				_data[i, 3] = (double)(((x3 >> 8) * r2 + 8192) >> 12);
				_data[i, 5] = (double)(((x0 >> 8) * r2 + 8192) >> 12);
			}
		}
		#endregion
		#region properties
		public double this[int x, int y]
		{
			get { return _data[x, y]; }
			set { _data[x, y] = value; }
		}
		public int Width
		{
			get { return _width; }
		}
		public int Height
		{
			get { return _height; }
		}
		#endregion
	}
}
