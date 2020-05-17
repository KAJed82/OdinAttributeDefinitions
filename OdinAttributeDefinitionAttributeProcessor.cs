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
			return true;
		}

		public override bool CanProcessChildMemberAttributes( InspectorProperty parentProperty, MemberInfo member )
		{
			return true;
		}

		public override void ProcessSelfAttributes( InspectorProperty property, List<Attribute> attributes )
		{
			List<OdinAttributeDefinition> definitions = OdinAttributeDefinition.GetDefinitions<T>();

			foreach ( var definition in definitions )
			{
				foreach ( var a in definition.AddedSelfAttributes )
					attributes.Add( a );
			}

			foreach ( var definition in definitions )
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
		}

		public override void ProcessChildMemberAttributes( InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes )
		{
			List<OdinAttributeDefinition> definitions = OdinAttributeDefinition.GetDefinitions<T>();

			#region Add PropertyGroup Attributes
			// get list of definitions for this type
			switch ( member.MemberType )
			{
				case MemberTypes.Field:
					var field = member as FieldInfo;

					foreach ( var definition in OdinAttributeDefinition.GetDefinitions( field.FieldType ) )
					{
						foreach ( var a in definition.AddedSelfAttributes.OfType<PropertyGroupAttribute>() )
							attributes.Add( a );
					}
					break;

				case MemberTypes.Property:
					var property = member as PropertyInfo;

					foreach ( var definition in OdinAttributeDefinition.GetDefinitions( property.PropertyType ) )
					{
						foreach ( var a in definition.AddedSelfAttributes.OfType<PropertyGroupAttribute>() )
							attributes.Add( a );
					}
					break;
			}

			foreach ( var definition in definitions )
			{
				var memberAttributes = definition.GetAddedMemberAttributes( member.Name );
				if ( memberAttributes != null )
				{
					foreach ( var a in memberAttributes )
						attributes.Add( a );
				}
			}
			#endregion

			#region Remove Attributes
			// get list of definitions for this type
			switch ( member.MemberType )
			{
				case MemberTypes.Field:
					var field = member as FieldInfo;
					foreach ( var definition in OdinAttributeDefinition.GetDefinitions( field.FieldType ) )
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
					foreach ( var definition in OdinAttributeDefinition.GetDefinitions( property.PropertyType ) )
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

			// Remove all instances of these attributes?
			foreach ( var definition in definitions )
			{
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
			#endregion
		}
	}
}