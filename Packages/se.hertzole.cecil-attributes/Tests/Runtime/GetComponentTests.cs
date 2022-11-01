// This is because ReSharper doesn't know we're doing code weaving.
// ReSharper disable SuspiciousTypeConversion.Global

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hertzole.CecilAttributes.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Hertzole.CecilAttributes.Tests
{
	public class GetComponentTests
	{
		private readonly List<GameObject> objects = new List<GameObject>();

		#region Component reference
		private class ComponentReference : MonoBehaviour
		{
			[GetComponent]
			public ComponentReference reference;
		}

		private class ComponentReferenceParent : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Parent)]
			public ComponentReference reference;
		}

		private class ComponentReferenceChild : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Children)]
			public ComponentReference reference;
		}
		#endregion
		
		#region Component reference inheritance
		private class ComponentReference1 : MonoBehaviour
		{
			[GetComponent]
			public ComponentReference2 reference1;
		}
		
		private class ComponentReference2 : ComponentReference1
		{
			[GetComponent]
			public ComponentReference2 reference2;
		}

		private class ComponentReferenceParent1 : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Parent)]
			public ComponentReference reference1;
		}
		
		private class ComponentReferenceParent2 : ComponentReferenceParent1
		{
			[GetComponent(target = GetComponentTarget.Parent)]
			public ComponentReference reference2;
		}
		
		private class ComponentReferenceChild1 : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Children)]
			public ComponentReference reference1;
		}
		
		private class ComponentReferenceChild2 : ComponentReferenceChild1
		{
			[GetComponent(target = GetComponentTarget.Children)]
			public ComponentReference reference2;
		}

		private class CReference : BReference { }

		private class BReference : AReference
		{
			[GetComponent]
			public AReference referenceA;
		}

		private class AReference : MonoBehaviour
		{
			[GetComponent]
			public AReference referenceB;
		}

		private class DReference : CReference
		{
			[GetComponent]
			public AReference referenceD;
		}
		#endregion
		
		#region Component array references
		private class ComponentsReference : MonoBehaviour
		{
			[GetComponent]
			public ComponentsReference[] reference;
		}

		private class ComponentsReferenceParent : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Parent)]
			public ComponentReference[] reference;
		}

		private class ComponentsReferenceChild : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Children)]
			public ComponentReference[] reference;
		}
		#endregion
		
		#region Component array references inheritance
		private class ComponentsReference1 : MonoBehaviour
		{
			[GetComponent]
			public ComponentsReference2[] reference1;
		}
		
		private class ComponentsReference2 : ComponentsReference1
		{
			[GetComponent]
			public ComponentsReference2[] reference2;
		}
		
		private class ComponentsReferenceParent1 : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Parent)]
			public ComponentReference[] reference1;
		}
		
		private class ComponentsReferenceParent2 : ComponentsReferenceParent1
		{
			[GetComponent(target = GetComponentTarget.Parent)]
			public ComponentReference[] reference2;
		}
		
		private class ComponentsReferenceChild1 : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Children)]
			public ComponentReference[] reference1;
		}
		
		private class ComponentsReferenceChild2 : ComponentsReferenceChild1
		{
			[GetComponent(target = GetComponentTarget.Children)]
			public ComponentReference[] reference2;
		}
		#endregion

		#region Component list references
		private class ComponentListReference : MonoBehaviour
		{
			[GetComponent]
			public List<ComponentListReference> reference;
		}

		private class ComponentListReferenceParent : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Parent)]
			public List<ComponentReference> reference;
		}

		private class ComponentListReferenceChild : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Children)]
			public List<ComponentReference> reference;
		}
		#endregion

		#region Component list references inheritance
		private class ComponentListReference1 : MonoBehaviour
		{
			[GetComponent]
			public List<ComponentListReference2> reference1;
		}
		
		private class ComponentListReference2 : ComponentListReference1
		{
			[GetComponent]
			public List<ComponentListReference2> reference2;
		}
		
		private class ComponentListReferenceParent1 : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Parent)]
			public List<ComponentReference> reference1;
		}
		
		private class ComponentListReferenceParent2 : ComponentListReferenceParent1
		{
			[GetComponent(target = GetComponentTarget.Parent)]
			public List<ComponentReference> reference2;
		}
		
		private class ComponentListReferenceChild1 : MonoBehaviour
		{
			[GetComponent(target = GetComponentTarget.Children)]
			public List<ComponentReference> reference1;
		}
		
		private class ComponentListReferenceChild2 : ComponentListReferenceChild1
		{
			[GetComponent(target = GetComponentTarget.Children)]
			public List<ComponentReference> reference2;
		}
		#endregion
		
		[UnityTearDown]
		public IEnumerator TearDown()
		{
			for (int i = 0; i < objects.Count; i++)
			{
				Object.Destroy(objects[i]);
			}

			yield break;
		}

		#if !CECIL_ATTRIBUTES_EXPERIMENTAL_GETCOMPONENT
		[UnityTest]
		public IEnumerator GetComponent()
		{
			GameObject obj = new GameObject("", typeof(ComponentReference));
			objects.Add(obj);

			ComponentReference comp = obj.GetComponent<ComponentReference>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.IsNotNull(comp.reference);
		}

		[UnityTest]
		public IEnumerator GetComponentInParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentReferenceParent));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentReferenceParent comp = child.GetComponent<ComponentReferenceParent>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.IsNotNull(comp.reference);
		}

		[UnityTest]
		public IEnumerator GetComponentInChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentReferenceChild));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentReferenceChild comp = parent.GetComponent<ComponentReferenceChild>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.IsNotNull(comp.reference);
		}

		[UnityTest]
		public IEnumerator GetComponentInheritance()
		{
			GameObject obj = new GameObject("", typeof(ComponentReference2));
			objects.Add(obj);

			ComponentReference2 comp = obj.GetComponent<ComponentReference2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.IsNotNull(comp.reference1);
			Assert.IsNotNull(comp.reference2);
		}

		[UnityTest]
		public IEnumerator GetComponentInheritanceParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentReferenceParent2));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentReferenceParent2 comp = child.GetComponent<ComponentReferenceParent2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.IsNotNull(comp.reference1);
			Assert.IsNotNull(comp.reference2);
		}

		[UnityTest]
		public IEnumerator GetComponentInheritanceChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentReferenceChild2));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentReferenceChild2 comp = parent.GetComponent<ComponentReferenceChild2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.IsNotNull(comp.reference1);
			Assert.IsNotNull(comp.reference2);
		}

		[UnityTest]
		public IEnumerator GetComponentWeirdOrder()
		{
			GameObject obj = new GameObject("", typeof(DReference));
			objects.Add(obj);

			DReference comp = obj.GetComponent<DReference>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.referenceA);
			Assert.IsNull(comp.referenceB);
			Assert.IsNull(comp.referenceD);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.IsNotNull(comp.referenceA);
			Assert.IsNotNull(comp.referenceB);
			Assert.IsNotNull(comp.referenceD);
		}

		[UnityTest]
		public IEnumerator GetComponents()
		{
			GameObject obj = new GameObject("", typeof(ComponentsReference));
			objects.Add(obj);

			ComponentsReference comp = obj.GetComponent<ComponentsReference>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference.Length);
		}

		[UnityTest]
		public IEnumerator GetComponentsInParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentsReferenceParent));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentsReferenceParent comp = child.GetComponent<ComponentsReferenceParent>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference.Length);
		}

		[UnityTest]
		public IEnumerator GetComponentsInChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentsReferenceChild));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentsReferenceChild comp = parent.GetComponent<ComponentsReferenceChild>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference.Length);
		}
		
		[UnityTest]
		public IEnumerator GetComponentsInheritance()
		{
			GameObject obj = new GameObject("", typeof(ComponentsReference2));
			objects.Add(obj);

			ComponentsReference2 comp = obj.GetComponent<ComponentsReference2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Length);
			Assert.AreEqual(1, comp.reference2.Length);
		}

		[UnityTest]
		public IEnumerator GetComponentsInheritanceParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentsReferenceParent2));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentsReferenceParent2 comp = child.GetComponent<ComponentsReferenceParent2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Length);
			Assert.AreEqual(1, comp.reference2.Length);
		}

		[UnityTest]
		public IEnumerator GetComponentsInheritanceChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentsReferenceChild2));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentsReferenceChild2 comp = parent.GetComponent<ComponentsReferenceChild2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Length);
			Assert.AreEqual(1, comp.reference2.Length);
		}

		[UnityTest]
		public IEnumerator GetComponentList()
		{
			GameObject obj = new GameObject("", typeof(ComponentListReference));
			objects.Add(obj);

			ComponentListReference comp = obj.GetComponent<ComponentListReference>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference.Count);
		}

		[UnityTest]
		public IEnumerator GetComponentListInParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentListReferenceParent));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentListReferenceParent comp = child.GetComponent<ComponentListReferenceParent>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference.Count);
		}

		[UnityTest]
		public IEnumerator GetComponentListInChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentListReferenceChild));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentListReferenceChild comp = parent.GetComponent<ComponentListReferenceChild>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference.Count);
		}
		
		[UnityTest]
		public IEnumerator GetComponentListInheritance()
		{
			GameObject obj = new GameObject("", typeof(ComponentListReference2));
			objects.Add(obj);

			ComponentListReference2 comp = obj.GetComponent<ComponentListReference2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Count);
			Assert.AreEqual(1, comp.reference2.Count);
		}

		[UnityTest]
		public IEnumerator GetComponentListInheritanceParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentListReferenceParent2));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentListReferenceParent2 comp = child.GetComponent<ComponentListReferenceParent2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Count);
			Assert.AreEqual(1, comp.reference2.Count);
		}

		[UnityTest]
		public IEnumerator GetComponentListInheritanceChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentListReferenceChild2));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentListReferenceChild2 comp = parent.GetComponent<ComponentListReferenceChild2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is IGetComponent);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((IGetComponent) comp).FetchComponents();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Count);
			Assert.AreEqual(1, comp.reference2.Count);
		}
