using UnityEngine;

public class LightBits : MonoBehaviour {

    SpriteAnimator spriteAnimator;
    Vector3 speed;

    void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
    }

    void Start() {
        transform.position = new Vector3(transform.position.x + Random.Range(6, 11), transform.position.y - Random.Range(6, 11), 0);
        speed = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 2f), 0);

        spriteAnimator.fps = Random.Range(12, 21);
        spriteAnimator.Play("s_light_bits");
        spriteAnimator.currentFrame = Random.Range(0, 3);
        spriteAnimator.looping = false;
    }

    void Update() {
        transform.position += speed;
        speed = new Vector3(speed.x * 0.9f, speed.y - 0.08f, 0);
    }
}
