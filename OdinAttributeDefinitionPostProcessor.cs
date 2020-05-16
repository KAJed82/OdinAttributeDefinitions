

using UnityEditor;

/// <summary>
/// Makes sure the cached definitions are cleared when something is imported... it's a bit lewd but it works.
/// </summary>
public class OdinAttributeDefinitionPostProcessor : AssetPostprocessor
{
	private static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths )
	{
		OdinAttributeDefinition.allDefinitions = null;
	}
}