#region

using LuckyDice.custom.network;

#endregion

namespace LuckyDice.custom.items.dice
{
    public class D20 : DiceItem
    {
        public override void Start()
        {
            base.Start();
            outcomes.Add(Event.MaskedChaos);
            // outcomes.Add(Event.SpawnGoldBarForAll);
            // outcomes.Add(Event.SpawnGoldBar);
            // outcomes.Add(Event.SpawnGoldBar);
            // outcomes.Add(Event.SpawnPickleJar);
            // outcomes.Add(Event.SpawnPickleJar);
            // outcomes.Add(Event.HolyJihad);
            // outcomes.Add(Event.SpawnCoilhead);
            // outcomes.Add(Event.SpawnCoilhead);
            // outcomes.Add(Event.SpawnJester);
            // outcomes.Add(Event.SpawnJester);
            // outcomes.Add(Event.SpawnFlowerman);
            // outcomes.Add(Event.SpawnMasked);
            // outcomes.Add(Event.SpawnMasked);
            // outcomes.Add(Event.RandomizeLocks);
            // outcomes.Add(Event.RandomizeLocks);
            /*
             * 1/15 chance to get a gold bar for all
             * 2/15 chance to get a gold bar
             * 2/15 chance to get a pickle jar
             * 1/15 chance to get holy jihad
             * 2/15 chance to get a coilhead
             * 2/15 chance to get a jester
             * 1/15 chance to get a flowerman
             * 2/15 chance to get a masked player
             * 2/15 chance to randomize locks
             */
        }
    }
}