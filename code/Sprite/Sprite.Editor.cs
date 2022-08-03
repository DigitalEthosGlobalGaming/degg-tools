using Sandbox;
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
			Create( );
		}

		public void Create()
		{
			foreach(var sprite in Sprites)
			{
				var size = GetPngSize( sprite.Image );
				sprite.SetHeight( (int)size.y );
				sprite.SetWidth( (int)size.x );
			}
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
				Log.Info( extension );
				if ( asset.AssetType == AssetType.ImageFile )
				{
					if ( asset.AbsolutePath.EndsWith( ".png" ) )
					{
						e.Menu.AddSeparator();
						e.Menu.AddOption( "Create Sprite", "collections", action: () => CreateMaterial( asset ) );
					}
				} else if ( extension == "sprite" )
				{
					e.Menu.AddSeparator();
					e.Menu.AddOption( "Build Sprite", "collections", action: () => CreateMaterial( asset ) );
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

		public void BuildSprite()
		{
			var data = this.ToKV3File();
			System.IO.File.WriteAllText( this.ResourcePath, data.ToString() );
			Log.Info( this.ResourcePath );

			return;
			var spriteAsset = AssetSystem.FindByPath( this.ResourcePath );

			MainAssetBrowser.Instance?.UpdateAssetList();
			MainAssetBrowser.Instance?.FocusOnAsset( spriteAsset );
			Utility.InspectorObject = spriteAsset;

			spriteAsset.Compile( true );
		}

		public static void CreateMaterial( Asset asset )
		{

			var targetPath = asset.RelativePath;
			var basePath = asset.AbsolutePath.Substring( 0, asset.AbsolutePath.Length - targetPath.Length );
			var pathWithoutExtension = targetPath.Replace( ".png", "" );

			var materialResource = $"{pathWithoutExtension}_sprite.vmat";
			var spriteResource = $"{pathWithoutExtension}.sprite";
			var imageResource = $"{pathWithoutExtension}_sprite.png";

			var png = Png.Open( asset.AbsolutePath );
			png.ToPowerOf2( basePath + imageResource );

			var template = (new SpriteTemplate()).GetContent();
			template = template.Replace( "__texture_translucency__", imageResource );
			template = template.Replace( "__texture_color__", imageResource );


			System.IO.File.WriteAllText( basePath + materialResource, template );

			var newAsset = AssetSystem.RegisterFile( basePath + materialResource );
			MainAssetBrowser.Instance?.UpdateAssetList();



			var sprite = new SpriteSheetResource();

			var file = sprite.ToKV3File();


			System.IO.File.WriteAllText( basePath + spriteResource, file.ToString() );
			var spriteAsset = AssetSystem.RegisterFile( basePath + spriteResource );
			MainAssetBrowser.Instance?.UpdateAssetList();
			MainAssetBrowser.Instance?.FocusOnAsset( spriteAsset );
			Utility.InspectorObject = spriteAsset;

			spriteAsset.Compile( true );
		}


	}
}
