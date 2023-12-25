using LuckyDice.custom.network;

namespace LuckyDice.custom.items.dice
{
    public class D20 : DiceItem
    {
        public override void Start()
        {
            base.Start();
            outcomes.Add(Event.Bleed);
            outcomes.Add(Event.SpawnCoilhead);
            outcomes.Add(Event.SpawnFlowerman);
            outcomes.Add(Event.SpawnMasked);
            outcomes.Add(Event.SpawnJester);
            outcomes.Add(Event.SpawnCentipede);
        }
    }
}