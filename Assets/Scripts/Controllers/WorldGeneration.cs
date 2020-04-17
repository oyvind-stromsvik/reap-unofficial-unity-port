using UnityEngine;
using System.Collections;

public class WorldGeneration : MonoBehaviour {

    [Header("Tilemap")]
    public Texture2D tex;
    public Material mat;
    [Header("Randomization")]
    public bool randomSeed;
    public int seed;
    [Header("World size")]
    public int i_max = 384;
    public int j_max = 384;

    // perlin - real values of height
    // map - integer values of height and depth
    // octave - for calculations
    // forest - real values of forest, > 0 is forested
    float[,] perlin;
    [HideInInspector]
    public int[,] map;
    float[,] octave;
    [HideInInspector]
    public float[,] forest;
    // Contains all the forest sprites.
    GameObject[,] tiles;

    // References.
    DrawMap drawMap;
    ItemController items;
    GameController gameController;

    void Awake() {
        items = GameObject.Find("ItemController").GetComponent<ItemController>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        drawMap = GetComponent<DrawMap>();

        // Initialize the map arrays.
        perlin = new float[i_max, j_max];
        map = new int[i_max, j_max];
        octave = new float[i_max, j_max];
        forest = new float[i_max, j_max];

        // Initialize the forest sprite array.
        tiles = new GameObject[i_max, j_max];
    }

    public IEnumerator GenerateWorld() {
        // In GameMaker the world generation is delayed by 3 frames to ensure
        // the instructions screen is shown before the game locks up while it's
        // generating the world. I don't know how to wait for a specific number
        // of frames in Unity so I'm just waiting for 0.05 seconds instead which
        // is 3 frames if the game is running at 60 fps.
        yield return new WaitForSeconds(0.05f);

        // Set the random seed ensuring every iteration is the same.
        if (!randomSeed) {
            Random.InitState(seed);
        }

        // Create the world.
        CreateWorld();

        // Create all the items as well as the player instance.
        items.f_items_add();

        // MAKE MAPS!!!
        drawMap.MakeMap(map, forest, i_max, j_max);
        drawMap.MakeSecretMap(map, forest, i_max, j_max);

        // Let the game controller know we're ready to begin.
        gameController.ready_to_start = true;
    }

