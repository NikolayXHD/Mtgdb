using System;
using System.Collections;
using System.IO;

namespace DrawingEx.IconEncoder
{
	/// <summary>
	/// Zusammenfassung für IconCollection.
	/// </summary>
	public class IconCollection:CollectionBase
	{
		#region classes
		/// <summary>
		/// used for extracting icons from a dll
		/// </summary>
		private class DLLExtractor
		{
			#region variables
			private static ArrayList __resnames;
			#endregion
			private DLLExtractor(){}
			/// <summary>
			/// constructs a new iconcollection from a file
			/// </summary>
			public static IconCollection FromFile(string filename)
			{
				if(filename==null || filename=="")
					throw new ArgumentNullException("filename");
				//load library handle
				IntPtr hLibrary=Kernel32.LoadLibraryEx(
					filename,
					IntPtr.Zero,
					Kernel32.LOAD_LIBRARY_AS_DATAFILE);
				if(hLibrary==IntPtr.Zero)
					throw new Exception("loading the library failed");

				ArrayList resourceids;
				//load resource names
				lock(typeof(DLLExtractor))
				{
					DLLExtractor.__resnames=new ArrayList();
					Kernel32.EnumResourceNames(hLibrary,
						Kernel32.RT_GROUP_ICON,
						new Kernel32.EnumProc(DLLExtractor.EnumProcedure),
						IntPtr.Zero);
					resourceids=new ArrayList(DLLExtractor.__resnames);
				}
				//load icons
				IconCollection ret=new IconCollection();
				try
				{
					for (int i=0; i<resourceids.Count; i++)
					{
						ret.Add(IconFromLibrary(hLibrary,resourceids[i]));
					}
				}
				catch(Exception e)
				{
					throw new Exception("dll file invalid",e);
				}
				finally
				{
					Kernel32.FreeLibrary(hLibrary);
				}
				return ret;
			}
			/// <summary>
			/// used to enumerate resource names
			/// </summary>
			private static bool EnumProcedure(IntPtr  hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam)
			{
				if(Kernel32.IS_INTRESOURCE(lpszName))
					__resnames.Add(lpszName.ToInt32());
				else
					__resnames.Add(System.Runtime.InteropServices.Marshal.PtrToStringAnsi(lpszName));
				return true;
			}
			/// <summary>
			/// constructs an icon from a dll resource
			/// </summary>
			private static Icon IconFromLibrary(IntPtr hlibrary, object resourceid)
			{
				IntPtr hicon;
				//is_intresource
				if(resourceid is int)
					hicon=Kernel32.FindResource(hlibrary,(int)resourceid,Kernel32.RT_GROUP_ICON);
				else if(resourceid is string)
					hicon=Kernel32.FindResource(hlibrary,(string)resourceid,Kernel32.RT_GROUP_ICON);
				else
					throw new ArgumentException("resourceid is invalid type","resourceid");
				//open stream
				MEMICONDIRENTRY[] entries;
				using (Stream str=Kernel32.GetStreamFromResource(hlibrary,hicon))
				{
					ICONDIR header=new ICONDIR(str);
					if(header.idType!=1)
						throw new Exception("this is not an icon file");
					if(header.idCount<1)
						throw new Exception("no iconimages contained");
					//read headers
					entries=new MEMICONDIRENTRY[header.idCount];
					for(int i=0; i<entries.Length; i++)
					{
						entries[i]=new MEMICONDIRENTRY(str);
					}
				}
				Icon ret=new Icon();
				//read images
				for(int i=0; i<entries.Length; i++)
				{
					//stream for single image
					using(Stream str=Kernel32.GetStreamFromResource(hlibrary,
							  Kernel32.FindResource(hlibrary,entries[i].ID,Kernel32.RT_ICON)))
					{
						ret.Images.Add(IconImage.FromStream(str));
					}
				}
				return ret;
			}
		}
		#endregion
		/// <summary>
		/// constructs a blank iconcollection
		/// </summary>
		public IconCollection(){}
		/// <summary>
		/// constructs a iconcollection from a dll file
		/// </summary>
		public static IconCollection FromDLL(string filename)
		{
			return DLLExtractor.FromFile(filename);
		}
		#region collection members
		/// <summary>
		/// adds the specified icon to the collection
		/// </summary>
		public void Add(Icon value)
		{
			if(value==null)
				return;
			this.List.Add(value);
		}
		/// <summary>
		/// gets or sets the icon at the specified position
		/// </summary>
		public Icon this[int index]
		{
			get{return (Icon)this.List[index];}
			set
			{
				if (value==null) return;
				this.List[index]=value;
			}
		}
		#endregion
	}
}
