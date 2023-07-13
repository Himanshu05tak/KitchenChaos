using System;
using ScriptableObjects;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Counters
{
    public class DeliveryManagerUI : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private Transform recipeTemplate;

        private void Awake()
        {
            recipeTemplate.gameObject.SetActive(false);
        }

        private void Start()
        {
            DeliveryManager.Instance.OnRecipeSpawned += DeliveryManagerOnRecipeSpawned;
            DeliveryManager.Instance.OnRecipeComplete += DeliveryManagerOnRecipeComplete;
        }

        private void DeliveryManagerOnRecipeComplete(object sender, EventArgs e)
        {
            UpdateVisual();
        }

        private void DeliveryManagerOnRecipeSpawned(object sender, EventArgs e)
        {
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            foreach (Transform child in container)
            {
                if(child==recipeTemplate) continue;
                Destroy(child.gameObject);
            }

            foreach (var recipeSo in DeliveryManager.Instance.GetWaitingRecipeSoList())
            {
                var recipeTransform = Instantiate(recipeTemplate, container);
                recipeTransform.gameObject.SetActive(true);
                recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSo(recipeSo);
            }
        }
    }
}
