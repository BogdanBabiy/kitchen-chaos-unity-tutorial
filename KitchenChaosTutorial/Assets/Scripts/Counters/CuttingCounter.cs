using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    // Fires the event when ANY cutting counter does a cut action
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }
    
    public event EventHandler<IHasProgress.onProgressChangedEventArgs> onProgressChanged;

    public event EventHandler onCut;
    
    [SerializeField] private CuttingRecipeSO[] cuttingReciperSoArray;

    private int cuttingProgress;
    public override void Interact(Player player)
    {
        // Counter has no objects
        if (!HasKitchenObject())
        {
            // Give object from player to counter
            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                CuttingRecipeSO cuttingRecipeSo = GetCuttingReciperSoWithInput(player.GetKitchenObject().GetKitchenObjectSO());

                player.GetKitchenObject().SetKitchenObjectParent(this);
                cuttingProgress = 0;
                
                onProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs
                {
                    progressNormalized = (float) cuttingProgress / cuttingRecipeSo.cuttingProgressMax
                });
            }
        }
        else // Counter has an object
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is holding a plate
                    // Instead of casting can also do "as PlateKitchenObject"
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
            else
            {
                // Give object to the player
                GetKitchenObject().SetKitchenObjectParent(player);   
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject()&& HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;
            CuttingRecipeSO cuttingRecipeSo = GetCuttingReciperSoWithInput(GetKitchenObject().GetKitchenObjectSO());

            onCut?.Invoke(this,EventArgs.Empty);
            OnAnyCut?.Invoke(this,EventArgs.Empty);
            onProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs
            {
                progressNormalized = (float) cuttingProgress / cuttingRecipeSo.cuttingProgressMax
            });
            if (cuttingProgress >= cuttingRecipeSo.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSo = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(this, outputKitchenObjectSo);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSo)
    {
        CuttingRecipeSO cuttingRecipeSo = GetCuttingReciperSoWithInput(inputKitchenObjectSo);

        return cuttingRecipeSo != null;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSo)
    {
        CuttingRecipeSO cuttingRecipeSo = GetCuttingReciperSoWithInput(inputKitchenObjectSo);

        return cuttingRecipeSo?.output;
    }

    private CuttingRecipeSO GetCuttingReciperSoWithInput(KitchenObjectSO inputKitchenObjectSo)
    {
        foreach (var cuttingRecipeSo in cuttingReciperSoArray)
        {
            if (cuttingRecipeSo.input == inputKitchenObjectSo)
            {
                return cuttingRecipeSo;
            }
        }
        return null;
    }
}
