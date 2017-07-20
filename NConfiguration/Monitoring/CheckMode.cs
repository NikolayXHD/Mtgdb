using System;

namespace NConfiguration.Monitoring
{
	[Flags]
	public enum CheckMode: byte
	{
		None = 0,
		Attr = 0x1,
		Hash = 0x2,
		All = Attr | Hash
	}
}

