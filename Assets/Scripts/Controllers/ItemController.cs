using UnityEngine;
using System.Collections.Generic;

public class ItemController : MonoBehaviour {

    // The player prefab.
    [Header("Player prefab")]
    public GameObject player;
    // The actual instanced player which we control and interact with.
    GameObject playerInstance;

    // Prefabs for items that can't be interacted with after they've been placed.
    [Header("Static prefabs")]
    public GameObject bridge;

    // Prefabs for particle effects.
    [Header("Particle prefabs")]
    public GameObject wood_bits;
    public GameObject floor_bits;
    public GameObject light_bits;

    // Prefabs for items we can interact with.
    [Header("Dynamic prefabs")]
    public GameObject raft;
    public GameObject axe;
    public GameObject log;
    public GameObject shovel;
    public GameObject map;
    public GameObject hoe;
    public GameObject food;
    public GameObject puzzle_piece_A;
    public GameObject puzzle_piece_B;
    public GameObject puzzle_piece_C;
    public GameObject puzzle_piece_D;
    public GameObject soil;
    public GameObject puzzle;
    public GameObject treasure;

    // For keeping track of instances of items we can interact with.
    [HideInInspector]
    public List<GameObject> rafts;
    [HideInInspector]
    public List<GameObject> axes;
    [HideInInspector]
    public List<GameObject> logs;
    [HideInInspector]
    public List<GameObject> shovels;
    [HideInInspector]
    public List<GameObject> maps;
    [HideInInspector]
    public List<GameObject> hoes;
    [HideInInspector]
    public List<GameObject> foods;
    [HideInInspector]
    public List<GameObject> puzzle_pieces_A;
    [HideInInspector]
    public List<GameObject> puzzle_pieces_B;
    [HideInInspector]
    public List<GameObject> puzzle_pieces_C;
    [HideInInspector]
    public List<GameObject> puzzle_pieces_D;
    [HideInInspector]
    public List<GameObject> soils;
    [HideInInspector]
    public List<GameObject> puzzles;
    [HideInInspector]
    public List<GameObject> treasures;

    // References.
    WorldGeneration world;
    GameController gameController;

