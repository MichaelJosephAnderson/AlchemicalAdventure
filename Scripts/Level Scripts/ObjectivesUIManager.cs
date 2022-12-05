using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesUIManager : MonoBehaviour
{
   [SerializeField] private List<SO_IngredientData> _ingredientsInLevel;
   [SerializeField] private List<Image> _ingredientImages;
   [SerializeField] private List<GameObject> _UIFrames;
   [SerializeField] private List<Image> _checks;

   [SerializeField] private List<int> _imageIndex;
   private int _imagePosition;

   private void OnEnable()
   {
      SetInitialReferences();
   }

   private void SetInitialReferences()
   {
      _imageIndex = new List<int>();
      foreach (var image in _ingredientImages)
      {
         image.sprite = null;
         image.gameObject.SetActive(false);
      }
      
      for (int i = 0; i < _ingredientsInLevel.Count; i++)
      {
         _ingredientImages[i].sprite = _ingredientsInLevel[i]._ingredientImage;
         _imageIndex.Add(_ingredientsInLevel[i]._ingredientIndex);
         _ingredientImages[i].gameObject.SetActive(true);
      }

      for (int i = 0; i < _ingredientImages.Count; i++)
      {
         if (_ingredientImages[i].sprite == null)
         {
            _UIFrames[i].gameObject.SetActive(false);
         }
      }
   }

   public void UpdateUI(SO_IngredientData ingredientCollected)
   {
      _imagePosition = _imageIndex.IndexOf(ingredientCollected._ingredientIndex);
      _checks[_imagePosition].gameObject.SetActive(true);
   }
   
}
