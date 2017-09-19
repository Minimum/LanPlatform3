using System;
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
                if (editedEvent.Name.Length > 0 && editedEvent.StartTime > 0 && editedEvent.StartTime < editedEvent.EndTime)
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

        // GET		/{id}/guest

        // GET		/{id}/guest/{guestId}

        // POST	    /{id}/guest/{guestId}

        // DELETE	/{id}/guest/{guestId}
    }
}
