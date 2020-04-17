using UnityEngine;

public class Bridge : MonoBehaviour {

    WorldGeneration world;
    SpriteAnimator spriteAnimator;

    void Awake() {
        world = GameObject.Find("WorldGeneration").GetComponent<WorldGeneration>();
        spriteAnimator = GetComponent<SpriteAnimator>();
    }

    void Start () {
        world.f_array_set_map((int)transform.position.x / 16, (int)transform.position.y / 16, 1);
        spriteAnimator.fps = 3;
        spriteAnimator.Play("s_bridge");
        spriteAnimator.currentFrame = Random.Range(0, spriteAnimator.animations[0].frames.Length);
    }
}
