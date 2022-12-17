using UnityEngine;

namespace Hertzole.CecilAttributes.Tests
{
	public class RequiredBase : MonoBehaviour { }

	public class RequiredBaseWithAwake : RequiredBase
	{
		[SerializeField]
		[Required]
		private BoxCollider box = default;

		public BoxCollider Box { get { return box; } set { box = value; } }

		private void Awake()
		{
			Debug.Log("Base Awake");
		}
	}

	public class RequiredBaseWithVirtualAwake : RequiredBase
	{
		[SerializeField]
		[Required]
		private BoxCollider box = default;

		public BoxCollider Box { get { return box; } set { box = value; } }

		protected virtual void Awake()
		{
			Debug.Log("Base Awake");
		}
	}

	public class RequiredBaseWithoutAwake : RequiredBase
	{
		[SerializeField]
		[Required]
		private BoxCollider box = default;

		public BoxCollider Box { get { return box; } set { box = value; } }
	}

	public class RequiredChild_WithAwake_ParentWithoutAwake : RequiredBaseWithoutAwake
	{
		[SerializeField]
		[Required]
		private Renderer ren = default;

		public Renderer Ren { get { return ren; } set { ren = value; } }

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

		public Renderer Ren { get { return ren; } set { ren = value; } }

		protected override void Awake()
		{
			base.Awake();
			Debug.Log("Child Awake");
		}
	}

	public class RequiredChild_WithoutAwake_ParentWithAwake : RequiredBaseWithAwake
	{
		[SerializeField]
		[Required]
		private Renderer ren = default;

		public Renderer Ren { get { return ren; } set { ren = value; } }
	}

	public class Base : RequiredBase
	{
		private void Awake()
		{
			Debug.Log("Base Awake");
		}
	}

	public class Child : Base
	{
		[SerializeField]
		[Required]
		private Renderer ren = default;
	}
}