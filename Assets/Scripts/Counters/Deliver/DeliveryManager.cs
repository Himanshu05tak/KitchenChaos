using System;
using System.Collections.Generic;
using System.Linq;
using Counters.Plate;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Counters
{
    public class DeliveryManager : MonoBehaviour
    {
        public event EventHandler OnRecipeSpawned;
        public event EventHandler OnRecipeComplete;
        public event EventHandler OnRecipeFailed;
        public event EventHandler OnRecipeSuccess;
        
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
            {
                _spawnRecipeTimer = _spawnRecipeTimerMax;
                if (_waitingRecipeSoList.Count < _waitingRecipesMax)
                {
                    var waitingRecipeSo = recipeListSo.RecipeSOList[Random.Range(0, recipeListSo.RecipeSOList.Count)];
                    Debug.Log($"{waitingRecipeSo.recipeName}");
                    _waitingRecipeSoList.Add(waitingRecipeSo);
                    OnRecipeSpawned?.Invoke(this,EventArgs.Empty);
                }
            }
        }

        public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
        {
            for (var i = 0; i < _waitingRecipeSoList.Count; i++)
            {
                var waitingRecipeSo = _waitingRecipeSoList[i];
                if (waitingRecipeSo.KitchenObjectSOList.Count !=
                    plateKitchenObject.GetKitchenObjectSoList().Count) continue;
                //Has the same number of ingredient
                var plateContentsMatchesRecipe = true;
                foreach (var ingredientFound in waitingRecipeSo.KitchenObjectSOList.
                             Select(recipeKitchenObjectSo => plateKitchenObject.GetKitchenObjectSoList().
                             Any(plateKitchenObjectSo => plateKitchenObjectSo == recipeKitchenObjectSo)).
                             Where(ingredientFound => !ingredientFound))
                {
                    plateContentsMatchesRecipe = false;
                    //This recipe ingredient wasn't found  on the plate
                }
                if (!plateContentsMatchesRecipe) continue;
                //player delivered the correct recipe
                Debug.Log("Player delivered  the correct recipe");
                _waitingRecipeSoList.RemoveAt(i);
                OnRecipeComplete?.Invoke(this,EventArgs.Empty);
                OnRecipeSuccess?.Invoke(this,EventArgs.Empty);
                return;
            }
            //No matches found
            //Player did not deliver a correct recipe
            OnRecipeFailed?.Invoke(this,EventArgs.Empty);

            Debug.Log("Player did not deliver a correct recipe");

        }

        public List<RecipeSO> GetWaitingRecipeSoList()
        {
            return _waitingRecipeSoList;
        }
    }
}
