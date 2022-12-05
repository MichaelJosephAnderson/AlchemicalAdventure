using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BasicMovement : MonoBehaviour
{
    private PlayerMovementController playerMovementController;
    private LevelMaster levelMaster;

    public Vector3 destination, nextPos, previousPos;

    private Vector3 up = Vector3.zero,
        right = new Vector3(0, 90, 0),
        down = new Vector3(0, 180, 0),
        left = new Vector3(0, 270, 0),
        currentDirection = Vector3.zero;

    [SerializeField] private Transform playerGeo = null;
    [SerializeField] private Transform cameraObject = null;
    [SerializeField] private GameObject spectatorView = null;
    [SerializeField] private List<Sprite> _walkAnim;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private LayerMask _ignoreMe;

    public bool isMoving;

    public int _speedBuffMultiplyer = 1;
    
    public float speed;
    public bool canMove;
    public bool shouldFall;
    
    public bool isFalling;

    private bool nextIsSuperJump = false;
    private int _spriteNum;

    private PhotonView _view;
    private void OnEnable()
    {
        SetInitialReferences();
    }


    void SetInitialReferences()
    {
        _view = GetComponent<PhotonView>();
        playerMovementController = GetComponent<PlayerMovementController>();
        levelMaster = FindObjectOfType<LevelMaster>();
        
        currentDirection = up;
        nextPos = Vector3.forward + new Vector3(0, 0, 0.5f);
        destination = transform.position;
        speed = baseSpeed;

        if (!_view.IsMine)
        {
            spectatorView.SetActive(false);
        }
    }

    public void Move() 
    {
        if (_view.IsMine)
        {
             GetObjectSpeed();

             if (Input.GetKeyDown(KeyCode.Tab))
             {
                 spectatorView.SetActive(!spectatorView.activeSelf);
             }

             if (Vector3.Distance(transform.position, destination) >= 0.01f)
            {
                isMoving = true;
                transform.position = Vector3.MoveTowards(transform.position, destination, (speed * _speedBuffMultiplyer) * Time.deltaTime);
            }
            else
            {
                isMoving = false;
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity))
                {
                    if (Vector3.Distance(destination, hitInfo.point) >= 2)
                    {
                        shouldFall = true;
                    }
                }
                else 
                {
                    shouldFall = true;
                }

                if (shouldFall)
                {
                    Fall();
                }

            }

            if (!isFalling)
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    //MoveUp
                    nextPos = cameraObject.rotation * Vector3.forward;
                    previousPos = nextPos;
                    currentDirection = cameraObject.localEulerAngles + up;
                    canMove = true;
                }
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    //move down
                    nextPos = cameraObject.rotation * Vector3.back;
                    previousPos = nextPos;
                    currentDirection = cameraObject.localEulerAngles + down;
                    canMove = true;
                }
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    //move Left
                    nextPos = cameraObject.rotation * Vector3.left;
                    previousPos = nextPos;
                    currentDirection = cameraObject.localEulerAngles + left;
                    canMove = true;
                }
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    //Move Right
                    nextPos = cameraObject.rotation * Vector3.right;
                    previousPos = nextPos;
                    currentDirection = cameraObject.localEulerAngles + right;
                    canMove = true;
                }

                /*
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    nextIsSuperJump = true;
                    nextPos = previousPos;
                    canMove = true;
                }
                */

                if (Vector3.Distance(destination, transform.position) <= 0.01f)
                {
                    playerGeo.localEulerAngles = currentDirection;

                    if (canMove)
                    {
                        if (IsValid() || nextIsSuperJump)
                        {
                            if (nextIsSuperJump)
                            {
                                if (Physics.Raycast(transform.position, playerGeo.forward, 1.5f))
                                {
                                    if (SuperIsValid())
                                    {
                                        //does the super jump
                                        SetMove(100, 1.5f);
                                    }
                                    else
                                    {
                                        nextPos = Vector3.zero;
                                        //super bonk
                                    }
                                }
                                else
                                {
                                    SetMove(100, 0);
                                }
                            }
                            else
                            {
                                SetMove(10, 0);
                            }
                        }
                        else
                        {
                            nextPos = Vector3.zero;
                            //bonked
                        }
                    }
                }
            }
        }
    }

    private void SetMove(int moveScore, float yCheckHight)
    {
        destination = transform.position + (nextPos * moveDistance) + new Vector3(0, yCheckHight, 0);
        if (Physics.Raycast(destination, Vector3.down, out RaycastHit hitInfo, 2f, ~_ignoreMe))
        {
            destination = hitInfo.point + new Vector3(0,0.5f,0);
        }
        canMove = false;
        nextIsSuperJump = false;
        levelMaster.CallEventScoreIncrease(moveScore);
        if (_spriteNum != 3)
        {
            _spriteNum++;
            _renderer.sprite = _walkAnim[_spriteNum];
        }
        else
        {
            _spriteNum = 0;
            _renderer.sprite = _walkAnim[_spriteNum];
        }
    }

    public void Fall()
    {
        isFalling = true;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down, fallSpeed * Time.deltaTime);
        destination = transform.position + Vector3.down;
        transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);

        if (Physics.Raycast(destination, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, ~_ignoreMe))
        {
            if (Vector3.Distance(destination, hitInfo.point) <= 2)
            {
                shouldFall = false;
                destination = hitInfo.point + new Vector3(0,0.5f,0);
                isFalling = false;
            }
        }
    }

    public void ResetScale()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    private bool IsValid()
    {
        if (Physics.Raycast(transform.position, playerGeo.forward, out RaycastHit hitInfo, 1f))
        {
            if (hitInfo.transform.gameObject.layer == 11)  //Layer 11 = Enemy Layer
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    private bool SuperIsValid()
    {
        if (Physics.Raycast(transform.position + new Vector3(0, 1.5f, 0), playerGeo.forward, out RaycastHit hitInfo, 1.5f))
        {
            if (hitInfo.transform.gameObject.layer == 11)  //Layer 11 = Enemy Layer
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    private void GetObjectSpeed()
    {
        if (Physics.Raycast(destination, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity))
        {
            if (hitInfo.transform.CompareTag("Slow"))
            {
                speed = 10;
            }
            else if (hitInfo.transform.CompareTag("Medium"))
            {
                speed = 20;
            }
            else if (hitInfo.transform.CompareTag("Fast"))
            {
                speed = 30;
            }
            else
            {
                speed = baseSpeed;
            }
        }
        else 
        {
            speed = baseSpeed;
        }
    }

    private void OnDrawGizmos()
    {
        Physics.Raycast(destination, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity);
        Gizmos.DrawSphere(hitInfo.point, .1f);
        Gizmos.DrawSphere(destination, 0.2f);
    }
}
