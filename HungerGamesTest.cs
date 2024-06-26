﻿using Arena;
using ArenaVisualizer;
using DongUtility;
using GraphData;
using HungerGames.Animals;
using HungerGames.Interface;
using HungerGamesCore.Interface;
using NeuralNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using static NeuralNet.Node;

namespace HungerGames
{
    class HungerGamesTest
    {
        private const double hareToLynxRatio = 10;

        private const int nLynx = 10;
        private const int nHare = (int)(nLynx * hareToLynxRatio);

        private const int arenaHeight = 50;
        private const int arenaWidth = 50;

        private const double maxTime = 30; // only thing u should / can change (make it shorter? -> 15)
        private const double timeStep = .01;

        public static void Run()
        {
            WPFUtility.ConsoleManager.ShowConsoleWindow();
            //TrainPerceptrons();
            Display();
        }

        public static void Display()
        {
            var arena = new HungerGamesArena(arenaWidth, arenaHeight);

            var master = new GameMaster(arena);

            master.AddChooser(new ChooserMichelleSun());
            master.AddChooser(new ChooserDefault());

            master.DefaultIsNonPerceptron = true;

            master.AddAllAnimals(nHare, nLynx);

            var sim = new HungerGamesTestWindow(arena);

            sim.Manager.AddLeaderBoard(GetLeaderBars(master, true),
                () => GetLeaderBoardScores(arena, master));
            sim.Manager.AddLeaderBoard(GetLeaderBars(master, false),
                () => GetLynxScores(arena, master));

            sim.Show();
        }

        public static void TrainPerceptrons()
        {
            List<Perceptron> pList_lynx = new List<Perceptron>();
            List<Perceptron> p_nextRound_lynx = new List<Perceptron>();

            const int nTries = 50;
            double longestTime = 0;
            double shortestTime = double.MaxValue;
            int mostHares = 0;
            int fewestHares = int.MaxValue;

            Boolean toContinue = true;

            Perceptron bestLynx = null;
            Perceptron bestHare = null;

            Random r = new Random();

            var harePerceptron = Perceptron.ReadFromFile(FileUtilities.GetMainProjectDirectory() + "Perceptrons/MichelleSunHarePerceptron(3)-trainedOffLynx(round_1).pcp");
            //var harePerceptron = new Perceptron(8, 2);

            // Train the perceptron
            for (int i = 0; i < nTries; ++i)
            {
                var probability = r.NextDouble();
                //var harePerceptron = new Perceptron(8, 2);
                //=========================================================
                var lynxPerceptron = new Perceptron(13, 2);
                //var lynxPerceptron = Perceptron.ReadFromFile(FileUtilities.GetMainProjectDirectory() + "Perceptrons/MichelleSunLynxPerceptron(3)-trainedOffHare(round_3).pcp");
                //=========================================================
                //var lynxPerceptron = new Perceptron(8, 2, 5, ActivationFunctionChoice.Sigmoid);
                if (probability < 0.25)
                {
                    //harePerceptron.RandomWeights(5);

                    //lynxPerceptron.RandomWeights(55);
                    //Console.WriteLine("Random weights: 55");

                    lynxPerceptron.RandomWeights(5);
                    Console.WriteLine("Random weights: 5");
                }
                else if (probability > 0.25 && probability < 0.5)
                {
                    //harePerceptron.RandomWeights(17);
                    lynxPerceptron.RandomWeights(17);
                    Console.WriteLine("Random weights: 17");
                }
                else if (probability > 0.5 && probability < 0.75)
                {
                    //harePerceptron.RandomWeights(80);
                    lynxPerceptron.RandomWeights(58);
                    Console.WriteLine("Random weights: 58");
                }
                else
                {
                    //harePerceptron.RandomWeights(130);
                    lynxPerceptron.RandomWeights(90);
                    Console.WriteLine("Random weights: 90");
                }

                Console.WriteLine("before RunArena()");

                var (time, nHares) = RunArena(harePerceptron, lynxPerceptron);
                if (time < shortestTime || (time == shortestTime && nHares < fewestHares))
                {
                    bestLynx = lynxPerceptron;
                    shortestTime = time;
                    fewestHares = nHares;

                    pList_lynx.Add(lynxPerceptron.Clone());
                }
                //if (time > longestTime || (time == longestTime && nHares > mostHares))
                //{
                //    bestHare = harePerceptron;
                //    longestTime = time;
                //    mostHares = nHares;
                //}

                Console.WriteLine("On i = " + i + " => Time: " + time);
            }
            Console.WriteLine("Finished first round of random weights. Best hares: " + fewestHares + ". pList size: " + pList_lynx.Count);

            //make new perceptrons by random mutations from previous best
            Random random = new Random();
            while (toContinue)
            {
                foreach (var p in pList_lynx)
                {
                    for (int i = 0; i < 10; i++) //create 50 new perceptrons per p in pList
                    {
                        double probability = random.NextDouble();
                        double probability2 = random.NextDouble();
                        if (probability < 0.1)
                        {
                            p.RandomWeights(50);
                        }
                        else if (probability < 0.3)
                        {
                            p.RandomWeights(96.5);
                        }
                        else if (probability < 0.5)
                        {
                            p.RandomWeights(140.5);
                        }
                        else if (probability < 0.7)
                        {
                            p.RandomWeights(190.5);
                        }
                        else
                        {
                            p.RandomWeights(298.5);
                        }

                        p_nextRound_lynx.Add(p.Clone());
                    }
                }
                Console.WriteLine("Made new perceptrons by random mutations. p_nextRound size: " + p_nextRound_lynx.Count);

                pList_lynx.Clear();
                int counter = 0;
                foreach (var p in p_nextRound_lynx)
                {
                    //double timeToDieAvg = 0;

                    //for (int j = 0; j < 3; j++)
                    //{
                        var (time, nHares) = RunArena(harePerceptron, p);
                    //}
                    Console.WriteLine("On perceptron number " + counter + ". Time: " + time + ". Hares: " + nHares);

                    if (time < shortestTime || (time == shortestTime && nHares < fewestHares))
                    {
                        shortestTime = time;
                        fewestHares = nHares;
                        pList_lynx.Add(p.Clone());
                        bestLynx = p.Clone();
                    }
                    counter++;
                }
                Console.WriteLine("Finished second round with mutations. Best hares: " + fewestHares + ". pList size: " + pList_lynx.Count);
                p_nextRound_lynx.Clear();

                //stop repeating when improvement stops
                if ((pList_lynx.Count() <= 0 || shortestTime <= 10) && bestLynx != null)
                {
                    Console.WriteLine("Improvement stopped.");
                    bestLynx.WriteToFile(FileUtilities.GetMainProjectDirectory() + "Perceptrons/MichelleSunLynxPerceptron(3)-trainedOffHare(round_8).pcp");
                    //bestHare.WriteToFile(FileUtilities.GetMainProjectDirectory() + "Perceptrons/MichelleSunHarePerceptron(3)-trainedOffLynx(round_3).pcp");
                    toContinue = false;
                }
            }
            ////if (bestLynx != null && bestHare != null)
            ////if (bestHare != null)
            //if (bestLynx != null)
            //{
            //    bestLynx.WriteToFile(FileUtilities.GetMainProjectDirectory() + "Perceptrons/MichelleSunLynxPerceptron(3)-trainedOffHare(round_2).pcp");
            //    //bestHare.WriteToFile(FileUtilities.GetMainProjectDirectory() + "Perceptrons/MichelleSunHarePerceptron(3)-trainedOffLynx(round_3).pcp");
            //}
        }

