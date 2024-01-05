using GameNetcodeStuff;

namespace LuckyDice.custom.monobehaviour.def
{
    public abstract class BaseSpawnItemForAllEvent : BaseSpawnItemEvent
    {
        public override void AddPlayer(PlayerControllerB player)
        {
            foreach (PlayerControllerB p in StartOfRound.Instance.allPlayerScripts)
                base.AddPlayer(p);
        }
    }
}