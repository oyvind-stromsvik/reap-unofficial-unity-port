using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    // The opening title screen.
    public GameObject titleScreen;
    // The title text so we can animate it.
    public Transform titleText;
    // The text informing you about difficulty and what buttons to press.
    public GameObject textLeftSide;
    public GameObject textRightSide;
    // The instructions screen shown while the game is generating the world.
    public GameObject instructionsScreen;
    // The prompt shown when the world is generated.
    public GameObject pressKeyToBeginText;
    // The main UI shown when we play the game.
    public GameObject gameScreen;

    // 0 - normal, 1 - hard
    [HideInInspector]
    public int difficulty = 0;
    // 0 - title screen, 1 - loading
    int stage = 0;
    // Public so we ensure the player can't move until we allow him to. 
    [HideInInspector]
    public bool start = false;
    // Public so the world generation can tell us when we're ready to start.
    [HideInInspector]
    public bool ready_to_start = false;
    // for fly-in animation and delay before pressing starting input
    int counter_intro = 0;
    int counter_continuous = 0;

    WorldGeneration world;
    InputController input;

    float initialTitlePositionY;

    void Awake() {
        world = GameObject.Find("WorldGeneration").GetComponent<WorldGeneration>();
        input = GameObject.Find("InputController").GetComponent<InputController>();
        Application.targetFrameRate = 60;
        Screen.SetResolution(1280, 720, false);
    }

    void Start() {
        initialTitlePositionY = titleText.position.y;
    }

    void Update() {
        if (!start) {
            switch (stage) {
                case 0:
                    counter_intro++;

                    titleScreen.SetActive(true);
                    titleText.position = new Vector3(titleText.position.x, initialTitlePositionY + 6 * Mathf.Sin(counter_intro * 2 * Mathf.Deg2Rad), 0);
                    if (counter_intro > 90) {
                        textLeftSide.SetActive(true);
                        textRightSide.SetActive(true);

                        if (input._B || input._A) {
                            if (input._B) {
                                difficulty = 1;
                            }
                            if (input._A) {
                                difficulty = 0;
                            }
                            SoundController.instance.PlaySound("snd_UI");
                            stage = 1;
                            StartCoroutine(world.GenerateWorld());
                        }
                    }
                    break;

                case 1:
                    counter_continuous++;
                    if (counter_continuous > 360) {
                        counter_continuous -= 360;
                    }
                        
                    titleScreen.SetActive(false);
                    instructionsScreen.SetActive(true);
                    if (ready_to_start) {
                        if ((counter_continuous / 20) % 4 < 2) {
                            pressKeyToBeginText.GetComponent<Text>().color = Colors.x_shadow;
                        }
                        else {
                            pressKeyToBeginText.GetComponent<Text>().color = Colors.x_bush;
                        }
                        pressKeyToBeginText.SetActive(true);
                        if (input._B || input._A) {
                            SoundController.instance.PlaySound("snd_UI");
                            start = true;
                            titleScreen.SetActive(false);
                            instructionsScreen.SetActive(false);
                            gameScreen.SetActive(true);
                        }
                    }
                    break;
            }
        }
    }
}
