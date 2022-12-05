using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLifetime : MonoBehaviour
{
   [SerializeField] private GameObject _objToDes;
   [SerializeField] private float _lifetime;

   private void Update()
   {
      if (_lifetime <= 0)
      {
         Destroy(_objToDes);
      }
      else
      {
         _lifetime -= Time.deltaTime;
      }
   }
}