        public static Tuple<double, int> RunArena(Perceptron harePerceptron, Perceptron lynxPerceptron)
        {
            var arena = new HungerGamesArena(arenaWidth, arenaHeight);

            var master = new GameMaster(arena);

            master.AddChooser(new LocationChooserPerceptron<HareIntelligenceMichelleSun, LynxIntelligenceMichelleSun>
                (harePerceptron, lynxPerceptron));
            //master.AddChooser(new ChooserMichelleSun());
            master.AddChooser(new ChooserDefault());

            master.DefaultIsNonPerceptron = true;

            master.AddAllAnimals(nHare, nLynx, harePerceptron, lynxPerceptron);

            var hareName = master.Choosers[0].GetName(true);

            //Console.WriteLine($"Hares remaining: {arena.GetObjects(hareName).Count()}"); --> 100 hARES originally !

            while (arena.Continue && arena.Time < maxTime)
            {
                arena.Tick(arena.Time + timeStep);
            }

            Console.WriteLine($"Time: {arena.Time}  Hares remaining: {arena.GetObjects(hareName).Count()}");
            return new Tuple<double, int>( arena.Time, arena.GetObjects(hareName).Count());
        }

        static private List<LeaderBarPrototype> GetLeaderBars(GameMaster gm, bool hare)
        {
            var leaderBars = new List<LeaderBarPrototype>();
            foreach (var chooser in gm.Choosers)
            {
                var color = chooser.MakeOrganism(null, hare).Color;
                var bar = new LeaderBarPrototype(chooser.GetName(hare), color);
                leaderBars.Add(bar);
            }
            return leaderBars;
        }

        static private List<double> GetLeaderBoardScores(ArenaEngine arena, GameMaster gm)
        {
            var data = new List<double>();
            foreach (var chooser in gm.Choosers)
            {
                var list = arena.GetObjects(chooser.GetName(true));
                data.Add(list.Count());
            }
            return data;
        }

        static private List<double> GetLynxScores(ArenaEngine arena, GameMaster gm)
        {
            var data = new List<double>();
            foreach (var chooser in gm.Choosers)
            {
                var list = arena.GetObjects(chooser.GetName(false));
                var sum = list.Sum((x) => ((Lynx)x).HaresEaten);
                data.Add(sum);
            }
            return data;
        }
    }
}
