using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "KitchenObjectSlices",menuName = "KitchenObjectSlices/Item")]
    public class CuttingRecipeSO : ScriptableObject
    {
        public KitchenObjectSO input;
        public KitchenObjectSO output;
        public int cuttingProgressMax;
    }
}