    void CreateWorld() {
        // MAP
        f_perlin_noise();

        /*----------------------------------------------------------------------/
          CIRCULARIZE
        /----------------------------------------------------------------------*/
        f_matrix_rescale(ref perlin, -128, 300);

        for (int _i = 0; _i < i_max; _i++) {
            for (int _j = 0; _j < j_max; _j++) {
                float _r = Utils.point_distance(_i, _j, i_max / 2, j_max / 2) * 1.333f;
                perlin[_i, _j] -= _r;
                // volcanize !!!
                if (perlin[_i, _j] > 256) perlin[_i, _j] = 256 - perlin[_i, _j];
            }
        }

        // map to type
        for (int _i = 0; _i < i_max; _i++) {
            for (int _j = 0; _j < j_max; _j++) {
                if (perlin[_i, _j] < -32) map[_i, _j] = -1000; // deep water
                else if (perlin[_i, _j] < 0) map[_i, _j] = -100; // shallow water
                else if (perlin[_i, _j] < 32) map[_i, _j] = 1;  // land
                else if (perlin[_i, _j] < 64) map[_i, _j] = 2;  // land
                else if (perlin[_i, _j] < 96) map[_i, _j] = 3;  // land
                else if (perlin[_i, _j] < 128) map[_i, _j] = 4;  // land
                else if (perlin[_i, _j] < 160) map[_i, _j] = 5;  // land
                else if (perlin[_i, _j] < 192) map[_i, _j] = 6;  // land
                else map[_i, _j] = 7;  // mountain
            }
        }

        // Ensure the edges of the map are deep water, I guess?
        for (int _i = 0; _i < i_max; _i++) {
            map[_i, 0] = -1000;
            map[_i, 1] = Mathf.Min(map[_i, 1], -100);
            map[_i, j_max - 2] = Mathf.Min(map[_i, j_max - 2], -100);
            map[_i, j_max - 1] = -1000;
        }

        for (int _j = 0; _j < j_max; _j++) {
            map[0, _j] = -1000;
            map[1, _j] = Mathf.Min(map[1, _j], -100);
            map[i_max - 2, _j] = Mathf.Min(map[i_max - 2, _j], -100);
            map[i_max - 1, _j] = -1000;
        }

        /*----------------------------------------------------------------------/
          RIVERS
        /----------------------------------------------------------------------*/
        float _river_count = 0;
        int _river_wriggle = 8;

        while (_river_count < i_max * 2) {
            _river_count++;

            int _i = Random.Range(20, i_max - 20);
            int _j = Random.Range(20, j_max - 20);

            if (perlin[_i, _j] > 8) {
                float _river_length = 0;
                while (map[_i, _j] > -100 && _river_length < 75 && _i > 1 && _j > 1 && _i < i_max - 2 && _j < j_max - 2) {
                    map[_i, _j] = -Mathf.Abs(map[_i, _j]); // river
                    _river_length++;

                    float _lowest_h = perlin[_i, _j];
                    int _lowest_i = _i;
                    int _lowest_j = _j;

                    float _start_h = _lowest_h;

                    float _new_h = perlin[_i + 1, _j] - Random.Range(0, _river_wriggle) * Random.value;
                    if (_new_h < _lowest_h) {
                        _lowest_i = _i + 1;
                        _lowest_j = _j;
                        _lowest_h = _new_h;
                    }
                    _new_h = perlin[_i - 1, _j] - Random.Range(0, _river_wriggle) * Random.value;
                    if (_new_h < _lowest_h) {
                        _lowest_i = _i - 1;
                        _lowest_j = _j;
                        _lowest_h = _new_h;
                    }
                    _new_h = perlin[_i, _j + 1] - Random.Range(0, _river_wriggle) * Random.value;
                    if (_new_h < _lowest_h) {
                        _lowest_i = _i;
                        _lowest_j = _j + 1;
                        _lowest_h = _new_h;
                    }
                    _new_h = perlin[_i, _j - 1] - Random.Range(0, _river_wriggle) * Random.value;
                    if (_new_h < _lowest_h) {
                        _lowest_i = _i;
                        _lowest_j = _j - 1;
                        _lowest_h = _new_h;
                    }
                    _new_h = perlin[_i + 1, _j + 1] - Random.Range(0, _river_wriggle) * Random.value;
                    if ((_new_h - _start_h) * 0.7f < _lowest_h - _start_h) {
                        _lowest_i = _i + 1;
                        _lowest_j = _j + 1;
                        _lowest_h = _start_h + (_new_h - _start_h) * 0.7f;
                    }
                    _new_h = perlin[_i - 1, _j + 1] - Random.Range(0, _river_wriggle) * Random.value;
                    if ((_new_h - _start_h) * 0.7f < _lowest_h - _start_h) {
                        _lowest_i = _i - 1;
                        _lowest_j = _j + 1;
                        _lowest_h = _start_h + (_new_h - _start_h) * 0.7f;
                    }
                    _new_h = perlin[_i + 1, _j - 1] - Random.Range(0, _river_wriggle) * Random.value;
                    if ((_new_h - _start_h) * 0.7f < _lowest_h - _start_h) {
                        _lowest_i = _i + 1;
                        _lowest_j = _j - 1;
                        _lowest_h = _start_h + (_new_h - _start_h) * 0.7f;
                    }
                    _new_h = perlin[_i - 1, _j - 1] - Random.Range(0, _river_wriggle) * Random.value;
                    if ((_new_h - _start_h) * 0.7f < _lowest_h - _start_h) {
                        _lowest_i = _i - 1;
                        _lowest_j = _j - 1;
                        _lowest_h = _start_h + (_new_h - _start_h) * 0.7f;
                    }

                    _i = _lowest_i;
                    _j = _lowest_j;
                }
            }
        }

        /*----------------------------------------------------------------------/
          FORESTS and CLIFFS
        /----------------------------------------------------------------------*/
        for (int _i = 0; _i < i_max; _i++) {
            for (int _j = 0; _j < j_max; _j++) {
                forest[_i, _j] = 0;
            }
        }

        f_perlin_noise_stage(16, i_max, j_max, ref forest);
        f_perlin_noise_stage(4, i_max, j_max, ref octave);
        for (int _i = 0; _i < i_max; _i++) {
            for (int _j = 0; _j < j_max; _j++) {
                forest[_i, _j] += 2 * octave[_i, _j];
            }
        }
        f_perlin_noise_stage(2, i_max, j_max, ref octave);
        for (int _i = 0; _i < i_max; _i++) {
            for (int _j = 0; _j < j_max; _j++) {
                forest[_i, _j] += 4 * octave[_i, _j];
            }
        }

        f_matrix_rescale(ref forest, -400, 600);

        for (int _i = 0; _i < i_max; _i++) {
            for (int _j = 0; _j < j_max; _j++) {
                if (map[_i, _j] < 0) forest[_i, _j] = -1000;
                else forest[_i, _j] -= perlin[_i, _j];
            }
        }

        //tile_add(bg_land, 0, 0, 16, 16, 8, 8, 0);
        //tile_add(bg_land, 0, 0, 16, 16, -8 + i_max * 16, -8 + j_max * 16, 0);

        for (int _i = 1; _i < i_max; _i++) {
            for (int _j = 1; _j < j_max; _j++) {
                f_tiles_generate(_i, _j);
            }
        }
    }

