using UnityEngine;

public class FloorBits : MonoBehaviour {

    SpriteAnimator spriteAnimator;
    Vector3 speed;

    void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
    }

    void Start() {
        transform.position = new Vector3(transform.position.x + Random.Range(6, 11), transform.position.y - Random.Range(6, 11), 0);
        speed = new Vector3(Random.Range(-1f, 1f), Random.Range(1f, 1f), 0);

        spriteAnimator.fps = Random.Range(10, 16);
        if (Utils.f_chance(0.5f)) {
            spriteAnimator.Play("s_floor_bits");
        }
        else {
            spriteAnimator.Play("s_floor_bits_alt");
        }
        spriteAnimator.currentFrame = Random.Range(0, 3);
        spriteAnimator.looping = false;
    }

    void Update() {
        transform.position += speed;
        speed = new Vector3(speed.x * 0.8f, speed.y * 0.8f, 0);
    }
}
