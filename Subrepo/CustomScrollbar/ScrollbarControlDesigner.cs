using System.ComponentModel;
using System.Windows.Forms.Design;

namespace CustomScrollbar
{
	internal class ScrollbarControlDesigner : ControlDesigner
	{
		public override SelectionRules SelectionRules
		{
			get
			{
				SelectionRules selectionRules = base.SelectionRules;
				PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(Component)["AutoSize"];
				if (propDescriptor != null)
				{
					bool autoSize = (bool) propDescriptor.GetValue(Component);
					if (autoSize)
					{
						selectionRules = SelectionRules.Visible | SelectionRules.Moveable | SelectionRules.BottomSizeable | SelectionRules.TopSizeable;
					}
					else
					{
						selectionRules = SelectionRules.Visible | SelectionRules.AllSizeable | SelectionRules.Moveable;
					}
				}

				return selectionRules;
			}
		}
	}
}