using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaster : MonoBehaviour
{
    public delegate void GeneralEventHandler();
    public event GeneralEventHandler EventPlayerDies;
    public event GeneralEventHandler EventPlayerReset;
    public event GeneralEventHandler EventPlayerGetsIngredient;

    public delegate void HealthEventHandler();
    public event HealthEventHandler EventPlayerGetsHit;
    public event HealthEventHandler EventPlayerGainsALife;

    public void CallEventPlayerGetsHit()
    {
        if (EventPlayerGetsHit != null)
        {
            EventPlayerGetsHit();
        }
    }

    public void CallEventPlayerGainsALife()
    {
        if (EventPlayerGainsALife != null)
        {
            EventPlayerGainsALife();
        }
    }

    public void CallEventPlayerDies()
    {
        if (EventPlayerDies != null)
        {
            EventPlayerDies();
        }
    }

    public void CallEventPlayerResets() 
    {
        if (EventPlayerReset != null) 
        {
            EventPlayerReset();
        }
    }

    public void CallEventPlayerGetsIngredient()
    {
        if (EventPlayerGetsIngredient != null)
        {
            EventPlayerGetsIngredient();
        }
    }
}
