<h1>Usage</h1>

<h3>%TYPENAME</h3> 
Starts a set of attributes / member attributes for a type<br>

<h3>+new STYLINGATTRIBUTE( params )</h3>
Write the constructor for the attribute you want to add to this type

<h3>-STYLINGATTRIBUTE</h3>
Write the attribute type you want to remove and all instances will be removed from this type<br>

<br>

<h3>\*memberName+new STYLINGATTRIBUTE( params )</h3>
Write the constructor for the attribute you want to add to members with this name

<h3>\*memberName-STYLINGATTRIBUTE</h3>
Write the attribute type you want to remove and all instances will be removed from members with this name<br>
<br>

<h3>; Some comment text</h3>
Comment<br>

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