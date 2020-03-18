using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class EventManager
    {
        private readonly SystemManager systemManager;

        public EventManager(SystemManager systemManager)
        {
            this.systemManager = systemManager;
        }

        public void Crawl()
        {
            Type systemType;
            for (int i = 0; i < systemManager.TotalSystemsRegistered; i++)
            {
                if (systemManager.Systems[i] is IEventAnnouncer)
                {
                    for (int j = 0; j < systemManager.TotalSystemsRegistered; j++)
                    {
                        if (i != j && systemManager.Systems[j] is IEventListener)
                        {
                            systemType = systemManager.Systems[i].GetType();

                            if (systemManager.Systems[j].Subscriptions.Contains(systemType))
                            {
                                ((IEventAnnouncer)systemManager.Systems[i]).Announcement += ((IEventListener)systemManager.Systems[j]).HandleEvent;
                            }
                        }
                    }
                }
            }
        }
    }
}
