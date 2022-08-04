using Sandbox;
using System.Collections.Generic;
using Tools;

namespace DeggTools
{
	public partial class SpriteSheetResource : GameResource
	{	
		public List<KeyValuePair<string, string>> GetMaterials()
		{
			var generated = GetRelativeGeneratedFolderPath();


			var materials = new List<KeyValuePair<string,string>>();
			foreach ( var sprite in Sprites )
			{
				var imageString = sprite.Image ?? "";
				if (imageString != "")
				{
					var code = sprite.Code;
					var newAssetPath = generated + code;
					imageString = newAssetPath + ".vmat";
				}
				materials.Add( new KeyValuePair<string, string>( sprite.Code, imageString));				
			}

			return materials;
		}

		public void Validate()
		{
			var index = 0;
			var codes = new Dictionary<string, bool>();
			foreach ( var item in Sprites )
			{
				bool isValid = true;
				var code = (item.Code ?? "").ToLower().Trim();

				if ((item.Image ?? "") == "")
				{
					isValid = false;
				}

				if ( code == "" )
				{
					isValid = false;
				}

				if (codes.ContainsKey( code ) )
				{
					throw new System.Exception( $"Sprites must have a unique code {code}-{index}-{this.ResourcePath}" );
				}

				codes[item.Code] = true;


				if (!isValid)
				{
					throw new System.Exception( $"Sprites must all have a code and image to build {index}-{this.ResourcePath}" );
				}
				index = index + 1;
			}

			System.IO.Directory.CreateDirectory( GetGeneratedFolderPath());
		}
		public void CreateMaterials()
		{
			foreach ( var item in Sprites )
			{
				if ( item.Image == null )
				{
					throw new System.Exception( "All sprites must have a file" );
				}
				CreateMaterial( item );
			}
		}

		public void CreateMaterial(Sprite sprite)
		{
			var generated = GetGeneratedFolderPath();
			var code = sprite.Code;
			var path = sprite.Image;

			var newAssetPath = generated + code;
			var newImagePath = newAssetPath + ".png";

			var asset = DeggToolUtils.FindAsset( path );

			var newMatrialPath = newAssetPath + ".vmat";

			var png = Png.Open( asset.AbsolutePath );
			png.ToPowerOf2( newImagePath );

			var template = (new SpriteTemplate()).GetContent();
			template = template.Replace( "__texture_translucency__", newImagePath );
			template = template.Replace( "__texture_color__", newImagePath );


			System.IO.File.WriteAllText( newMatrialPath, template );

			AssetSystem.RegisterFile( newMatrialPath );
			MainAssetBrowser.Instance?.UpdateAssetList();
		}


		public void CreateModel()
		{
			CreateMaterials();

			var asset = GetRelatedAsset();
			var materials = GetMaterials();

			var defaultMaterial = materials.Find( ( item ) =>
			{
				return item.Value != "";
			} ).Value;

			if ( defaultMaterial == null || defaultMaterial == "")
			{
				throw new System.Exception( "Atleast 1 sprite must have an image" );
			}

			var materialAsset = DeggToolUtils.FindAsset( defaultMaterial );


			if ( materialAsset == null)
			{
				throw new System.Exception( $"Material not found {defaultMaterial}" );
			}


			var baseNode = new KVObject("baseNode");
			var root = new KVObject( "rootNode" );
			baseNode.AddProperty( "rootNode", root );
			root.AddProperty( "_class", "RootNode" );
			var children = new KVObject( "children", true );
			root.AddProperty( "children", children );
			root.AddProperty( "model_archetype", "" );
			root.AddProperty( "primary_associated_entity", "" );
			root.AddProperty( "anim_graph_name", "" );
			root.AddProperty( "base_model_name", "" );



			var renderMeshList = new KVObject( "RenderMeshList" );
			children.AddProperty( renderMeshList );
			renderMeshList.AddProperty( "_class", "RenderMeshList" );

			var renderMeshListChildren = new KVObject( "RenderMeshChildren", true );
			renderMeshList.AddProperty( "children", renderMeshListChildren );

			var renderMeshChild = new KVObject( "RenderPrimitivePlane" );
			renderMeshChild.AddProperty( "_class", "RenderPrimitivePlane" );
			renderMeshChild.AddProperty( "name", "base" );
			renderMeshChild.AddProperty( "parent_bone", "" );
			renderMeshChild.AddProperty( "material_name", defaultMaterial );
			renderMeshChild.AddProperty( "origin", new Vector3(0,0,0) );
			renderMeshChild.AddProperty( "angles", new Vector3( 0, 180, 0 ) );
			renderMeshChild.AddProperty( "max_u", 1.0f );
			renderMeshChild.AddProperty( "max_v", 1.0f );
			renderMeshChild.AddProperty( "width", 10.0f );
			renderMeshChild.AddProperty( "height", 10.0f );
			renderMeshChild.AddProperty( "segments_x", 1.0f );
			renderMeshChild.AddProperty( "segments_y", 1.0f );

			renderMeshListChildren.AddProperty( renderMeshChild );

			var materialGroupList = children.CreateChild( "MaterialGroupList" );

			materialGroupList.AddProperty( "_class", "MaterialGroupList" );

			var materialGroupListChildren = materialGroupList.CreateChild( "children", true ); 

			var index = 0;
			foreach (var mat in materials)
			{
				index = index + 1;
				var code = mat.Key.Trim().ToLower();

				if (mat.Key == null || mat.Key == "")
				{
					code = index.ToString();
				}

				var materialGroup = materialGroupListChildren.CreateChild(code);
				materialGroup.AddProperty( "_class", "MaterialGroup" );
				materialGroup.AddProperty( "name", code );
				var remaps = materialGroup.CreateChild( "remaps", true );

				if ( mat.Value != defaultMaterial )
				{
					var remapObject = remaps.CreateChild( "remap" );


					remapObject.AddProperty( "from", defaultMaterial );
					remapObject.AddProperty( "to", mat.Value );
				}
			}


			var file = new KV3File( baseNode, KV3EncodingFormats.GENERIC, KV3FileFormats.MODELDOC_29 );


			var newFilePath = asset.AbsolutePath + ".vmdl";



			System.IO.File.WriteAllText( newFilePath, file.ToString() );
			var spriteAsset = AssetSystem.RegisterFile( newFilePath );
			MainAssetBrowser.Instance?.UpdateAssetList();
			MainAssetBrowser.Instance?.FocusOnAsset( spriteAsset );
			Utility.InspectorObject = spriteAsset;
		}

	}
}
