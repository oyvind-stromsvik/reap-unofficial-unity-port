using UnityEngine;

public class Raft : MonoBehaviour {

    WorldGeneration world;

    float x_visual;
    float y_visual;

    GameObject sprite;

    SpriteRenderer spriteRenderer;

    void Awake() {
        world = GameObject.Find("WorldGeneration").GetComponent<WorldGeneration>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
        world.f_array_set_map((int)transform.position.x / 16, (int)transform.position.y / 16, 10000);
        x_visual = transform.position.x;
        y_visual = transform.position.y;

        sprite = new GameObject();
        sprite.AddComponent<SpriteRenderer>();
        sprite.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
        sprite.GetComponent<SpriteRenderer>().material = spriteRenderer.material;
        sprite.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder;
        sprite.name = "raft_sprite";

        spriteRenderer.enabled = false;
    }

    void Update() {
        x_visual = Utils.f_approach(x_visual, transform.position.x, 1);
        y_visual = Utils.f_approach(y_visual, transform.position.y, 1);

        sprite.transform.position = new Vector3((int) x_visual, (int) y_visual, 0);
    }
}
