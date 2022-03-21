using System;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces
{
    [Flags]
    public enum MessageDirection
    {
        In = 0b_0001,
        Out = 0b_0010,
        Both = In | Out
    }
}