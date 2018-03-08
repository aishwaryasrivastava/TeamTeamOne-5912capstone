﻿using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponEquip : MonoBehaviour
{
    public PlayerMovementController checker;
    public Text ammoCount;
    public GameObject[] weapons;
    public bool[] state;
    private int current, next;
    public const int Pipe = 0, AK = 1, Sniper = 2, M4 = 3, HandGun = 4;
    public static float timer;

	// Use this for initialization
	void Start ()
	{	    
        current = 1;
        next = 0;
        SaveState();
        RestoreFromState();
        SetActiveWeapon(0);
	}

    public void RestoreFromState()
    {
        for (var i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(state[i]);
            weapons[i].GetComponent<WeaponSwitchState>().Able = state[i];
        }
        SetActiveWeapon(0);
    }

    public void SaveState()
    {
        state = new[]
        {
            weapons[0].GetComponent<WeaponSwitchState>().Able,
            weapons[1].GetComponent<WeaponSwitchState>().Able, weapons[2].GetComponent<WeaponSwitchState>().Able,
            weapons[3].GetComponent<WeaponSwitchState>().Able, weapons[4].GetComponent<WeaponSwitchState>().Able,
            
        };
    }
	
	// Update is called once per frame
	void Update ()
	{
	    timer -= Time.deltaTime;
	    if (timer > 0) return;
	    if (Input.GetKeyDown(KeyCode.Alpha0)) SetActiveNone();
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetActiveWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetActiveWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetActiveWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetActiveWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetActiveWeapon(4);
    }

    public void SetAble(int id)
    {
        weapons[id].GetComponent<WeaponSwitchState>().Gain();
        SetActiveWeapon(id);
    }

    private void SetActiveNone()
    {
        if (current != -1) weapons[current].SetActive(false);
        current = -1;
        ammoCount.GetComponent<ToggleScript>().SwapToHere(current);
        checker.inTheRed = true;
    }

    private void SetActiveWeapon(int sel)
    {
        if (weapons[sel].GetComponent<WeaponSwitchState>().Able)
        {
            next = sel;
            ReadyToSwitch();
            checker.inTheRed = true;
        }
    }

    private void ReadyToSwitch()
    {
        if(current != -1) weapons[current].SetActive(false);
        current = next;
        weapons[current].SetActive(true);
        ammoCount.GetComponent<ToggleScript>().SwapToHere(current);
        timer = 0.6f;
    }
}
