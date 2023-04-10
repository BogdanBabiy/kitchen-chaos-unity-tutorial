using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;
    
    private void Awake(){
        iconTemplate.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in transform)
        {
           if (child != iconTemplate) Destroy(child.gameObject);
        }
        List<KitchenObjectSO> kitchenObjectSOList = plateKitchenObject.GetKitchenObjectSOList();
        foreach (var kitchenObjectSO in kitchenObjectSOList)
        {
           Transform iconTransform = Instantiate(iconTemplate, transform);
           iconTransform.gameObject.SetActive(true);
           iconTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
