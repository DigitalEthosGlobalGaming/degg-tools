using System.IO;

namespace DeggTools
{

	public enum KV3FileFormats
	{
		GENERIC = 0,
		MODELDOC_29 = 1
	}

	public enum KV3EncodingFormats
	{
		GENERIC = 0
	}

	public class KV3File
	{
		// <!-- kv3 encoding:text:version{e21c7f3c-8a33-41c5-9977-a76d3a32aa0d} format:generic:version{7412167c-06e9-4698-aff2-e63eb59037e7} -->
		public string Encoding { get; private set; }
		public string Format { get; private set; }

		public KVObject Root { get; private set; }

		public static string GetKv3FileFormatString( KV3FileFormats format)
		{
			switch ( format )
			{
				case KV3FileFormats.GENERIC:
					return "generic:version{7412167c-06e9-4698-aff2-e63eb59037e7}";
				case KV3FileFormats.MODELDOC_29:
					return "modeldoc29:version{3cec427c-1b0e-4d48-a90a-0436f33a6041}";
				default:
					break;
			}
			return "generic:version{7412167c-06e9-4698-aff2-e63eb59037e7}";
		}

		public static string GetKv3EncodingFormatString( KV3EncodingFormats format )
		{
			return "text:version{e21c7f3c-8a33-41c5-9977-a76d3a32aa0d}";
		}

		public KV3File(
			KVObject root,
			KV3EncodingFormats encoding = 0,
			KV3FileFormats format = 0 )
		{
			Root = root;
			Encoding = GetKv3EncodingFormatString(encoding);
			Format = GetKv3FileFormatString( format );;
		}

		public override string ToString()
		{
			using ( var output = new StringWriter() )
			using ( var writer = new IndentedTextWriter( output, "\t" ) )
			{
				writer.WriteLine( string.Format( "<!-- kv3 encoding:{0} format:{1} -->", Encoding, Format ) );
				Root.Serialize( writer );

				return output.ToString();
			}
		}
	}
}
