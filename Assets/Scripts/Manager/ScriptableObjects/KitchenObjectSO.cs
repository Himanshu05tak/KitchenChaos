using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "KitchenObject",menuName = "KitchenObject/Item")]
    public class KitchenObjectSO : ScriptableObject
    {
        public Transform prefab;
        public Sprite sprite;
        public string objectName;
    }
}
