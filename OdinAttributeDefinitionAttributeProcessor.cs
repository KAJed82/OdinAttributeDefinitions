using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace OdinAttributeDefinitions
{
	public class OdinAttributeDefinitionAttributeProcessor<T> : OdinAttributeProcessor<T>
	{
		public override bool CanProcessSelfAttributes( InspectorProperty property )
		{
			return OdinAttributeDefinition.GetDefinitions<T>().Any( x => x.AddedSelfAttributes.Count > 0 || x.RemovedSelfAttributes.Count > 0 );
		}

		public override bool CanProcessChildMemberAttributes( InspectorProperty parentProperty, MemberInfo member )
		{
			return true;
		}

		public override void ProcessSelfAttributes( InspectorProperty property, List<Attribute> attributes )
		{
			var definitions = OdinAttributeDefinition.GetDefinitions<T>();

			foreach ( var definition in definitions.Where( x => x.MatchesCondition( property ) ) )
			{
				if ( definition.RemoveAllSelfAttributes )
				{
					attributes.Clear();
					break;
				}

				foreach ( var a in definition.RemovedSelfAttributes )
				{
					for ( int i = attributes.Count - 1; i >= 0; --i )
					{
						if ( attributes[i].GetType() == a.GetType() )
							attributes.RemoveAt( i );
					}
				}
			}

			foreach ( var definition in definitions.Where( x => x.MatchesCondition( property ) ) )
			{
				foreach ( var a in definition.AddedSelfAttributes )
					attributes.Add( a );
			}
		}

		public override void ProcessChildMemberAttributes( InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes )
		{
			var definitions = OdinAttributeDefinition.GetDefinitions<T>();

			#region Remove Attributes
			bool removedAll = false;

			// Remove all instances of these attributes?
			foreach ( var definition in definitions.Where( x => x.MatchesCondition( parentProperty ) ) )
			{
				if ( definition.RemoveAllMemberAttributes || definition.GetRemovedMemberAllAttributes( member.Name ) )
				{
					attributes.Clear();
					removedAll = true;
					break;
				}

				var memberAttributes = definition.GetRemovedMemberAttributes( member.Name );
				if ( memberAttributes != null )
				{
					foreach ( var t in memberAttributes )
					{
						for ( int i = attributes.Count - 1; i >= 0; --i )
						{
							if ( attributes[i].GetType() == t )
								attributes.RemoveAt( i );
						}
					}
				}
			}

			if ( !removedAll )
			{
				// get list of definitions for this type
				switch ( member.MemberType )
				{
					case MemberTypes.Field:
						var field = member as FieldInfo;
						foreach ( var definition in OdinAttributeDefinition.GetDefinitions( field.FieldType ).Where( x => x.MatchesCondition( parentProperty ) ) )
						{
							foreach ( var a in definition.RemovedSelfAttributes )
							{
								for ( int i = attributes.Count - 1; i >= 0; --i )
								{
									if ( attributes[i].GetType() == a.GetType() )
										attributes.RemoveAt( i );
								}
							}
						}
						break;

					case MemberTypes.Property:
						var property = member as PropertyInfo;
						foreach ( var definition in OdinAttributeDefinition.GetDefinitions( property.PropertyType ).Where( x => x.MatchesCondition( parentProperty ) ) )
						{
							foreach ( var t in definition.RemovedSelfAttributes )
							{
								for ( int i = attributes.Count - 1; i >= 0; --i )
								{
									if ( attributes[i].GetType() == t )
										attributes.RemoveAt( i );
								}
							}
						}
						break;
				}
			}
			#endregion

			#region Add PropertyGroup Attributes
			// get list of definitions for this type
			switch ( member.MemberType )
			{
				case MemberTypes.Field:
					var field = member as FieldInfo;

					foreach ( var definition in OdinAttributeDefinition.GetDefinitions( field.FieldType ).Where( x => x.MatchesCondition( parentProperty ) ) )
					{
						foreach ( var a in definition.AddedSelfAttributes.OfType<PropertyGroupAttribute>() )
							attributes.Add( a );
					}
					break;

				case MemberTypes.Property:
					var property = member as PropertyInfo;

					foreach ( var definition in OdinAttributeDefinition.GetDefinitions( property.PropertyType ).Where( x => x.MatchesCondition( parentProperty ) ) )
					{
						foreach ( var a in definition.AddedSelfAttributes.OfType<PropertyGroupAttribute>() )
							attributes.Add( a );
					}
					break;
			}
			#endregion

			#region Add Attributes
			foreach ( var definition in definitions.Where( x => x.MatchesCondition( parentProperty ) ) )
			{
				var memberAttributes = definition.GetAddedMemberAttributes( member.Name );
				if ( memberAttributes != null )
				{
					foreach ( var a in memberAttributes )
						attributes.Add( a );
				}
			}
			#endregion
		}
	}
}