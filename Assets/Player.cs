﻿using Assets.defs;
using Assets.util;
using MonsterLove.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.action;

public enum PlayerStates
{
    Idle,
    PlacingSettlement,
    PlacingCity,
    PlacingRoad,
}

public class Player : MonoBehaviour
{
    public BoardView Board;
    public PlayerColor Color;
    public GameCoordinator Coordinator;

    private bool isMyTurn = false;

    private PrefabContainer prefabContainer;

    private StateMachine<PlayerStates> stateMachine;

    private UnitTypes placingUnitType;

	void Start ()
	{
        prefabContainer = GameObject.Find("PrefabContainer").GetComponent<PrefabContainer>();
        stateMachine = StateMachine<PlayerStates>.Initialize(this, PlayerStates.Idle);
	}

    public void StartTurn()
    {
        Debug.Log(string.Format("Starting turn for {0}", Color.ToString()));
        isMyTurn = true;
    }

    public void EndTurn()
    {
        isMyTurn = false;
    }
	
	public void Idle_Update()
    {
        if (isMyTurn)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                stateMachine.ChangeState(PlayerStates.PlacingSettlement);
                placingUnitType = UnitTypes.Settlement;
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                stateMachine.ChangeState(PlayerStates.PlacingCity);
                placingUnitType = UnitTypes.City;
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                stateMachine.ChangeState(PlayerStates.PlacingRoad);
            }
        }
    }

    public void PlacingSettlement_Update()
    {
        Vector3? mousePos = RaycastMouse();
        if (mousePos.HasValue)
        {
            PlaceSettlementAction action = new PlaceSettlementAction(HexCorner.GetNearestCorner(mousePos.Value), Color);
            Board.SetCurrentAction(action);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Board.ApplyCurrentAction();
            stateMachine.ChangeState(PlayerStates.Idle);
        }
    }

    private Vector3? RaycastMouse()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit))
        {
            return hit.point;
        }
        else
        {
            return null;
        }
    }
}
