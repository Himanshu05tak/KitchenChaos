using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "FryingRecipe",menuName = "FryingRecipe/Item")]
    public class FryingRecipeSO : ScriptableObject
    {
        public KitchenObjectSO input;
        public KitchenObjectSO output;
        public float fryingTimerMax;
    }
}
