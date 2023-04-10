using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    
    public static DeliveryManager Instance { get; private set; }
    
    [SerializeField] private RecipeListSO recipeListSO;
    
    private List<RecipeSO> waititngRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int recipesDeliveredAmount;

    private void Awake()
    {
        Instance = this;
        
        waititngRecipeSOList = new();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        
        if (spawnRecipeTimer <= 0f && waititngRecipeSOList.Count < waitingRecipesMax)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            RecipeSO waitingRecipeSO = GetRandomRecipeSO();
            waititngRecipeSOList.Add(waitingRecipeSO);
            
            OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
        }
    }

    private RecipeSO GetRandomRecipeSO()
    {
        int recipeSOListCount = recipeListSO.recipeSOList.Count;
        return recipeListSO.recipeSOList[Random.Range(0,recipeSOListCount)];
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        foreach(var waitingRecipeSO in waititngRecipeSOList)
        {
            if (waitingRecipeSO.KitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                bool plateIngredientsMatchRecipe = true;
                // Has the same number of ingredients
                foreach (var recipekitchenObjectSO in waitingRecipeSO.KitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    // Cycling through all the ingredients on the recipe
                    foreach (var plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // Cycling through all the ingredients on the plate
                        if (plateKitchenObjectSO == recipekitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound)
                    {
                        // Recipe ingredient was not found on the plate
                        plateIngredientsMatchRecipe = false;
                    }
                }

                if (plateIngredientsMatchRecipe)
                {
                    // Player delivered the correct recipe
                    waititngRecipeSOList.Remove(waitingRecipeSO);
                    OnRecipeCompleted?.Invoke(this,EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this,EventArgs.Empty);
                    recipesDeliveredAmount++;
                    return;
                }
            }
        }
        
        // No matches found!
        // PLayer didn't deliver valid recipe
        // Play Delivery Failed SFX
        OnRecipeFailed?.Invoke(this,EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waititngRecipeSOList;
    }

    public int GetRecipesDeliveredAmount()
    {
        return recipesDeliveredAmount;
    }
}
