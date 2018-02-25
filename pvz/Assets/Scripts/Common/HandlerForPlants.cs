﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerForPlants : MonoBehaviour
{
    public AudioClip plantGrow;
    public AudioClip seedLift;
    public AudioClip seedCancel;
    public GameObject tempPlant;
    public GameObject selectedPlant;
    private Card selectedCard;
    private int row = -1;
    private int col = -1;

    void Update()
    {
        HandleMouseForPlant();
        HandleMouseDownForPlant();
    }
    void HandleMouseForPlant()
    {
        if (selectedPlant)
        {
            Vector3 pos = Utility.GetMouseWorldPos();
            Vector3 plantPos = pos;
            plantPos.y -= 0.3f;
            selectedPlant.transform.position = plantPos;
            if (StageMap.IsPointInMap(pos))
            {
                StageMap.GetRowAndCol(pos,out row,out col);
                if (tempPlant.GetComponent<PlantGrow>().CanGrowInMap(row,col))
                {
                    tempPlant.transform.position = StageMap.SetPlantPos(row, col);
                    tempPlant.GetComponent<SpriteDisplay>().SetOrderByRow(row);
                }
                else
                {
                    col = row = -1;
                    tempPlant.transform.position=new Vector3(1000,1000,0);
                }
            }
            else
            {
                col = row = -1;
                tempPlant.transform.position = new Vector3(1000, 1000, 0);
            }
        }
    }

    void HandleMouseDownForPlant()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D collider = Physics2D.OverlapPoint(Utility.GetMouseWorldPos());
            if (collider != null)
            {
                CancelSelectCard();
                if (collider.gameObject.tag == "Card")
                {
                    collider.gameObject.SendMessage("OnSelected");
                    AudioManager.GetInstance().PlaySound(seedLift);
                }
            }
            else if (selectedPlant)
            {
                if (row != -1)
                {
                    selectedPlant.transform.position = StageMap.SetPlantPos(row, col);
                    selectedPlant.GetComponent<PlantGrow>().Grow(row,col);
                    AudioManager.GetInstance().PlaySound(plantGrow);
                    selectedPlant = null;
                    Destroy(tempPlant);
                    tempPlant = null;
                    selectedCard.Growed();
                    selectedPlant = null;
                }
                else
                {
                    CancelSelectCard();
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            CancelSelectCard();
        }
    }

    void CancelSelectCard()
    {
        if (selectedCard)
        {
            Destroy(tempPlant);
            Destroy(selectedPlant);
            selectedPlant = tempPlant = null;
            if(selectedCard.isGrowed==false)
                selectedCard.state = Card.State.Normal;
            selectedCard = null;
        }
        AudioManager.GetInstance().PlaySound(seedCancel);
    }
    public void SetSelectedCard(Card card)
    {
        card.state = Card.State.CD;
        selectedCard = card;
        tempPlant = Instantiate(card.plant);
        tempPlant.transform.position=new Vector3(1000,1000,0);
        tempPlant.GetComponent<SpriteDisplay>().SetAlpha(0.6f);
        selectedPlant = Instantiate(card.plant);
        selectedPlant.transform.position = new Vector3(1000, 1000, 0);
        selectedPlant.GetComponent<SpriteDisplay>().SetOrder(15000);
    }
}
