using System.Collections.Generic;

namespace Mtgdb.Data
{
	public abstract class Fields<TDoc>
	{
		public Dictionary<string, IField<TDoc>> ByName { get; protected set; }
		protected static readonly FieldBuilder<TDoc> Builder = new FieldBuilder<TDoc>();
	}
}