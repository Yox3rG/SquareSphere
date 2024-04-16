using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Testing
{
    public class Test : MonoBehaviour
    {
        FruitWrapper wrapper = new FruitWrapper();
        FruitWrapperArrayList wrapper2 = new FruitWrapperArrayList();

        readonly string testFile = "fruits.json";

        void Start()
        {
            //FillFruits();

            //TestSave(wrapper);
            //TestTestSave(new List<int>(new int[5] { 1, 2, 3, 4, 5 }));

            //SaveLoadHandlerJSON<GreenApple>.Save(new GreenApple(), testFile);

            //DebugFruits(wrapper);

            FillFruitsArrayList();
            DebugFruits(wrapper2);

            //SaveLoadHandlerJSON<FruitWrapperArrayList>.Save(wrapper2, testFile);

        }

        private void DebugFruits(FruitWrapper wrapper)
        {
            foreach (Fruit fruit in wrapper.fruits)
            {
                fruit.DebugData();
            }
        }

        private void DebugFruits(FruitWrapperArrayList wrapper)
        {
            foreach (Fruit fruit in wrapper.fruits)
            {
                fruit.DebugData();
            }
        }

        private void FillFruits()
        {
            wrapper.fruits.Add(new Fruit());
            wrapper.fruits.Add(new Apple());
            wrapper.fruits.Add(new GreenApple());
            wrapper.fruits.Add(new WaterMelon());
        }

        private void FillFruitsArrayList()
        {
            wrapper2.fruits.Add(new Fruit());
            wrapper2.fruits.Add(new Apple());
            wrapper2.fruits.Add(new GreenApple());
            wrapper2.fruits.Add(new WaterMelon());
        }

        private void TestSave(FruitWrapper fruits)
        {
            SaveLoadHandlerJSON<FruitWrapper>.Save(fruits, testFile);
        }

        private void TestTestSave(List<int> ints)
        {
            SaveLoadHandlerJSON<List<int>>.Save(ints, testFile);
        }

        private List<Fruit> TestLoad()
        {
            return new List<Fruit>();
            //return SaveLoadHandlerJSON<List<Fruit>>.Load(testFile);
        }

    }

    [System.Serializable]
    public class FruitWrapper
    {
        public List<Fruit> fruits = new List<Fruit>();
    }

    [System.Serializable]
    public class FruitWrapperArrayList
    {
        public ArrayList fruits = new ArrayList();
    }

    [System.Serializable]
    public class Fruit
    {
        public int row, col;

        public Fruit()
        {
            row = 10;
            col = 10;
        }

        public virtual void DebugData()
        {
            Debug.Log("Row: " + row + " Col: " + col);
        }
    }

    [System.Serializable]
    public class Apple : Fruit
    {
        public int maxHp;
        public Color color;

        public Apple() : base()
        {
            maxHp = 15;
            color = Color.cyan;
        }

        public override void DebugData()
        {
            base.DebugData();
            Debug.Log(" MaxHP: " + maxHp + " Color: " + color);
        }
    }

    [System.Serializable]
    public class GreenApple : Apple
    {
        public int orientation;

        public GreenApple() : base()
        {
            orientation = 2;
        }

        public override void DebugData()
        {
            base.DebugData();
            Debug.Log(" Orientation: " + orientation);
        }
    }

    [System.Serializable]
    public class WaterMelon : Fruit
    {
        public PowerUp.ElementType type;

        public WaterMelon() : base()
        {
            type = PowerUp.ElementType.POWERUP_DINO;
        }

        public override void DebugData()
        {
            base.DebugData();
            Debug.Log(" Type: " + type);
        }
    }
}
