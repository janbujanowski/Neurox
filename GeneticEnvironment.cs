﻿using Newtonsoft.Json;
using srodowisko;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class GeneticEnvironment
    {
        public uint COUNTVALUESTOMAP = UInt32.MaxValue;
        public double VALUESRANGE = 2 - (-2);
        public int POPULATIONSIZE = 20;
        public int POPULATIONCOUNTLIMIT = 1000;
        public double MUTATIONPROBABILITY = 0.15;
        public int NUMBEROFEVOLUTIONTRIALS = 1;
        public int ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = 5000;
        public double ModyfikatorWyniku = 1;

        public int[] BestGenotype { get; private set; }
        public SelectionMethods SelectionMethod = SelectionMethods.Roulette;
        public CrossoverMethods CrossoverMethod = CrossoverMethods.OX;
        public int MUTATIONRETRIALS = 4;
        StockMarketEvaluator problemToResolve;
        ILogger _logger;

        public int[] GetRandomParametersArray(int nrProblemu)
        {
            int[] genotype = new int[problemToResolve.ProblemSize(nrProblemu)];
            for (int i = 0; i < problemToResolve.ProblemSize(nrProblemu); i++)
            {
                genotype[i] = CUBE.Next(-1000, 1000);
            }
            return genotype;
        }
        private static GeneticEnvironment _config;
        public static GeneticEnvironment INSTANCE
        {
            get
            {
                if (_config == null)
                {
                    _config = new GeneticEnvironment(IoCFactory.Resolve<ILogger>());
                }
                return _config;
            }
        }
        private GeneticEnvironment(ILogger loggerInstance)
        {
            problemToResolve = new StockMarketEvaluator(new DirectFileLogger());
            _logger = loggerInstance;
        }
        private static Random _CUBE;
        public static Random CUBE
        {
            get
            {
                if (_CUBE == null)
                {
                    _CUBE = new Random();
                }
                return _CUBE;
            }
        }
        public DateTime StopDate { get; private set; }
        public int NrProblemu { get; internal set; }
        public DateTime StartDate { get; internal set; }

        public static double SurvivalFunction(int[] sciezka)
        {
            return INSTANCE.problemToResolve.Ocena(sciezka);
        }

        internal void ParseParameters(string[] args)
        {
            try
            {
                INSTANCE.StopDate = DateTime.Parse(ConfigurationManager.AppSettings["StopDate"]);
                //INSTANCE.POPULATIONSIZE = Int32.Parse(args[1]);
                INSTANCE.MUTATIONPROBABILITY = double.Parse(ConfigurationManager.AppSettings["MUTATIONPROBABILITY"]);
                INSTANCE.MUTATIONRETRIALS = Int32.Parse(ConfigurationManager.AppSettings["MUTATIONRETRIALS"]);
                INSTANCE.SelectionMethod = (SelectionMethods)Enum.Parse(typeof(SelectionMethods), ConfigurationManager.AppSettings["SelectionMethod"]);
                INSTANCE.CrossoverMethod = (CrossoverMethods)Enum.Parse(typeof(CrossoverMethods), ConfigurationManager.AppSettings["CrossoverMethod"]);
                //INSTANCE.ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = Int32.Parse(args[6]);
                INSTANCE.ModyfikatorWyniku = double.Parse(ConfigurationManager.AppSettings["ScoreModifier"]);//1 lub -1 zaleznie od rodzaju problemu maksymalizacji/minimalizacji
                //INSTANCE.NrProblemu = Int32.Parse(args[8]);//numer zbioru
                var dataStopuOdMinut = DateTime.Now.AddMinutes(double.Parse(ConfigurationManager.AppSettings["MinutesLimit"]));
                if (dataStopuOdMinut < StopDate)
                {
                    INSTANCE.StopDate = dataStopuOdMinut;
                }
                Console.WriteLine($"DATA STARTU |{DateTime.Now.ToString()}|");
                Console.WriteLine($"DATA ZAKONCZENA |{INSTANCE.StopDate.ToString()}|");
                Console.WriteLine($"MUTATIONPROBABILITY : |{INSTANCE.MUTATIONPROBABILITY.ToString()}|");
                Console.WriteLine($"MUTATIONRETRIALS : |{INSTANCE.MUTATIONRETRIALS.ToString()}|");
                if (File.Exists(ConfigurationManager.AppSettings["StartingIndividual"]))
                {
                    var jsonContent = File.ReadAllText(ConfigurationManager.AppSettings["StartingIndividual"]);
                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        INSTANCE.BestGenotype = JsonConvert.DeserializeObject<Individual>(jsonContent).genotype;
                    }
                }
                _logger.LogInfo($"DATA STARTU |{DateTime.Now.ToString()}|");
                _logger.LogInfo($"DATA ZAKONCZENA |{INSTANCE.StopDate.ToString()}|");
                _logger.LogInfo($"MUTATIONPROBABILITY : |{INSTANCE.MUTATIONPROBABILITY.ToString()}|");
                _logger.LogInfo($"MUTATIONRETRIALS : |{INSTANCE.MUTATIONRETRIALS.ToString()}|");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "During parsing parameters");
            }
        }
    }
}
