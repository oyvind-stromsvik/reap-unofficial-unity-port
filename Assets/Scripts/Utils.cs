using UnityEngine;

public class Utils : MonoBehaviour {

    /**
     * Copied from the original source.
     */
    public static float f_approach(float from, float to, float speed) {
        if (from < to) {
            return Mathf.Min(from + speed, to);
        }
        else {
            return Mathf.Max(from - speed, to);
        }
    }

    /**
     * Basically copied from GameMaker because the function is used so much in the code.
     */
    public static float point_distance(int x1, int y1, int x2, int y2) {
        Vector2 _a = new Vector2(x1, y1);
        Vector2 _b = new Vector2(x2, y2);
        return Vector2.Distance(_a, _b);
    }

    /**
     * Copied from the original source.
     */
    public static bool f_chance(float value) {
        return value > Random.value;
    }

    /**
     * Just a shorthand for converting a bool to an integer using
     * System.Convert.ToInt32 because it's used a lot in the code.
     */
    public static int b2i(bool value) {
        return System.Convert.ToInt32(value);
    }
}
