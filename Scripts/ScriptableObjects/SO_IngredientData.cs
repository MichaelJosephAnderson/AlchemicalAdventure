using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObjects/SO_Ingredient", order = 1)]
public class SO_IngredientData : ScriptableObject
{
   public Sprite _ingredientImage;
   public int _ingredientIndex;
}
