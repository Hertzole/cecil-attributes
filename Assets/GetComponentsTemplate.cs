using System.Collections.Generic;
using Hertzole.CecilAttributes.Interfaces;
using UnityEngine;

public class GetComponentsTemplate : MonoBehaviour, IGetComponent
{
	[SerializeField] 
	private Transform trs = default;
	[SerializeField] 
	private Transform[] transforms = default;
	[SerializeField] 
	private List<Transform> transformsList = default;
		
	void IGetComponent.FetchComponents()
	{
		__CECIL__ATTRIBUTES__GENERATED__FetchComponents();
	}

	protected virtual void __CECIL__ATTRIBUTES__GENERATED__FetchComponents()
	{
		if (trs == null)
		{
			trs = GetComponentInParent<Transform>();
		}
		if (trs == null)
		{
			trs = GetComponentInChildren<Transform>(true);
			// GetComponentsInChildren<Transform>()
		}

		if (transforms == null || transforms.Length == 0)
		{
			transforms = GetComponentsInChildren<Transform>(false);
		}

		if (transformsList == null || transformsList.Count == 0)
		{
			transformsList ??= new List<Transform>();

			GetComponentsInChildren(false, transformsList);
		}
	}
}

public class TempScript : GetComponentsTemplate
{
	[SerializeField] 
	private TempScript temp = default;
	
	protected override void __CECIL__ATTRIBUTES__GENERATED__FetchComponents()
	{
		base.__CECIL__ATTRIBUTES__GENERATED__FetchComponents();
		if (temp == null)
		{
			temp = GetComponent<TempScript>();
		}
	}
}