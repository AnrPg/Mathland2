using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Minigame1 : MonoBehaviour
{

    public SceneController.MinigameName thisMinigameName;
    //public bool justCalled;
    public readonly int[] powersOf2 = {0, 1, 2, 3, 4};
    //public const string orthoCameraName = "MainCamera Orthogonal";
    public const string perspCameraName = "MainCamera Perspective";

    //public const int MAX_LARVAE_NUM = 15;
    [SerializeField] private GameObject allLarvae;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject questionText;
    [SerializeField] private GameObject answerText;
    [SerializeField] private GameObject woodenPanel;

    //private Camera orthoCamera;
    private Camera perspCamera;
    private GameObject player;
    private SceneController sceneController;

    private int numOfLarvae;
    private List<GameObject> larvaeInNest;
    private List<GameObject> movingLarvae;
    private int numOfKilledLarvae;
    private int correctAnswer;
    private bool showNewQuestion;
    private TEXDraw questionTextComponent;
    private TEXDraw answerTextComponent;

    //public int NumOfLarvae => numOfLarvae;

    void OnEnable()
    {
        thisMinigameName = SceneController.MinigameName.Minigame1;        
        showNewQuestion = true;        
        movingLarvae = new List<GameObject>();

        larvaeInNest = new List<GameObject>();
        Transform[] ts = allLarvae.GetComponentsInChildren<Transform>();        
        if (ts != null)
        {
            foreach (Transform t in ts)
            {
                if (t != null && t.parent != null)
                {
                    larvaeInNest.Add(t.gameObject);
                    t.gameObject.SetActive(false);
                }    
            }
            numOfLarvae = larvaeInNest.Count;
            allLarvae.SetActive(true);
        }
        else
        {
            // TODO: exit minigame with failed flag
            Debug.Log("Couldn't initialize list of larvae (GameObject: larvae, Variable: larvaeInNest)...");
        }    

        questionTextComponent = questionText.GetComponent<TEXDraw>();
        answerTextComponent = answerText.GetComponent<TEXDraw>();
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        sceneController = GameObject.FindGameObjectsWithTag("SceneController")[0].GetComponent<SceneController>();
        
        //orthoCamera = player.transform.Find(orthoCameraName).GetComponent<Camera>();
        perspCamera = player.transform.Find(perspCameraName).GetComponent<Camera>(); // It should always have two members even if they are null
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneController.ActiveMinigame == thisMinigameName)
        {
            if (numOfKilledLarvae >= numOfLarvae)
            {
                Debug.Log("You won!");

                // Precondition: all larvae are put again back in their nests at the end of the game
                /*
                foreach (GameObject larva in larvaeInNest)
                {
                    larva.SetActive(true);
                }
                */
                movingLarvae.Clear();
                Debug.Log("Larvae back in nest: " + larvaeInNest.Count + "/nInitial num of larvae in nests: " + numOfLarvae);

                sceneController.ActiveMinigame = SceneController.MinigameName.None;
                //orthoCamera.enabled = false;
                sceneController.Minigame1Object.SetActive(false);
                player.GetComponent<FPSInput>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                //perspCamera.enabled = true;
                numOfKilledLarvae = 0; // Should be last (or the whole "if block" be atomic).
            }

            if (showNewQuestion)
            {                
                showQuestion();
            }

            if ((movingLarvae != null) && (movingLarvae.Count > 0))
            {
                foreach (GameObject randomLarva in movingLarvae)
                {
                    float step = randomLarva.GetComponent<Larva>().LarvaSpeed * Time.deltaTime;
                    randomLarva.GetComponent<RectTransform>().localPosition = Vector2.MoveTowards(randomLarva.GetComponent<RectTransform>().localPosition, target.GetComponent<RectTransform>().localPosition, step);
                }
            }            
        }
    }

    public void showQuestion()
    {
        showNewQuestion = false;
        //justCalled = false;

        answerTextComponent.text = "";

        int randPower = UnityEngine.Random.Range(0, powersOf2.Length);
        questionTextComponent.text = string.Format(  @"2^{{{0}}}",  powersOf2[randPower]);
        correctAnswer = (int) Math.Pow(2, randPower);    
    }
    public void showTypedAnswer(string answer)
    {
        answerTextComponent.text += answer;
    }
    
    public void enterAnswer()
    {
        int answerInt;
        if (int.TryParse(answerTextComponent.text, out answerInt))
        {
            StartCoroutine(changePanelColor(answerInt == correctAnswer));
        }
        else
        {
            Debug.Log("Couldn't parse answer text as integer (to check answer for correctness)...!");
            showNewQuestion = true;
            showQuestion();
        }    
    }

    private IEnumerator changePanelColor(bool correct)
    {
        if (correct)
        {
            killLarva();
            woodenPanel.GetComponent<Image>().color = new Color(155.0f/255, 255.0f/255, 19.0f/255, 1);
        }
        else
        {
            larvaLeaveNest();
            woodenPanel.GetComponent<Image>().color = new Color(255.0f/255, 88.0f/255, 88.0f/255, 1);
        }

        yield return new WaitForSeconds(1);

        woodenPanel.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        showNewQuestion = true;
    }

    private void larvaLeaveNest()
    {
        int randomIndex = UnityEngine.Random.Range(0, larvaeInNest.Count);
        GameObject randomLarva = larvaeInNest[randomIndex];
        randomLarva.SetActive(true);
        larvaeInNest.RemoveAt(randomIndex);
        movingLarvae.Add(randomLarva);
        Debug.Log("randomLarva: " + randomLarva.gameObject.name);
    }
    
    private void killLarva()
    {
        if ((movingLarvae != null) && (movingLarvae.Count > 0))
        {
            int indexOfNearestLarva = 0;
            GameObject nearestLarva = movingLarvae[0];            
            float minDistance = Vector2.Distance(movingLarvae[0].GetComponent<RectTransform>().localPosition, target.GetComponent<RectTransform>().localPosition);
            for (int i = 1; i < movingLarvae.Count; i++)
            {
                float distanceFromTarget = Vector2.Distance(movingLarvae[i].GetComponent<RectTransform>().localPosition, target.GetComponent<RectTransform>().localPosition);
                
                if (distanceFromTarget < minDistance)
                {
                    minDistance = distanceFromTarget;
                    nearestLarva = movingLarvae[i];
                    indexOfNearestLarva = i;
                }
            }
            
            nearestLarva.GetComponent<RectTransform>().localPosition = nearestLarva.GetComponent<Larva>().NestAnchor.GetComponent<RectTransform>().localPosition;
            larvaeInNest.Add(nearestLarva);
            movingLarvae.RemoveAt(indexOfNearestLarva);
            nearestLarva.SetActive(false);
            numOfKilledLarvae++;            
        }
        else
        {
            int randomIndex = UnityEngine.Random.Range(0, larvaeInNest.Count);
            GameObject randomLarva = larvaeInNest[randomIndex];
            randomLarva.SetActive(false);
            numOfKilledLarvae++;
        }
        Debug.Log("Killed larvae: " + numOfKilledLarvae + " of total " + numOfLarvae + " larvae");
    }
}


