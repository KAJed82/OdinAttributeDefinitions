
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace OdinAttributeDefinitions
{
	public class AsErrorListAttribute : Attribute { }

	public class AsErrorListAttributeDrawer<T> : OdinAttributeDrawer<AsErrorListAttribute, T>
		where T : IList<string>
	{
		protected override void DrawPropertyLayout( GUIContent label )
		{
			if ( Property.Children.Count == 0 )
				return;

			if ( label != null )
				EditorGUILayout.LabelField( label );

			foreach ( var child in Property.Children )
				SirenixEditorGUI.ErrorMessageBox( child.ValueEntry.WeakSmartValue as string );
		}
	}
}