    void f_perlin_noise() {
        for (int _i = 0; _i < i_max; _i++) {
            for (int _j = 0; _j < j_max; _j++) {
                perlin[_i, _j] = 0;
                octave[_i, _j] = 0;
            }
        }

        for (int _n = 2; _n <= 32; _n *= 2) {
            f_perlin_noise_stage(_n, i_max, j_max, ref octave);

            for (int _i = 0; _i < i_max; _i++) {
                for (int _j = 0; _j < j_max; _j++) {
                    perlin[_i, _j] += octave[_i, _j];
                }
            }

        }
    }

    void f_perlin_noise_stage(int _n, int _i_max, int _j_max, ref float[,] _octave) {
        float _n_inv = 1f / _n;

        // generating
        // replace this with something pseudorandom later
        for (int _i = 0; _i < _i_max; _i += _n) {
            for (int _j = 0; _j < _j_max; _j += _n) {
                _octave[_i, _j] = Random.Range(0f, (float) _n); //    /32
            }
        }

        // cubic interpolation
        // interpolate each major row
        for (int _i = 0; _i < _i_max; _i += _n) {
            for (int _j = 0; _j < _j_max; _j += _n) {
                float _v1 = _octave[_i, _j];
                float _v2 = _octave[(_i + _n) % _i_max, _j];
                float _v3 = _octave[(_i + 2 * _n) % _i_max, _j];
                float _v0 = _octave[(_i - _n + _i_max) % _i_max, _j];

                for (int _k = 1; _k < _n; _k++) {
                    float _P = (_v3 - _v2) - (_v0 - _v1);
                    float _Q = (_v0 - _v1) - _P;
                    float _R = _v2 - _v0;
                    float _S = _v1;
                    float _x = _k * _n_inv;
                    _octave[_i + _k, _j] = _P * _x * _x * _x + _Q * _x * _x + _R * _x + _S;
                }
            }
        }

        // interpolate each major and minor column
        for (int _i = 0; _i < _i_max; _i++) {
            for (int _j = 0; _j < _j_max; _j += _n) {
                float _v1 = _octave[_i, _j];
                float _v2 = _octave[_i, (_j + _n) % _j_max];
                float _v3 = _octave[_i, (_j + 2 * _n) % _j_max];
                float _v0 = _octave[_i, (_j - _n + _j_max) % _j_max];

                for (int _l = 1; _l < _n; _l++) {
                    float _P = (_v3 - _v2) - (_v0 - _v1);
                    float _Q = (_v0 - _v1) - _P;
                    float _R = _v2 - _v0;
                    float _S = _v1;
                    float _y = _l * _n_inv;
                    _octave[_i, _j + _l] = _P * _y * _y * _y + _Q * _y * _y + _R * _y + _S;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="depth"></param>
    /// <returns>The newly created game object with the sprite.</returns>
    GameObject f_tile_add(int left, int top, int x, int y, int depth = 0) {
        int width = 16;
        int height = 16;
        // In Unity 0,0 is the bottom left of the sprite, in Game Maker it's the top left.
        Rect rect = new Rect(left * width, tex.height - top * height - height, width, height);
        Vector2 pivot = new Vector2(0, 1);
        Sprite sprite = Sprite.Create(tex, rect, pivot, 1);
        GameObject newObject = new GameObject();
        newObject.AddComponent<SpriteRenderer>();
        newObject.GetComponent<SpriteRenderer>().sprite = sprite;
        newObject.GetComponent<SpriteRenderer>().sortingOrder = depth;
        newObject.GetComponent<SpriteRenderer>().material = mat;
        newObject.transform.position = new Vector3(width * x - (width / 2), height * y - (height / 2), 0);
        newObject.transform.SetParent(transform);
        newObject.name = "tile_" + x + "_" + y + "_" + depth;
        return newObject;
    }

    void f_tiles_generate(int _i, int _j) {
        // land
        // These are reordered because in Game Maker 0,0 is top left,
        // in Unity 0,0 is bottom left.
        int _u2 = map[_i, _j - 1];
        int _u1 = map[_i, _j];
        int _u4 = map[_i - 1, _j];
        int _u3 = map[_i - 1, _j - 1];

        // all deep ocean
        if (_u1 == -1000 && _u2 == -1000 && _u3 == -1000 && _u4 == -1000) {
            return;
        }

        // coastline (1-14) and solid ground (0)
        var _value_land = 15 - Utils.b2i(_u1 >= 0) - 2 * Utils.b2i(_u2 >= 0) - 4 * Utils.b2i(_u3 >= 0) - 8 * Utils.b2i(_u4 >= 0);

        if (_value_land < 15) {
            // place land tiles down
            //f_tile_add(Random.Range(1, 5), _value_land, _i, _j);
            f_tile_add(1, _value_land, _i, _j);

            var _value = 15 - Utils.b2i(_u1 >= 2) - 2 * Utils.b2i(_u2 >= 2) - 4 * Utils.b2i(_u3 >= 2) - 8 * Utils.b2i(_u4 >= 2);

            if (_u1 < 0 || _u2 < 0 || _u3 < 0 || _u4 < 0) _value = 16;

            if (_value < 15) {
                if (_value == 0) {

                    _value = 15 - Utils.b2i(_u1 >= 3) - 2 * Utils.b2i(_u2 >= 3) - 4 * Utils.b2i(_u3 >= 3) - 8 * Utils.b2i(_u4 >= 3);

                    if (_u1 < 0 || _u2 < 0 || _u3 < 0 || _u4 < 0) _value = 16;

                    if (_value < 15) {
                        if (_value == 0) {

                            _value = 15 - Utils.b2i(_u1 >= 4) - 2 * Utils.b2i(_u2 >= 4) - 4 * Utils.b2i(_u3 >= 4) - 8 * Utils.b2i(_u4 >= 4);

                            if (_u1 < 0 || _u2 < 0 || _u3 < 0 || _u4 < 0) _value = 16;

                            if (_value < 15) {
                                if (_value == 0) {

                                    _value = 15 - Utils.b2i(_u1 >= 5) - 2 * Utils.b2i(_u2 >= 5) - 4 * Utils.b2i(_u3 >= 5) - 8 * Utils.b2i(_u4 >= 5);

                                    if (_u1 < 0 || _u2 < 0 || _u3 < 0 || _u4 < 0) _value = 16;

                                    if (_value < 15) {
                                        if (_value == 0) {

                                            _value = 15 - Utils.b2i(_u1 >= 6) - 2 * Utils.b2i(_u2 >= 6) - 4 * Utils.b2i(_u3 >= 6) - 8 * Utils.b2i(_u4 >= 6);

                                            if (_u1 < 0 || _u2 < 0 || _u3 < 0 || _u4 < 0) _value = 16;

                                            if (_value < 15) {
                                                if (_value == 0) {

                                                    _value = 15 - Utils.b2i(_u1 >= 7) - 2 * Utils.b2i(_u2 >= 7) - 4 * Utils.b2i(_u3 >= 7) - 8 * Utils.b2i(_u4 >= 7);

                                                    if (_u1 < 0 || _u2 < 0 || _u3 < 0 || _u4 < 0) _value = 16;

                                                    if (_value < 15) {
                                                        if (_value == 0) {
                                                            // do nothing!
                                                        }
                                                        else {
                                                            f_tile_add(Random.Range(11, 15), _value, _i, _j, 1);
                                                        }
                                                    }
                                                }
                                                else {
                                                    f_tile_add(Random.Range(11, 15), _value, _i, _j, 1);
                                                }
                                            }
                                        }
                                        else {
                                            f_tile_add(Random.Range(11, 15), _value, _i, _j, 1);
                                        }
                                    }
                                }
                                else {
                                    f_tile_add(Random.Range(11, 15), _value, _i, _j, 1);
                                }
                            }
                        }
                        else {
                            f_tile_add(Random.Range(11, 15), _value, _i, _j, 1);
                        }
                    }
                }
                else {
                    f_tile_add(Random.Range(11, 15), _value, _i, _j, 1);
                }
            }

            // forests
            // These are reordered because in Game Maker 0,0 is top left,
            // in Unity 0,0 is bottom left.
            float _v2 = forest[_i, _j - 1];
            float _v1 = forest[_i, _j];
            float _v4 = forest[_i - 1, _j];
            float _v3 = forest[_i - 1, _j - 1];

            float _value2 = 15 - Utils.b2i(_v1 >= 0) - 2 * Utils.b2i(_v2 >= 0) - 4 * Utils.b2i(_v3 >= 0) - 8 * Utils.b2i(_v4 >= 0);

            if (_value2 < 15) {
                tiles[_i, _j] = f_tile_add(Random.Range(15, 19), (int)_value2, _i, _j, 2);
            }
        }

        // sea
        if (_value_land != 0) {
            int _value3 = 15 - Utils.b2i(_u1 <= -100) - 2 * Utils.b2i(_u2 <= -100) - 4 * Utils.b2i(_u3 <= -100) - 8 * Utils.b2i(_u4 <= -100);

            if (_value3 == 0) {
                _value3 = 15 - Utils.b2i(_u1 == -100) - 2 * Utils.b2i(_u2 == -100) - 4 * Utils.b2i(_u3 == -100) - 8 * Utils.b2i(_u4 == -100);
                if (_value3 < 15) {
                    f_tile_add(Random.Range(8, 11), _value3, _i, _j);
                }

            }
            else if (_value3 < 15) {
                f_tile_add(Random.Range(5, 8), _value3, _i, _j);
            }
        }
    }

    void f_matrix_rescale(ref float[,] _A, int _low, int _high) {
        int _dif = _high - _low;

        float _min = 100000;
        float _max = -100000;
        for (int _i = 0; _i < _A.GetLength(1); _i++) {
            for (int _j = 0; _j < _A.GetLength(0); _j++) {
                if (_A[_i, _j] < _min) _min = _A[_i, _j];
                if (_A[_i, _j] > _max) _max = _A[_i, _j];
            }
        }

        for (int _i = 0; _i < _A.GetLength(1); _i++) {
            for (int _j = 0; _j < _A.GetLength(0); _j++) {
                _A[_i, _j] = _low + _dif * (_A[_i, _j] - _min) / (_max - _min);
            }
        }
    }
       
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <param name="_default"></param>
    /// <returns></returns>
    public int f_array_get_map(int _x, int _y, int _default) {
        if (_x < 0 || _x >= map.GetLength(1) || _y < 0 || _y >= map.GetLength(0)) {
            return _default;
        } 
        else {
            return map[_x, _y];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <param name="_value"></param>
    public void f_array_set_map(int _x, int _y, int _value) {
        if (_x < 0 || _x >= map.GetLength(1) || _y < 0 || _y >= map.GetLength(0)) {
        }
        else {
            map[_x, _y] = _value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <param name="_default"></param>
    /// <returns></returns>
    public float f_array_get_forest(int _x, int _y, int _default) {
        if (_x < 0 || _x >= forest.GetLength(1) || _y < 0 || _y >= forest.GetLength(0)){
            return _default;
        } 
        else {
            return forest[_x, _y];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_i"></param>
    /// <param name="_j"></param>
    public void f_forest_destroy(int _i, int _j) {
        if (forest[_i, _j] >= 0) {
            // Remove the forest from the forest array.
            forest[_i, _j] = -1;

            for (int _k = _i; _k <= _i + 1; _k++) {
                for (int _l = _j; _l <= _j + 1; _l++) {
                    // Destroy the actual game objects holding the forest sprites.
                    // Because the grid is offset there are 4 sprites per grid
                    // coordinate.
                    if (tiles[_k, _l] != null) {
                        Destroy(tiles[_k, _l]);
                        tiles[_k, _l] = null;
                    }

                    // These are reordered because in Game Maker 0,0 is top left,
                    // in Unity 0,0 is bottom left.
                    float _v2 = forest[_k, _l - 1];
                    float _v1 = forest[_k, _l];
                    float _v4 = forest[_k - 1, _l];
                    float _v3 = forest[_k - 1, _l - 1];
                    float _value = 15 - Utils.b2i(_v1 >= 0) - 2 * Utils.b2i(_v2 >= 0) - 4 * Utils.b2i(_v3 >= 0) - 8 * Utils.b2i(_v4 >= 0);
                    tiles[_k, _l] = f_tile_add(Random.Range(15, 19), (int)_value, _k, _l, 2);
                }
            }
            if (Utils.f_chance(0.333f)) {
                GameObject log_instance = Instantiate(items.log, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                items.logs.Add(log_instance);
            }
        }
    }
}
