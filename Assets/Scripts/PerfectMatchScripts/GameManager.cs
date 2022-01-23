using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Mirror;

public class GameManager : NetworkBehaviour
{

    [HideInInspector]public readonly SyncList<int> chosenFruitsList = new SyncList<int>();
    [SyncVar] public int chosenFruit;
    [SyncVar] public bool gameCanStart = false;
    [SyncVar] public bool canNowUpdateImages = false;
    [SyncVar] public int checkRoundFinished = 0;
    private int updatedRounds = 0;
    [SyncVar] public bool isGameOver = false;
    [SyncVar] public bool hasFailed = false;

    // PRIVATES
    private bool hasUpdated = false;
    private Dictionary<int,Sprite> fruitDictionary = new Dictionary<int, Sprite>();
    private readonly SyncHashSet<int> chosenFruitHashSet = new SyncHashSet<int>();
    [HideInInspector] readonly private SyncHashSet<int> alreadyChosenFruitHashSet = new SyncHashSet<int>();
    private const int TOTAL_NUMBER_OF_FRUIT = 16;
    private const int MAX_ROUND_NUMBER = 3;
    [SyncVar] private int roundNumber = 1;

    private void Awake()
    {
        PopulateFruitDictionary();
        if (isClientOnly) return;
        StartCoroutine("CheckRound");
    }
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
        checkRoundFinished = roundNumber;
        yield return new WaitUntil(()=> canNowUpdateImages);
       
    }
    
    [Server] IEnumerator ChooseBasedOnRound(int amountOfFruit, int amountOfEach)
    {
        print("running chooseBasedOnRound");
        chosenFruitsList.Clear();
        
        // Populate list with all hashset values first.
        chosenFruitsList.AddRange(chosenFruitHashSet);
        
        // Get the amount of individual fruit needed.
        while (chosenFruitHashSet.Count < amountOfFruit)
        {
            int choosenFruit = Random.Range(1, fruitDictionary.Count + 1);
            chosenFruitHashSet.Add(choosenFruit);
        }

        // Choose fruit for the round.
        ChooseFruit();

        // Loop through hashset and populate list with correct amount of each item in there.
        foreach (var fruit in chosenFruitHashSet)
        {
            int howManyInListAlready = 0;

            while (howManyInListAlready < amountOfEach && chosenFruitsList.Count <= TOTAL_NUMBER_OF_FRUIT)
            {
                howManyInListAlready = chosenFruitsList.Count(fruitName => fruitName == fruit);
                if (howManyInListAlready < amountOfEach)
                {
                    chosenFruitsList.Add(fruit);
                }
            }
        }
            
        // Shuffle list
        List<int> result = chosenFruitsList.OrderBy(x => Guid.NewGuid()).ToList();
        chosenFruitsList.Clear();
        foreach (var item in result) chosenFruitsList.Add(item);

        gameCanStart = true;
        canNowUpdateImages = true;
        hasUpdated = true;

        yield return null;
    }

    public IEnumerator UpdateImages(List<GameObject> tilesTransforms)
    {
        yield return new WaitUntil(()=> checkRoundFinished > ++roundNumber);
        int index = 0;
      
        foreach (var tile in tilesTransforms)
        {
            if (tile.GetComponentInChildren<PlatformTile>())
                {
                    if (index < chosenFruitsList.Count)
                    {
                        tile.GetComponentInChildren<PlatformTile>().SetImage(decodeSprite((int)chosenFruitsList[index]));
                        
                        string prefix = Random.Range(0, 2) % 2 == 0 ? "X" : "Y";
                        tile.name = prefix + ("-") + decodeSprite((int)chosenFruitsList[index]).name;
                        if (prefix == "Y")
                        {
                            tile.GetComponentInChildren<PlatformTile>().SetCanvasActive(false);
                        }
                        index++;
                    }
                }
        }
    }

    [Server] public void IncreaseRound() // only call on server in grid
    {
        if (isClientOnly) return;
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

    [Server] public void ChooseFruit()
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
