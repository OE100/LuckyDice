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
            // todo: add more outcomes
        }
    }
}