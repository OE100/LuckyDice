using System.Collections;
using GameNetcodeStuff;

namespace LuckyDice.Patches.custom.events
{
    public interface DiceEvent 
    {
        void AddPlayer(PlayerControllerB player);
        void RemovePlayer(PlayerControllerB player);
        void Run();
        void Stop();
        IEnumerator EventCoroutine();
    }
}