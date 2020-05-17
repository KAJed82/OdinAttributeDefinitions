using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor.Expressions;
using UnityEditor;
using UnityEngine;

namespace OdinAttributeDefinitions
{
	public class OdinAttributeDefinition : SerializedScriptableObject
	{
		[OdinSerialize] protected internal Type type;
		public Type Type => type;

		[DelayedProperty]
		[OnValueChanged( "OnSelfAttributesChanged", true )]
		[SerializeField] protected internal List<string> addedSelfAttributeStrings = new List<string>();

		[ListDrawerSettings( IsReadOnly = true )]
		[ShowInInspector]
		protected List<Attribute> addedSelfAttributes = new List<Attribute>();
		public IReadOnlyCollection<Attribute> AddedSelfAttributes => addedSelfAttributes;

		[DelayedProperty]
		[OnValueChanged( "OnSelfAttributesChanged", true )]
		[SerializeField] protected internal List<string> removedSelfAttributeStrings = new List<string>();

		[ListDrawerSettings( IsReadOnly = true )]
		[ShowInInspector]
		protected List<Type> removedSelfAttributes = new List<Type>();
		public IReadOnlyCollection<Type> RemovedSelfAttributes => removedSelfAttributes;

		[SerializeField] protected internal Dictionary<string, List<string>> addedMemberAttributeStrings = new Dictionary<string, List<string>>();

		[DisableIf( "@true" )]
		[ShowInInspector]
		protected Dictionary<string, List<Attribute>> addedMemberAttributes = new Dictionary<string, List<Attribute>>();

		public IReadOnlyCollection<Attribute> GetAddedMemberAttributes( string memberName )
		{
			List<Attribute> attributes;
			addedMemberAttributes.TryGetValue( memberName, out attributes );
			return attributes;
		}

		[SerializeField] protected internal Dictionary<string, List<string>> removedMemberAttributeStrings = new Dictionary<string, List<string>>();

		[DisableIf( "@true" )]
		[ShowInInspector]
		protected Dictionary<string, List<Type>> removedMemberAttributes = new Dictionary<string, List<Type>>();

		public IReadOnlyCollection<Type> GetRemovedMemberAttributes( string memberName )
		{
			List<Type> attributes;
			removedMemberAttributes.TryGetValue( memberName, out attributes );
			return attributes;
		}

		[AsErrorList]
		[ShowInInspector]
		protected List<string> errors = new List<string>();

		protected void OnSelfAttributesChanged()
		{
			OnAfterDeserialize();
		}

		protected override void OnAfterDeserialize()
		{
			if ( addedSelfAttributes == null )
				addedSelfAttributes = new List<Attribute>();
			if ( addedMemberAttributes == null )
				addedMemberAttributes = new Dictionary<string, List<Attribute>>();

			errors.Clear();

			#region Added Self Attributes
			addedSelfAttributes.Clear();
			foreach ( var s in addedSelfAttributeStrings )
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
						addedSelfAttributes.Add( attribute );
				}
			}
			#endregion

			#region Removed Self Attributes
			removedSelfAttributes.Clear();
			foreach ( var s in removedSelfAttributeStrings )
			{
				if ( string.IsNullOrEmpty( s ) )
					continue;

				if ( type == null )
				{
					errors.Add( $"{s}: No matching type found." );
				}
				else
				{
					removedSelfAttributes.Add( type );
				}
			}
			#endregion

			#region Added Member Attributes
			addedMemberAttributes.Clear();
			foreach ( var kvp in addedMemberAttributeStrings )
			{
				if ( !addedMemberAttributes.TryGetValue( kvp.Key, out var attributes ) )
					addedMemberAttributes[kvp.Key] = attributes = new List<Attribute>();

				foreach ( var s in kvp.Value )
				{
					if ( string.IsNullOrEmpty( s ) )
						continue;

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
			#endregion

			#region Removed Member Attributes
			removedMemberAttributes.Clear();
			foreach ( var kvp in removedMemberAttributeStrings )
			{
				if ( !removedMemberAttributes.TryGetValue( kvp.Key, out var attributes ) )
					removedMemberAttributes[kvp.Key] = attributes = new List<Type>();

				foreach ( var s in kvp.Value )
				{
					if ( string.IsNullOrEmpty( s ) )
						continue;

					var type = GetTypeFromString( s );
					if ( type == null )
					{
						errors.Add( $"{s}: No matching type found." );
					}
					else
					{
						attributes.Add( type );
					}
				}
			}
			#endregion
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
				.Where( x => type.ImplementsOrInherits( x.type ) ) // TODO: Allow definitions to apply to subclasses
				.ToList();
		}

		private static readonly TwoWaySerializationBinder Binder = new DefaultSerializationBinder();

		protected internal static Type GetTypeFromString( string typeString )
		{
			Type type = Binder.BindToType( typeString );

			if ( type == null )
				type = AssemblyUtilities.GetTypeByCachedFullName( typeString );

			if ( type == null )
				ExpressionUtility.TryParseTypeNameAsCSharpIdentifier( typeString, out type );

			return type;
		}
	}
}