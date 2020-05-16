using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor.Expressions;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

[ScriptedImporter( 1, "oad" )]
public class OdinAttributeDefinitionFileImporter : ScriptedImporter
{
	public override void OnImportAsset( AssetImportContext ctx )
	{
		var path = ctx.assetPath;
		var name = Path.GetFileNameWithoutExtension( path );

		var text = File.ReadAllText( path );
		var lines = text.Split( new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );

		var definitionFile = ScriptableObject.CreateInstance<OdinAttributeDefinitionFile>();
		definitionFile.name = name;

		ctx.AddObjectToAsset( name, definitionFile );
		ctx.SetMainObject( definitionFile );

		Type currentType = null;
		OdinAttributeDefinition currentDefinition = null;
		foreach ( var line in lines )
		{
			// If the first character is a % sign then it's a type
			if ( line[0] == '%' )
			{
				var substring = line.Substring( 1 );
				var type = GetTypeFromString( substring );
				if ( type == null )
				{
					Debug.LogError( $"{substring} could not be converted to a type." );
					break;
				}

				currentType = type;
				currentDefinition = ScriptableObject.CreateInstance<OdinAttributeDefinition>();
				currentDefinition.name = $"{type.FullName}_OAD";
				currentDefinition.type = type;
				ctx.AddObjectToAsset( currentDefinition.name, currentDefinition );

				definitionFile.definitions.Add( currentDefinition );
			}
			else if ( line[0] == '+' )
			{
				var substring = line.Substring( 1 );
				if ( currentDefinition == null )
				{
					Debug.LogError( $"{substring}: no active type." );
					break;
				}

				currentDefinition.selfAttributeStrings.Add( substring );
			}
			// TODO: Add '-' to allow removing attributes
			else if ( line[0] == '*' ) // *memberName+new LabelWidthAttribute
			{
				var substring = line.Substring( 1 );
				if ( currentDefinition == null )
				{
					Debug.LogError( $"{substring}: no active type." );
					break;
				}

				int plusIndex = substring.IndexOf( '+' );
				int minusIndex = substring.IndexOf( '-' );

				// Definitely not valid
				if ( plusIndex == -1 && minusIndex == -1 )
				{
					Debug.LogError( $"{substring}: No member action found (+,-)." );
					break;
				}

				int useIndex = plusIndex >= 0 ? plusIndex : minusIndex;

				string memberSubstring = substring.Substring( 0, useIndex );
				if ( useIndex + 1 >= substring.Length )
				{
					Debug.LogError( $"{substring}: No attribute found" );
					break;
				}

				string attributeSubstring = substring.Substring( useIndex + 1 );

				if ( !currentDefinition.memberAttributeStrings.TryGetValue( memberSubstring, out var attributes ) )
					currentDefinition.memberAttributeStrings[memberSubstring] = attributes = new List<string>();

				attributes.Add( attributeSubstring );
			}
		}
	}

	private static readonly TwoWaySerializationBinder Binder = new DefaultSerializationBinder();

	protected static Type GetTypeFromString( string typeString )
	{
		Type type = Binder.BindToType( typeString );

		if ( type == null )
			type = AssemblyUtilities.GetTypeByCachedFullName( typeString );

		if ( type == null )
			ExpressionUtility.TryParseTypeNameAsCSharpIdentifier( typeString, out type );

		return type;
	}
}
