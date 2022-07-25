using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Sandbox.Extensions
{
	public class MaterialExtensions
	{

		[Event( "asset.contextmenu", Priority = 100 )]
		public static void OnAssetContextMenu_CopySection( AssetContextMenu e )
		{
			if ( e.SelectedList.Count == 1 )
			{
				var asset = e.SelectedList.First();

				Log.Info( asset.AssetType );
				e.Menu.AddSeparator();
				e.Menu.AddOption( "yoyo Thumbnail", "collections", action: () => asset.RebuildThumbnail( true ) );
			}
		}
	}
}
