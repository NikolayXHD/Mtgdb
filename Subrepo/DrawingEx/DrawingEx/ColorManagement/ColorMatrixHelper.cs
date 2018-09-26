using System;
using System.Drawing.Imaging;

namespace DrawingEx.ColorManagement
{
	/// <summary>
	/// static access to colormatrix helper functions
	/// </summary>
	public class ColorMatrixHelper
	{
		private ColorMatrixHelper(){}
		/// <summary>
		/// returns a given hsl transformation matrix
		/// of the specified angle. specify only
		/// degrees (0° - 360°), no relative values.
		/// </summary>
		private static ColorMatrix GetHSLMatrix(int degrees)
		{
			degrees=Math.Min(360,Math.Max(0,degrees));
			float scale;
			if (degrees<120)
			{
				#region shift red to green
				scale=(float)degrees/120f;
				return new ColorMatrix(
					new float[][]{
									 //output	 r			 g			 b
									 new float[]{1f-scale	,scale		,0f			,0f,0f},//r
									 new float[]{0f			,1f-scale	,scale		,0f,0f},//g		input
									 new float[]{scale		,0f			,1f-scale	,0f,0f},//b
									 new float[]{0f,0f,0f,1f,0f},
									 new float[]{0f,0f,0f,0f,1f}
								 });
				#endregion
			}
			else if (degrees<240)
			{
				#region shift green to blue
				scale=(float)(degrees-120)/120f;
				return new ColorMatrix(
					new float[][]{
									 //output	 r			 g			 b
									 new float[]{0f			,1f-scale	,scale		,0f,0f},//r
									 new float[]{scale		,0f			,1f-scale	,0f,0f},//g		input
									 new float[]{1f-scale	,scale		,0f			,0f,0f},//b
									 new float[]{0f,0f,0f,1f,0f},
									 new float[]{0f,0f,0f,0f,1f}
								 });
				#endregion
			}
			else
			{
				#region shift blue to red
				scale=(float)(degrees-240)/120f;
				return new ColorMatrix(
					new float[][]{
									 //output	 r			 g			 b
									 new float[]{scale		,0f			,1f-scale	,0f,0f},//r
									 new float[]{1f-scale	,scale		,0f			,0f,0f},//g		input
									 new float[]{0f			,1f-scale	,scale		,0f,0f},//b
									 new float[]{0f,0f,0f,1f,0f},
									 new float[]{0f,0f,0f,0f,1f}
								 });
				#endregion
			}
		}
	}
}
