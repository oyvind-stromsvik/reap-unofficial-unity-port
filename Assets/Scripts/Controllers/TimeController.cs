using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeController : MonoBehaviour {

    public Transform sun;
    public Transform dot1;
    public Transform dot2;
    public Transform dot3;
    public Transform dot4;
    public Image mask;
    public Image overlay;

    float time;
    float time_speed;
    float view_radius;
    float dying_fadeout;

    // References.
    [HideInInspector]
    // Player is assigned by the player after he is spawned.
    public Player player;
    GameController gameController;
    ItemController items;

    void Awake() {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        items = GameObject.Find("ItemController").GetComponent<ItemController>();
    }

    void Start () {
        time = 0;
        time_speed = 1f / 45;
    }

	void Update () {
        if (player == null) {
            return;
        }

        if (player.energy <= 0) {
            player.energy = 0;
            time_speed = Utils.f_approach(time_speed, 1, 0.02f);
        }
        else {
            if (gameController.difficulty == 1) {
                time_speed = Utils.f_approach(time_speed, 1f / 30, 0.05f);
            }
            else {
                time_speed = Utils.f_approach(time_speed, 1f / 45, 0.05f);
            }
        }

        time += time_speed;
        if (time > 360) {
            time = 0;
            player.food--;

            if (player.food <= 0) {
                player.dying = true;
                SoundController.instance.PlaySound("snd_dying");
                player.GetComponent<SpriteAnimator>().Play("s_player_dying");
                player.GetComponent<SpriteAnimator>().currentFrame = 0;
                player.GetComponent<SpriteAnimator>().fps = 5;
            }
            else {
                player.energy = player.energy_max;
                f_grow();
            }
        }

        // In GameMaker the sin function takes an angle in degrees 
        // while in Unity it expects an angle in radians.
        view_radius = 240 + 120 * Mathf.Sin(time * Mathf.Deg2Rad);

        // Map the radius value which goes between 120 to 360 to 0.33 to 1
        // and pass it to the mask shader.
        float maskFloat = view_radius / 360;
        mask.material.SetFloat("_Radius", maskFloat);
        mask.material.renderQueue = mask.defaultMaterial.renderQueue;

        DrawCircle();

        Color tempColor = overlay.color;
        if (time >= 45 && time <= 135) {
            tempColor.a = 0;
        }
        else if (time <= 315 && time >= 225) {
            tempColor.a = 0.7f;
        }
        else if (time < 45 || time > 315) {
            tempColor.a = 0.35f - 0.35f * Mathf.Sin(time * 2 * Mathf.Deg2Rad);
        }
        else {
            tempColor.a = 0.35f + 0.35f * Mathf.Sin(time * 2 * Mathf.Deg2Rad);
        }
        
        if (player.dying) {
            if (dying_fadeout > 0 || time >= 135) {
                dying_fadeout++;
                tempColor.a = Mathf.Clamp(dying_fadeout / 120, 0, 1);
                if (dying_fadeout >= 360) {
                    SceneManager.LoadScene(0);
                }
                if (time > 270) {
                    time = 270;
                }
            }
        }

        //overlay.color = tempColor;
    }

    void DrawCircle() {
        var _r = view_radius / 2;
        var _a = 180 + time;
        var _x = 0;
        var _y = 0;

        dot1.position = new Vector3(
            Camera.main.transform.position.x + Mathf.Round(_x + _r + 0.5f), 
            Camera.main.transform.position.y + Mathf.Round(_y), 
            0
        );
        dot2.position = new Vector3(
            Camera.main.transform.position.x + Mathf.Round(_x - _r + 0.5f), 
            Camera.main.transform.position.y + Mathf.Round(_y), 
            0
        );
        dot3.position = new Vector3(
            Camera.main.transform.position.x + Mathf.Round(_x), 
            Camera.main.transform.position.y + Mathf.Round(_y - _r + 0.5f), 
            0
        );
        dot4.position = new Vector3(
            Camera.main.transform.position.x + Mathf.Round(_x), 
            Camera.main.transform.position.y + Mathf.Round(_y + _r + 0.5f), 
            0
        );

        // In GameMaker the sin and cos function takes an angle in degrees while in Unity they expect an angle in radians.
        sun.position = new Vector3(
            Camera.main.transform.position.x + Mathf.Round(_x + _r * Mathf.Cos(_a * Mathf.Deg2Rad)),
            Camera.main.transform.position.y + Mathf.Round(_y - _r * Mathf.Sin(_a * Mathf.Deg2Rad)), 
            0
        );
    }

    void f_grow() {
        for (int i = 0; i < items.soils.Count; i++) {
            if (items.soils[i].GetComponent<Soil>().filled && items.soils[i].GetComponent<Soil>().stage < 3) {
                items.soils[i].GetComponent<Soil>().mini_stage += Random.Range(0, 0.5f);
                if (items.soils[i].GetComponent<Soil>().mini_stage > 1) {
                    items.soils[i].GetComponent<Soil>().mini_stage = 0;
                    items.soils[i].GetComponent<Soil>().stage++;
                }
            }
        }
    }
}
