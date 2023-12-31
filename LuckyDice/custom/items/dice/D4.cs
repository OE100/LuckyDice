#region

using LuckyDice.custom.network;

#endregion

namespace LuckyDice.custom.items.dice
{
    public class D4 : DiceItem
    {
        public override void Start()
        {
            base.Start();
            outcomes.Add(Event.SpawnGoldBar);
            // outcomes.Add(Event.SpawnClownHorn);
            // outcomes.Add(Event.SpawnClownHorn);
            // outcomes.Add(Event.RandomizeLocks);
            // outcomes.Add(Event.SpawnCentipede);
            // outcomes.Add(Event.SpawnCentipede);
            // outcomes.Add(Event.SpawnFlowerman);
            // outcomes.Add(Event.SpawnClownHorn);
            // outcomes.Add(Event.SpawnClownHorn);
            // outcomes.Add(Event.Bleed);
            // outcomes.Add(Event.Bleed);
            // outcomes.Add(Event.Bleed);
            // outcomes.Add(Event.MaskedChaos);
            /*
             * 1/11 chance to spawn gold bar
             * 1/11 chance to randomize locks
             * 2/11 chance to spawn centipede
             * 1/11 chance to spawn flowerman
             * 2/11 chance to spawn clown horn
             * 3/11 chance to bleed
             * 1/11 chance to spawn masked chaos
             */
        }
    }
}