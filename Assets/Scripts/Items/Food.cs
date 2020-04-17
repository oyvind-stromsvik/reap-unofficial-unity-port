using UnityEngine;

public class Food : MonoBehaviour {

    SpriteAnimator spriteAnimator;

    void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
    }

    void Start() {
        spriteAnimator.Play("s_food");
        spriteAnimator.currentFrame = Random.Range(0, spriteAnimator.animations[0].frames.Length);
    }
}
