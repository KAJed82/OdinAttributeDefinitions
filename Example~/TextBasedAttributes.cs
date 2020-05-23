using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class Foo
{
	public int data;
}

[System.Serializable]
public class Bar
{
	public int data;
}

[System.Serializable]
public class MyCustomType
{
	public float someField;

	public string someOtherTextMember = "defaultText";
}

[CreateAssetMenu]
public class TextBasedAttributes : SerializedScriptableObject
{
	[BoxGroup( "Some Box" )]
	[InlineProperty]
	public Foo foo;

	[BoxGroup( "Some Box" )]
	[InlineProperty]
	public Bar bar;

	public MyCustomType custom;
}