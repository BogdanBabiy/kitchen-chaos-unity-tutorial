using System;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
          KitchenObject.SpawnKitchenObject(player, kitchenObjectSO);
          OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
        // else 
        // {
        //     player.GetKitchenObject().DestroySelf();
        // }
        // Play OpenClose Counter Animation
    }
}
