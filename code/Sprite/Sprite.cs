using Sandbox;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace DeggTools
{
	public struct Sprite
	{		
		public string Code { get; set; }

		[ResourceType( "png" )]
		public string Image { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public void SetHeight(int a)
		{
			Height = a;
		}

		public void SetWidth( int a )
		{
			Height = a;
		}
	}

	[GameResource( "Spritesheet", "sprite", "Describes a Sprite" )]
	public partial class SpriteSheetResource : GameResource
	{
		[ResourceType( "png" )]
		public List<Sprite> Sprites { get; set; }
	}
}
