using System;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DrawingEx.IconEncoder
{
	/// <summary>
	/// Zusammenfassung für IconEditor.
	/// </summary>
	public class IconEditor:UITypeEditor
	{
		#region variables
		private FileDialog _filedialog;
		#endregion
		public IconEditor(){}
		#region overrides
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if ((provider == null) ||
				(provider.GetService(typeof(IWindowsFormsEditorService)) == null))
				return value;
			if(_filedialog==null)
			{
				_filedialog=new OpenFileDialog();
				_filedialog.Filter="Icons(*.ico)|*.ico";
			}
			if(_filedialog.ShowDialog()==DialogResult.OK)
			{
				try
				{
					using(FileStream stream=new FileStream(_filedialog.FileName,
							  FileMode.Open,
							  FileAccess.Read,
							  FileShare.Read))
					{
						byte[] buffer=new byte[stream.Length];
						stream.Read(buffer,0,buffer.Length);
						using(MemoryStream mstr=new MemoryStream(buffer))
						{
							value=new Icon(mstr);
						}
					}
				}
				catch{}
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e)
		{
			if(e.Value is Icon)
			{
				Icon icn=(Icon)e.Value;
				Rectangle bounds=e.Bounds;
				bounds.Width--;
				bounds.Height--;
				if(icn.Images.Count>0)
					e.Graphics.DrawImage(icn.Images[0].Bitmap,bounds);
				e.Graphics.DrawRectangle(SystemPens.WindowFrame,bounds);
			}
		}

		#endregion
	}
}
