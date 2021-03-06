using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Grid_ : MonoBehaviour
{
    private const int xCount = 4;
    private int yCount = 4;
    private GameObject platform;
    [SerializeField] private GameManager perfectMatchManager;

    private GameObject platformParent;
    private List<GameObject> tilesTransforms = new List<GameObject>();

    private float timeUntilReveal = 14.0f;
    private float twoSecondTimer = 2.0f;

    // Booleans
    //private bool tilesOff = false;        // was unused so far
    private bool hasFinishedInitial = false;
    public bool revealingFruit = false;
    private bool hasRemovedIncorrectTiles = false;
    public GameObject ui;
    public bool canStart = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Load platform prefab from resources.
        platform = Resources.Load<GameObject>("Prefabs/platformPurple");
        StartCoroutine("GeneratePlatform");   
    }

    private void Update()
    {
        /*
         - 14 seconds of alternating
         - When at 0 seconds all off
         - Then 4 seconds to find correct tile
         - Dissappear for four seconds
          RESTART
         */
        if (canStart)
        {
            if (!hasFinishedInitial)
            {
                StartCoroutine("ToggleInitialImagesTiles");
            }
            
            if (perfectMatchManager.hasFailed) // game over
            {
                ui.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                ui.GetComponent<UI>().SetGameState(false);
            }

            if (!perfectMatchManager.isGameOver)
            {
                // Update UI with timer.
                ui.gameObject.transform.GetChild(0).gameObject.SetActive(!hasRemovedIncorrectTiles);
                ui.GetComponent<UI>().SetText(Mathf.Round(timeUntilReveal).ToString());
                // Countdown.
                timeUntilReveal -= Time.deltaTime;
                twoSecondTimer -= Time.deltaTime;

                if (timeUntilReveal > 0 && !revealingFruit) // Alternating:
                {
                    if (twoSecondTimer < 0)
                    {
                        StartCoroutine("ToggleImages");
                        twoSecondTimer = 2.0f;
                    }
                }
                else
                {
                    if (!revealingFruit)
                    {
                        // Turn all images off
                        ToggleAllImagesTiles();

                        // Reset timer for reveal
                        timeUntilReveal = 4.0f;

                        // Fruit is being shown
                        revealingFruit = true;
                    }

                    // Time has run out to get to correct tile.
                    if (timeUntilReveal < 0 && !hasRemovedIncorrectTiles)
                    {
                        if (!hasRemovedIncorrectTiles)
                        {
                            // Bring back images
                            ToggleAllImagesTiles(true);

                            // Remove all incorrect tiles.
                            RemoveIncorrectTiles(true);

                            hasRemovedIncorrectTiles = true;

                            // Reset timer.
                            timeUntilReveal = 4.0f;
                        }
                    }
                    else
                    {
                        // Reach zero again, bring back all tiles
                        if (timeUntilReveal < 0)
                        {
                            // Reset timer
                            timeUntilReveal = 14.0f; // Needs to be 14.0f

                            // No longer reveal fruit.
                            revealingFruit = false;

                            // Bring back tiles objects
                            ToggleAllTiles(true);

                            ToggleAllImagesTiles(true);

                            // Reset has removed
                            hasRemovedIncorrectTiles = false;

                            // Choose next fruit
                            perfectMatchManager.IncreaseRound();
                            perfectMatchManager.updatedRounds++;
                            
                            StartCoroutine(perfectMatchManager.UpdateImages(tilesTransforms));
                            
                        }
                    }
                }
            }
            else
            {
                // GameOver
                ToggleAllImagesTiles(false);
                ui.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    IEnumerator GeneratePlatform()
    {
        yield return new WaitUntil(() => perfectMatchManager.gameCanStart);
            platformParent = new GameObject();
            platformParent.name = "Platform";
            platformParent.transform.parent = gameObject.transform;

            float xAmount = -1.5f;
            float zAmount = -1.5f;
            int index = 0;
            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    Vector3 position = new Vector3(transform.position.x + (xAmount + 0.5f), transform.position.y,
                        transform.position.z + (zAmount + 0.5f));
                    GameObject tile = Instantiate(platform, position, transform.rotation);
                    tile.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);

                    string prefix = Random.Range(0, 2) % 2 == 0 ? "X" : "Y";

                    Sprite sprite = perfectMatchManager.decodeSprite(perfectMatchManager.chosenFruitsList[index]);
                    tile.GetComponent<PlatformTile>().SetImage(sprite);
                    tile.GetComponent<PlatformTile>().SetCanvasActive(false);
                    tile.name = prefix + ("-") + sprite.name;
                    tile.transform.parent = platformParent.transform;
                    
                    zAmount += 1.8f;
                    index++;
                }

                xAmount += 1.8f;
                zAmount = -1.5f;
            }
        StoreAllTileTransforms();
    }



    void StoreAllTileTransforms()
    {
        foreach (var tile in platformParent.transform.GetComponentsInChildren<Transform>())
        {
            if (tile.GetComponentInChildren<PlatformTile>())
            {
                tilesTransforms.Add(tile.gameObject);
            }
        }
    }

    void RemoveIncorrectTiles(bool remove)
    {
        foreach (var tile in tilesTransforms)
        {
            if (tile.GetComponent<PlatformTile>())
            {
                if (remove)
                {
                    if (tile.transform.name.Contains(perfectMatchManager.decodeSprite(perfectMatchManager.chosenFruit).name))
                    {
                        tile.transform.gameObject.SetActive(true);
                    }
                    else
                    {
                        tile.transform.gameObject.SetActive(false);
                    }
                }
                else
                {
                    tile.transform.gameObject.SetActive(true);
                }
            }
        }
    }

    IEnumerator ToggleImages()
    {
        yield return new WaitForSeconds(1);

        foreach (var tile in tilesTransforms)
        {
            if (tile.GetComponentInChildren<PlatformTile>())
            {
                tile.gameObject.GetComponentInChildren<PlatformTile>().ToggleImage();
            }
        }
    }

    void ToggleAllTiles(bool active = false)
    {
        foreach (var tile in tilesTransforms)
        {
            tile.transform.gameObject.SetActive(active);
        }
    }

    void ToggleAllImagesTiles(bool active = false, bool onlyCertain = false)
    {
        foreach (var tile in tilesTransforms)
        {
            if (tile.GetComponentInChildren<PlatformTile>())
            {
                PlatformTile platformTile = tile.GetComponentInChildren<PlatformTile>();
                if (onlyCertain)
                {
                    if (platformTile.name.Contains("X-"))
                    {
                        platformTile.SetCanvasActive(true);
                    }
                }
                else
                {
                    tile.gameObject.GetComponentInChildren<PlatformTile>().SetCanvasActive(active); 
                }
            }
        }
    }
    
    IEnumerator ToggleInitialImagesTiles()
    {
        foreach (var tile in tilesTransforms)
        {
            if (tile.GetComponentInChildren<PlatformTile>())
            {
                PlatformTile platformTile = tile.GetComponentInChildren<PlatformTile>();
             
                    if (platformTile.name.Contains("X-"))
                    {
                        platformTile.SetCanvasActive(true);
                    }
            }
        }

        hasFinishedInitial = true;
        yield return new WaitUntil(() => hasFinishedInitial);
    }
}