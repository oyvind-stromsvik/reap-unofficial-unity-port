using UnityEngine;

public class Treasure : MonoBehaviour {

    public bool dug_up = false;

    // References.
    SpriteRenderer spriteRenderer;
    ItemController items;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        items = GameObject.Find("ItemController").GetComponent<ItemController>();
    }

    void Update() {
        if (dug_up) {
            spriteRenderer.enabled = true;
            if (Utils.f_chance(0.5f)) {
                Instantiate(items.light_bits, new Vector3(transform.position.x, transform.position.y + 3, 0), Quaternion.identity);
            }
        }
    }
}
