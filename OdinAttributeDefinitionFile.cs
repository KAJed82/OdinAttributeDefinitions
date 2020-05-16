using System.Collections.Generic;
using UnityEngine;

namespace OdinAttributeDefinitions
{
	public class OdinAttributeDefinitionFile : ScriptableObject
	{
		[SerializeField] protected internal List<OdinAttributeDefinition> definitions = new List<OdinAttributeDefinition>();
		public IReadOnlyCollection<OdinAttributeDefinition> Definitions => definitions;
	}
}