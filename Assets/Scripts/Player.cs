using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    [HideInInspector]
    public int energy;
    [HideInInspector]
    public int energy_max;
    [HideInInspector]
    public int food;
    [HideInInspector]
    public int food_max;

    int energy_step = 1;
    int energy_paddle = 2;
    int energy_chop = 12;
    int energy_raft = 36;
    int energy_bridge = 24;
    //int energy_plant = 2; Not actually used. Probably forgotten.
    int energy_till = 24;
    int energy_reap = 12;

    bool sleeping;
    [HideInInspector]
    public bool dying;

    // A counter to count for how many frames the left or right hand
    // buttons are being held down.
    int left_hand_counter = 0;
    int right_hand_counter = 0;

    // Tells us what we're currently holding in our left and/or right hand.
    [HideInInspector]
    public string left_hand = "none";
    [HideInInspector]
    public string right_hand = "none";

    // Holds the instance of the game object we're currently holding. We don't
    // have access to immediate drawing methods like Game Maker's draw_sprite()
    // so instead I'm moving the instances from the world to the Player when we
    // drop them or pick them up.
    GameObject left_hand_game_object = null;
    GameObject right_hand_game_object = null;

    // Counters for actions the player can do.
    int counter_chop = 0;
    int counter_raft = 0;
    int counter_bridge = 0;
    int counter_till = 0;
    int counter_plant = 0;
    int counter_reap = 0;
    int counter_map = 0;
    int counter_secret = 0;

    // The x and y position of the Player.
    float x;
    float y;

    // Horizontal and vertical input.
    // These are just so we don't have to type input._h and input._v all the time.
    int _h;
    int _v;

    float move_h;
    float move_v;

    // References.
    SpriteAnimator spriteAnimator;
    WorldGeneration world;
    ItemController items;
    DrawMap mapController;
    UIController uiController;
    GameController gameController;
    TimeController timeController;
    InputController input;

    void Awake() {
        input = GameObject.Find("InputController").GetComponent<InputController>();
        world = GameObject.Find("WorldGeneration").GetComponent<WorldGeneration>();
        items = GameObject.Find("ItemController").GetComponent<ItemController>();
        mapController = GameObject.Find("WorldGeneration").GetComponent<DrawMap>();
        uiController = GameObject.Find("UIController").GetComponent<UIController>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        timeController = GameObject.Find("TimeController").GetComponent<TimeController>();
        spriteAnimator = GetComponent<SpriteAnimator>();
    }

    void Start() {
        uiController.player = this;
        timeController.player = this;

        if (gameController.difficulty == 1) {
            energy_max = 120;
        }
        else {
            energy_max = 180;
        }
        energy = energy_max;

        if (gameController.difficulty == 1) {
            food_max = 8;
        }
        else {
            food_max = 16;
        }
        food = food_max;
    }

    void Update() {
        if (!gameController.start) {
            return;
        }

        if (energy > 0) {

            if (sleeping) {
                sleeping = false;
                spriteAnimator.Play("s_player_south");
            }

            _h = input._h;
            _v = input._v;

            // Assign transform position to variables similarily named as those in
            // Game Maker for ease of use.
            x = transform.position.x;
            y = transform.position.y;

            if (input._A) {
                left_hand_counter++;
            }
            else if (left_hand_counter < 15 && left_hand_counter > 0) {
                // pick up or drop
                left_hand_counter = 0;

                int _x = Mathf.RoundToInt(x / 16) * 16;
                int _y = Mathf.RoundToInt(y / 16) * 16;

                if (left_hand == "none") {
                    f_item_pickup(_x, _y, ref left_hand, ref left_hand_game_object);
                    // see if anything can be picked up
                }
                else if (world.f_array_get_map(_x / 16, _y / 16, -1000) > 0) { // not on a raft
                    f_item_drop(_x, _y, ref left_hand, ref left_hand_game_object);
                }
            }
            else {
                left_hand_counter = 0;
            }

            if (input._B) {
                right_hand_counter++;
            }
            else if (right_hand_counter < 15 && right_hand_counter > 0) {
                // pick up or drop
                right_hand_counter = 0;

                int _x = Mathf.RoundToInt(x / 16) * 16;
                int _y = Mathf.RoundToInt(y / 16) * 16;

                if (right_hand == "none") {
                    f_item_pickup(_x, _y, ref right_hand, ref right_hand_game_object);
                    // see if anything can be picked up
                }
                else if (world.f_array_get_map(_x / 16, _y / 16, -1000) > 0) { // not on a raft
                    f_item_drop(_x, _y, ref right_hand, ref right_hand_game_object);
                }
            }
            else {
                right_hand_counter = 0;
            }

            if (_v < 0) {
                spriteAnimator.Play("s_player_south");
            }
            else if (_h < 0) {
                spriteAnimator.Play("s_player_west");
            }
            else if (_h > 0) {
                spriteAnimator.Play("s_player_east");
            }
            else if (_v > 0) {
                spriteAnimator.Play("s_player_north");
            }

            bool _chopping = false;
            bool _rafting = false;
            bool _bridging = false;
            bool _tilling = false;
            bool _planting = false;
            bool _reaping = false;
            bool _mapping = false;
            bool _secreting = false;

            if (x % 16 == 0 && y % 16 == 0) {
                int _i = Mathf.RoundToInt(x / 16);
                int _j = Mathf.RoundToInt(y / 16);

                int _map = world.f_array_get_map(_i + _h, _j + _v, -1000);
                float _for = world.f_array_get_forest(_i + _h, _j + _v, -1);

                if (_map >= 0 && _for < 0 && (_h != 0 || _v != 0) && !(world.f_array_get_map(_i + _h, _j, -1000) < 0 && world.f_array_get_map(_i, _j + _v, -1000) < 0)) { // walking
                    move_h = _h;
                    move_v = _v;

                    Instantiate(items.floor_bits, new Vector3(x, y - 2, 0), Quaternion.identity);

                    energy -= energy_step;

                    spriteAnimator.fps = 10;
                }
                else if (world.f_array_get_map(_i, _j, 10000) == 10000 && _map <= -100 && (_h != 0 || _v != 0)) { // moving on a raft
                    move_h = _h * 0.5f;
                    move_v = _v * 0.5f;

                    spriteAnimator.currentFrame = 0;
                    spriteAnimator.fps = 0;

                    energy -= energy_paddle;

                    // Find the raft beneath us and move it with us.
                    int i = 0;
                    foreach (GameObject raft in items.rafts) {
                        if (raft.transform.position.x == x && raft.transform.position.y == y) {
                            raft.transform.position += new Vector3(
                                _h * 16,
                                _v * 16,
                                0
                            );
                        }
                        i++;
                    }

                    // Set the position we came from to be deep water.
                    world.f_array_set_map(_i, _j, -100);
                    // Set the position we're going to to be raft.
                    world.f_array_set_map(_i + _h, _j + _v, 10000);
                }
                else { // not moving, so do other checks...
                    move_h = 0;
                    move_v = 0;

                    spriteAnimator.currentFrame = 0;
                    spriteAnimator.fps = 0;

                    // chop that tree?
                    if (_for >= 0 && ((left_hand == "axe" && left_hand_counter > 0) || (right_hand == "axe" && right_hand_counter > 0))) {
                        counter_chop++;
                        _chopping = true;
                        if (Utils.f_chance(0.4f)) {
                            Instantiate(items.wood_bits, new Vector3(x + _h * 12, y + _v * 12, 0), Quaternion.identity);
                        }
                        SoundController.instance.PlaySound("snd_hit", 0.4f);

                        if (counter_chop >= 60) {
                            // CHOP THAT TREE
                            counter_chop = 0;
                            for (int i = 0; i < 10; i++) {
                                Instantiate(items.wood_bits, new Vector3(x + _h * 16, y + _v * 16, 0), Quaternion.identity);
                            }
                            SoundController.instance.PlaySound("snd_hit_done");
                            energy -= energy_chop;
                            world.f_forest_destroy(_i + _h, _j + _v);

                        }
                    }
                    // build that bridge?
                    else if (_map > -100 && _map < 0 && ((left_hand == "log" && left_hand_counter > 0) || (right_hand == "log" && right_hand_counter > 0))) {
                        counter_bridge++;
                        _bridging = true;
                        if (Utils.f_chance(0.5f)) {
                            Instantiate(items.wood_bits, new Vector3(x + _h * 12, y + _v * 12, 0), Quaternion.identity);
                        }
                        SoundController.instance.PlaySound("snd_hit", 0.4f);
                        if (counter_bridge >= 90) {
                            // BUILD THAT BRIDGE
                            counter_bridge = 0;
                            for (int i = 0; i < 10; i++) {
                                Instantiate(items.wood_bits, new Vector3(x + _h * 8, y + _v * 8, 0), Quaternion.identity);
                            }
                            SoundController.instance.PlaySound("snd_hit_done");
                            energy -= energy_bridge;
                            Instantiate(items.bridge, new Vector3((_i + _h) * 16, (_j + _v) * 16, 0), Quaternion.identity);
                            if (left_hand == "log") {
                                left_hand = "none";
                                Destroy(left_hand_game_object);
                            }
                            else {
                                right_hand = "none";
                                Destroy(right_hand_game_object);
                            }
                        }

                    }
                    // build that raft?
                    else if (_map <= -100 && ((left_hand == "log" && left_hand_counter > 0) || (right_hand == "log" && right_hand_counter > 0))) {
                        counter_raft++;
                        _rafting = true;
                        if (Utils.f_chance(0.666f)) {
                            Instantiate(items.wood_bits, new Vector3(x + _h * 12, y + _v * 12, 0), Quaternion.identity);
                        }
                        SoundController.instance.PlaySound("snd_hit", 0.4f);
                        if (counter_raft >= 120) {
                            // BUILD THAT RAFT
                            counter_raft = 0;
                            for (int i = 0; i < 10; i++) {
                                Instantiate(items.wood_bits, new Vector3(x + _h * 8, y + _v * 8, 0), Quaternion.identity);
                            }
                            SoundController.instance.PlaySound("snd_hit_done");
                            energy -= energy_raft;
                            GameObject instantiatedRaft = Instantiate(items.raft, new Vector3((_i + _h) * 16, (_j + _v) * 16, 0), Quaternion.identity) as GameObject;
                            items.rafts.Add(instantiatedRaft);
                            if (left_hand == "log") {
                                left_hand = "none";
                                Destroy(left_hand_game_object);
                            }
                            else {
                                right_hand = "none";
                                Destroy(right_hand_game_object);
                            }
                        }
                    }
                    // till that ground?
                    else if (_map >= 0 && _for < 0 && ((left_hand == "shovel" && left_hand_counter > 0) || (right_hand == "shovel" && right_hand_counter > 0))) {
                        counter_till++;
                        bool _skip = false;
                        for (int i = 0; i < items.soils.Count; i++) {
                            if (items.soils[i].transform.position.x == x && items.soils[i].transform.position.y == y) {
                                _skip = true;
                            }
                        }
                        if (!_skip) {
                            _tilling = true;
                            if (Utils.f_chance(0.333f)) {
                                Instantiate(items.light_bits, new Vector3(x, y - 3, 0), Quaternion.identity);
                            }
                            SoundController.instance.PlaySound("snd_dig");
                        }
                        if (counter_till >= 90) {

                            for (int i = 0; i < items.treasures.Count; i++) {
                                if (items.treasures[i].transform.position.x == x && items.treasures[i].transform.position.y == y) {
                                    // you win!
                                    _skip = true;
                                    if (!items.treasures[i].GetComponent<Treasure>().dug_up) {
                                        items.treasures[i].GetComponent<Treasure>().dug_up = true;
                                        for (int j = 0; j < 20; j++) {
                                            Instantiate(items.light_bits, new Vector3(x, y - 3, 0), Quaternion.identity);
                                        }
                                        for (int j = 0; j < 10; j++) {
                                            Instantiate(items.wood_bits, new Vector3(x, y, 0), Quaternion.identity);
                                        }
                                        SoundController.instance.PlaySound("snd_dig_done");
                                    }
                                }
                            }

                            // TILL THAT GROUND
                            counter_till = 0;
                            if (!_skip) {
                                SoundController.instance.PlaySound("snd_dig_done");
                                energy -= energy_till;
                                GameObject instantiatedSoil = Instantiate(items.soil, new Vector3((_i + _h) * 16, (_j + _v) * 16, 0), Quaternion.identity) as GameObject;
                                items.soils.Add(instantiatedSoil);
                            }
                        }
                    }
                    // eat that turnip?
                    else if (_map >= 0 && _for < 0 && ((left_hand == "food" && left_hand_counter > 0) || (right_hand == "food" && right_hand_counter > 0)) && food < food_max) {
                        counter_plant++;
                        _planting = true;
                        if (counter_plant >= 60) {
                            for (int i = 0; i < 10; i++) {
                                Instantiate(items.light_bits, new Vector3(x, y + 2, 0), Quaternion.identity);
                            }
                            SoundController.instance.PlaySound("snd_eat");
                            // EAT THAT TURNIP
                            counter_plant = 0;
                            if (left_hand == "food") {
                                left_hand = "none";
                                Destroy(left_hand_game_object);
                            }
                            else {
                                right_hand = "none";
                                Destroy(right_hand_game_object);
                            }
                            food++;
                        }
                    }
                    // check out that map?
                    else if ((left_hand == "map" && left_hand_counter > 0) || (right_hand == "map" && right_hand_counter > 0)) {
                        counter_map++;
                        _mapping = true;
                    }
                    // puzzle that secret?
                    else if ((left_hand == "puzzle" && left_hand_counter > 0) || (right_hand == "puzzle" && right_hand_counter > 0)) {
                        counter_secret++;
                        _secreting = true;
                    }
                    // reap that ground?
                    else if (_map >= 0 && _for < 0 && ((left_hand == "hoe" && left_hand_counter > 0) || (right_hand == "hoe" && right_hand_counter > 0))) {
                        counter_reap++;

                        bool _skip = true;
                        for (int i = 0; i < items.soils.Count; i++) {
                            if (items.soils[i].transform.position.x == x && items.soils[i].transform.position.y == y && items.soils[i].GetComponent<Soil>().filled) {
                                _skip = false;
                            }
                        }

                        if (!_skip) {
                            _reaping = true;
                            if (Utils.f_chance(0.2f)) {
                                Instantiate(items.light_bits, new Vector3(x, y - 3, 0), Quaternion.identity);
                            }
                            SoundController.instance.PlaySound("snd_dig");
                        }

                        if (counter_reap >= 60) {
                            for (int i = 0; i < 10; i++) {
                                Instantiate(items.light_bits, new Vector3(x, y - 3, 0), Quaternion.identity);
                            }
                            SoundController.instance.PlaySound("snd_dig_done");
                            // REAP THAT GROUND
                            counter_reap = 0;
                            for (int i = 0; i < items.soils.Count; i++) {
                                if (items.soils[i].transform.position.x == x && items.soils[i].transform.position.y == y && items.soils[i].GetComponent<Soil>().filled) {
                                    int _temp = Random.Range(0, 2);
                                    GameObject food_instance = Instantiate(items.food, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                                    food_instance.GetComponent<SpriteAnimator>().currentFrame = _temp;
                                    items.foods.Add(food_instance);
                                    if (items.soils[i].GetComponent<Soil>().stage == 3) {
                                        GameObject food_instance2 = Instantiate(items.food, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                                        food_instance2.GetComponent<SpriteAnimator>().currentFrame = _temp + 2;
                                        items.foods.Add(food_instance2);
                                    }
                                    energy -= energy_reap;
                                    items.soils[i].GetComponent<Soil>().filled = false;
                                    items.soils[i].GetComponent<Soil>().stage = 0;
                                }
                            }
                        }
                    }
                }
            }

            // Show the map.
            if (counter_map > 15) {
                mapController.mapObject.SetActive(true);
            }
            else if (counter_secret > 15) {
                mapController.secretObject.SetActive(true);
            }
            else {
                mapController.mapObject.SetActive(false);
                mapController.secretObject.SetActive(false);
            }

            if (!_chopping) counter_chop = 0;
            if (!_rafting) counter_raft = 0;
            if (!_bridging) counter_bridge = 0;
            if (!_tilling) counter_till = 0;
            if (!_planting) counter_plant = 0;
            if (!_reaping) counter_reap = 0;
            if (!_mapping) counter_map = 0;
            if (!_secreting) counter_secret = 0;
        }
        else if (!sleeping && !dying) {
            if (x % 16 == 0 && y % 16 == 0) {
                move_h = 0;
                move_v = 0;
                sleeping = true;
                SoundController.instance.PlaySound("snd_sleep");
                spriteAnimator.Play("s_player_sleeping");
                spriteAnimator.fps = 7;
                for (int i = 0; i < 6; i++) {
                    Instantiate(items.light_bits, new Vector3(x, y - 3, 0), Quaternion.identity);
                }
            }

        }

        if (dying) {
            move_h = 0;
            move_v = 0;
            if (spriteAnimator.currentFrame >= 14) {
                spriteAnimator.currentFrame = 14;
            }
        }

        if (move_h != 0 || move_v != 0) {
            if (move_h != 0) {
                x += 2 * move_h;
            }

            if (move_v != 0) {
                y += 2 * move_v;
            }

            transform.position = new Vector3(x, y, transform.position.z);
        }

        Camera.main.transform.position = new Vector3(transform.position.x + 8, transform.position.y, Camera.main.transform.position.z);

        // Draw any items we're currently holding.
        DrawItems();
    }

    void f_item_drop(int _x, int _y, ref string _hand, ref GameObject _item) {
        // Not entirely sure, but I think this is just the "not on a raft" test again.
        if (world.f_array_get_map(_x / 16, _y / 16, -1000) == 10000) {
            return;
        }

        // Place the dropped item in the game world.
        _item.transform.SetParent(null);
        _item.transform.position = new Vector3(_x, _y, 0);

        // Play the item drop sound effect.
        SoundController.instance.PlaySound("snd_drop");

        // Make sure the dropped object is drawn behind the player with the
        // default sprite. Also stop the sprite animator from animating so 
        // that it doesn't override the default sprite.
        _item.GetComponent<SpriteRenderer>().sortingOrder = 4;
        if (_item.GetComponent<SpriteAnimator>() != null) {
            _item.GetComponent<SpriteRenderer>().sprite = _item.GetComponent<SpriteAnimator>().defaultSprite;
            _item.GetComponent<SpriteAnimator>().Stop();
        }

        // Add the dropped item back into the correct list.
        switch (_item.tag) {
            case "axe":
                items.axes.Add(_item);
                break;
            case "hoe":
                items.hoes.Add(_item);
                break;
            case "shovel":
                items.shovels.Add(_item);
                break;
            case "log":
                _item.GetComponent<SpriteAnimator>().Play("s_log");
                _item.GetComponent<SpriteAnimator>().currentFrame = Random.Range(0, _item.GetComponent<SpriteAnimator>().animations[0].frames.Length);
                items.logs.Add(_item);
                break;
            case "puzzleA":
                items.puzzle_pieces_A.Add(_item);
                f_puzzle_check_drop(_item.transform.position);
                break;
            case "puzzleB":
                items.puzzle_pieces_B.Add(_item);
                f_puzzle_check_drop(_item.transform.position);
                break;
            case "puzzleC":
                items.puzzle_pieces_C.Add(_item);
                f_puzzle_check_drop(_item.transform.position);
                break;
            case "puzzleD":
                items.puzzle_pieces_D.Add(_item);
                f_puzzle_check_drop(_item.transform.position);
                break;
            case "puzzle":
                items.puzzles.Add(_item);
                break;
            case "map":
                items.maps.Add(_item);
                break;
            case "food":
                bool _planted = false;

                _item.GetComponent<SpriteAnimator>().Play("s_food");

                // PLANT THAT TURNIP
                for (int i = 0; i < items.soils.Count; i++) {
                    if (items.soils[i].transform.position.x == x && items.soils[i].transform.position.y == y && !items.soils[i].GetComponent<Soil>().filled) {
                        _planted = true;
                        for (int j = 0; j < 6; j++) {
                            Instantiate(items.light_bits, new Vector3(x, y -2, 0), Quaternion.identity);
                        }
                        items.soils[i].GetComponent<Soil>().filled = true;
                        items.soils[i].GetComponent<Soil>().stage = 1;
                        Destroy(_item);
                    }
                }

                if (!_planted) {
                    _item.GetComponent<SpriteAnimator>().currentFrame = Random.Range(0, _item.GetComponent<SpriteAnimator>().animations[0].frames.Length);
                    items.foods.Add(_item);
                }
                break;
        }

        // Empty the hand we dropped.
        _hand = "none";
        _item = null;
    }

    void f_item_pickup(int _x, int _y, ref string _hand, ref GameObject _item) {
        if (f_item_pickup_helper(items.axes, _x, _y, ref _hand, ref _item)) return;
        if (f_item_pickup_helper(items.shovels, _x, _y, ref _hand, ref _item)) return;
        if (f_item_pickup_helper(items.hoes, _x, _y, ref _hand, ref _item)) return;
        if (f_item_pickup_helper(items.foods, _x, _y, ref _hand, ref _item)) return;
        if (f_item_pickup_helper(items.puzzle_pieces_A, _x, _y, ref _hand, ref _item)) return;
        if (f_item_pickup_helper(items.puzzle_pieces_B, _x, _y, ref _hand, ref _item)) return;
        if (f_item_pickup_helper(items.puzzle_pieces_C, _x, _y, ref _hand, ref _item)) return;
        if (f_item_pickup_helper(items.puzzle_pieces_D, _x, _y, ref _hand, ref _item)) return;
        if (f_item_pickup_helper(items.maps, _x, _y, ref _hand, ref _item)) return;
        if (f_item_pickup_helper(items.logs, _x, _y, ref _hand, ref _item)) return;
        if (f_item_pickup_helper(items.puzzles, _x, _y, ref _hand, ref _item)) return;

        // Found nothing to pickup.
        _hand = "none";
        _item = null;
    }

    bool f_item_pickup_helper(List<GameObject> items, int _x, int _y, ref string _hand, ref GameObject _item) {
        for (int i = 0; i < items.Count; i++) {
            if (items[i].transform.position.x == _x && items[i].transform.position.y == _y) {
                items[i].transform.SetParent(transform);
                _hand = items[i].tag;
                _item = items[i];
                items.RemoveAt(i);
                SoundController.instance.PlaySound("snd_pickup", 0.8f);
                return true;
            }
        }

        return false;
    }

    void DrawItems() {
        // Make sure the items are drawn in front of the player when we're holding them.
        if (left_hand_game_object != null) {
            left_hand_game_object.GetComponent<SpriteRenderer>().sortingOrder = 6;
        }
        if (right_hand_game_object != null) {
            right_hand_game_object.GetComponent<SpriteRenderer>().sortingOrder = 6;
        }

        switch (left_hand) {
            // AXE
            case "axe":
                if (spriteAnimator.currentAnimation.name == "s_player_east") {
                    DrawItemsHelper("left", new Vector3(x, y + 2, 0), "s_axe_lr", Mathf.RoundToInt(counter_chop * ((float)Utils.b2i(left_hand_counter > 5) / 3)));
                }
                else {
                    DrawItemsHelper("left", new Vector3(x, y + 2, 0), "s_axe_l", Mathf.RoundToInt(counter_chop * ((float)Utils.b2i(left_hand_counter > 5) / 3)));
                }
                break;

            // HOE
            case "hoe":
                if (spriteAnimator.currentAnimation.name == "s_player_east") {
                    DrawItemsHelper("left", new Vector3(x, y + 2, 0), "s_hoe_lr", Mathf.RoundToInt(counter_reap * ((float)Utils.b2i(left_hand_counter > 5) / 3)));
                }
                else {
                    DrawItemsHelper("left", new Vector3(x, y + 2, 0), "s_hoe_l", Mathf.RoundToInt(counter_reap * ((float)Utils.b2i(left_hand_counter > 5) / 3)));
                }
                break;

            // SHOVEL
            case "shovel":
                if (spriteAnimator.currentAnimation.name == "s_player_east") {
                    DrawItemsHelper("left", new Vector3(x, y + 2, 0), "s_shovel_lr", Mathf.RoundToInt(counter_till * ((float)Utils.b2i(left_hand_counter > 5) / 3)));
                }
                else {
                    DrawItemsHelper("left", new Vector3(x, y + 2, 0), "s_shovel_l", Mathf.RoundToInt(counter_till * ((float)Utils.b2i(left_hand_counter > 5) / 3)));
                }
                break;
            
            // LOG
            case "log":
                int _temp = Utils.b2i(counter_bridge > 5 || counter_raft > 5);
                DrawItemsHelper("left", new Vector3(x - 6 + _h * 3 * _temp, y + 2 + _v * 3 * _temp, 0), "s_log_hand", 0);
                break;

            // FOOD
            case "food":
                DrawItemsHelper("left", new Vector3(x - 6 + Mathf.RoundToInt((float)(Utils.b2i(left_hand_counter > 0) * counter_plant) / 12), y + 1 + Mathf.RoundToInt((float)(Utils.b2i(left_hand_counter > 0) * counter_plant) / 24), 0), "s_food_hand", 0);
                break;

            // PUZZLE PIECES
            case "puzzleA":
            case "puzzleB":
            case "puzzleC":
            case "puzzleD":
                DrawItemsHelper("left", new Vector3(x - 6, y + 1, 0), "s_puzzle_piece", 1);
                break;

            // PUZZLE MAP
            case "puzzle":
                DrawItemsHelper("left", new Vector3(x - 7, y + 1, 0), "s_puzzle", 1);
                break;

            // MAP
            case "map":
                DrawItemsHelper("left", new Vector3(x - 8, y + 1, 0), "s_map", 1);
                break;
        }

        switch (right_hand) {
            // AXE
            case "axe":
                if (spriteAnimator.currentAnimation.name == "s_player_west") {
                    DrawItemsHelper("right", new Vector3(x + 7, y + 2, 0), "s_axe_r", Mathf.RoundToInt(counter_chop * ((float)Utils.b2i(right_hand_counter > 5) / 3)));
                }
                else {
                    DrawItemsHelper("right", new Vector3(x + 7, y + 2, 0), "s_axe", Mathf.RoundToInt(counter_chop * ((float)Utils.b2i(right_hand_counter > 5) / 3)));
                }
                break;

            // HOE
            case "hoe":
                if (spriteAnimator.currentAnimation.name == "s_player_west") {
                    DrawItemsHelper("right", new Vector3(x + 7, y + 2, 0), "s_hoe_r", Mathf.RoundToInt(counter_reap * ((float)Utils.b2i(right_hand_counter > 5) / 3)));
                }
                else {
                    DrawItemsHelper("right", new Vector3(x + 7, y + 2, 0), "s_hoe", Mathf.RoundToInt(counter_reap * ((float)Utils.b2i(right_hand_counter > 5) / 3)));
                }
                break;

            // SHOVEL
            case "shovel":
                if (spriteAnimator.currentAnimation.name == "s_player_west") {
                    DrawItemsHelper("right", new Vector3(x + 7, y + 2, 0), "s_shovel_r", Mathf.RoundToInt(counter_till * ((float)Utils.b2i(right_hand_counter > 5) / 3)));
                }
                else {
                    DrawItemsHelper("right", new Vector3(x + 7, y + 2, 0), "s_shovel", Mathf.RoundToInt(counter_till * ((float)Utils.b2i(right_hand_counter > 5) / 3)));
                }
                break;

            // LOG
            case "log":
                int _temp = Utils.b2i(counter_bridge > 5 || counter_raft > 5);
                DrawItemsHelper("right", new Vector3(x + 7 + _h * 3 * _temp, y + 2 + _v * 3 * _temp, 0), "s_log_hand", 0);
                break;
            
            // FOOD
            case "food":
                DrawItemsHelper("right", new Vector3(x + 5 - Mathf.RoundToInt((float)(Utils.b2i(right_hand_counter > 0) * counter_plant) / 12), y + 1 + Mathf.RoundToInt((float)(Utils.b2i(right_hand_counter > 0) * counter_plant) / 24), 0), "s_food_hand", 0);
                break;

            // PUZZLE PIECES
            case "puzzleA":
            case "puzzleB":
            case "puzzleC":
            case "puzzleD":
                DrawItemsHelper("right", new Vector3(x + 5, y + 1, 0), "s_puzzle_piece", 1);
                break;

                // PUZZLE MAP
            case "puzzle":
                DrawItemsHelper("right", new Vector3(x + 6, y + 1, 0), "s_puzzle", 1);
                break;

                // MAP
            case "map":
                DrawItemsHelper("right", new Vector3(x + 6, y + 1, 0), "s_map", 1);
                break;
        }
    }

    void DrawItemsHelper(string hand, Vector3 drawPosition, string animationName, int currentFrame) {
        if (hand == "left") {
            left_hand_game_object.transform.position = drawPosition;
            left_hand_game_object.GetComponent<SpriteAnimator>().Play(animationName);
            left_hand_game_object.GetComponent<SpriteAnimator>().currentFrame = currentFrame;
        }
        else {
            right_hand_game_object.transform.position = drawPosition;
            right_hand_game_object.GetComponent<SpriteAnimator>().Play(animationName);
            right_hand_game_object.GetComponent<SpriteAnimator>().currentFrame = currentFrame;
        }
    }

    void f_puzzle_check_drop(Vector3 pos) {
        if (items.puzzle_pieces_A[0].transform.position == pos &&
            items.puzzle_pieces_B[0].transform.position == pos &&
            items.puzzle_pieces_C[0].transform.position == pos &&
            items.puzzle_pieces_D[0].transform.position == pos) 
        {
            // the pieces are together!!!
            for (int i = 0; i < 20; i++) {
                Instantiate(items.light_bits, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            }

            GameObject puzzle_instance = Instantiate(items.puzzle, new Vector3(pos.x, pos.y, 0), Quaternion.identity) as GameObject;
            puzzle_instance.GetComponent<SpriteAnimator>().Play("s_puzzle");
            puzzle_instance.GetComponent<SpriteAnimator>().currentFrame = 0;
            items.puzzles.Add(puzzle_instance);

            Destroy(items.puzzle_pieces_A[0]);
            items.puzzle_pieces_A.Clear();
            Destroy(items.puzzle_pieces_B[0]);
            items.puzzle_pieces_B.Clear();
            Destroy(items.puzzle_pieces_C[0]);
            items.puzzle_pieces_C.Clear();
            Destroy(items.puzzle_pieces_D[0]);
            items.puzzle_pieces_D.Clear();
        }
    }
}
