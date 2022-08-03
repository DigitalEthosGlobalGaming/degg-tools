﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace DeggTools
{
	//Different type of value blocks for KeyValues (All in use for KV3)
	public enum KVType
	{
		STRING_MULTI, //STRING_MULTI doesn't have an ID
		NULL,
		BOOLEAN,
		INTEGER,
		FLAGGED_STRING, //TODO: Remove!
		DOUBLE = 5,
		STRING,
		ARRAY = 8,
		OBJECT
	}

	//Datastructure for a KV Object
	public class KVObject
	{
		public string Key { get; private set; }
		public Dictionary<string, KVValue> Properties { get; private set; }
		private bool IsArray;
		public int Count { get; private set; }

		public KVObject( string name )
		{
			Key = name;
			Properties = new Dictionary<string, KVValue>();
			IsArray = false;
			Count = 0;
		}

		public KVObject( string name, bool isArray )
			: this( name )
		{
			IsArray = isArray;
		}

		//Add a property to the structure
		public virtual void AddProperty( string name, KVValue value )
		{
			if ( IsArray )
			{
				// Make up a key for the dictionary
				Properties.Add( Count.ToString(), value );
			}
			else
			{
				Properties.Add( name, value );
			}

			Count++;
		}

		//Add a property to the structure
		public virtual void AddProperty(KVObject[] arr )
		{
			for(var i = 0; i <arr.Length; i++ )
			{
				AddProperty( i.ToString(), arr[i] );
			}

		}

		//Add a property to the structure
		public virtual void AddProperty( string name, string value )
		{
			var kvValue = new KVValue( KVType.STRING, value );
			AddProperty(name, kvValue );
		}

		//Add a property to the structure
		public virtual void AddProperty( string name, int value )
		{
			var kvValue = new KVValue( KVType.INTEGER, value );
			AddProperty( name, kvValue );
		}

		//Add a property to the structure
		public virtual void AddProperty( string name, float value )
		{
			var kvValue = new KVValue( KVType.DOUBLE, value );
			AddProperty( name, kvValue );
		}

		public virtual void AddProperty( string name, KVObject value )
		{
			var kvValue = new KVValue( KVType.OBJECT, value );
			AddProperty( name, kvValue );
		}
		public virtual void AddProperty( KVObject value )
		{
			var kvValue = new KVValue( KVType.OBJECT, value );
			AddProperty( value.Key, kvValue );
		}

		public virtual void AddResource( string name, string value )
		{
			var kvValue = new KVFlaggedValue( KVType.STRING, KVFlag.Resource, value );
			AddProperty( name, kvValue );
		}

		public void Serialize( IndentedTextWriter writer )
		{
			if ( IsArray )
			{
				SerializeArray( writer );
			}
			else
			{
				SerializeObject( writer );
			}
		}

		//Serialize the contents of the KV object
		private void SerializeObject( IndentedTextWriter writer )
		{
			//Don't enter the top-most object
			if ( Key != null )
			{
				writer.WriteLine();
			}

			writer.WriteLine( "{" );
			writer.Indent++;

			foreach ( var pair in Properties )
			{
				writer.Write( pair.Key );
				writer.Write( " = " );

				PrintValue( writer, pair.Value );

				writer.WriteLine();
			}

			writer.Indent--;
			writer.Write( "}" );
		}

		private void SerializeArray( IndentedTextWriter writer )
		{
			//Need to preserve the order
			writer.WriteLine();
			writer.WriteLine( "[" );
			writer.Indent++;
			for ( var i = 0; i < Count; i++ )
			{
				PrintValue( writer, Properties[i.ToString()] );

				writer.WriteLine( "," );
			}

			writer.Indent--;
			writer.Write( "]" );
		}

		private string EscapeUnescaped( string input, char toEscape )
		{
			if ( input.Length == 0 )
			{
				return input;
			}

			int index = 1;
			while ( true )
			{
				index = input.IndexOf( toEscape, index );

				//Break out of the loop if no more occurrences were found
				if ( index == -1 )
				{
					break;
				}

				if ( input.ElementAt( index - 1 ) != '\\' )
				{
					input = input.Insert( index, "\\" );
				}

				//Don't read this one again
				index++;
			}

			return input;
		}

		//Print a value in the correct representation
		private void PrintValue( IndentedTextWriter writer, KVValue kvValue )
		{
			KVType type = kvValue.Type;
			object value = kvValue.Value;
			var flagValue = kvValue as KVFlaggedValue;
			if ( flagValue != null )
			{
				switch ( flagValue.Flag )
				{
					case KVFlag.Resource:
						writer.Write( "resource:" );
						break;
					case KVFlag.DeferredResource:
						writer.Write( "deferred_resource:" );
						break;
					default:
						throw new InvalidOperationException( "Trying to print unknown flag" );
				}
			}
			Log.Info( type );

			switch ( type )
			{
				case KVType.OBJECT:
				case KVType.ARRAY:
					((KVObject)value).Serialize( writer );
					break;
				case KVType.FLAGGED_STRING:
					writer.Write( (string)value );
					break;
				case KVType.STRING:
					writer.Write( "\"" );
					writer.Write( EscapeUnescaped( (string)value, '"' ) );
					writer.Write( "\"" );
					break;
				case KVType.STRING_MULTI:
					writer.Write( "\"\"\"\n" );
					writer.Write( (string)value );
					writer.Write( "\n\"\"\"" );
					break;
				case KVType.BOOLEAN:
					writer.Write( (bool)value ? "true" : "false" );
					break;
				case KVType.DOUBLE:
					writer.Write( ((double)value).ToString( "#0.000000", CultureInfo.InvariantCulture ) );
					break;
				case KVType.INTEGER:
					writer.Write( (int)value );
					break;
				case KVType.NULL:
					writer.Write( "null" );
					break;
				default:
					// Unknown type encountered
					throw new InvalidOperationException( "Trying to print unknown type." );
			}
		}
	}
}
