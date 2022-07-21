using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class InteractibleStats : ScriptableObject
{
    //[SerializeField] public InteractionType interactionType;
    [SerializeField] public List<bool> availableInteractionTypeList = new List<bool>();
    //[SerializeField] public ReactionType reactionType;
    [SerializeField] public List<bool> availableReactionTypeList = new List<bool>();
    //[SerializeField] public Color interactionColor;




}
