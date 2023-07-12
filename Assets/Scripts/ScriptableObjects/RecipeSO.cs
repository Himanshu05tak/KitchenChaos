using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
   [CreateAssetMenu(menuName = "RecipeSO", fileName = "RecipeSO", order = 0)]
   public class RecipeSO : ScriptableObject
   {
      public List<KitchenObjectSO> KitchenObjectSOList;
      public string recipeName;
   }
}
