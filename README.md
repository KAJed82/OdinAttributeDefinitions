<h1>Usage</h1>

%TYPENAME // starts a set of attributes / member attributes for a type
+new STYLINGATTRIBUTE( params ) // write the constructor for the attribute you want to add to this type (no support for default params yet)
-STYLINGATTRIBUTE // write the attribute type you want to remove and all instances will be removed from this type

\*memberName+new STYLINGATTRIBUTE( params ) // write the constructor for the attribute you want to add to members with this name (no support for default params yet)
\*memberName-STYLINGATTRIBUTE // write the attribute type you want to remove and all instances will be removed from members with this name

<h1>Example</h1>

~~~~
%float
+new LabelWidthAttribute(60)
+new GUIColorAttribute(1,0,0,1)

%int
+new LabelWidthAttribute(60)
+new GUIColorAttribute(0,1,0,1)

%MyCustomType
*someField+new LabelTextAttribute( "$someOtherTextMember" )
~~~~