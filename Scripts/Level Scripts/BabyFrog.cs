using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BabyFrog : MonoBehaviourPunCallbacks
{
    private LevelMaster levelMaster;
    private IngredientGotHandler _ingredientGotHandler;
    private ObjectivesUIManager _objectivesUIManager;

    [SerializeField] private SO_IngredientData _ingredientData;
    private void OnEnable()
    {
        SetInitialReferences();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            photonView.RPC("CallEverything", RpcTarget.All);
        }
    }

    void SetInitialReferences() 
    {
        levelMaster = FindObjectOfType<LevelMaster>();
        _ingredientGotHandler = FindObjectOfType<IngredientGotHandler>();
        _objectivesUIManager = FindObjectOfType<ObjectivesUIManager>();
    }

    [PunRPC]
    void CallEverything()
    {
        _objectivesUIManager.UpdateUI(_ingredientData);
        _ingredientGotHandler.AddToInventory(_ingredientData);
        levelMaster.CallEventPlayerGetsBabyFrog();
        Destroy(gameObject);
    }
}
