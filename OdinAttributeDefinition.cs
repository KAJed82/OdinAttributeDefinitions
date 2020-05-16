using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities.Editor.Expressions;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class OdinAttributeDefinition : SerializedScriptableObject
{
	[OdinSerialize] protected internal Type type;
	public Type Type => type;

	[DelayedProperty]
	[OnValueChanged( "OnSelfAttributesChanged", true )]
	[SerializeField] protected internal List<string> selfAttributeStrings = new List<string>();

	[ListDrawerSettings( IsReadOnly = true )]
	[ShowInInspector]
	protected List<Attribute> selfAttributes = new List<Attribute>();
	public IReadOnlyCollection<Attribute> SelfAttributes => selfAttributes;

	[SerializeField] protected internal Dictionary<string, List<string>> memberAttributeStrings = new Dictionary<string, List<string>>();

	[DisableIf( "@true" )]
	[ShowInInspector]
	protected Dictionary<string, List<Attribute>> memberAttributes = new Dictionary<string, List<Attribute>>();
	public Dictionary<string, List<Attribute>> MemberAttributes => memberAttributes;

	[AsErrorList]
	[ShowInInspector]
	protected List<string> errors = new List<string>();

	protected void OnSelfAttributesChanged()
	{
		OnAfterDeserialize();
	}

	protected override void OnAfterDeserialize()
	{
		if ( selfAttributes == null )
			selfAttributes = new List<Attribute>();
		if ( memberAttributes == null )
			memberAttributes = new Dictionary<string, List<Attribute>>();

		errors.Clear();
		selfAttributes.Clear();
		foreach ( var s in selfAttributeStrings )
		{
			if ( string.IsNullOrEmpty( s ) )
				continue;

			// Ideally these should be done in context of the property, right?
			var attributeDelegate = ExpressionUtility.ParseExpression( s, true, null, out var error, false );
			if ( !string.IsNullOrEmpty( error ) )
			{
				errors.Add( $"{s}: {error}" );
			}
			else
			{
				Attribute attribute = attributeDelegate.DynamicInvoke() as Attribute;
				if ( attribute == null )
					errors.Add( $"{s}: Did not result in an attribute" );
				else
					selfAttributes.Add( attribute );
			}
		}

		memberAttributes.Clear();
		foreach ( var kvp in memberAttributeStrings )
		{
			if ( !memberAttributes.TryGetValue( kvp.Key, out var attributes ) )
				memberAttributes[kvp.Key] = attributes = new List<Attribute>();

			foreach ( var s in kvp.Value )
			{
				if ( string.IsNullOrEmpty( s ) )
					continue;

				// Ideally these should be done in context of the property, right?
				var attributeDelegate = ExpressionUtility.ParseExpression( s, true, null, out var error, false );
				if ( !string.IsNullOrEmpty( error ) )
				{
					errors.Add( $"{s}: {error}" );
				}
				else
				{
					Attribute attribute = attributeDelegate.DynamicInvoke() as Attribute;
					if ( attribute == null )
						errors.Add( $"{s}: Did not result in an attribute" );
					else
						attributes.Add( attribute );
				}
			}
		}
	}

	protected override void OnBeforeSerialize()
	{
		var thing = ExpressionUtility.ParseExpression( "new LabelAboveAttribute()", true, null, out var error );
		var thingResult = thing.DynamicInvoke();
	}

	public static List<OdinAttributeDefinition> GetDefinitions<T>()
	{
		return GetDefinitions( typeof( T ) );
	}

	protected internal static List<OdinAttributeDefinition> allDefinitions;

	public static List<OdinAttributeDefinition> GetDefinitions( Type type )
	{
		if ( allDefinitions == null )
		{
			allDefinitions = AssetDatabase
			.FindAssets( $"t:{typeof( OdinAttributeDefinition )}" )
			.Select( x => AssetDatabase.GUIDToAssetPath( x ) )
			.Distinct()
			.SelectMany( x => AssetDatabase.LoadAllAssetsAtPath( x ) )
			.OfType<OdinAttributeDefinition>()
			.ToList();
		}
		return allDefinitions
			.Where( x => type == x.Type ) // TODO: Allow definitions to apply to subclasses
			.ToList();
	}
}

public class OdinAttributeDefinitionPostProcessor : AssetPostprocessor
{
	static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths )
	{
		OdinAttributeDefinition.allDefinitions = null;
	}
}

public class AsErrorListAttribute : Attribute { }

public class AsErrorListAttributeDrawer<T> : OdinAttributeDrawer<AsErrorListAttribute, T>
	where T : IList<string>
{
	protected override void DrawPropertyLayout( GUIContent label )
	{
		if ( Property.Children.Count == 0 )
			return;

		if ( label != null )
			EditorGUILayout.LabelField( label );

		foreach ( var child in Property.Children )
			SirenixEditorGUI.ErrorMessageBox( child.ValueEntry.WeakSmartValue as string );
	}
}
