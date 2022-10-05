using UnityEngine;

namespace Hertzole.CecilAttributes.Tests
{
	public class RequiredBase : MonoBehaviour { }

	public class RequiredBaseWithAwake : RequiredBase
	{
		[SerializeField]
		[Required]
		private BoxCollider box = default;

		private void Awake()
		{
			Debug.Log("Awake");
		}
	}

	public class RequiredBaseWithVirtualAwake : RequiredBase
	{
		[SerializeField]
		[Required]
		private BoxCollider box = default;

		protected virtual void Awake()
		{
			Debug.Log("Awake");
		}
	}

	public class RequiredBaseWithoutAwake : RequiredBase
	{
		[SerializeField]
		[Required]
		private BoxCollider box = default;
	}

	public class RequiredChild_WithAwake_ParentWithoutAwake : RequiredBaseWithoutAwake
	{
		[SerializeField] 
		[Required]
		private Renderer ren = default;
		
		private void Awake()
		{
			Debug.Log("Child Awake");
		}
	}

	public class RequiredChild_WithAwake_ParentWithVirtualAwake : RequiredBaseWithVirtualAwake
	{
		[SerializeField] 
		[Required]
		private Renderer ren = default;

		protected override void Awake()
		{
			Debug.Log("Child Awake");
		}
	}
}