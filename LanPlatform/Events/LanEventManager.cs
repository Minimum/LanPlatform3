using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using LanPlatform.Accounts;
using LanPlatform.DAL;
using LanPlatform.Engine;
using LanPlatform.Models;
using LanPlatform.Settings;

namespace LanPlatform.Events
{
    public class LanEventManager
    {
        public const String FlagCreateEvent = "LanEventCreate";
        public const String FlagEditEvent = "LanEventEdit";
        public const String FlagDeleteEvent = "LanEventDelete";

        public const String FlagCreateGuest = "LanEventGuestCreate";
        public const String FlagEditGuest = "LanEventGuestEdit";
        public const String FlagDeleteGuest = "LanEventGuestDelete";

        public const String SettingCurrentEvent = "CurrentEvent";

        protected PlatformContext Context;

        protected AppInstance Instance;

        public LanEventManager(AppInstance instance)
        {
            Context = instance.Context;

            Instance = instance;
        }

        public void Install()
        {
            SettingsManager settings = Instance.Settings;

            if (settings.GetSettingByName(SettingCurrentEvent) == null)
            {
                PlatformSetting setting = new PlatformSetting();

                setting.Name = SettingCurrentEvent;
                setting.Title = "Current Event";
                setting.Description = "The current LAN event.";
                setting.Value = "0";

                settings.AddSetting(setting);
            }

            // Create tables
            LanEventGuest eventGuest = Context.LanEventGuest.SingleOrDefault(s => s.Id == 0);
            LanEvent lanEvent = Context.LanEvent.SingleOrDefault(s => s.Id == 0);

            return;
        }

        public static void PostAuthTasks(UserAccount account, AppInstance instance)
        {
            new LanEventManager(instance).PostAuth(account);

            return;
        }

        protected void PostAuth(UserAccount account)
        {
            // TODO: Remove this, create checkin API action

            PlatformSetting currentEvent = Instance.Settings.GetSettingByName(SettingCurrentEvent);

            long eventId = currentEvent.ToInt64();

            // Is there a current event?
            if (eventId > 0)
            {
                // Load event
                LanEvent party = GetEventById(eventId);

                // Is the event id valid?
                if (party != null)
                {
                    // Load event entry for account
                    LanEventGuest guestEntry = GetEventGuest(eventId, account.Id);

                    // Does the entry exist?
                    if (guestEntry == null)
                    {
                        // If entry does not exist, generate new entry
                        guestEntry = new LanEventGuest();

                        guestEntry.Account = account.Id;
                        guestEntry.Event = party.Id;
                        guestEntry.Arrived = EngineUtil.CurrentTime;

                        Context.LanEventGuest.Add(guestEntry);

                        // Increment account's events
                        account.TotalEvents++;
                        account.LastEvent = eventId;
                    }
                    else if (guestEntry.Arrived == 0)
                    {
                        // If entry is marked as invited but not arrived, set to arrived
                        guestEntry.Arrived = EngineUtil.CurrentTime;

                        // Increment account's events
                        account.TotalEvents++;
                        account.LastEvent = eventId;
                    }
                }
            }

            return;
        }

        // Events

        public LanEvent GetEventById(long id)
        {
            return Context.LanEvent.SingleOrDefault(s => s.Id == id);
        }

        public void AddEvent(LanEvent lanEvent)
        {
            Context.LanEvent.Add(lanEvent);

            return;
        }

        public void RemoveEvent(LanEvent lanEvent)
        {
            Context.LanEvent.Remove(lanEvent);

            return;
        }

        // Event Guests

        public List<LanEventGuest> GetEventGuests(LanEvent lanEvent)
        {
            return GetEventGuests(lanEvent.Id);
        }

        public List<LanEventGuest> GetEventGuests(long eventId)
        {
            return Context.LanEventGuest.Where(s => s.Event == eventId).ToList();
        }

        public LanEventGuest GetEventGuest(long eventId, long accountId)
        {
            return Context.LanEventGuest.SingleOrDefault(s => s.Account == accountId && s.Event == eventId);
        }

        public void AddEventGuest(LanEventGuest guest)
        {
            Context.LanEventGuest.Add(guest);

            return;
        }

        public void RemoveEventGuest(LanEventGuest guest)
        {
            Context.LanEventGuest.Remove(guest);

            return;
        }

    }
}