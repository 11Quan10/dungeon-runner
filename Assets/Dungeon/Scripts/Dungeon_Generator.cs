using UnityEngine;
using System;
using System.Collections.Generic;

public class Dungeon_Generator : MonoBehaviour
{

    public GameObject base_floor;
    public GameObject roof;
    public GameObject center_column;
    public GameObject center_wall_1;
    public GameObject center_wall_2;
    public GameObject closed_wall_1;
    public GameObject closed_wall_2;
    public GameObject hall_1;
    public GameObject hall_2;
    public GameObject inner_arch;
    public GameObject open_wall_1;
    public GameObject open_wall_2;

    public GameObject enemy;

    public int generation_radius = 3;
    public GameObject player;

    public int x;
    public int y;

    private Dictionary<string, Dungeon_Section> sections;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        x = 0;
        y = 0;
        sections = new Dictionary<string, Dungeon_Section>();
        reloadSections();
    }

    // Update is called once per frame
    void Update()
    {
        int newX = (int) Math.Floor ((player.transform.position.x + 12) / 24);
        int newY = (int) Math.Floor ((player.transform.position.z + 12) / 24);

        if (newX != x || newY != y) {

            x = newX;
            y = newY;

            // Adds new sections
            reloadSections();

        }
    }

    void reloadSections () {
        int startX = x - generation_radius;
        int startY = y - generation_radius;

        for (int i = 0; i < generation_radius * 2; i++) {
            for (int j = 0; j < generation_radius * 2; j++) {

                if (inside_radius(startX + i, startY + j)) {
                    addSection (startX + i, startY + j);
                }

            }
        }

    }

    bool inside_radius (int ax, int ay) {
        return (x - ax) * (x - ax) + (y - ay) * (y - ay) <= generation_radius * generation_radius;
    }

    void addSection (int x, int y) {
        string key = Dungeon_Section.getKey(x, y);
        if (!sections.ContainsKey(key)) {
            sections[key] = new Dungeon_Section (x, y, this);
        }
    }

    public GameObject get_hall () {
        return new System.Random().Next(2) == 0 ? hall_1 : hall_2;
    }

    public GameObject get_closed_wall () {
        return new System.Random().Next(2) == 0 ? closed_wall_1 : closed_wall_2;
    }

    public GameObject get_inner_wall () {
        return new System.Random().Next(2) == 0 ? center_wall_1 : center_wall_2;
    }

    public GameObject get_open_wall () {
        return new System.Random().Next(2) == 0 ? open_wall_1 : open_wall_2;
    }

    public class Dungeon_Section {

        public int x;
        public int y;

        private List<GameObject> components;
        private List<GameObject> enemies;

        public Dungeon_Section (int x, int y, Dungeon_Generator gen) {

            this.x = x;
            this.y = y;

            enemies = new List<GameObject>();
            components = new List<GameObject>();

            Vector3 position = new Vector3 (x * 24, 0, y * 24);
            
            components.Add(Instantiate (gen.base_floor, position, Quaternion.Euler(0, 0, 0)));
            components.Add(Instantiate (gen.roof, position, Quaternion.Euler(-90, 0, 0)));

            if (!(x == 0 && y == 0)) {
                enemies.Add(Instantiate (gen.enemy, position + new Vector3(1.5f, 0, 1.5f), Quaternion.identity));
                enemies.Add(Instantiate (gen.enemy, position + new Vector3(-1.5f, 0, -1.5f), Quaternion.identity));
            }

            double theta;
            int addX, addY;

            for (int i = 0; i < 4; i++) {
                theta = i * (Math.PI / 2);

                addY = (int) -Math.Cos(theta);
                addX = (int) -Math.Sin(theta);

                components.Add(Instantiate (gen.get_open_wall(), position + new Vector3(addX * 6, 0, addY * 6), Quaternion.Euler(-90, i * 90, 0)));

                if (!gen.sections.ContainsKey(getKey(x+addX, y+addY))) {
                    components.Add(Instantiate (gen.get_hall(), position + new Vector3(addX * 12, 0, addY * 12), Quaternion.Euler(-90, i * 90, 0)));
                }

            }

            System.Random rand = new System.Random();
            
            components.Add(Instantiate(gen.center_column, position + new Vector3(2.5f, 0, 2.5f), Quaternion.Euler(-90, rand.Next(4) * 90, 0)));
            if (rand.Next(2) == 0) components.Add(Instantiate(gen.inner_arch, position + new Vector3(2.5f, 0, 4.25f), Quaternion.Euler(-90, 90, 0)));
            if (rand.Next(2) == 0) components.Add(Instantiate(gen.inner_arch, position + new Vector3(4.25f, 0, 2.5f), Quaternion.Euler(-90, 0, 0)));
            components.Add(Instantiate(gen.center_column, position + new Vector3(-2.5f, 0, 2.5f), Quaternion.Euler(-90, rand.Next(4) * 90, 0)));
            if (rand.Next(2) == 0) components.Add(Instantiate(gen.inner_arch, position + new Vector3(-2.5f, 0, 4.25f), Quaternion.Euler(-90, 90, 0)));
            if (rand.Next(2) == 0) components.Add(Instantiate(gen.inner_arch, position + new Vector3(-4.25f, 0, 2.5f), Quaternion.Euler(-90, 0, 0)));
            components.Add(Instantiate(gen.center_column, position + new Vector3(2.5f, 0, -2.5f), Quaternion.Euler(-90, rand.Next(4) * 90, 0)));
            if (rand.Next(2) == 0) components.Add(Instantiate(gen.inner_arch, position + new Vector3(2.5f, 0, -4.25f), Quaternion.Euler(-90, 90, 0)));
            if (rand.Next(2) == 0) components.Add(Instantiate(gen.inner_arch, position + new Vector3(4.25f, 0, -2.5f), Quaternion.Euler(-90, 0, 0)));
            components.Add(Instantiate(gen.center_column, position + new Vector3(-2.5f, 0, -2.5f), Quaternion.Euler(-90, rand.Next(4) * 90, 0)));
            if (rand.Next(2) == 0) components.Add(Instantiate(gen.inner_arch, position + new Vector3(-2.5f, 0, -4.25f), Quaternion.Euler(-90, 90, 0)));
            if (rand.Next(2) == 0) components.Add(Instantiate(gen.inner_arch, position + new Vector3(-4.25f, 0, -2.5f), Quaternion.Euler(-90, 0, 0)));


            int randRot = rand.Next(4);
            theta = randRot * (Math.PI / 2);
            addY = (int) -Math.Cos(theta);
            addX = (int) -Math.Sin(theta);

            switch (rand.Next(5)) {
                case 0:
                    components.Add(Instantiate(gen.get_inner_wall(), position + new Vector3(addX * 2.5f, 0, addY * 2.5f), Quaternion.Euler(-90, randRot * 90, 0)));
                    components.Add(Instantiate(gen.get_inner_wall(), position + new Vector3(addX * -2.5f, 0, addY * -2.5f), Quaternion.Euler(-90, -randRot * 90, 0)));
                    break;
                case 1:
                    components.Add(Instantiate(gen.get_inner_wall(), position + new Vector3(addX * 2.5f, 0, addY * 2.5f), Quaternion.Euler(-90, randRot * 90, 0)));
                    break;
                case 2:
                    components.Add(Instantiate(gen.get_inner_wall(), position + new Vector3(addX * 2.5f, 0, addY * 2.5f), Quaternion.Euler(-90, randRot * 90, 0)));
                    components.Add(Instantiate(gen.get_inner_wall(), position + new Vector3(addY * 2.5f, 0, addX * 2.5f), Quaternion.Euler(-90, randRot * 90 + 90, 0)));
                    break;
                default:
                    break;
            }

        }

        public void destroy () {
            for (int i = 0; i < components.Count; i++) {
                Destroy(components[i]);
            }
            components.Clear();
        }

        public string get_key () {
            return x.ToString() + " " + y.ToString();
        }

        public static string getKey (int x, int y) {
            return x.ToString() + " " + y.ToString();
        }

    }

}
