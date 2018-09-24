using System;

namespace Mtgdb
{
	public struct HsvColor
	{
		public HsvColor(float h, float s, float v)
		{
			H = modulo(h, 360);
			S = s;
			V = v;
		}

		public HsvColor Transform(Func<float, float> h = null, Func<float, float> s = null, Func<float, float> v = null) =>
			new HsvColor(h?.Invoke(H) ?? H, s?.Invoke(S) ?? S, v?.Invoke(V) ?? V);

		public float H { get; }
		public float S { get; }
		public float V { get; }

		private static float modulo(float v, int d)
		{
			v = v % d;
			if (v < 0)
				v += d;

			return v;
		}
	}
}