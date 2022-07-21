using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Unit Stats",menuName = "Scriptable Objects/Unit/Stats Template")]
public class UnitStats : ScriptableObject
{
    [SerializeField] public int team;
    [SerializeField] public float maxHealth;
    [SerializeField] public float movementSpeed;
}
