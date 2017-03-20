using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using FluentAssertions;
using Cecs475.Pokemon.Core; // Doesn't even work.

namespace Cecs475.Pokemon.Test
{
    public class PokemonTests
    {
        [Fact]
        public void AttackTest()
        {
            // We have to use Core.Pokemon, otherwise it thinks we're referencing Cecs475.Pokemon.
            // In reality, we want Cecs475.Pokemon.Core.Pokemon...
            // We could rename the namespace so that it doesn't include the word "Pokemon" in it.
            Core.Pokemon a = new Core.Pokemon(75, 201, 123, 181);
            Core.Pokemon b = new Core.Pokemon(78, 270, 210, 163);
            Core.Pokemon.SeedRandom(1);
            a.AttackTarget(80, b);
            b.HitPoints.Should().Be(233.94403567421205); // 270 - 36.05596432578796632147239263803680981
        }
    }
}
