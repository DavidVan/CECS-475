using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cecs475.Pokemon.Core
{

    public class Pokemon
    {

        private static Random mRandom = new Random();

        public Pokemon(int level, double hitPoints, int attackPoints, int defensePoints)
        {
            Level = level;
            HitPoints = hitPoints;
            AttackPoints = attackPoints;
            DefensePoints = defensePoints;
        }

        public int Level
        {
            get; private set;
        }

        public double HitPoints
        {
            get; private set;
        }

        public int AttackPoints
        {
            get; private set;
        }

        public int DefensePoints
        {
            get; private set;
        }

        public static void SeedRandom(int seed)
        {
            mRandom = new Random(seed);
        }

        public void AttackTarget(int power, Pokemon pokemon)
        {
            double modifier = (mRandom.NextDouble() * (1 - 0.85)) + 0.85;
            double damage = (((2.0 * Level + 10) / 250) * ((double) AttackPoints / pokemon.DefensePoints) * power + 2) * modifier;
            pokemon.HitPoints -= damage;
        }

    }

}
