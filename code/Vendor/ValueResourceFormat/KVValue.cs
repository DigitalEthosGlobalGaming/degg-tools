
namespace DeggTools
{

	// Class to hold type + value
	public class KVValue
	{
		public KVType Type { get; private set; }
		public object Value { get; private set; }

		public KVValue( KVType type, object value )
		{
			Type = type;
			Value = value;
		}
	}
}
