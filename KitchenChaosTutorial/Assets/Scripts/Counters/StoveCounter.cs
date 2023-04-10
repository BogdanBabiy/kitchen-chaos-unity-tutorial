using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.onProgressChangedEventArgs> onProgressChanged;
    public event EventHandler<onStateChangedEventArg> onStateChanged;
    public class onStateChangedEventArg : EventArgs
    {
        public State state;
    }
    
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSoArray;
    
    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSo;
    private State currentState;

    private void Start()
    {
        currentState = State.Idle;
        onStateChanged?.Invoke(this, new onStateChangedEventArg
        {
            state = currentState
        });
    }

    void Update()
    {
        if (HasKitchenObject())
        {
            switch (currentState)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    onProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer/fryingRecipeSo.fryingTimeMax
                    });
                    
                    if (fryingTimer > fryingRecipeSo.fryingTimeMax)
                    {
                        //Fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(this, fryingRecipeSo.output);

                        currentState = State.Fried;
                        onStateChanged?.Invoke(this, new onStateChangedEventArg
                        {
                            state = currentState
                        });
                       
                    }
                    break;
                case State.Fried:
                    // Get the new recipe for cooked -> burned meat patty
                    fryingRecipeSo = GetFryingRecipeSoWithInput(GetKitchenObject().GetKitchenObjectSO());
                    burningTimer += Time.deltaTime;
                    onProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer/fryingRecipeSo.fryingTimeMax
                    });
                    if (burningTimer > fryingRecipeSo.fryingTimeMax)
                    {
                        //Fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(this, fryingRecipeSo.output);

                        currentState = State.Burned;
                        onStateChanged?.Invoke(this, new onStateChangedEventArg
                        {
                            state = currentState
                        });
                    }
                    break;
                case State.Burned:
                    onProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                    break;
            }  
        }
    }

    public override void Interact(Player player)
    {
        fryingTimer = 0f;
        burningTimer = 0f;
        onProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
        
        // Counter has no objects
        if (!HasKitchenObject())
        {
            // Give object from player to counter
            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                fryingRecipeSo = GetFryingRecipeSoWithInput(player.GetKitchenObject().GetKitchenObjectSO());

                player.GetKitchenObject().SetKitchenObjectParent(this);
               
                currentState = State.Frying;
                onStateChanged?.Invoke(this, new onStateChangedEventArg
                {
                    state = currentState
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
                        
                        currentState = State.Idle;
                        onStateChanged?.Invoke(this, new onStateChangedEventArg
                        {
                            state = currentState
                        });
                    }
                }
            }
            else
            {
                // Give object to the player
                GetKitchenObject().SetKitchenObjectParent(player);
                currentState = State.Idle;
                onStateChanged?.Invoke(this, new onStateChangedEventArg
                {
                    state = currentState
                });
            }
           
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSo)
    {
        FryingRecipeSO fryingRecipeSo = GetFryingRecipeSoWithInput(inputKitchenObjectSo);

        return fryingRecipeSo != null;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSo)
    {
        FryingRecipeSO fryingRecipeSo = GetFryingRecipeSoWithInput(inputKitchenObjectSo);

        return fryingRecipeSo?.output;
    }

    private FryingRecipeSO GetFryingRecipeSoWithInput(KitchenObjectSO inputKitchenObjectSo)
    {
        foreach (var fryingRecipeSo in fryingRecipeSoArray)
        {
            if (fryingRecipeSo.input == inputKitchenObjectSo)
            {
                return fryingRecipeSo;
            }
        }
        return null;
    }
}
