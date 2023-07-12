using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "RecipeListSO", fileName = "RecipeListSO", order = 0)]
    public class RecipeListSO : ScriptableObject
    {
        public List<RecipeSO> RecipeSOList;
    }
}
