using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Counters.Plate
{
    public class PlateCompleteVisual : MonoBehaviour
    {
        [SerializeField] private PlateKitchenObject plateKitchenObject;
        [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSoGameObjectList;
        private void Start()
        {
            plateKitchenObject.OnIngredientAdded += PlateKitchenObjectOnOnIngredientAdded;
            
            foreach (var kitchenObjectSoGameObject in kitchenObjectSoGameObjectList)
            {
                kitchenObjectSoGameObject.gameObject.SetActive(false);
            }
        }

        private void PlateKitchenObjectOnOnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
        {
            foreach (var kitchenObjectSoGameObject in kitchenObjectSoGameObjectList.Where(kitchenObjectSoGameObject => kitchenObjectSoGameObject.kitchenObjectSo == e.KitchenObjectSo))
            {
                kitchenObjectSoGameObject.gameObject.SetActive(true);
            }
        }
        
        [Serializable]
        public struct KitchenObjectSO_GameObject
        {
            public KitchenObjectSO kitchenObjectSo;
            public GameObject gameObject;
        }
    }
}
