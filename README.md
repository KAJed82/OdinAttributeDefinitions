<h1>Usage</h1>

**%TYPENAME** - Starts a set of attributes / member attributes for a type<br>
**+new STYLINGATTRIBUTE( params )** - Write the constructor for the attribute you want to add to this type (no support for default params yet)<br>
**-STYLINGATTRIBUTE** - Write the attribute type you want to remove and all instances will be removed from this type<br>
<br>
**\*memberName+new STYLINGATTRIBUTE( params )** - Write the constructor for the attribute you want to add to members with this name (no support for default params yet)<br>
**\*memberName-STYLINGATTRIBUTE** - Write the attribute type you want to remove and all instances will be removed from members with this name<br>
<br>
**; Some comment text** - Comment<br>

<h1>Example</h1>

~~~~
%float
+new LabelWidthAttribute(60)
+new GUIColorAttribute(1,0,0,1)

%int
+new LabelWidthAttribute(60)
+new GUIColorAttribute(0,1,0,1)

; This is my custom type
%MyCustomType
*someField+new LabelTextAttribute( "$someOtherTextMember" )
~~~~