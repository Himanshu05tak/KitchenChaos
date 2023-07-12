using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingleUI : MonoBehaviour
{
   [SerializeField] private Image image;
  
   public void SetKitchenObjectSo(KitchenObjectSO kitchenObjectSo)
   {
      image.sprite = kitchenObjectSo.sprite;
   }
}
