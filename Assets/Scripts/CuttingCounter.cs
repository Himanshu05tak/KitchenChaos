using System.Linq;
using Controller;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

public class CuttingCounter : BaseCounter
{  
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSos;
    
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //There is no kitchen Object here
            Debug.Log("There is no kitchen Object ");
            if (player.HasKitchenObject())
            {
                //Player is carrying something 
                if(HasRecipeWithInput(player.GetKitchenObject().GetKitchenObject))
                    player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //Player is not carrying anything
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                //Player is carrying something 
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                //Player is not carrying anything
            }
            //There is a kitchen Object here
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (!HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObject)) return;

        var outputKitchenObjectSo = GetOutputForInput(GetKitchenObject().GetKitchenObject);
        GetKitchenObject().DestroySelf();
        KitchenObject.SpawnKitchenObject(outputKitchenObjectSo, this);
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSo)
    {
        return cuttingRecipeSos.Any(cuttingRecipeSo => cuttingRecipeSo.input == inputKitchenObjectSo);
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSo)
    {
        return (from cuttingRecipeSo in cuttingRecipeSos where cuttingRecipeSo.input == inputKitchenObjectSo select cuttingRecipeSo.output).FirstOrDefault();
    } 
}
