<h1>Usage</h1>

<h3>%TYPENAME</h3> 
Starts a set of attributes / member attributes for a type<br>

<h3>+new STYLINGATTRIBUTE( params )</h3>
Write the constructor for the attribute you want to add to this type

<h3>-STYLINGATTRIBUTE</h3>
Write the attribute type you want to remove and all instances will be removed from this type<br>

<h3>\*memberName+new STYLINGATTRIBUTE( params )</h3>
Write the constructor for the attribute you want to add to members with this name

<h3>\*memberName-STYLINGATTRIBUTE</h3>
Write the attribute type you want to remove and all instances will be removed from members with this name<br>

<h3>; Some comment text</h3>
Comment<br>

<h1>Example</h1>

~~~~
; This is my custom type
%MyCustomType
+new HideLabelAttribute()
+new BoxGroupAttribute( "Custom", true, false, 0 )
*someField+new LabelTextAttribute( "$someOtherTextMember" )

%TextBasedAttributes
*foo+new HorizontalGroupAttribute(0.5f,0,0,0)
*foo+new InlinePropertyAttribute()
*foo+new LabelWidthAttribute(40)

*bar+new HorizontalGroupAttribute(0.5f,0,0,0)
*bar+new InlinePropertyAttribute()
*bar+new LabelWidthAttribute(40)

*custom+new PropertyOrderAttribute( -1 )
~~~~
~~~~
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

public class TextBasedAttributes : SerializedMonoBehaviour
{
	public Foo foo;
	public Bar bar;

	public MyCustomType custom;
}
~~~~

![Example](Images~/TextBasedAttributesExample.png?raw=true "Example")
