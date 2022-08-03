using System.Collections.Generic;
using System.Linq;
using Tools;

namespace DeggTools
{
	public static class DeggToolUtils
	{
		public static List<Inspector> GetInspectors()
		{
			List<Inspector> inspectors = new List<Inspector>();
			var all = Window.All.ToList();

			foreach ( var windows in all )
			{
				foreach ( var child in windows.Children )
				{
					if (child is Inspector i)
					{
						inspectors.Add( i );
					}
				}
			}

			return inspectors;
		}

		public static Inspector GetFirstInspector()
		{
			List<Inspector> inspectors = new List<Inspector>();
			var all = Window.All.ToList();

			foreach ( var windows in all )
			{
				foreach ( var child in windows.Children )
				{
					if ( child is Inspector i )
					{
						inspectors.Add( i );
					}
				}
			}

			if ( inspectors.Count > 0)
			{
				return inspectors.First();
			}
			return null;
		}
	}
}
