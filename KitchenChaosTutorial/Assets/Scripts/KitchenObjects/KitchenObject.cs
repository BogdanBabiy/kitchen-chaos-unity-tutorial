using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private IKitchenObjectParent kitchenObjectParent;
    
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public static void SpawnKitchenObject(IKitchenObjectParent kitchenObjectParent, KitchenObjectSO kitchenObjectSo )
    {
        // Spawn Kitchen Object and give to player
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSo.prefab);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) // <- refers to the new counter
    {
        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError($"{kitchenObjectParent} already has a KitchenObject!");
        }
        else
        {
            // this.kitchenObjectParent refers to the current kitchenObjectParent
            if (this.kitchenObjectParent != null)
            {
                this.kitchenObjectParent.ClearKitchenObject();
            }
            this.kitchenObjectParent = kitchenObjectParent;
            kitchenObjectParent.SetKitchenObject(this);
            transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
            transform.localPosition =Vector3.zero;
        }
    }

    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }
}
