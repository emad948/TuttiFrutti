using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Mirror;

public class GameManager : NetworkBehaviour
{

    private Dictionary<int,Sprite> fruitDictionary = new Dictionary<int, Sprite>();
    [HideInInspector]private readonly SyncList<enumFruits> chosenFruitsList = new SyncList<enumFruits>();
    private readonly SyncHashSet<enumFruits> chosenFruitHashSet = new SyncHashSet<enumFruits>();
    [HideInInspector] readonly public SyncHashSet<enumFruits> alreadyChosenFruitHashSet = new SyncHashSet<enumFruits>();
    private const int TOTAL_NUMBER_OF_FRUIT = 16;
    private const int MAX_ROUND_NUMBER = 3;

    private int roundNumber = 1;
    public Sprite chosenFruit;
    //private bool hasChosenFruit = false; // so far unused
    public bool gameCanStart = false;
    public bool canNowUpdateImages = false;
    public bool checkRoundFinished = false;
    public bool isGameOver = false;
    public bool hasFailed = false;
    private void Awake()
    {
        PopulateFruitDictionary();
        if (!isServer) return;
        StartCoroutine("CheckRound");
    }
    
    public enum enumFruits : int{
        apple = 1,
        banana = 2,
        cherry = 3,
        orange = 4,
        watermelon = 5,
        grape = 6
    } 
    public readonly SyncList<enumFruits> intFruits = new SyncList<enumFruits>(); 

    void PopulateFruitDictionary()
    {
        fruitDictionary.Add(1,Resources.Load<Sprite>("Images/apple"));
        fruitDictionary.Add(2,Resources.Load<Sprite>("Images/banana"));
        fruitDictionary.Add(3,Resources.Load<Sprite>("Images/cherry"));
        fruitDictionary.Add(4,Resources.Load<Sprite>("Images/orange"));
        fruitDictionary.Add(5,Resources.Load<Sprite>("Images/watermelon"));
        fruitDictionary.Add(6,Resources.Load<Sprite>("Images/grape"));
    }

    public Sprite decodeSprite(int n){
        return fruitDictionary[n];
    }



    public IEnumerator CheckRound()
    {
        switch (roundNumber)
        {
            case 1:
                StartCoroutine(ChooseBasedOnRound(2,8));
                break;
            case 2:
                StartCoroutine(ChooseBasedOnRound(4,4));
                break;
            case 3:
                StartCoroutine(ChooseBasedOnRound(6,3));
                break;
        }
        checkRoundFinished = true;
        yield return new WaitUntil(()=> canNowUpdateImages);
       
    }
    
    IEnumerator ChooseBasedOnRound(int amountOfFruit, int amountOfEach)
    {
        chosenFruitsList.Clear();
        
        // Populate list with all hashset values first.
        chosenFruitsList.AddRange(chosenFruitHashSet);
        
        // Get the amount of individual fruit needed.
        while (chosenFruitHashSet.Count < amountOfFruit)
        {
            int randomFruitPosition = Random.Range(1, fruitDictionary.Count + 1);
            Sprite fruitChosen = fruitDictionary[randomFruitPosition];
            chosenFruitHashSet.Add(fruitChosen);
        }

        // Choose fruit for the round.
        ChooseFruit();

        // Loop through hashset and populate list with correct amount of each item in there.
        foreach (var fruit in chosenFruitHashSet)
        {
            int howManyInListAlready = 0;

            while (howManyInListAlready < amountOfEach && chosenFruitsList.Count <= TOTAL_NUMBER_OF_FRUIT)
            {
                howManyInListAlready = chosenFruitsList.Count(fruitName => fruitName.name.Contains(fruit.name));
                if (howManyInListAlready < amountOfEach)
                {
                    chosenFruitsList.Add(fruit);
                }
            }
        }
            
        // Shuffle list
        chosenFruitsList = chosenFruitsList.OrderBy(x => Guid.NewGuid()).ToList();
        gameCanStart = true;
        canNowUpdateImages = true;

        yield return null;
    }

    public IEnumerator UpdateImages(List<GameObject> tilesTransforms)
    {
        yield return new WaitUntil(()=> checkRoundFinished);
        int index = 0;
      
        foreach (var tile in tilesTransforms)
        {
            if (tile.GetComponentInChildren<PlatformTile>())
                {
                    if (index < chosenFruitsList.Count)
                    {
                        tile.GetComponentInChildren<PlatformTile>().SetImage(chosenFruitsList[index]);
                        
                        string prefix = Random.Range(0, 2) % 2 == 0 ? "X" : "Y";
                        tile.name = prefix + ("-") + chosenFruitsList[index].name;
                        if (prefix == "Y")
                        {
                            tile.GetComponentInChildren<PlatformTile>().SetCanvasActive(false);
                        }
                        index++;
                    }
                }
        }
    }

    public void IncreaseRound()
    {
        if (roundNumber + 1 <= MAX_ROUND_NUMBER)
        {
            roundNumber += 1;
            //hasChosenFruit = false; // so far unused
            StartCoroutine("CheckRound");
        }
        else
        {
            isGameOver = true;
            FindObjectOfType<UI>().SetGameState(true);
        }
    }

    public void ChooseFruit()
    {
        foreach (var fruitInHashset in chosenFruitHashSet)
        {
            if (!alreadyChosenFruitHashSet.Contains(fruitInHashset))
            {
                chosenFruit = fruitInHashset;
                alreadyChosenFruitHashSet.Add(chosenFruit);
                break;
            }
        }
    }
}
