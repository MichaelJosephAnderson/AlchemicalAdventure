using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientGotHandler : MonoBehaviour
{
    public static event Action _PlayerReturnToGame;
    public static event Action _PlayerCrafting;
    
    private LevelMaster _levelMaster;
    private PlayerMaster _playerMaster;
    private PlayerMovementController _playerMovementController;

    [SerializeField] private Canvas _IngredientCollectedUI;
    [SerializeField] private Canvas _PotionCraftingUI;
    [SerializeField] private Canvas _RecipieUI;

    [SerializeField] private Image _ingredientSprite;
    [SerializeField] private TMP_Text _timeRemainingText;
    [SerializeField] private TMP_Text _currentScoreText;
    [SerializeField] private Image _outlineYes;
    [SerializeField] private Image _outlineNo;

    [SerializeField] private List<SO_IngredientData> _inventory;
    [SerializeField] private List<Image> _inventoryImages;
    [SerializeField] private List<Image> _inventoryHighlights;
    [SerializeField] private List<Image> _craftingHighlighs;
    [SerializeField] private Image _selection1;
    [SerializeField] private Image _selection2;
    [SerializeField] private Image _finalPotion;
    [SerializeField] private TMP_Text _selectionText;
    [SerializeField] private TMP_Text _potionText;
    [SerializeField] private TMP_Text _finalText;
    [SerializeField] private List<Sprite> _potionImages;
    [SerializeField] private Image _potionOutlineYes;
    [SerializeField] private Image _potionOutlineNo;

    private int _currentSelectedIngredient;
    private bool _updateSelection;
    private int _numSelectionsMade;
    private SO_IngredientData _ingredient1;
    private SO_IngredientData _ingredient2;
    private int _craftingIndex;
    private bool _updatePotion;
    private int _potionIndex;
    private bool _leaveWindow;
    private bool _madePotion;
    
    private float _timeRemaining;
    private bool _active;
    private bool _crafPotions;
    private bool _currentlyCrafting;
    private bool _doneSelecting;
    private bool _doneCrafting;
    
    private void OnEnable()
    {
        SetInitialReferences();
        SpawnPlayers._OnPlayerSpawn += SetupPlayer;
        //_levelMaster.EventPlayerGetsBabyFrog += IngredientGot;
    }

    private void OnDisable()
    {
        SpawnPlayers._OnPlayerSpawn -= SetupPlayer;
        //_levelMaster.EventPlayerGetsBabyFrog -= IngredientGot;
        _playerMaster.EventPlayerGetsIngredient -= IngredientGot;
    }

    private void SetupPlayer()
    {
        _playerMaster = FindObjectOfType<PlayerMaster>();
        _playerMovementController = FindObjectOfType<PlayerMovementController>();
        _playerMaster.EventPlayerGetsIngredient += IngredientGot;
    }

    void SetInitialReferences()
    {
        _levelMaster = FindObjectOfType<LevelMaster>();
        _IngredientCollectedUI.gameObject.SetActive(false);
        _outlineYes.gameObject.SetActive(false);
        _inventory = new List<SO_IngredientData>();
    }

    private void Update()
    {
        //if player opens/closes crafting window
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_active)
            {
                _active = false;
                _PlayerReturnToGame?.Invoke();
                //_playerMaster.CallEventPlayerResets();
                    
                _IngredientCollectedUI.gameObject.SetActive(false);
                _outlineYes.gameObject.SetActive(false);
                _outlineNo.gameObject.SetActive(true);
            }
            else
            {
                _PlayerCrafting?.Invoke();
                _IngredientCollectedUI.gameObject.SetActive(true);
                _active = true;
            }
        }
        //show crafting recepies
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _RecipieUI.gameObject.SetActive(true);
        }else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _RecipieUI.gameObject.SetActive(false);
        }
        //handle crafting
        if (_active)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _outlineYes.gameObject.SetActive(true);
                _outlineNo.gameObject.SetActive(false);
                _crafPotions = true;
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                _outlineNo.gameObject.SetActive(true);
                _outlineYes.gameObject.SetActive(false);
                _crafPotions = false;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_crafPotions)
                {
                    SetUpInventory();
                    
                    _PotionCraftingUI.gameObject.SetActive(true);
                    _IngredientCollectedUI.gameObject.SetActive(false);
                    _active = false;
                    _currentlyCrafting = true;
                }
                else
                {
                    _active = false;
                    _PlayerReturnToGame?.Invoke();
                    //_playerMaster.CallEventPlayerResets();
                    
                    _IngredientCollectedUI.gameObject.SetActive(false);
                    _outlineYes.gameObject.SetActive(false);
                    _outlineNo.gameObject.SetActive(true);
                }
            }
        }else if (_currentlyCrafting)
        {
            if (!_doneCrafting)
            {
                if (!_doneSelecting)
                {
                    SelectIngredients();
                }
                else
                {
                    CraftPotion();
                }   
            }
            else
            {
                ConfirmPotion();
            }
        }
    }

    void IngredientGot()
    {
        //_active = true;

        _ingredientSprite.sprite = _inventory[0]._ingredientImage;
        
        _timeRemaining = _levelMaster._levelTimer;
        _timeRemaining = Mathf.Round(_timeRemaining);
        _timeRemainingText.text = "Time Remaining: " + _timeRemaining.ToString();

        int _scoreToAdd = (int) _timeRemaining * 2;
        _currentScoreText.text = "Bonus Points: " + _scoreToAdd.ToString();
        _levelMaster.CallEventScoreIncrease(_scoreToAdd);
        
        //_IngredientCollectedUI.gameObject.SetActive(true);
    }

    public void AddToInventory(SO_IngredientData _item)
    {
        _inventory.Insert(0, _item);
    }

    private void SetUpInventory()
    {
        foreach (var image in _inventoryImages)
        {
            image.gameObject.SetActive(false);
        }
        foreach (var image in _inventoryHighlights)
        {
            image.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < _inventory.Count; i++)
        {
            _inventoryImages[i].sprite = _inventory[i]._ingredientImage;
            _inventoryImages[i].gameObject.SetActive(true);
        }
        foreach (var image in _craftingHighlighs)
        {
            image.gameObject.SetActive(false);
        }

        _ingredient1 = null;
        _ingredient2 = null;
        _selection1.gameObject.SetActive(false);
        _selection2.gameObject.SetActive(false);
        _numSelectionsMade = 0;
        _selectionText.text = "";
        _potionText.text = "";
        _finalText.text = "";
        _potionOutlineYes.gameObject.SetActive(false);
        _potionOutlineNo.gameObject.SetActive(false);
        _inventoryHighlights[0].gameObject.SetActive(true);
        _currentSelectedIngredient = 0;
        _doneSelecting = false;
        _doneCrafting = false;
        _madePotion = false;
    }

    private void SelectIngredients()
    {
        if (_inventory.Count == 1)
        {
            _doneCrafting = true;
            _potionOutlineYes.gameObject.SetActive(true);
            _leaveWindow = true;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _doneCrafting = true;
            _potionOutlineYes.gameObject.SetActive(true);
            _leaveWindow = true;
            _potionIndex = 3;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_currentSelectedIngredient != 0)
            {
                _currentSelectedIngredient--;
            }
            else
            {
                _currentSelectedIngredient = _inventory.Count - 1;
            }

            _updateSelection = true;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_currentSelectedIngredient != _inventory.Count - 1)
            {
                _currentSelectedIngredient++;
            }
            else
            {
                _currentSelectedIngredient = 0;
            }

            _updateSelection = true;
        }

        if (_updateSelection)
        {
            foreach (var image in _inventoryHighlights)
            {
                image.gameObject.SetActive(false);
            }
            _inventoryHighlights[_currentSelectedIngredient].gameObject.SetActive(true);
            _updateSelection = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_numSelectionsMade == 0)
            {
                Debug.Log("Here?");
                _selection1.sprite = _inventory[_currentSelectedIngredient]._ingredientImage;
                _selection1.gameObject.SetActive(true);
                _ingredient1 = _inventory[_currentSelectedIngredient];
                _numSelectionsMade++;
            }else if (_numSelectionsMade == 1)
            {
                _selection2.sprite = _inventory[_currentSelectedIngredient]._ingredientImage;
                _ingredient2 = _inventory[_currentSelectedIngredient];
                _selection2.gameObject.SetActive(true);
                foreach (var image in _inventoryHighlights)
                {
                    image.gameObject.SetActive(false);
                }
                foreach (var image in _craftingHighlighs)
                {
                    image.gameObject.SetActive(false);
                }
        
                _craftingHighlighs[0].gameObject.SetActive(true);
                _selectionText.text = "Put Ingredient Back?";
                _doneSelecting = true;
            }
        }
    }

    private void CraftPotion()
    {

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_craftingIndex != 0)
            {
                _craftingIndex--;
            }
            else
            {
                _craftingIndex = _craftingHighlighs.Count - 1;
            }

            _updatePotion = true;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_craftingIndex != _craftingHighlighs.Count - 1)
            {
                _craftingIndex++;
            }
            else
            {
                _craftingIndex = 0;
            }

            _updatePotion = true;
        }

        if (_updatePotion)
        {
            foreach (var image in _craftingHighlighs)
            {
                image.gameObject.SetActive(false);
            }
            _craftingHighlighs[_craftingIndex].gameObject.SetActive(true);
            if (_craftingIndex == 0)
            {
                _selectionText.text = "Put Ingredient Back?";
            }else if (_craftingIndex == 1)
            {
                _selectionText.text = "Put Ingredient Back?";
            }else if (_craftingIndex == 2)
            {
                _selectionText.text = "Confirm Potion";
            }

            _updatePotion = false;
        }

        UpdatePotion();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_craftingIndex == 0)
            {
                _selection1.sprite = _selection2.sprite;
                _selection2.gameObject.SetActive(false);
                _ingredient1 = _ingredient2;
                _ingredient2 = null;
                _numSelectionsMade = 1;
                _doneSelecting = false;
                _inventoryHighlights[0].gameObject.SetActive(true);
                _currentSelectedIngredient = 0;
                _craftingIndex = 0;
                _finalPotion.gameObject.SetActive(false);
            }
            else if (_craftingIndex == 1)
            {
                _selection2.gameObject.SetActive(false);
                _ingredient2 = null;
                _numSelectionsMade = 1;
                _doneSelecting = false;
                _inventoryHighlights[0].gameObject.SetActive(true);
                _currentSelectedIngredient = 0;
                _craftingIndex = 0;
                _finalPotion.gameObject.SetActive(false);
            }
            else if (_craftingIndex == 2)
            {
                _selection1.gameObject.SetActive(false);
                _selection2.gameObject.SetActive(false);
                _finalPotion.gameObject.SetActive(false);
                _potionOutlineYes.gameObject.SetActive(true);
                _leaveWindow = true;
                _doneCrafting = true;
                _madePotion = true;
            }

            foreach (var image in _craftingHighlighs)
            {
                image.gameObject.SetActive(false);
            }
            _craftingIndex = 0;
        }
    }

    private void UpdatePotion()
    {
        if (_ingredient1._ingredientIndex == _ingredient2._ingredientIndex)
        {
            _potionText.text = "Same Ingredient";
            _potionIndex = 3;
        }
        else if((_ingredient1._ingredientIndex == 1 && _ingredient2._ingredientIndex == 4) || (_ingredient1._ingredientIndex == 4 && _ingredient2._ingredientIndex == 1))
        {
            _potionText.text = "Extra Hit Potion";
            _potionIndex = 0;
        }else if ((_ingredient1._ingredientIndex == 2 && _ingredient2._ingredientIndex == 5) || (_ingredient1._ingredientIndex == 5 && _ingredient2._ingredientIndex == 2))
        {
            _potionText.text = "Score + Potion";
            _potionIndex = 1;
        }else if ((_ingredient1._ingredientIndex == 3 && _ingredient2._ingredientIndex == 5) || (_ingredient1._ingredientIndex == 5 && _ingredient2._ingredientIndex == 3))
        {
            _potionText.text = "Speed Potion";
            _potionIndex = 2;
        }
        else
        {
            _potionText.text = "Invalid Recipe";
            _potionIndex = 3;
        }

        _finalPotion.sprite = _potionImages[_potionIndex];
        _finalPotion.gameObject.SetActive(true);
    }

    private void ConfirmPotion()
    {
        if (_inventory.Count == 1)
        {
            _selectionText.text = "Not Enough Ingredients";
            _finalText.text = "Leave Crafting?";
            _inventoryHighlights[0].gameObject.SetActive(false);
            _potionIndex = 3;
        }
        else if (_potionIndex == 3 && _inventory.Count != 1)
        {
            _finalText.text = "No Potion, Still Wanna Leave?";
        }
        else
        {
            _finalText.text = "Use Potion & Leave?";
        }
        
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _potionOutlineYes.gameObject.SetActive(true);
            _potionOutlineNo.gameObject.SetActive(false);
            _leaveWindow = true;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _potionOutlineNo.gameObject.SetActive(true);
            _potionOutlineYes.gameObject.SetActive(false);
            _leaveWindow = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_leaveWindow)
            {
                //use Potion
                _playerMovementController.UsePotion(_potionIndex);

                if (_madePotion)
                {
                    _inventory.Remove(_ingredient1);
                    _inventory.Remove(_ingredient2);
                }

                _doneSelecting = false;
                _doneCrafting = false;
                _currentlyCrafting = false;
                _active = false;
                _PlayerReturnToGame?.Invoke();
                //_playerMaster.CallEventPlayerResets();

                _PotionCraftingUI.gameObject.SetActive(false);
                _potionOutlineYes.gameObject.SetActive(false);
                _potionOutlineNo.gameObject.SetActive(true);
                _outlineNo.gameObject.SetActive(true);
                _outlineYes.gameObject.SetActive(false);
                _crafPotions = false;
            }
            else
            {
                SetUpInventory();
            }
        }
    }
}
