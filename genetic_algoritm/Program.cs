using System;
using System.Collections.Generic;
using System.IO;
namespace genetic_algoritm
{
    class Item
    {
        public static int COUNTOFITEMS { get; private set; } = 100;
        public int weight { get; private set; }
        public int cost { get; private set; }
        private Random random = new Random();
        public Item()
        {
            weight = random.Next(1, 11);
            cost = random.Next(2, 21);
        }
        public static Item[] GenerateArray()
        {
            Item[] items = new Item[COUNTOFITEMS];
            for (int i = 0; i < COUNTOFITEMS; i++)
            {
                items[i] = new Item();
            }
            return items;
        }
        public static string ToString(Item[] items)
        {
            string a = "";
            for (int i = 0; i < COUNTOFITEMS; i++)
            {
                a += $"{items[i].weight}; {items[i].cost} \n";
            }
            return a;
        }
    }

    class Population
    {
        private readonly int COUNTOFPOPULATION = 100;
        private readonly int CROSSINGFACTOR = 50;
        private readonly int CAPACITY = 250;
        private readonly int MUTATIONPROBABILITY = 5;// in range 0 to 100
        private readonly int ITERATIONS = 1000;

        private Item[] items;
        private bool[,] population;
        public Population(Item[] items)
        {
            this.items = items;
            population = new bool[COUNTOFPOPULATION, Item.COUNTOFITEMS];
        }
        public void Start()
        {
            Console.WriteLine($"Itetation \t Weight \t Cost");
            InitPopulation();
            for (int i = 0; i < ITERATIONS; i++)
            {
                int[] changed = Crossing();
                if (changed[0] != -1)
                {
                    Mutatuion(changed[0]);
                    LocalImprovement(changed[0]);
                }
                if (changed[1] != -1)
                {
                    Mutatuion(changed[1]);
                    LocalImprovement(changed[1]);
                }
                if ((i + 1) % 20 == 0)
                {
                    Console.WriteLine($"{i + 1,7 } {WeightOfSetInMartix(SetWithMaxCost()),13} \t {CostOfSetInMartix(SetWithMaxCost()),11}");
                    //Console.WriteLine($"Itetation: {i} \t  Weight: {WeightOfSetInMartix(VertexWithMaxCost())} \t Cost: {CostOfSetInMartix(VertexWithMaxCost())}");
                }
            }

        }
        private void InitPopulation()
        {
            for (int i = 0; i < COUNTOFPOPULATION; i++)
            {
                for (int j = 0; j < Item.COUNTOFITEMS; j++)
                {
                    if (i == j)
                        population[i, j] = true;
                    else
                        population[i, j] = false;
                }
            }
        }
        private int[] Crossing()
        {
            int[] changed = { -1, -1 };
            int maxCost = SetWithMaxCost();
            int random = RandomPopulation(maxCost);
            bool[] first = MatrixToSet(maxCost);
            bool[] second = MatrixToSet(random);
            bool[] firstChild = GetChild(first, second);
            bool[] secondChild = GetChild(second, first);
            int firstChildWeight = WeightOfSet(firstChild);
            int secondChildWeight = WeightOfSet(secondChild);
            int less = SetWithMinWeight();
            if (CanInsert(firstChildWeight, less))
            {
                SetToMatrix(firstChild, less);
                changed[0] = less;
            }
            less = SetWithMinWeight();
            if (CanInsert(secondChildWeight, less))
            {
                SetToMatrix(secondChild, less);
                changed[1] = less;
            }

            return changed;
        }
        private bool CanInsert(int weight, int less)
        {
            if (weight <= CAPACITY && weight >= WeightOfSetInMartix(less))
            {
                return true;
            }
            return false;
        }
        private bool[] GetChild(bool[] first, bool[] second)
        {
            bool[] child = new bool[Item.COUNTOFITEMS];
            for (int i = 0; i < COUNTOFPOPULATION; i++)
            {
                if (CROSSINGFACTOR <= i)
                    child[i] = first[i];
                else
                    child[i] = second[i];
            }
            return child;
        }
        private void Mutatuion(int changed)
        {
            Random rand = new Random();
            int mutation = rand.Next(101);
            int gen = rand.Next(Item.COUNTOFITEMS);
            if (mutation <= MUTATIONPROBABILITY)
            {
                if (population[changed, gen])
                    population[changed, gen] = false;
                else
                    population[changed, gen] = true;
            }
            if (WeightOfSetInMartix(changed) <= CAPACITY)
            {
                return;
            }
            else
            {
                if (population[changed, gen])
                    population[changed, gen] = false;
                else
                    population[changed, gen] = true;
            }
        }
        private int SetWithMinWeight()
        {
            int minWeight = int.MaxValue;
            int vertex = -1;
            for (int i = 0; i < COUNTOFPOPULATION; i++)
            {
                int tmp = WeightOfSetInMartix(i);
                if (minWeight > tmp)
                {
                    minWeight = tmp;
                    vertex = i;
                }
            }

            return vertex;
        }
        private int WeightOfSet(bool[] itemSet)
        {
            int weightOfSet = 0;
            for (int i = 0; i < Item.COUNTOFITEMS; i++)
            {
                if (itemSet[i] == true)
                {
                    weightOfSet += items[i].weight;
                }
            }
            return weightOfSet;
        }
        private int WeightOfSetInMartix(int numOfSet)
        {
            int weightOfSet = 0;
            for (int i = 0; i < Item.COUNTOFITEMS; i++)
            {
                if (population[numOfSet, i] == true)
                {
                    weightOfSet += items[i].weight;
                }
            }
            return weightOfSet;
        }
        private int SetWithMaxCost()
        {
            int maxCost = 0;
            int vertex = -1;
            for (int i = 0; i < COUNTOFPOPULATION; i++)
            {
                int tmp = CostOfSetInMartix(i);
                if (maxCost < tmp)
                {
                    maxCost = tmp;
                    vertex = i;
                }
            }
            return vertex;
        }
        private int CostOfSet(bool[] itemSet)
        {
            int costOfSet = 0;
            for (int i = 0; i < Item.COUNTOFITEMS; i++)
            {
                if (itemSet[i] == true)
                {
                    costOfSet += items[i].cost;
                }
            }
            return costOfSet;
        }
        private int CostOfSetInMartix(int numOfSet)
        {
            int costOfSet = 0;
            for (int i = 0; i < Item.COUNTOFITEMS; i++)
            {
                if (population[numOfSet, i] == true)
                {
                    costOfSet += items[i].cost;
                }
            }
            return costOfSet;
        }
        private bool[] MatrixToSet(int numOfSet)
        {
            bool[] result = new bool[Item.COUNTOFITEMS];
            for (int i = 0; i < COUNTOFPOPULATION; i++)
            {
                result[i] = population[numOfSet, i];
            }
            return result;
        }
        private int RandomPopulation(int numOfSet)
        {
            int result;
            Random rand = new Random();
            do
            {
                result = rand.Next(COUNTOFPOPULATION);
            } while (result == numOfSet);
            return result;
        }
        private void LocalImprovement(int changed)
        {
            int minWeight = int.MaxValue;
            for (int i = 0; i < Item.COUNTOFITEMS; i++)
            {
                if (population[changed, i] == false)
                {
                    if (items[i].weight < minWeight)
                        minWeight = items[i].weight;
                }
            }
            Stack<int> maxCostMinWeight = new Stack<int>();
            for (int i = 0; i < Item.COUNTOFITEMS; i++)
            {
                if (population[changed, i] == false && items[i].weight == minWeight)
                    maxCostMinWeight.Push(i);
            }
            int BestVertex = -1;
            int maxCost = 0;

            while (maxCostMinWeight.Count != 0)
            {
                int tmp = maxCostMinWeight.Pop();
                if (items[tmp].cost > maxCost)
                {
                    maxCost = items[tmp].cost;
                    BestVertex = tmp;
                }
            }
            if (BestVertex != -1)
            {
                bool[] temp = MatrixToSet(changed);
                temp[BestVertex] = true;
                if (WeightOfSet(temp) <= CAPACITY)
                    population[changed, BestVertex] = true;
            }
        }
        private void SetToMatrix(bool[] set, int numOfSet)
        {
            for (int i = 0; i < Item.COUNTOFITEMS; i++)
            {
                population[numOfSet, i] = set[i];
            }
        }
        public override string ToString()
        {
            bool[] arr = MatrixToSet(SetWithMaxCost());
            string a = "";
            a += $"weight \t cost \n";
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == true)
                {
                    a += $"{items[i].weight,3}  {items[i].cost,7} \n";
                }
            }
            a += $"total weight: {WeightOfSet(arr)}, total cost: {CostOfSet(arr)} \n";
            return a;
        }
        public void SaveToFile(string fileName)
        {
            StreamWriter sw = new StreamWriter(fileName);
            string a = "";
            a += ToString();
            sw.WriteLine(a);
            sw.Close();
            StreamWriter sw2 = new StreamWriter("Items" + fileName);
            sw2.WriteLine(Item.ToString(items));
            sw2.Close();
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("------------------------");
            Item[] items;
            items = Item.GenerateArray();
            Console.WriteLine(Item.ToString(items));
            Population b = new Population(items);
            b.Start();
            Console.WriteLine("------------------------");
            Console.WriteLine("Your solution: ");
            Console.WriteLine(b.ToString());
            Console.WriteLine("------------------------");
            Console.WriteLine("Do you want to save result to file? \n 1. Yes \n 2. No ");
            int a = Convert.ToInt32(Console.ReadLine());
            switch (a)
            {
                case 1:
                    Console.WriteLine("Enter file name:");
                    string fileName = Console.ReadLine();
                    b.SaveToFile(fileName);
                    Console.WriteLine($"Result saved to {fileName}");
                    Console.WriteLine($"Set of items saved to {"Items" + fileName}");
                    break;
                case 2:
                    Console.WriteLine("Ending program");
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    return;
            }
        }
    }
}
