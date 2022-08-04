using Sandbox;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace DeggTools
{
	public enum SpriteShapes {
		Square,
		Circle
		
	}
	public struct Sprite
	{		
		public string Code { get; set; }

		[ResourceType( "png" )]
		public string Image { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public SpriteShapes Shape { get; set; }

	}

	[GameResource( "Spritesheet", "sprite", "Describes a Sprite" )]
	public partial class SpriteSheetResource : GameResource
	{
		[ResourceType( "vmdl" )]
		public string Model { get; set; }
		[ResourceType( "png" )]
		public List<Sprite> Sprites { get; set; }
	}
}
