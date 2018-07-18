using System.Collections.Generic;
using Mtgdb.Controls.Statistics;

namespace Mtgdb.Controls
{
	public abstract class Fields<TDoc>
	{
		public Dictionary<string, IField<TDoc>> ByName { get; protected set; }
		protected static readonly FieldBuilder<TDoc> Builder = new FieldBuilder<TDoc>();
	}
}