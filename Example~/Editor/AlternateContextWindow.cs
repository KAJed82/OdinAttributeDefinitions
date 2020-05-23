using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace OdinAttributeDefinitions
{
	public class AlternateContextWindow : OdinEditorWindow
	{
		public static bool Drawing { get; protected set; }

		[MenuItem( "Tools/Odin Attribute Definitions Example/Alternate View Context" )]
		public static void Open()
		{
			var w = GetWindow<AlternateContextWindow>();
			w.Show();
		}

		protected override IEnumerable<object> GetTargets()
		{
			if ( Selection.activeObject is GameObject )
			{
				var go = Selection.activeObject as GameObject;
				yield return Selection.activeObject;

				foreach ( var c in go.GetComponents<MonoBehaviour>() )
					yield return c;
			}
			else
			{
				yield return Selection.activeObject;
			}
		}

		protected override void OnGUI()
		{
			Drawing = true;

			base.OnGUI();

			Drawing = false;
		}
	}
}
