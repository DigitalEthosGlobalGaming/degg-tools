
using Sandbox;
using System.IO;

namespace DeggTools
{
	public static class FileUtilities
	{
		public static void CreateRelativeDirectory(string relativePath)
		{

			var sourceFile = FileSystem.Root.GetFullPath( $"/{relativePath}" );

			if ( !Directory.Exists( sourceFile ) )
			{
				Directory.CreateDirectory( sourceFile );
			}
		}


		public static void GetOrCreateFile( string relativePath, string content )
		{
			Log.Info( relativePath );
			using StreamWriter file = new(relativePath);
			file.Write( content );
		}
	}
}
