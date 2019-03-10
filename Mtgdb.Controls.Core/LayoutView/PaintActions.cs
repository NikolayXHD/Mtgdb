using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	internal class PaintActions
	{
		public List<Action<PaintEventArgs>> Back { get; } = new List<Action<PaintEventArgs>>();
		public List<Action<PaintEventArgs>> FieldData { get; } = new List<Action<PaintEventArgs>>();
		public List<Action<PaintEventArgs>> FieldButtons { get; } = new List<Action<PaintEventArgs>>();
		public List<Action<PaintEventArgs>> AlignButtons { get; } = new List<Action<PaintEventArgs>>();
		public List<Action<PaintEventArgs>> Selection { get; } = new List<Action<PaintEventArgs>>();
	}
}