    void Awake() {
        world = GameObject.Find("WorldGeneration").GetComponent<WorldGeneration>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void f_items_add() {
        // Look for a suitable position in the world and instantiate the player
        // and one axe there.
        bool _found = false;
        while (!_found) {
            var _i = Random.Range(world.i_max / 4, 3 * world.i_max / 4 + 1);
            var _j = Random.Range(world.j_max / 4, 3 * world.j_max / 4 + 1);

            if (world.map[_i, _j] > 0 && world.map[_i, _j + 1] > 0 && world.forest[_i, _j] < -1 && world.forest[_i, _j + 1] < -4) {
                if ((world.map[_i - 1, _j] <= -100 ||
                    world.map[_i - 1, _j - 1] <= -100 ||
                    world.map[_i - 1, _j + 1] <= -100 ||
                    world.map[_i + 1, _j] <= -100 ||
                    world.map[_i + 1, _j - 1] <= -100 ||
                    world.map[_i + 1, _j + 1] <= -100 ||
                    world.map[_i, _j + 1] <= -100 ||
                    world.map[_i, _j - 1] <= -100 ||
                    world.map[_i, _j - 2] <= -100 ||
                    world.map[_i, _j + 2] <= -100 ||
                    world.map[_i + 2, _j] <= -100 ||
                    world.map[_i - 2, _j] <= -100) &&
                    (world.forest[_i - 1, _j] > 0 ||
                    world.forest[_i - 1, _j - 1] > 0 ||
                    world.forest[_i - 1, _j + 1] > 0 ||
                    world.forest[_i + 1, _j] > 0 ||
                    world.forest[_i + 1, _j - 1] > 0 ||
                    world.forest[_i + 1, _j + 1] > 0 ||
                    world.forest[_i, _j + 1] > 0 ||
                    world.forest[_i, _j - 1] > 0 ||
                    world.forest[_i, _j - 2] > 0 ||
                    world.forest[_i, _j + 2] > 0 ||
                    world.forest[_i + 2, _j] > 0 ||
                    world.forest[_i - 2, _j] > 0)) {
                    playerInstance = Instantiate(player, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                    GameObject _item_instance = Instantiate(axe, new Vector3(_i * 16, (_j + 1) * 16, 0), Quaternion.identity) as GameObject;
                    axes.Add(_item_instance);
                    _found = true;
                }
            }
        }

        // Get the position of the player so we can create items around that spot.
        var _I = (int)playerInstance.transform.position.x / 16;
        var _J = (int)playerInstance.transform.position.y / 16;

        // main shovel
        _found = false;
        while (!_found) {
            int _r = 12 + 4 * gameController.difficulty;
            int _i = _I + Random.Range(-_r, _r + 1);
            int _j = _J + Random.Range(-_r, _r + 1);
            if (world.map[_i, _j] > 0 && Utils.point_distance(_i, _j, _I, _J) > _r * 3 / 4 && Utils.point_distance(_i, _j, _I, _J) < _r) {
                if (world.forest[_i, _j] > 0) {
                    world.f_forest_destroy(_i, _j);
                    world.f_forest_destroy(_i + Random.Range(-1, 1), _j + 1);
                    world.f_forest_destroy(_i, _j + Random.Range(-1, 1 + 1));
                }
                GameObject _item_instance = Instantiate(shovel, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                shovels.Add(_item_instance);
                _found = true;
            }
        }

        // main hoe
        _found = false;
        while (!_found) {
            int _r = 16 + 6 * gameController.difficulty;
            int _i = _I + Random.Range(-_r, _r + 1);
            int _j = _J + Random.Range(-_r, _r + 1);
            if (world.map[_i, _j] > 0 && Utils.point_distance(_i, _j, _I, _J) > _r * 3 / 4 && Utils.point_distance(_i, _j, _I, _J) < _r) {
                if (world.forest[_i, _j] > 0) {
                    world.f_forest_destroy(_i, _j);
                    world.f_forest_destroy(_i + Random.Range(-1, 1), _j + 1);
                    world.f_forest_destroy(_i, _j + Random.Range(-1, 1 + 1));
                }
                GameObject _item_instance = Instantiate(hoe, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                hoes.Add(_item_instance);
                _found = true;
            }
        }

        // map
        _found = false;
        while (!_found) {
            int _r = 20 + 8 * gameController.difficulty;
            int _i = _I + Random.Range(-_r, _r + 1);
            int _j = _J + Random.Range(-_r, _r + 1);
            if (world.map[_i, _j] > 0 && Utils.point_distance(_i, _j, _I, _J) > _r * 3 / 4 && Utils.point_distance(_i, _j, _I, _J) < _r) {
                if (world.forest[_i, _j] > 0) {
                    world.f_forest_destroy(_i, _j);
                    world.f_forest_destroy(_i + Random.Range(-1, 1), _j + 1);
                    world.f_forest_destroy(_i, _j + Random.Range(-1, 1 + 1));
                }
                GameObject _item_instance = Instantiate(map, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                maps.Add(_item_instance);
                _found = true;
            }
        }

        // main food
        for (int i = 0; i < 6; i++) {
            _found = false;
            while (!_found) {
                int _i = _I + Random.Range(-24, 24 + 1);
                int _j = _J + Random.Range(-24, 24 + 1);

                if (world.map[_i, _j] > 0 && world.forest[_i, _j] < 0 && Utils.point_distance(_i, _j, _I, _J) > 8 && Utils.point_distance(_i, _j, _I, _J) < 24) {
                    GameObject _item_instance = Instantiate(food, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                    foods.Add(_item_instance);
                    _found = true;
                }
            }
        }

        // food!
        for (int i = 0; i < 40; i++) {
            _found = false;
            while (!_found) {
                int _i = Random.Range(10, world.i_max - 10 + 1);
                int _j = Random.Range(10, world.j_max - 10 + 1);
                if (world.map[_i, _j] > 0) {
                    if (world.forest[_i, _j] > 0) {
                        if (Utils.f_chance(0.25f)) {
                            world.f_forest_destroy(_i, _j);
                            world.f_forest_destroy(_i + Random.Range(-1, 1), _j + 1);
                            world.f_forest_destroy(_i, _j + Random.Range(-1, 1 + 1));
                            GameObject _item_instance = Instantiate(food, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                            foods.Add(_item_instance);
                            _found = true;
                        }
                    }
                    else {
                        GameObject _item_instance = Instantiate(food, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                        foods.Add(_item_instance);
                        _found = true;
                    }
                }
            }
        }

        // more tools!
        for (int i = 0; i < 13 - 4 * gameController.difficulty; i++) {
            _found = false;
            while (!_found) {
                int _i = Random.Range(10, world.i_max - 10 + 1);
                int _j = Random.Range(10, world.j_max - 10 + 1);
                if (world.map[_i, _j] > 0) {
                    if (world.forest[_i, _j] > 0) {
                        world.f_forest_destroy(_i, _j);
                        world.f_forest_destroy(_i + Random.Range(-1, 1), _j + 1);
                        world.f_forest_destroy(_i, _j + Random.Range(-1, 1 + 1));
                    }
                    int _rand = Random.Range(1, 4);
                    switch (_rand) {
                        case 1:
                            GameObject axe_instance = Instantiate(axe, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                            hoes.Add(axe_instance);
                            break;
                        case 2:
                            GameObject shovel_instance = Instantiate(shovel, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                            hoes.Add(shovel_instance);
                            break;
                        case 3:
                            GameObject hoe_instance = Instantiate(hoe, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                            hoes.Add(hoe_instance);
                            break;
                    }
                    _found = true;
                }
            }
        }

        // puzzle pieces!!!
        _found = false;
        while (!_found) {
            int _i = Random.Range(30, world.i_max - 30 + 1);
            int _j = Random.Range(30, world.j_max - 30 + 1);
            if (world.map[_i, _j] > 0 && Utils.point_distance(_i, _j, world.i_max / 2, world.j_max / 2) > 32) {
                if (world.forest[_i, _j] > 0) {
                    world.f_forest_destroy(_i, _j);
                    world.f_forest_destroy(_i + Random.Range(-1, 1), _j + 1);
                    world.f_forest_destroy(_i, _j + Random.Range(-1, 1 + 1));
                }
                GameObject _item_instance = Instantiate(puzzle_piece_A, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                puzzle_pieces_A.Add(_item_instance);
                _found = true;
            }
        }

        _found = false;
        while (!_found) {
            int _i = Random.Range(30, world.i_max - 30 + 1);
            int _j = Random.Range(30, world.j_max - 30 + 1);
            if (world.map[_i, _j] > 0 && Utils.point_distance(_i, _j, world.i_max / 2, world.j_max / 2) > 32) {
                if (world.forest[_i, _j] > 0) {
                    world.f_forest_destroy(_i, _j);
                    world.f_forest_destroy(_i + Random.Range(-1, 1), _j + 1);
                    world.f_forest_destroy(_i, _j + Random.Range(-1, 1 + 1));
                }
                GameObject _item_instance = Instantiate(puzzle_piece_B, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                puzzle_pieces_B.Add(_item_instance);
                _found = true;
            }
        }

        _found = false;
        while (!_found) {
            int _i = Random.Range(30, world.i_max - 30 + 1);
            int _j = Random.Range(30, world.j_max - 30 + 1);
            if (world.map[_i, _j] > 0 && Utils.point_distance(_i, _j, world.i_max / 2, world.j_max / 2) > 32) {
                if (world.forest[_i, _j] > 0) {
                    world.f_forest_destroy(_i, _j);
                    world.f_forest_destroy(_i + Random.Range(-1, 1), _j + 1);
                    world.f_forest_destroy(_i, _j + Random.Range(-1, 1 + 1));
                }
                GameObject _item_instance = Instantiate(puzzle_piece_C, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                puzzle_pieces_C.Add(_item_instance);
                _found = true;
            }
        }

        _found = false;
        while (!_found) {
            int _i = Random.Range(30, world.i_max - 30 + 1);
            int _j = Random.Range(30, world.j_max - 30 + 1);
            if (world.map[_i, _j] > 0 && Utils.point_distance(_i, _j, world.i_max / 2, world.j_max / 2) > 32) {
                if (world.forest[_i, _j] > 0) {
                    world.f_forest_destroy(_i, _j);
                    world.f_forest_destroy(_i + Random.Range(-1, 1), _j + 1);
                    world.f_forest_destroy(_i, _j + Random.Range(-1, 1 + 1));
                }
                GameObject _item_instance = Instantiate(puzzle_piece_D, new Vector3(_i * 16, _j * 16, 0), Quaternion.identity) as GameObject;
                puzzle_pieces_D.Add(_item_instance);
                _found = true;
            }
        }

        // Whenever an item was set to spawn on a tile where there was forest we
        // destroyed that forest, this resulted in a lot of logs being generated.
        // Destroy all these logs before the game begins.
        foreach (GameObject log in logs) {
            Destroy(log);
        }
        logs.Clear();
    }
}
