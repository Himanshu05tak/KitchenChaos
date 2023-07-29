using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
   [CreateAssetMenu(menuName = "ListSO/KitchenObjectList", fileName = "KitchenObjectList", order = 1)]
   public class KitchenObjectListSO : ScriptableObject
   {
      [FormerlySerializedAs("kitchenObjectListSo")] public List<KitchenObjectSO> kitchenObjectList;
   }
}