/*
if ((larvaePrefab != null || larvaePrefab.Length > 0) && Input.GetKey(KeyCode.X) && spawnedLarvae < MAX_LARVAE_NUM)
        {
            Debug.Log("Try to spawn larva...");
            System.Random rnd = new System.Random();
            if (rnd.Next(0, 1) == 0) // spawn larva
            {
                GameObject newLarva = Instantiate(larvaePrefab[UnityEngine.Random.Range(0, larvaePrefab.Length)], new Vector2(rnd.Next(0, 1), rnd.Next(0, 1)), Quaternion.identity, larvaePlaceholder.transform) as GameObject;
                if (newLarva.GetComponent<Larva>() != null)
                {
                    larvae.Add(newLarva.GetComponent<Larva>());  
                    spawnedLarvae++;                  
                }
                else
                {
                    Debug.Log("Spawning larva failed...");
                    GameObject.Destroy(newLarva);
                }
            }
        }
        */


/*
        System.Random rnd = new System.Random();
        larvaeNest nestCorner = (larvaeNest) rnd.Next(0, Enum.GetNames(typeof(larvaeNest)).Length); // Length assessing not so performant...
        
        switch (nestCorner)
        {
            case larvaeNest.topLeft:
            {
                if (larvaeTopLeft != null && larvaeTopLeft.Count > 0)
                {
                    int randIndex = UnityEngine.Random.Range(0, larvaeTopLeft.Count);
                    GameObject randomLarva = larvaeTopLeft[randIndex];
                    movingLarvae.Add(randomLarva);
                    larvaeTopLeftAwayFromNest.Add(randomLarva);
                    larvaeTopLeft.RemoveAt(randIndex);
                    if (larvaeTopLeft.Count <= 0)
                    {
                        emptyNestsNum++;
                    }
                    Debug.Log("Larva from the top left nest is heading for the apple...!");
                    break;
                }
                // else do not break, but try the other corners instead                    
                goto case larvaeNest.topRight;
            }
            case larvaeNest.topRight:
            {
                if (larvaeTopRight != null && larvaeTopRight.Count > 0)
                {
                    int randIndex = UnityEngine.Random.Range(0, larvaeTopRight.Count);
                    GameObject randomLarva = larvaeTopRight[randIndex];
                    movingLarvae.Add(randomLarva);
                    larvaeTopRightAwayFromNest.Add(randomLarva);
                    larvaeTopRight.RemoveAt(randIndex);
                    if (larvaeTopRight.Count <= 0)
                    {
                        emptyNestsNum++;
                    }
                    Debug.Log("Larva from the top right nest is heading for the apple...!");
                    break;
                }
                // else do not break, but try the other corners instead
                goto case larvaeNest.bottomLeft;
            }
            case larvaeNest.bottomLeft:
            {
                if (larvaeBottomLeft != null && larvaeBottomLeft.Count > 0)
                {
                    int randIndex = UnityEngine.Random.Range(0, larvaeBottomLeft.Count);
                    GameObject randomLarva = larvaeBottomLeft[randIndex];
                    movingLarvae.Add(randomLarva);
                    larvaeBottomLeftAwayFromNest.Add(randomLarva);
                    larvaeBottomLeft.RemoveAt(randIndex);
                    if (larvaeBottomLeft.Count <= 0)
                    {
                        emptyNestsNum++;
                    }
                    Debug.Log("Larva from the bottom left nest is heading for the apple...!");
                    break;
                }
                // else do not break, but try the other corners instead
                goto case larvaeNest.bottomRight;
            }
            case larvaeNest.bottomRight:
            {
                if (larvaeBottomRight != null && larvaeBottomRight.Count > 0)
                {
                    int randIndex = UnityEngine.Random.Range(0, larvaeBottomRight.Count);
                    GameObject randomLarva = larvaeBottomRight[randIndex];
                    movingLarvae.Add(randomLarva);
                    larvaeBottomRightAwayFromNest.Add(randomLarva);
                    larvaeBottomRight.RemoveAt(randIndex);
                    if (larvaeBottomRight.Count <= 0)
                    {
                        emptyNestsNum++;
                    }
                    Debug.Log("Larva from the bottom right nest is heading for the apple...!");
                }
                break;                 
            }

        }

        if (emptyNestsNum >= Enum.GetNames(typeof(larvaeNest)).Length)
        {
            Debug.Log("You won!");
            
            foreach (GameObject larva in larvaeTopLeftAwayFromNest)
            {
                larvaeTopLeft.Add(larva);
                larva.GetComponent<RectTransform>().localPosition = topLeftNestAnchor.GetComponent<RectTransform>().localPosition;
            }
            larvaeTopLeftAwayFromNest.Clear();
            foreach (GameObject larva in larvaeTopRightAwayFromNest)
            {
                larvaeTopRight.Add(larva);
                larva.GetComponent<RectTransform>().localPosition = topRightNestAnchor.GetComponent<RectTransform>().localPosition;
            }
            larvaeTopRightAwayFromNest.Clear();
            foreach (GameObject larva in larvaeBottomLeftAwayFromNest)
            {
                larvaeBottomLeft.Add(larva);
                larva.GetComponent<RectTransform>().localPosition = bottomLeftNestAnchor.GetComponent<RectTransform>().localPosition;
            }
            larvaeBottomLeftAwayFromNest.Clear();
            foreach (GameObject larva in larvaeBottomRightAwayFromNest)
            {
                larvaeBottomRight.Add(larva);
                larva.GetComponent<RectTransform>().localPosition = bottomRightNestAnchor.GetComponent<RectTransform>().localPosition;
            }
            larvaeBottomRightAwayFromNest.Clear();
            
            emptyNestsNum = 0;                

            sceneController.activeMinigame = SceneController.minigameName.None;
            orthoCamera.enabled = false;
            player.GetComponent<FPSInput>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            perspCamera.enabled = true;
            
        }
        */