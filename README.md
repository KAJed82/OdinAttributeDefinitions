<h1>Usage</h1>

<h3>?</h3>
<h4>**ONLY SUPPORTED WITH ODIN INSPECTOR v3+**</h4>
Checks a conditional expression before trying to apply this set of definitions.<br>
If used before any type attribute defintions it will apply to all attribute definitons in the file.<br>
If used inside a type attribute definition it will only apply to that type attribute definition.

<h3>%TYPENAME</h3> 
Starts a set of attributes / member attributes for a type<br>

<h3>+new STYLINGATTRIBUTE( params )</h3>
Write the constructor for the attribute you want to add to this type

<h3>-STYLINGATTRIBUTE</h3>
Write the attribute type you want to remove and all instances will be removed from this type before applying your own<br>

<h3>--</h3>
When used on a line by itself after a %TYPENAME this will remove all self attributes before applying your own.

<h3>---</h3>
When used on a line by itself after a %TYPENAME this will remove all self attributes and all member attributes before applying your own.

<h3>*memberName+new STYLINGATTRIBUTE( params )</h3>
Write the constructor for the attribute you want to add to members with this name

<h3>*memberName-STYLINGATTRIBUTE</h3>
Write the attribute type you want to remove and all instances will be removed from members with this name before applying your own

<h3>*memberName--</h3>
Remove all member attributes before applying your own.

<h3>; Some comment text</h3>
Comment<br>

<h1>Example</h1>
filename: MyAttributes.oad<br>

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

<h1>Alternate Context Example (from Example~ folder)</h1>

![Example](Images~/AlternateContextExample.png?raw=true "AlternateContextExample")
