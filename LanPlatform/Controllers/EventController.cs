using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using LanPlatform.Accounts;
using LanPlatform.DTO.Events;
using LanPlatform.Events;
using LanPlatform.Models;
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
                    catch (Exception e)
                    {
                        instance.SetError("SAVE_ERROR");
                    }
                }
                else
                {
                    instance.SetError("INVALID_DTO");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetEvent(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);

            instance.Data = events.GetEventById(id);

            if (instance.Data == null)
            {
                instance.SetError("INVALID_EVENT");
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
                            instance.SetError("SAVE_ERROR");
                        }
                    }
                    else
                    {
                        instance.SetError("INVALID_EVENT");
                    }
                }
                else
                {
                    instance.SetError("INVALID_DTO");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
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
                    catch (Exception e)
                    {
                        instance.SetError("SAVE_ERROR");
                    }
                }
                else
                {
                    instance.SetError("INVALID_EVENT");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
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

                            // TODO: Handle concurrency issues w/ target account

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
                                instance.SetError("SAVE_ERROR");
                            }
                        }
                        else
                        {
                            instance.SetData(new LanEventGuestDto(record), "LanEventGuest");
                        }
                    }
                    else
                    {
                        instance.SetError("INVALID_USER");
                    }
                }
                else
                {
                    instance.SetError("INVALID_EVENT");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
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
                instance.SetError("INVALID_EVENT");
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
                instance.SetError("INVALID_EVENT");
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
                                // TODO: Handle concurrency issues w/ target account

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
                            instance.SetError("SAVE_ERROR");
                        }
                    }
                    else
                    {
                        instance.SetError("INVALID_GUEST");
                    }
                }
                else
                {
                    instance.SetError("INVALID_EVENT");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
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

                        // TODO: Handle concurrency issues w/ target account

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
                            instance.SetError("SAVE_ERROR");
                        }
                    }
                    else
                    {
                        instance.SetData(true, "bool");
                    }
                }
                else
                {
                    instance.SetError("INVALID_EVENT");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }
    }
}