#else
		[UnityTest]
		public IEnumerator GetComponent()
		{
			GameObject obj = new GameObject("", typeof(ComponentReference));
			objects.Add(obj);

			ComponentReference comp = obj.GetComponent<ComponentReference>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.IsNotNull(comp.reference);
		}

		[UnityTest]
		public IEnumerator GetComponentInParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentReferenceParent));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentReferenceParent comp = child.GetComponent<ComponentReferenceParent>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.IsNotNull(comp.reference);
		}

		[UnityTest]
		public IEnumerator GetComponentInChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentReferenceChild));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentReferenceChild comp = parent.GetComponent<ComponentReferenceChild>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.IsNotNull(comp.reference);
		}

		[UnityTest]
		public IEnumerator GetComponentInheritance()
		{
			GameObject obj = new GameObject("", typeof(ComponentReference2));
			objects.Add(obj);

			ComponentReference2 comp = obj.GetComponent<ComponentReference2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.IsNotNull(comp.reference1);
			Assert.IsNotNull(comp.reference2);
		}

		[UnityTest]
		public IEnumerator GetComponentInheritanceParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentReferenceParent2));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentReferenceParent2 comp = child.GetComponent<ComponentReferenceParent2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.IsNotNull(comp.reference1);
			Assert.IsNotNull(comp.reference2);
		}

		[UnityTest]
		public IEnumerator GetComponentInheritanceChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentReferenceChild2));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentReferenceChild2 comp = parent.GetComponent<ComponentReferenceChild2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.IsNotNull(comp.reference1);
			Assert.IsNotNull(comp.reference2);
		}

		[UnityTest]
		public IEnumerator GetComponentWeirdOrder()
		{
			GameObject obj = new GameObject("", typeof(DReference));
			objects.Add(obj);

			DReference comp = obj.GetComponent<DReference>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.referenceA);
			Assert.IsNull(comp.referenceB);
			Assert.IsNull(comp.referenceD);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.IsNotNull(comp.referenceA);
			Assert.IsNotNull(comp.referenceB);
			Assert.IsNotNull(comp.referenceD);
		}

		[UnityTest]
		public IEnumerator GetComponents()
		{
			GameObject obj = new GameObject("", typeof(ComponentsReference));
			objects.Add(obj);

			ComponentsReference comp = obj.GetComponent<ComponentsReference>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference.Length);
		}

		[UnityTest]
		public IEnumerator GetComponentsInParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentsReferenceParent));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentsReferenceParent comp = child.GetComponent<ComponentsReferenceParent>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference.Length);
		}

		[UnityTest]
		public IEnumerator GetComponentsInChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentsReferenceChild));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentsReferenceChild comp = parent.GetComponent<ComponentsReferenceChild>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference.Length);
		}
		
		[UnityTest]
		public IEnumerator GetComponentsInheritance()
		{
			GameObject obj = new GameObject("", typeof(ComponentsReference2));
			objects.Add(obj);

			ComponentsReference2 comp = obj.GetComponent<ComponentsReference2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Length);
			Assert.AreEqual(1, comp.reference2.Length);
		}

		[UnityTest]
		public IEnumerator GetComponentsInheritanceParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentsReferenceParent2));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentsReferenceParent2 comp = child.GetComponent<ComponentsReferenceParent2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Length);
			Assert.AreEqual(1, comp.reference2.Length);
		}

		[UnityTest]
		public IEnumerator GetComponentsInheritanceChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentsReferenceChild2));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentsReferenceChild2 comp = parent.GetComponent<ComponentsReferenceChild2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Length);
			Assert.AreEqual(1, comp.reference2.Length);
		}

		[UnityTest]
		public IEnumerator GetComponentList()
		{
			GameObject obj = new GameObject("", typeof(ComponentListReference));
			objects.Add(obj);

			ComponentListReference comp = obj.GetComponent<ComponentListReference>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference.Count);
		}

		[UnityTest]
		public IEnumerator GetComponentListInParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentListReferenceParent));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentListReferenceParent comp = child.GetComponent<ComponentListReferenceParent>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference.Count);
		}

		[UnityTest]
		public IEnumerator GetComponentListInChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentListReferenceChild));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentListReferenceChild comp = parent.GetComponent<ComponentListReferenceChild>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference.Count);
		}
		
		[UnityTest]
		public IEnumerator GetComponentListInheritance()
		{
			GameObject obj = new GameObject("", typeof(ComponentListReference2));
			objects.Add(obj);

			ComponentListReference2 comp = obj.GetComponent<ComponentListReference2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Count);
			Assert.AreEqual(1, comp.reference2.Count);
		}

		[UnityTest]
		public IEnumerator GetComponentListInheritanceParent()
		{
			GameObject parent = new GameObject("", typeof(ComponentReference));
			GameObject child = new GameObject("", typeof(ComponentListReferenceParent2));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentListReferenceParent2 comp = child.GetComponent<ComponentListReferenceParent2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Count);
			Assert.AreEqual(1, comp.reference2.Count);
		}

		[UnityTest]
		public IEnumerator GetComponentListInheritanceChild()
		{
			GameObject parent = new GameObject("", typeof(ComponentListReferenceChild2));
			GameObject child = new GameObject("", typeof(ComponentReference));
			objects.Add(parent);
			objects.Add(child);

			child.transform.SetParent(parent.transform);

			yield return null;

			ComponentListReferenceChild2 comp = parent.GetComponent<ComponentListReferenceChild2>();

			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is ISerializationCallbackReceiver);
			Assert.IsNull(comp.reference1);
			Assert.IsNull(comp.reference2);

			((ISerializationCallbackReceiver) comp).OnBeforeSerialize();

			yield return null;

			Assert.AreEqual(1, comp.reference1.Count);
			Assert.AreEqual(1, comp.reference2.Count);
		}
		#endif
	}
}