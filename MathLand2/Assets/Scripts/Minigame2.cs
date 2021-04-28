using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Minigame2 : MonoBehaviour
{

    public SceneController.MinigameName thisMinigameName;
    //public bool justCalled;
    public readonly int[] powersOf2 = {0, 1, 2, 3, 4};
    //public const string orthoCameraName = "MainCamera Orthogonal";
    public const string perspCameraName = "MainCamera Perspective";

    [SerializeField] private int numOfQuestions;
    [SerializeField] private GameObject spawnPositionsParent;
    [SerializeField] private GameObject SmileysParent;
    [SerializeField] private GameObject allPots;
    [SerializeField] private GameObject Background;
    [SerializeField] private Canvas canvas;
    [SerializeField] private int[] expressionAnimations;
    
    //private Camera orthoCamera;
    private Camera perspCamera;
    private GameObject player;
    private SceneController sceneController;
    private TimedSimpleSpawner timedSimpleSpawner; // TODO: assess if this variable really needs to exist as field

    private int numOfSpawnedQuestions;
    private int numOfPlacedSmileys;
    private List<GameObject> spawnPositions;
    private List<GameObject> movingSmileys;
    private List<GameObject> listOfPots;
    //private bool showNewQuestion;

    private void OnEnable()
    {
        thisMinigameName = SceneController.MinigameName.Minigame2;        
        //showNewQuestion = true;        
        movingSmileys = new List<GameObject>();

        spawnPositions = new List<GameObject>();
        Transform[] ts = GetComponentsOnlyInChildren_NonRecursive<Transform>(spawnPositionsParent);
        if (ts != null)
        {
            foreach (Transform t in ts)
            {
                if (t != null && t.parent != null)
                {
                    spawnPositions.Add(t.gameObject); 
                    //t.gameObject.SetActive(false);
                }    
            }
            spawnPositionsParent.SetActive(true);
        }
        else
        {
            // TODO: exit minigame with failed flag
            Debug.Log("Couldn't initialize list of smileys (GameObject: spawnPositionsParent, Variable: spawnPositions)...");
        }

        listOfPots = new List<GameObject>();
        ts = GetComponentsOnlyInChildren_NonRecursive<Transform>(allPots);        
        if (ts != null)
        {
            foreach (Transform t in ts)
            {
                if (t != null && t.parent != null)
                {
                    listOfPots.Add(t.gameObject);
                    //t.gameObject.SetActive(false);
                }    
            }
            allPots.SetActive(true);
        }
        else
        {
            // TODO: exit minigame with failed flag
            Debug.Log("Couldn't initialize list of pots (GameObject: pots, Variable: allPots)...");
        }   

        player = GameObject.FindGameObjectsWithTag("Player")[0];
        sceneController = GameObject.FindGameObjectsWithTag("SceneController")[0].GetComponent<SceneController>();
        timedSimpleSpawner = GetComponent<TimedSimpleSpawner>();

        //orthoCamera = player.transform.Find(orthoCameraName).GetComponent<Camera>();
        perspCamera = player.transform.Find(perspCameraName).GetComponent<Camera>(); // It should always have two members even if they are null
    }

    // Update is called once per frame
    void Update()
    {
        // All functionality of this script is wanted only when this game is active
        if (sceneController.ActiveMinigame == thisMinigameName)
        {    
            //Debug.Log("Started timed simple spawner for Minigame2");
            timedSimpleSpawner.IsActive = true;
            
            changeAnimation();
            
            if (numOfSpawnedQuestions > numOfQuestions)
            {
                timedSimpleSpawner.IsActive = false;
            }

            // Game ended
            if (numOfPlacedSmileys >= numOfQuestions)
            {
                exitMinigame();
            }

            /*
            if (showNewQuestion)
            {
                Debug.Log("Minigame 2");
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
            */       
        }
    }

    private void changeAnimation()
    {
        // Change animation of each smiley randomly at random time
        foreach (GameObject smiley in movingSmileys)
        {
            if (smiley.activeSelf)
            {
                UnityEngine.Random.InitState(DateTime.Now.Millisecond);

                if (UnityEngine.Random.value < 0.005f)
                {    
                    int randExpression = UnityEngine.Random.Range(0, expressionAnimations.Length);
                    //Debug.Log("randExpression: " + randExpression + "\nexpressionAnimations.Length: " + expressionAnimations.Length);
                    GetComponentsOnlyInChildren_NonRecursive<Animator>(smiley)?[0].SetInteger("Expression", randExpression);
                }
            }
        }
    }

    private void exitMinigame()
    {
        Debug.Log("You won!");
        
        // Prepare the game for the next time by resetting what was changed 
        foreach (GameObject pot in listOfPots)
        {
            pot.SetActive(false);
        }
        movingSmileys.Clear();
        
        sceneController.ActiveMinigame = SceneController.MinigameName.None;
        //orthoCamera.enabled = false;
        timedSimpleSpawner.IsActive = false;
        sceneController.Minigame2Object.SetActive(false);
        player.GetComponent<FPSInput>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //perspCamera.enabled = true;
        numOfPlacedSmileys = 0; // Should be last (or the whole "if block" be atomic).
    }

    public void objectIsSpawned(object[] wrappedParameters)
    {
        GameObject spawnedObject = ((GameObject) wrappedParameters[0]);
        Type objectType = ((Type) wrappedParameters[1]);

        numOfSpawnedQuestions++;
        spawnedObject.SetActive(true);
        spawnedObject.transform.SetParent(SmileysParent.transform);
        movingSmileys.Add(spawnedObject);
        
        spawnedObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
        spawnedObject.transform.localScale = new Vector3(1f, 1f, 1f);
        Canvas.ForceUpdateCanvases();
        //StartCoroutine(lateRenderSpawnedObj(spawnedObject));
    }

    private IEnumerator lateRenderSpawnedObj(GameObject spawnedObject)
    {        
        yield return new WaitForEndOfFrame(); // This is called after LateUpdate() <-- when the GUIs are rendered
        spawnedObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
        spawnedObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    /// <summary>
    /// Gets the components only in children transform search. Not recursive, ie not grandchildren! 
    /// </summary>
    /// <returns>The components only in children transform search.</returns>
    /// <param name="parent">Parent, ie "this".</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    //  https://gist.github.com/Adjuvant/aa2b4d3ff36203f38156c62081efb924 of Adjuvan
    private T[] GetComponentsOnlyInChildren_NonRecursive<T>(GameObject parent) where T : Component
    {
        if (parent.transform.childCount <= 0) return null;

        var output = new List<T>();

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            var component = parent.transform.GetChild(i).GetComponent<T>();
            if (component != null)
                if(typeof(T).Equals(typeof(Animator)))
                {
                    Debug.Log("Found " + component.name + " in " + parent.name);
                }
                
                output.Add(component);
        }

        if (output.Count > 0)
            return output.ToArray();

        return null;
    }
}

/*
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
            woodenPanel.GetComponent<Image>().color = new Color(155, 255, 19, 1);
        }
        else
        {
            larvaLeaveNest();
            woodenPanel.GetComponent<Image>().color = new Color(255, 88, 88, 1);
        }

        yield return new WaitForSeconds(1);

        woodenPanel.GetComponent<Image>().color = new Color(255, 255, 255, 1);
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
*/