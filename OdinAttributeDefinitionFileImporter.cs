using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_2020_1_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using UnityEngine;

namespace OdinAttributeDefinitions
{
	[ScriptedImporter( 3, "oad" )]
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

			string globalCondition = string.Empty;
			Type currentType = null;
			OdinAttributeDefinition currentDefinition = null;

			for ( int lineIndex = 0; lineIndex < lines.Length; ++lineIndex )
			{
				// Strip leading spaces
				string line = lines[lineIndex].Trim();

				if ( line[0] == '?' ) // If the first character is a ? then this is a condition
				{
					var substring = line.Substring( 1 );
					if ( currentDefinition == null )
					{
						if ( string.IsNullOrEmpty( globalCondition ) )
							globalCondition = substring;
						else
							Debug.LogError( $"Only a single global condition per OAD. '{substring}' ignored." );
					}
					else
					{
						if ( string.IsNullOrEmpty( globalCondition ) && string.IsNullOrEmpty( currentDefinition.requiredCondition ) )
							currentDefinition.requiredCondition = substring;
						else
						{
							if ( !string.IsNullOrEmpty( globalCondition ) )
								Debug.LogError( $"Global condition is already set. Local '{substring}' ignored." );
							if ( !string.IsNullOrEmpty( currentDefinition.requiredCondition ) )
								Debug.LogError( $"Local condition is already set. '{substring}' ignored." );
						}
					}
				}
				else if ( line[0] == '%' ) // If the first character is a % sign then it's a type
				{
					var substring = line.Substring( 1 );
					var type = OdinAttributeDefinition.GetTypeFromString( substring );
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

					currentDefinition.addedSelfAttributeStrings.Add( substring );
				}
				else if ( line[0] == '-' )
				{
					var substring = line.Substring( 1 );
					if ( currentDefinition == null )
					{
						Debug.LogError( $"{substring}: no active type." );
						break;
					}

					if ( line.Length >= 2 && line[1] == '-' ) // -- Means remove all attributes from this type
					{
						currentDefinition.removeAllSelfAttributes = true;
						if ( line.Length >= 3 && line[2] == '-' ) // --- Means remove all attributes from this type and all it's members
							currentDefinition.removeAllMemberAttributes = true;
					}
					else
					{
						currentDefinition.removedSelfAttributeStrings.Add( substring );
					}
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

					if ( plusIndex >= 0 )
					{
						string memberSubstring = substring.Substring( 0, plusIndex );
						if ( plusIndex + 1 >= substring.Length )
						{
							Debug.LogError( $"{substring}: No attribute found" );
							break;
						}

						string attributeSubstring = substring.Substring( plusIndex + 1 );

						List<string> attributes = null;
						if ( !currentDefinition.addedMemberAttributeStrings.TryGetValue( memberSubstring, out attributes ) )
							currentDefinition.addedMemberAttributeStrings[memberSubstring] = attributes = new List<string>();

						attributes.Add( attributeSubstring );
					}
					else
					{
						string memberSubstring = substring.Substring( 0, minusIndex );
						if ( minusIndex + 1 >= substring.Length )
						{
							Debug.LogError( $"{substring}: No attribute found" );
							break;
						}

						string attributeSubstring = substring.Substring( minusIndex + 1 );
						if ( attributeSubstring.Trim() == "-" ) // Found '--', remove all attributes from member name
						{
							currentDefinition.removeMemberAttributesAll.Add( memberSubstring );
						}
						else
						{
							List<string> attributes = null;
							if ( !currentDefinition.removedMemberAttributeStrings.TryGetValue( memberSubstring, out attributes ) )
								currentDefinition.removedMemberAttributeStrings[memberSubstring] = attributes = new List<string>();

							attributes.Add( attributeSubstring );
						}
					}
				}
				else if ( line[0] == ';' )
				{
					continue;
				}
			}

			foreach ( var definition in definitionFile.definitions )
			{
				if ( string.IsNullOrEmpty( definition.requiredCondition ) )
					definition.requiredCondition = globalCondition;
			}
		}
	}
}