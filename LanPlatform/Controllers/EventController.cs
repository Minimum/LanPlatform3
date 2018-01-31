using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using LanPlatform.Accounts;
using LanPlatform.DTO.Events;
using LanPlatform.Events;
using LanPlatform.Models;
using LanPlatform.Settings;
using Newtonsoft.Json;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/event")]
    public class EventController : ApiController
    {
        [HttpPut]
        [Route("")]
        public HttpResponseMessage CreateEvent([FromBody] LanEventDto lanEvent)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);

            if (instance.Accounts.CheckAccess(LanEventManager.FlagCreateEvent))
            {
                if (lanEvent.Name.Length > 0 && lanEvent.StartTime > 0 && lanEvent.StartTime < lanEvent.EndTime)
                {
                    LanEvent newEvent = new LanEvent();

                    newEvent.Name = lanEvent.Name;
                    newEvent.StartTime = lanEvent.StartTime;
                    newEvent.EndTime = lanEvent.EndTime;

                    events.AddEvent(newEvent);

                    try
                    {
                        instance.Context.SaveChanges();

                        instance.SetData(new LanEventDto(newEvent), "LanEvent");
                    }
                    catch (Exception)
                    {
                        instance.SetError("SaveError");
                    }
                }
                else
                {
                    instance.SetError("InvalidRequestObject");
                }
            }
            else
            {
                instance.SetAccessDenied(LanEventManager.FlagCreateEvent);
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetEvent(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);

            instance.SetData(new LanEventDto(events.GetEventById(id)));

            if (instance.Data == null)
            {
                instance.SetError("InvalidEvent");
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("{id}")]
        public HttpResponseMessage EditEvent(long id, [FromBody] LanEventDto editedEvent)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);

            if (instance.Accounts.CheckAccess(LanEventManager.FlagEditEvent))
            {
                if (editedEvent.Name.Length > 0 && editedEvent.StartTime > 0 &&
                    editedEvent.StartTime < editedEvent.EndTime)
                {
                    LanEvent lanEvent = events.GetEventById(id);

                    if (lanEvent != null)
                    {
                        lanEvent.Name = editedEvent.Name;
                        lanEvent.StartTime = editedEvent.StartTime;
                        lanEvent.EndTime = editedEvent.EndTime;

                        try
                        {
                            instance.Context.SaveChanges();

                            instance.SetData(new LanEventDto(lanEvent), "LanEvent");
                        }
                        catch (Exception e)
                        {
                            if (e is OptimisticConcurrencyException)
                            {
                                instance.SetError("ConcurrencyError");
                            }
                            else
                            {
                                instance.SetError("SaveError");
                            }
                        }
                    }
                    else
                    {
                        instance.SetError("InvalidEvent");
                    }
                }
                else
                {
                    instance.SetError("InvalidRequestObject");
                }
            }
            else
            {
                instance.SetAccessDenied(LanEventManager.FlagEditEvent);
            }

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("{id}")]
        public HttpResponseMessage DeleteEvent(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);

            if (instance.Accounts.CheckAccess(LanEventManager.FlagDeleteEvent))
            {
                LanEvent lanEvent = events.GetEventById(id);

                if (lanEvent != null)
                {
                    events.RemoveEvent(lanEvent);

                    try
                    {
                        instance.Context.SaveChanges();

                        instance.SetData(true, "bool");
                    }
                    catch (Exception)
                    {
                        instance.SetError("SaveError");
                    }
                }
                else
                {
                    instance.SetError("InvalidEvent");
                }
            }
            else
            {
                instance.SetAccessDenied( LanEventManager.FlagDeleteEvent);
            }

            return instance.ToResponse();
        }

        [HttpPut]
        [Route("{id}/guest")]
        public HttpResponseMessage CreateEventGuest(long id, [FromBody] LanEventGuestDto guest)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);

            if (instance.Accounts.CheckAccess(LanEventManager.FlagCreateGuest))
            {
                LanEvent lanEvent = events.GetEventById(id);

                if (lanEvent != null)
                {
                    UserAccount targetAccount = instance.Accounts.GetAccount(guest.Account);

                    if (targetAccount != null)
                    {
                        LanEventGuest record = events.GetEventGuest(id, guest.Account);

                        if (record == null)
                        {
                            record = new LanEventGuest();

                            record.Event = id;
                            record.Account = guest.Account;
                            record.Arrived = guest.Arrived;
                            record.Departed = guest.Departed;
                            record.Invited = guest.Invited;

                            events.AddEventGuest(record);

                            // Adjust target's total events if necessary
                            if (record.Arrived > 0)
                            {
                                targetAccount.TotalEvents++;
                            }

                            try
                            {
                                instance.Context.SaveChanges();

                                instance.SetData(new LanEventGuestDto(record), "LanEventGuest");
                            }
                            catch (Exception e)
                            {
                                if (e is OptimisticConcurrencyException)
                                {
                                    instance.SetError("ConcurrencyError");
                                }
                                else
                                {
                                    instance.SetError("SaveError");
                                }
                            }
                        }
                        else
                        {
                            instance.SetData(new LanEventGuestDto(record), "LanEventGuest");
                        }
                    }
                    else
                    {
                        instance.SetError("InvalidUser");
                    }
                }
                else
                {
                    instance.SetError("InvalidEvent");
                }
            }
            else
            {
                instance.SetAccessDenied(LanEventManager.FlagCreateGuest);
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}/guest")]
        public HttpResponseMessage GetAllEventGuests(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);

            LanEvent lanEvent = events.GetEventById(id);

            if (lanEvent != null)
            {
                List<LanEventGuest> guests = events.GetEventGuests(id);

                instance.SetData(LanEventGuestDto.ConvertList(guests), "LanEventGuestList");
            }
            else
            {
                instance.SetError("InvalidEvent");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}/guest/{guestId}")]
        public HttpResponseMessage GetEventGuest(long id, long guestId)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);

            LanEvent lanEvent = events.GetEventById(id);

            if (lanEvent != null)
            {
                instance.SetData(new LanEventGuestDto(events.GetEventGuest(id, guestId)), "LanEventGuest");
            }
            else
            {
                instance.SetError("InvalidEvent");
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("{id}/guest/{guestId}")]
        public HttpResponseMessage EditEventGuest(long id, long guestId, [FromBody] LanEventGuestDto editedGuest)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.Accounts.CheckAccess(LanEventManager.FlagEditGuest))
            {
                LanEventManager events = new LanEventManager(instance);
                LanEvent lanEvent = events.GetEventById(id);

                if (lanEvent != null)
                {
                    LanEventGuest guest = events.GetEventGuest(id, guestId);

                    if (guest != null)
                    {
                        if (editedGuest.Invited >= 0)
                        {
                            guest.Invited = editedGuest.Invited;
                        }

                        if (editedGuest.Departed >= editedGuest.Arrived && editedGuest.Arrived >= 0)
                        {
                            guest.Departed = editedGuest.Departed;

                            if (editedGuest.Arrived != guest.Arrived)
                            {
                                // Adjust target's total events if necessary
                                if (guest.Arrived == 0)
                                {
                                    UserAccount target = instance.Accounts.GetAccount(guestId);

                                    if(target != null)
                                        target.TotalEvents++;
                                }
                                else if (editedGuest.Arrived == 0)
                                {
                                    UserAccount target = instance.Accounts.GetAccount(guestId);

                                    if (target != null)
                                        target.TotalEvents--;
                                }

                                guest.Arrived = editedGuest.Arrived;
                            }
                        }

                        try
                        {
                            instance.Context.SaveChanges();

                            instance.SetData(new LanEventGuestDto(guest), "LanEventGuest");
                        }
                        catch (Exception e)
                        {
                            if (e is OptimisticConcurrencyException)
                            {
                                instance.SetError("ConcurrencyError");
                            }
                            else
                            {
                                instance.SetError("SaveError");
                            }
                        }
                    }
                    else
                    {
                        instance.SetError("InvalidGuest");
                    }
                }
                else
                {
                    instance.SetError("InvalidEvent");
                }
            }
            else
            {
                instance.SetAccessDenied(LanEventManager.FlagEditGuest);
            }

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("{id}/guest/{guestId}")]
        public HttpResponseMessage DeleteEventGuest(long id, long guestId)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.Accounts.CheckAccess(LanEventManager.FlagDeleteGuest))
            {
                LanEventManager events = new LanEventManager(instance);
                LanEvent lanEvent = events.GetEventById(id);

                if (lanEvent != null)
                {
                    LanEventGuest guest = events.GetEventGuest(id, guestId);

                    if (guest != null)
                    {
                        UserAccount target = instance.Accounts.GetAccount(guestId);

                        // Adjust target's total events if necessary
                        if (target != null && guest.Arrived > 0)
                        {
                            target.TotalEvents--;
                        }

                        events.RemoveEventGuest(guest);

                        try
                        {
                            instance.Context.SaveChanges();

                            instance.SetData(true, "bool");
                        }
                        catch (Exception e)
                        {
                            if (e is OptimisticConcurrencyException)
                            {
                                instance.SetError("ConcurrencyError");
                            }
                            else
                            {
                                instance.SetError("SaveError");
                            }
                        }
                    }
                    else
                    {
                        instance.SetData(true, "bool");
                    }
                }
                else
                {
                    instance.SetError("InvalidEvent");
                }
            }
            else
            {
                instance.SetAccessDenied(LanEventManager.FlagDeleteGuest);
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("current")]
        public HttpResponseMessage GetCurrentEvent()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);

            instance.SetData(new LanEventDto(events.GetCurrentEvent()));

            if (instance.Data == null)
            {
                instance.SetError("InvalidEvent");
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("current/checkin")]
        public HttpResponseMessage CurrentEventCheckin()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);
            UserAccount localAccount = instance.LocalAccount;

            if (instance.LoggedIn && instance.LocalClient)
            {
                LanEvent lanEvent = events.GetCurrentEvent();

                // Is there an active event?
                if (lanEvent != null)
                {
                    LanEventGuest guestEntry = events.GetEventGuest(lanEvent.Id, localAccount.Id);

                    // Does the entry exist?
                    if (guestEntry == null)
                    {
                        // If entry does not exist, generate new entry
                        guestEntry = new LanEventGuest();

                        guestEntry.Account = localAccount.Id;
                        guestEntry.Event = lanEvent.Id;
                        guestEntry.Arrived = instance.Time;

                        events.AddEventGuest(guestEntry);

                        // Increment account's events
                        localAccount.TotalEvents++;
                        localAccount.LastEvent = lanEvent.Id;

                        instance.SetData(true, "bool");
                    }
                    else if (guestEntry.Arrived == 0)
                    {
                        // If entry is marked as invited but not arrived, set to arrived
                        guestEntry.Arrived = instance.Time;

                        // Increment account's events
                        localAccount.TotalEvents++;
                        localAccount.LastEvent = lanEvent.Id;

                        instance.SetData(true, "bool");
                    }
                    else
                    {
                        instance.SetData(false, "bool");
                    }
                }
                else
                {
                    instance.SetError("NoEvent");
                }
            }
            else
            {
                instance.SetAccessDenied("AnonymousUser");
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("current/{id}")]
        public HttpResponseMessage SetCurrentEvent(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);

            if (instance.Accounts.CheckAccess(LanEventManager.FlagSetCurrentEvent))
            {
                LanEvent lanEvent = events.GetEventById(id);

                if (lanEvent != null)
                {
                    PlatformSetting currentEvent = instance.Settings.GetSettingByName(LanEventManager.SettingCurrentEvent);

                    currentEvent.Value = lanEvent.Id.ToString();

                    try
                    {
                        instance.Context.SaveChanges();

                        instance.SetData(true, "bool");
                    }
                    catch (Exception e)
                    {
                        if (e is OptimisticConcurrencyException)
                        {
                            instance.SetError("ConcurrencyError");
                        }
                        else
                        {
                            instance.SetError("SaveError");
                        }
                    }
                }
                else
                {
                    instance.SetError("InvalidEvent");
                }
            }
            else
            {
                instance.SetAccessDenied(LanEventManager.FlagSetCurrentEvent);
            }

            return instance.ToResponse();
        }
    }
}
