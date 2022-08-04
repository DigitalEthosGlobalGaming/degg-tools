using Sandbox;
using System.Linq;
using Tools;

namespace DeggTools
{
	public static partial class DeggToolUtils
	{
		public static Asset FindAsset(GameResource g)
		{
			return AssetSystem.All.ToList().First<Asset>( ( item ) =>
			 {
				 return g.ResourcePath == item.RelativePath;
			 } );
		}

		public static Asset FindAsset( string resourcePath )
		{
			return AssetSystem.All.ToList().FirstOrDefault<Asset>( ( item ) =>
			{
				return resourcePath == item.RelativePath;
			} );
		}
	}
}
