using System;
using UnityEngine;

    public class BaseCounter : MonoBehaviour, IKitchenObjectParent
    {
        public static event EventHandler OnAnyObjectPlaced;
        
        public static void ResetStaticData()
        {
            OnAnyObjectPlaced = null;
        }
        
        [SerializeField] private Transform counterTopPoint;

        private KitchenObject kitchenObject;
       
        
        // protected = cannot call BaseCounter.Method,  Only ChildCounter.Method
        // virtual = child classes can have a separate implementation
        // abstract = child class MUST have their own implementation
        public virtual void Interact(Player player)
        {
            Debug.LogError("BaseCounter.Interact();");
        }
        
        public virtual void InteractAlternate(Player player)
        {
            Debug.LogError("BaseCounter.InteractAlternate();");
        }

        public Transform GetKitchenObjectFollowTransform()
        {
            return counterTopPoint;
        }

        public void SetKitchenObject(KitchenObject kitchenObject)
        {
            this.kitchenObject = kitchenObject;
            
            if (kitchenObject != null) OnAnyObjectPlaced?.Invoke(this,EventArgs.Empty);
        }

        public KitchenObject GetKitchenObject()
        {
            return kitchenObject;
        }

        public void ClearKitchenObject()
        {
            kitchenObject = null;
        }

        public bool HasKitchenObject()
        {
            return kitchenObject != null;
        }
    }