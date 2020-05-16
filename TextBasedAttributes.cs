using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Bobby
{
	public int so;
}

public class Sometimes
{
	public int hello;
}

public class TextBasedAttributes : SerializedMonoBehaviour
{
	public bool check;
	public float bob;

	public Bobby yup;

	[System.NonSerialized]
	public Sometimes some;
	[System.NonSerialized]
	public List<Sometimes> more;
}

public class TextBasedAttributeProcessor<T> : OdinAttributeProcessor<T>
{
	static TextBasedAttributeProcessor()
	{
		// Where to find the file?
		// one file with includes?
		// many files?
	}

	protected List<OdinAttributeDefinition> definitions;

	public TextBasedAttributeProcessor()
	{
		definitions = OdinAttributeDefinition.GetDefinitions<T>();
	}

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
		foreach ( var a in definitions.SelectMany( x => x.SelfAttributes ) )
			attributes.Add( a );
	}

	public override void ProcessChildMemberAttributes( InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes )
	{
		// get list of definitions for this type
		switch ( member.MemberType )
		{
			case MemberTypes.Field:
				var field = member as FieldInfo;

				foreach ( var a in OdinAttributeDefinition.GetDefinitions(field.FieldType).SelectMany( x => x.SelfAttributes ) )
					attributes.Add( a );
				break;

			case MemberTypes.Property:
				var property = member as PropertyInfo;

				foreach ( var a in OdinAttributeDefinition.GetDefinitions( property.PropertyType ).SelectMany( x => x.SelfAttributes ) )
					attributes.Add( a );
				break;
		}

		// get list of definitions for this field
		foreach ( var definition in definitions )
		{
			if ( definition.MemberAttributes.TryGetValue( member.Name, out var memberAttributes ) )
			{
				foreach ( var a in memberAttributes )
					attributes.Add( a );
			}
		}
	}
}