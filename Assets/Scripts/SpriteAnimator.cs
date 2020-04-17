using UnityEngine;

public class SpriteAnimator : MonoBehaviour {

    [System.Serializable]
    public class Animation {
        public string name;
        public Sprite[] frames;
    }

    public Animation[] animations;

    [HideInInspector]
    public bool framerateIndependent = false;

    [HideInInspector]
    public bool looping = true;

    // Set this to control the speed of the animation.
    [HideInInspector]
    public int fps = 0;
    // Set this to control the current frame of the animation. F. ex. setting
    // fps to 0 and this to 0 let's us show only the initial frame of the animation
    // for as long as we wish.
    [HideInInspector]
    public int currentFrame = 0;
    // The default sprite is the one we've assigned to the sprite renderer.
    // Store it here so we can put it back later if we need to. We'll lose the
    // one on the sprite renderer when we play an animation.
    [HideInInspector]
    public Sprite defaultSprite;
    [HideInInspector]
    public Animation currentAnimation;

    SpriteRenderer spriteRenderer;
    float timer = 0;
    int frameCounter = 0;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
        currentAnimation = null;
    }

    void Update() {
        // Don't do anything if there is no animation assigned.
        if (currentAnimation == null) {
            return;
        }

        if (currentFrame == currentAnimation.frames.Length && !looping) {
            Destroy(gameObject);
        }

        // This is a really clever way of handling looping. 
        // Taken from GameMaker, and inspired by Daniel Linssen's code.
        currentFrame = currentFrame % currentAnimation.frames.Length;

        // If the framerate is zero don't calculate the next frame, just keep
        // returning the current frame. We can change the current frame
        // from outside this script and animate the sprite this way as well.
        if (fps == 0) {
            spriteRenderer.sprite = currentAnimation.frames[currentFrame];
            return;
        }

        spriteRenderer.sprite = currentAnimation.frames[currentFrame];

        if (framerateIndependent) {
            timer += Time.deltaTime;
            if (timer >= (1f / fps)) {
                timer = 0;
                currentFrame++;
            }
        }
        else {
            frameCounter++;
            if (frameCounter >= Mathf.FloorToInt(60 / fps)) {
                frameCounter = 0;
                currentFrame++;
            }
        }
    }

    public void Play(string name) {
        if (currentAnimation != null && currentAnimation.name == name) {
            return;
        }

        bool found = false;
        foreach (Animation animation in animations) {
            if (animation.name == name) {
                currentAnimation = animation;
                found = true;
                break;
            }
        }

        if (!found) {
            Debug.LogError("Animation " + name + " not found.");
        }
    }

    public void Stop() {
        currentAnimation = null;
    }
}
