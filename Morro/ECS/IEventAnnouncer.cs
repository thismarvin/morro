using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    interface IEventAnnouncer
    {
        EventHandler<EventArgs> Announcement { get; set; }
    }

    static class IEventAnnouncerHelper
    {
        public static void AnnounceEvent(this IEventAnnouncer announcer)
        {
            announcer.Announcement?.Invoke(null, EventArgs.Empty);
        }
    }
}
