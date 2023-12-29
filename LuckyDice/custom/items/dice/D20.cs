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
            outcomes.Add(Event.HolyJihad);
        }
    }
}