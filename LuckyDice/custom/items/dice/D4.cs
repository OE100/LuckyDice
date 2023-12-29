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
            outcomes.Add(Event.RandomizeLocks);
            outcomes.Add(Event.SpawnCentipede);
            outcomes.Add(Event.SpawnCentipede);
            outcomes.Add(Event.SpawnFlowerman);
            outcomes.Add(Event.SpawnClownHorn);
            outcomes.Add(Event.SpawnClownHorn);
            outcomes.Add(Event.Bleed);
            outcomes.Add(Event.Bleed);
            outcomes.Add(Event.Bleed);
            /*
             * 1/10 chance to spawn gold bar
             * 1/10 chance to randomize locks
             * 2/10 chance to spawn centipede
             * 1/10 chance to spawn flowerman
             * 2/10 chance to spawn clown horn
             * 3/10 chance to bleed
             */
        }
    }
}