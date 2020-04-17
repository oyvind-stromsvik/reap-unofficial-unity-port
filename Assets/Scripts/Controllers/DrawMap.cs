using UnityEngine;
using UnityEngine.UI;

public class DrawMap : MonoBehaviour {

    public GameObject mapObject;
    public GameObject secretObject;
    public Texture2D mapMarker;

    // References.
    ItemController items;

    void Awake() {
        items = GameObject.Find("ItemController").GetComponent<ItemController>();
    }

    public void MakeMap(int[,] map, float[,] forest, int i_max, int j_max) {
        // Create the texture.
        int width = i_max;
        int height = j_max;
        TextureFormat format = TextureFormat.RGB24;
        bool useMipMaps = false;
        Texture2D tex = new Texture2D(width, height, format, useMipMaps);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;

        // Generate the map.
        // NOTE: The original loop goes between 1 and max - 1, I don't understand
        //   why, but in Unity that leaves a white border around the edge of the map
        //   so I've changed it to fill in the entire map.
        for (var _i = 0; _i < i_max; _i++) {
            for (var _j = 0; _j < j_max; _j++) {

                if (forest[_i, _j] >= 0) {
                    if (forest[_i - 1, _j] < 0 || forest[_i + 1, _j] < 0 || forest[_i, _j - 1] < 0 || forest[_i, _j + 1] < 0) {
                        tex.SetPixel(_i, _j, Colors.x_black);
                    }
                    else {
                        tex.SetPixel(_i, _j, Colors.x_bush);
                    }
                }
                else {
                    if (map[_i, _j] < 0) {
                        tex.SetPixel(_i, _j, Colors.x_white);
                    }
                    else {
                        if (map[_i - 1, _j] < 0 || map[_i + 1, _j] < 0 || map[_i, _j - 1] < 0 || map[_i, _j + 1] < 0) {
                            tex.SetPixel(_i, _j, Colors.x_shadow);
                        }
                        else {
                            tex.SetPixel(_i, _j, Colors.x_sand);
                        }
                    }
                }
            }
        }

        // Add the 4 puzzle piece markers to our texture.
        AddMarker(ref tex, mapMarker, items.puzzle_pieces_A[0]);
        AddMarker(ref tex, mapMarker, items.puzzle_pieces_B[0]);
        AddMarker(ref tex, mapMarker, items.puzzle_pieces_C[0]);
        AddMarker(ref tex, mapMarker, items.puzzle_pieces_D[0]);

        // Apply the map to the texture.
        tex.Apply();

        // Create the sprite.
        Rect rect = new Rect(0, 0, tex.width, tex.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        float pixelsPerUnit = 1;
        Sprite sprite = Sprite.Create(tex, rect, pivot, pixelsPerUnit);

        // Assign it to the UI object and set it to be the correct size.
        mapObject.transform.Find("Sprite").GetComponent<Image>().sprite = sprite;
        mapObject.transform.Find("Sprite").GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width, tex.height);
    }

    public void MakeSecretMap(int[,] map, float[,] forest, int i_max, int j_max) {
        // Instantiate the treasure chest. It is hidden until we dig it up.
        GameObject treasure_instance = null;
        bool _found = false;
        while (!_found) {
            int _i = Random.Range(30, i_max - 30);
            int _j = Random.Range(30, j_max - 30);
            if (map[_i, _j] > 0 && forest[_i, _j] < 0){
                int _sea_count = 0;
                int _for_count = 0;
                for (int _k = _i - 3; _k <= _i + 3; _k++) {
                    for (int _l = _j - 3; _l <= _j + 3; _l++) {
                        if (map[_k, _l] < 0) {
                            _sea_count++;
                        }
                        else if (forest[_k, _l] >= 0) {
                            _for_count++;
                        }
                    }
                }
                if (_sea_count >= 4 && _for_count >= 3) {
                    treasure_instance = Instantiate(items.treasure, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                    items.treasures.Add(treasure_instance);
                    _found = true;
                }
            }
        }

        // Find the camera that is a child of the treasure.
        Camera cam = treasure_instance.transform.Find("TreasureCamera").GetComponent<Camera>();
        cam.gameObject.SetActive(true);

        // Use the treasure camera to take a screenshot of the area
        // surrounding the treasure. This becomes our treasure map.
        // NOTE: This could probably be handled by the camera we already have,
        //   but this works so I'm not going to spend more time on it now.
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 0);
        rt.isPowerOfTwo = false;
        rt.useMipMap = false;
        cam.targetTexture = rt;
        Texture2D tex = new Texture2D(cam.pixelWidth, cam.pixelHeight, TextureFormat.RGB24, false);
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(cam.pixelRect, 0, 0);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        cam.gameObject.SetActive(false);

        // Create the sprite.
        Rect rect = new Rect(0, 0, cam.pixelWidth, cam.pixelHeight);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        float pixelsPerUnit = 1;
        Sprite sprite = Sprite.Create(tex, rect, pivot, pixelsPerUnit);

        // Assign it to the UI object and set it to be the correct size.
        secretObject.transform.Find("Sprite").GetComponent<Image>().sprite = sprite;
        secretObject.transform.Find("Sprite").GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width, tex.height);
    }

    void AddMarker(ref Texture2D mapTexture, Texture2D markerTexture, GameObject puzzlePiece) {
        for (int x = 0; x < markerTexture.width; x++) {
            for (int y = 0; y < markerTexture.height; y++) {
                Color col = markerTexture.GetPixel(x, y);
                if (col.a > 0) {
                    mapTexture.SetPixel(
                        x + (int)puzzlePiece.transform.position.x / 16 - (int) Mathf.Floor(markerTexture.width / 2), 
                        y + (int) puzzlePiece.transform.position.y / 16 - (int) Mathf.Floor(markerTexture.height / 2), 
                        col
                    );
                }
            }
        }
    }
}
