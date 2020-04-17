using UnityEngine;

public class UIController : MonoBehaviour {

    public Transform energyIndicator;
    public Transform energyPin;
    public Transform foodIndicator;
    public Transform foodPin;
    public Transform tips_left_side;
    public Transform tips_right_side;

    float UI_energy_angle = 450;
    float UI_energy_target;
    float UI_food_angle = 450;
    float UI_food_target;

    // References.
    [HideInInspector]
    // Player is assigned by the player after he is spawned.
    public Player player;
    GameController gameController;

    void Awake() {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    /**
     * In this method we draw the tips for the item(s) we're currently holding.
     */
    void Update() {
        if (player == null) {
            return;
        }

        switch (player.left_hand) {
            case "axe":
                DrawTipsHelper("tips_axe", tips_left_side);
                break;
            case "hoe":
                DrawTipsHelper("tips_hoe", tips_left_side);
                break;
            case "shovel":
                DrawTipsHelper("tips_shovel", tips_left_side);
                break;
            case "log":
                DrawTipsHelper("tips_log", tips_left_side);
                break;
            case "food":
                DrawTipsHelper("tips_food", tips_left_side);
                break;
            case "puzzleA":
            case "puzzleB":
            case "puzzleC":
            case "puzzleD":
            case "puzzle":
            case "map":
                DrawTipsHelper("tips_unknown", tips_left_side);
                break;
            default:
                DrawTipsHelper("", tips_left_side);
                break;
        }

        switch (player.right_hand) {
            case "axe":
                DrawTipsHelper("tips_axe", tips_right_side);
                break;
            case "hoe":
                DrawTipsHelper("tips_hoe", tips_right_side);
                break;
            case "shovel":
                DrawTipsHelper("tips_shovel", tips_right_side);
                break;
            case "log":
                DrawTipsHelper("tips_log", tips_right_side);
                break;
            case "food":
                DrawTipsHelper("tips_food", tips_right_side);
                break;
            case "puzzleA":
            case "puzzleB":
            case "puzzleC":
            case "puzzleD":
            case "puzzle":
            case "map":
                DrawTipsHelper("tips_unknown", tips_right_side);
                break;
            default:
                DrawTipsHelper("", tips_right_side);
                break;
        }
    }

    static Material lineMaterial;
    static void CreateLineMaterial() {
        if (!lineMaterial) {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    /**
     * In this method we draw the lines and pins for the energy and food meters.
     */
    public void OnRenderObject() {
        if (player == null || !gameController.start) {
            return;
        }

        CreateLineMaterial();

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw the energy indicator.
        GL.Begin(GL.LINES);
        GL.Color(Colors.x_black);
        float _r = 30;
        UI_energy_target = 90 + player.energy * (360f / player.energy_max);
        UI_energy_angle = Utils.f_approach(UI_energy_angle, UI_energy_target, Mathf.Sqrt(Mathf.Abs(UI_energy_target - UI_energy_angle)) * 0.6f);
        float _a = UI_energy_angle * Mathf.Deg2Rad;
        energyPin.position = new Vector3(energyIndicator.position.x + _r * Mathf.Cos(_a), energyIndicator.position.y + _r * Mathf.Sin(_a), 0);
        GL.Vertex3(energyIndicator.position.x + 2 * Mathf.Cos(_a), energyIndicator.position.y + 2 * Mathf.Sin(_a), 0);
        GL.Vertex3(energyIndicator.position.x + _r * Mathf.Cos(_a), energyIndicator.position.y + _r * Mathf.Sin(_a), 0);
        GL.End();

        // Draw the food indicator.
        GL.Begin(GL.LINES);
        GL.Color(Colors.x_black);
        float _r2 = 30;
        UI_food_target = 90 + player.food * (360f / player.food_max);
        UI_food_angle = Utils.f_approach(UI_food_angle, UI_food_target, Mathf.Sqrt(Mathf.Abs(UI_food_target - UI_food_angle)) * 0.2f);
        float _a2 = UI_food_angle * Mathf.Deg2Rad;
        foodPin.position = new Vector3(foodIndicator.position.x + _r2 * Mathf.Cos(_a2), foodIndicator.position.y + _r2 * Mathf.Sin(_a2), 0);
        GL.Vertex3(foodIndicator.position.x + 2 * Mathf.Cos(_a2), foodIndicator.position.y + 2 * Mathf.Sin(_a2), 0);
        GL.Vertex3(foodIndicator.position.x + _r2 * Mathf.Cos(_a2), foodIndicator.position.y + _r2 * Mathf.Sin(_a2), 0);
        GL.End();

        GL.PopMatrix();
    }

    /**
     * A helper method for showing the tips.
     */
    void DrawTipsHelper(string name, Transform parent) {
        foreach (Transform child in parent) {
            if (child.name == name) {
                child.gameObject.SetActive(true);
            }
            else {
                child.gameObject.SetActive(false);
            }
        }
    }
}
