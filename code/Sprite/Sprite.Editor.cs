using Sandbox;
using Sandbox.Internal;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace DeggTools
{
	public partial class SpriteSheetResource : GameResource
	{
		protected override void PostReload()
		{
			base.PostReload();
			Process( );
		}

		public string GetGeneratedFolderPath()
		{			
			return GetRelatedAsset().AbsolutePath.Replace( ".sprite", "_generated/" );
		}
		public string GetRelativeGeneratedFolderPath()
		{
			return GetRelatedAsset().RelativePath.Replace( ".sprite", "_generated/" );
		}

		public Asset GetRelatedAsset()
		{
			return DeggToolUtils.FindAsset( this );
		}

		public void Process()
		{
			var newSprites = new List<Sprite>();
			var spritesArray = Sprites.ToArray();
			for ( int i = 0; i < spritesArray.Count(); i++ )
			{
				var sprite = spritesArray[i];
				if ( sprite.Image != "" )
				{
					Asset imageAsset = DeggToolUtils.FindAsset(sprite.Image);

					if ( imageAsset != null )
					{
						var size = GetPngSize( imageAsset.AbsolutePath );
						sprite.Width = ( (int)size.y );
						sprite.Height = ( (int)size.x );
					}
				}
				newSprites.Add( sprite );
			}
			Sprites = newSprites;

			this.Model = this.ResourcePath + ".vmdl";
		}

		public Vector2 GetPngSize(string path)
		{
			var size = new Vector2();
			if ( path != null )
			{
				var png = Png.Open( path );
				size.x = png.Width;
				size.y = png.Height;
			}

			return size;
		}



		[Event( "asset.contextmenu", Priority = 100 )]
		public static void OnAssetContextMenu_CreateSprite( AssetContextMenu e )
		{
			if ( e.SelectedList.Count == 1 )
			{
				var asset = e.SelectedList.First();
				var extension = asset.AssetType.FileExtension;

				if ( extension == "sprite" )
				{
					e.Menu.AddSeparator();
					e.Menu.AddOption( "Build Sprite", "collections", action: () => BuildSprite( asset ) );
				}
			}
		}
		public KV3File ToKV3File()
		{
			var root = new KVObject( "root" );
			var data = new KVObject( "data" );

			root.AddProperty( "data", data );

			var sprites = new KVObject( "Sprites", true );

			List<KVObject> arr = new List<KVObject>();

			foreach (var sprite in Sprites)
			{
				
				var spriteKv = new KVObject( "data" );
				spriteKv.AddProperty( "Image", sprite.Image ?? "" );
				spriteKv.AddProperty( "Code", sprite.Code ?? "" );
				spriteKv.AddProperty( "Width", sprite.Width );
				spriteKv.AddProperty( "Height", sprite.Height);
				arr.Add( spriteKv );
			}
			sprites.AddProperty( arr.ToArray() );

			data.AddProperty( sprites );

			var file = new KV3File( root );
			return file;
		}

		public static void BuildSprite( SpriteSheetResource sprite )
		{

			var asset = DeggToolUtils.FindAsset( sprite );
			BuildSprite( sprite, asset );
		}

		public static void BuildSprite( SpriteSheetResource sprite, Asset asset )
		{
			if ( sprite != null && asset != null )
			{
				sprite.Validate();
				sprite.Process();
				sprite.CreateModel();
				asset.SaveToDisk( sprite );
				asset.SaveToMemory( sprite );
				asset.Compile( false );

				MainAssetBrowser.Instance?.UpdateAssetList();
				MainAssetBrowser.Instance?.FocusOnAsset( asset );
				Utility.InspectorObject = asset;
			}

		}
		public static void BuildSprite( Asset asset )
		{
			var sprite = asset.LoadResource<SpriteSheetResource>();
			BuildSprite( sprite, asset );			
		}


	}
}
