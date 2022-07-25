using Sandbox;
using System.Linq;
using Tools;

namespace DeggTools
{
	[GameResource( "Sprite", "sprite", "Describes a Sprite" )]
	public partial class SpriteResource : GameResource
	{
		[ResourceType( "png" )]
		public string SourceImage { get; set; }
		[ResourceType( "vmat" )]
		public string Material { get; set; }

		protected override void PostReload()
		{
			base.PostReload();
			Log.Info( "T" );
		}

		[Event( "asset.contextmenu", Priority = 100 )]
		public static void OnAssetContextMenu_CreateSprite( AssetContextMenu e )
		{
			Log.Info( e );
			if ( e.SelectedList.Count == 1 )
			{
				var asset = e.SelectedList.First();
				if (asset.AssetType == AssetType.ImageFile)
				{
					if ( asset.AbsolutePath.EndsWith( ".png" ) )
					{
						e.Menu.AddSeparator();
						e.Menu.AddOption( "Create Sprite", "collections", action: () => SpriteResource.CreateMaterial( asset ) );
					}
				}				
			}
		}

		public KV3File ToKV3File()
		{
			var root = new KVObject("root");
			var data = new KVObject( "data" );
			data.AddProperty( "SourceImage", SourceImage );
			data.AddResource( "Material", Material );

			root.AddProperty( "data", data );
			var file = new KV3File( root );
			return file;

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

			var newAsset = AssetSystem.RegisterFile(basePath + materialResource );
			MainAssetBrowser.Instance?.UpdateAssetList();



			var sprite = new SpriteResource();
			sprite.SourceImage = asset.RelativePath;
			sprite.Material = newAsset.RelativePath;

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
