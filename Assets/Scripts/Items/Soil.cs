using UnityEngine;

public class Soil : MonoBehaviour {

    public bool filled = false;
    public int stage = 0;
    public float mini_stage = 0f;

    SpriteAnimator spriteAnimator;

    void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
    }

    void Start() {
        spriteAnimator.Play("s_soil");
    }

    void Update() {
        spriteAnimator.currentFrame = stage;
    }
}
