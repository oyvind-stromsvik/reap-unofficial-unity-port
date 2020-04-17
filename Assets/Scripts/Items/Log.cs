using UnityEngine;

public class Log : MonoBehaviour {

    SpriteAnimator spriteAnimator;

    void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
    }

    void Start() {
        spriteAnimator.Play("s_log");
        spriteAnimator.currentFrame = Random.Range(0, spriteAnimator.animations[0].frames.Length);
    }
}
