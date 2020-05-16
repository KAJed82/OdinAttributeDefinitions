using System.Collections.Generic;
using Sirenix.OdinInspector;

[System.Serializable]
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