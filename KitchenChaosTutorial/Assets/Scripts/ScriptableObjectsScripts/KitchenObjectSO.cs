using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] //Creates Menu Choice to create a new kitchen Object SO
public class KitchenObjectSO : ScriptableObject
{
    // SO = Scriptable Objects
    // Usefull for stuff that you will have many instances of like weapons

    // Only public for Scriptable Objects
    // Never write to a scriptable object field
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}
