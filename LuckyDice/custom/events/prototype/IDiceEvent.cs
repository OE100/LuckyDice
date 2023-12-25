using System.Collections;
using GameNetcodeStuff;

namespace LuckyDice.custom.events.prototype
{
    public interface IDiceEvent 
    {
        void AddPlayer(PlayerControllerB player);
        void RemovePlayer(PlayerControllerB player);
        void Run();
        void Stop();
        IEnumerator EventCoroutine();
    }
}