using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "BurningRecipe",menuName = "BurningRecipe/Item")]
    public class BurningRecipeSO : ScriptableObject
    {
        public KitchenObjectSO input;
        public KitchenObjectSO output;
        public float burningTimerMax;
    }
}
