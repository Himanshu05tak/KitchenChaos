using System.Collections.Generic;
using Counters.Plate;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Counters
{
    public class DeliveryManager : MonoBehaviour
    {
        public static DeliveryManager Instance { get; private set; }
        [SerializeField] private RecipeListSO recipeListSo;
        
        private List<RecipeSO> _waitingRecipeSoList;
        private float _spawnRecipeTimer;
        private readonly float _spawnRecipeTimerMax = 4f;
        private readonly int _waitingRecipesMax = 4;

        private void Awake()
        {
            Instance = this;
            _waitingRecipeSoList = new List<RecipeSO>();
        }

        private void Update()
        {
            _spawnRecipeTimer -= Time.deltaTime;
            if (_spawnRecipeTimer <= 0f)
                _spawnRecipeTimer = _spawnRecipeTimerMax;

            if (_waitingRecipeSoList.Count >= _waitingRecipesMax) return;
            var waitingRecipeSo = recipeListSo.RecipeSOList[Random.Range(0, recipeListSo.RecipeSOList.Count)];
            Debug.Log($"{waitingRecipeSo.recipeName}");
            _waitingRecipeSoList.Add(waitingRecipeSo);

        }

        public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
        {
            for (var i = 0; i < _waitingRecipeSoList.Count; i++)
            {
                var waitingRecipeSo = _waitingRecipeSoList[i];
                if (waitingRecipeSo.KitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSoList().Count)
                {
                    //Has the same number of ingredient
                    var plateContentsMatchesRecipe = true;
                    foreach (var recipeKitchenObjectSo in waitingRecipeSo.KitchenObjectSOList)
                    {
                        //Cycling through all ingredients in the recipe
                        var ingredientFound = false;
                        foreach (var plateKitchenObjectSo in plateKitchenObject.GetKitchenObjectSoList())
                        {
                            //Cycling through all ingredients in the plate
                            if (plateKitchenObjectSo == recipeKitchenObjectSo)
                            {
                                //Ingredient match
                                ingredientFound = true;
                                break;
                            }
                        }

                        if (!ingredientFound)
                        {
                            plateContentsMatchesRecipe = false;
                            //This recipe ingredient wasn't found  on the plate
                        }
                    }

                    if (plateContentsMatchesRecipe)
                    {
                        //player delivered the correct recipe
                        Debug.Log("Player delivered  the correct recipe");
                        _waitingRecipeSoList.RemoveAt(i);
                        return;
                    }
                }
            }
            //No matches found
            //Player did not deliver a correct recipe
            Debug.Log("Player did not deliver a correct recipe");

        }
    }
}
