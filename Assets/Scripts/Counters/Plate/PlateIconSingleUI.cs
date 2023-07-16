using UnityEngine;
using UnityEngine.UI;
using ScriptableObjects;

namespace Counters.Plate
{
   public class PlateIconSingleUI : MonoBehaviour
   {
      [SerializeField] private Image image;
  
      public void SetKitchenObjectSo(KitchenObjectSO kitchenObjectSo)
      {
         image.sprite = kitchenObjectSo.sprite;
      }
   }
}
