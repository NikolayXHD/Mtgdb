using System;
using System.Collections.Generic;

namespace NConfiguration
{
	public sealed class DeserializeChildException : FormatException
	{
		public string Path { get; private set; }

		public DeserializeChildException(string path, Exception ex)
			:base(string.Format("can't read '{0}' node", path), ex)
		{
			Path = path;
		}

		public string[] FullPath
		{
			get
			{
				List<string> parts = new List<string>();
				DeserializeChildException current = this;
				while (true)
				{
					parts.Add(current.Path);
					var innerChildEx = current.InnerException as DeserializeChildException;
					if (innerChildEx == null)
						return parts.ToArray();

					current = innerChildEx;
				}
			}
		}

		public Exception Reason
		{
			get
			{
				Exception reason = this;
				while (true)
				{
					var innerChildEx = reason.InnerException as DeserializeChildException;
					if (innerChildEx == null)
						return reason.InnerException;

					reason = innerChildEx;
				}
			}
		}
	}
}

