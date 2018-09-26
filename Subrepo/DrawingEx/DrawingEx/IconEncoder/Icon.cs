using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;

namespace DrawingEx.IconEncoder
{
	/// <summary>
	/// Zusammenfassung für Icon.
	/// </summary>
	[Serializable(),
	System.ComponentModel.Editor(typeof(IconEditor),
		typeof(System.Drawing.Design.UITypeEditor))]
	public class Icon:ISerializable
	{
		#region classes
		/// <summary>
		/// collection for images inside icon
		/// </summary>
		public class IconImageCollection:CollectionBase
		{
			public IconImageCollection(){}
			/// <summary>
			/// adds an IconImage to the collection
			/// </summary>
			public void Add(IconImage img)
			{
				if (img==null)
					throw new ArgumentNullException("img");
				if(this.Count>(short.MaxValue-1))
					throw new Exception("number of icons too high");
				this.List.Add(img);
			}
			/// <summary>
			/// gets or sets the iconimage at the specified index
			/// </summary>
			public IconImage this[int index]
			{
				get{return this.List[index] as IconImage;}
				set{if(value!=null)this.List[index]=value;}
			}
		}
		#endregion
		#region variables
		private IconImageCollection _images=new IconImageCollection();
		#endregion
		#region constructors
		/// <summary>
		/// constructs a new empty icon
		/// </summary>
		public Icon(){}
		/// <summary>
		/// opens an icon from a stream
		/// </summary>
		public Icon(Stream str)
		{
			if(str==null)
				throw new ArgumentNullException("str");
			this.Load(str);
		}
		/// <summary>
		/// opens an icon from a file
		/// </summary>
		public Icon(string filename)
		{
			if(filename==null)
				throw new ArgumentNullException("filename");
			//open file
			FileStream fstr=null;
//			try
//			{
				fstr=new FileStream(filename,FileMode.Open);
				Load(fstr);
				fstr.Close();
				fstr=null;
//			}
//			catch(Exception e)
//			{
//				if(fstr!=null)
//					fstr.Close();
//				throw e;
//			}
		}
		/// <summary>
		/// deserialization constructor
		/// </summary>
		private Icon(SerializationInfo info, StreamingContext context)
		{
			byte[] icondata=(byte[])info.GetValue("IconData",typeof(byte[]));
			using(MemoryStream str=new MemoryStream(icondata))
			{
				Load(str);
			}
		}
		#endregion
		#region loading
		private void Load(Stream str)
		{
			//read the file header and check
			ICONDIR fileheader=new ICONDIR(str);
			if(fileheader.idType!=1)
				throw new Exception("this is not an icon file");
			if(fileheader.idCount<1)
				throw new Exception("no iconimages contained");
			//read direntries
			ICONDIRENTRY[] entries=new ICONDIRENTRY[fileheader.idCount];
			for(int i=0; i<fileheader.idCount; i++)
				entries[i]=new ICONDIRENTRY(str);
			//read images
			for(int i=0; i<fileheader.idCount; i++)
			{
				_images.Add(IconImage.FromStream(str));
			}
		}
		#endregion
		#region saving
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			MemoryStream str=new MemoryStream();
			this.Save(str);
			info.AddValue("IconData",str.ToArray(),typeof(byte[]));
		}
		/// <summary>
		/// saves the icon to a file
		/// </summary>
		public void Save(string filename)
		{
			if(filename==null)
				throw new ArgumentNullException("filename");
			//save to file
			FileStream fstr=null;
			try
			{
				fstr=new FileStream(filename,FileMode.Create);
				this.Save(fstr);
				fstr.Flush();
				fstr.Close();
			}
			catch(Exception e)
			{
				if (fstr!=null)
					fstr.Close();
				throw e;
			}
		}
		/// <summary>
		/// saves the icon to a stream
		/// </summary>
		public unsafe void Save(Stream str)
		{
			if(str==null)
				throw new ArgumentNullException("str");
			if(_images.Count<1)
				throw new Exception("icon is empty");
			//write file header
			ICONDIR fileheader=new ICONDIR((short)(_images.Count));
			fileheader.Write(str);
			//write direntries
			int fileoffset=sizeof(ICONDIR)+
				_images.Count*sizeof(ICONDIRENTRY);
			foreach(IconImage img in _images)
			{
				ICONDIRENTRY entry=img.GetEntry();
				entry.FileOffset=fileoffset;
				fileoffset+=entry.SizeInBytes;
				entry.Write(str);
			}
			//write images
			foreach(IconImage img in _images)
			{
				img.Write(str);
			}
		}
		#endregion
		#region properties
		/// <summary>
		/// returns the collection of images in the icon
		/// </summary>
		public Icon.IconImageCollection Images
		{
			get{return this._images;}
		}
		#endregion
	}
}
