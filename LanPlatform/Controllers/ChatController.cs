﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.DynamicData;
using System.Web.Http;
using LanPlatform.Accounts;
using LanPlatform.Chat;
using LanPlatform.DAL;
using LanPlatform.DTO.Chat;
using LanPlatform.Models;
using LanPlatform.Models.Requests;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/chat")]
    public class ChatController : ApiController
    {
        /*
         *  GET api/chat
         *  ---
         *  Info: Get currently active chat channels.
         */
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetActiveChannels()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            instance.Data = ChatChannelDto.ConvertList(instance.Context.ChatChannel.Where(s => s.Active).ToList());
            instance.DataType = "";

            return instance.ToResponse();
        }

        /*
         *  PUT api/chat
         *  ---
         *  Info: Create a new chat channel.
         */
        [HttpPut]
        [Route("")]
        public HttpResponseMessage AddChannel([FromBody] ChatChannelDto channel)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.CheckAccess(ChatManager.FlagAddChannel))
            {
                ChatChannel newChannel = new ChatChannel();

                newChannel.Title = channel.Title;
                newChannel.Greeting = channel.Greeting;
                newChannel.Active = channel.Active;

                instance.Context.ChatChannel.Add(newChannel);

                try
                {
                    instance.Context.SaveChanges();

                    instance.SetData(new ChatChannelDto(newChannel), "ChatChannel");
                }
                catch (Exception)
                {
                    instance.SetError("SaveError");
                }
            }
            else
            {
                instance.SetAccessDenied(ChatManager.FlagAddChannel);
            }

            return instance.ToResponse();
        }

        /*
         *  GET api/chat/{channelId}
         *  ---
         *  Info: Get a chat channel's info.
         */
        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetChannel(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            instance.Data = instance.Context.ChatChannel.Where(s => s.Id == id);
            instance.DataType = "ChatChannel";

            return instance.ToResponse();
        }

        /*
         *  POST api/chat/{channelId}
         *  ---
         *  Info: Edit a chat channel's info.
         *  Access: Platform:ChatEditChannel
         */
        [HttpPost]
        [Route("{id}")]
        public HttpResponseMessage EditChannel(long id, [FromBody] ChatChannelDto channelEdit)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.CheckAccess(ChatManager.FlagEditChannel))
            {
                ChatChannel channel = instance.Context.ChatChannel.SingleOrDefault(s => s.Id == id);

                if (channel != null)
                {
                    channel.Title = channelEdit.Title;
                    channel.Greeting = channelEdit.Greeting;
                    channel.Active = channelEdit.Active;

                    try
                    {
                        instance.Context.SaveChanges();

                        instance.SetData(new ChatChannelDto(channel), "ChatChannel");
                    }
                    catch (Exception e)
                    {
                        if (e is OptimisticConcurrencyException)
                        {
                            instance.SetError("SaveConcurrency");
                        }
                        else
                        {
                            instance.SetError("SaveError");
                        }
                    }
                }
                else
                {
                    instance.SetError("InvalidChannel");
                }
            }
            else
            {
                instance.SetAccessDenied(ChatManager.FlagEditChannel);
            }

            return instance.ToResponse();
        }

        /*
         *  DELETE api/chat/{channelId}
         *  ---
         *  Info: Delete a chat channel.
         *  Access: Platform:ChatDeleteChannel
         */
        [HttpDelete]
        [Route("{id}")]
        public HttpResponseMessage DeleteChannel(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.CheckAccess(ChatManager.FlagDeleteChannel))
            {
                ChatChannel target = instance.Context.ChatChannel.SingleOrDefault(s => s.Id == id);

                if (target != null)
                {
                    instance.Context.ChatChannel.Remove(target);

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
                    instance.SetData(true, "bool");
                }
            }
            else
            {
                instance.SetAccessDenied(ChatManager.FlagDeleteChannel);
            }

            return instance.ToResponse();
        }

        /*
         *  GET api/chat/{channelId}/message
         *  ---
         *  Info: Get a channel's chat messages based on a specified criteria.
         */
        [HttpGet]
        [Route("{id}/message")]
        public HttpResponseMessage GetMessages(long id, [FromUri] ChatMessageBrowseRequest search)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            PlatformContext context = instance.Context;

            search.SanityCheck();

            List<ChatMessage> messages = (from m in context.ChatMessage where m.Channel == id && m.Id >= search.Start select m).Take(search.Limit).ToList();

            instance.SetData(ChatMessageDto.ConvertList(messages), "ChatMessageList");

            return instance.ToResponse();
        }

        /*
         *  PUT api/chat/{channelId}/message
         *  ---
         *  Info: Writes a new message to a chat channel.
         *  Access: Platform:ChatAddMessage IF user does not have chat write access
         */
        [HttpPut]
        [Route("{id}/message")]
        public HttpResponseMessage AddMessage(long id, [FromBody] ChatMessageDto message)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.LocalAccount != null)
            {
                PlatformContext context = instance.Context;

                // Check for chat write access
                bool access = (from a in context.ChatAccess
                    join
                        r in context.Role on a.Role equals r.Id
                    join
                        ar in context.AccountRole on r.Id equals ar.Role
                    where ar.User == instance.LocalAccount.Id && a.Channel == id && a.CanWrite
                    select a).SingleOrDefault() != null;

                // If user does not have chat write access check for admin override
                if (!access)
                {
                    access = instance.CheckAccess(ChatManager.FlagAddMessage);
                }

                if (access)
                {
                    // Check for channel mutes
                    ChatMute mute = (from m in context.ChatMute
                        where m.Channel == id && m.User == instance.LocalAccount.Id && m.Expire <= instance.Time select m).SingleOrDefault();

                    if (mute == null)
                    {
                        // Create new message
                        ChatMessage newMessage = new ChatMessage();

                        newMessage.Channel = id;
                        newMessage.Author = instance.LocalAccount.Id;
                        newMessage.Hidden = false;
                        newMessage.Message = message.Message;
                        newMessage.Time = instance.Time;

                        context.ChatMessage.Add(newMessage);

                        // Save new message
                        try
                        {
                            context.SaveChanges();

                            instance.SetData(new ChatMessageDto(newMessage), "ChatMessage");
                        }
                        catch (Exception e)
                        {
                            if (e is OptimisticConcurrencyException)
                            {
                                instance.SetError("SaveConcurrency");
                            }
                            else
                            {
                                instance.SetError("SaveError");
                            }
                        }
                    }
                    else
                    {
                        instance.SetAccessDenied("ChannelMute");
                    }
                }
                else
                {
                    instance.SetAccessDenied("ChannelAccess");
                }
            }
            else
            {
                instance.SetAccessDenied("InvalidAccount");
            }

            return instance.ToResponse();
        }

        /*
         *  POST api/chat/{channelId}/message/{messageId}
         *  ---
         *  Info: Edits a chat message.
         *  Access: Platform:ChatEditMessage IF user is not the message owner
         */
        [HttpPost]
        [Route("{id}/message/{messageId}")]
        public HttpResponseMessage EditMessage(long id, long messageId, [FromBody] ChatMessageDto messageEdit)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount localAccount = instance.LocalAccount;

            if (localAccount != null)
            {
                PlatformContext context = instance.Context;

                ChatMessage message =
                    (from m in context.ChatMessage where m.Channel == id && m.Id == messageId select m)
                    .SingleOrDefault();

                // Check if message exists
                if (message != null)
                {
                    // Check for edit access
                    if (message.Author == localAccount.Id || instance.CheckAccess(ChatManager.FlagEditMessage))
                    {
                        // Apply changes
                        message.Hidden = messageEdit.Hidden;
                        message.Message = messageEdit.Message;

                        message.Editor = localAccount.Id;
                        message.EditorName = localAccount.DisplayName;
                        message.EditTime = instance.Time;

                        // Save changes
                        try
                        {
                            context.SaveChanges();

                            instance.SetData(new ChatMessageDto(message), "ChatMessage");
                        }
                        catch (Exception e)
                        {
                            if (e is OptimisticConcurrencyException)
                            {
                                instance.SetError("SaveConcurrency");
                            }
                            else
                            {
                                instance.SetError("SaveError");
                            }
                        }
                    }
                    else
                    {
                        instance.SetAccessDenied("EditMessage");
                    }
                }
                else
                {
                    instance.SetError("InvalidMessage");
                }
            }
            else
            {
                instance.SetAccessDenied("InvalidAccount");
            }

            return instance.ToResponse();
        }

        /*
         *  DELETE api/chat/{channelId}/message/{messageId}
         *  ---
         *  Info: Deletes a message from a chat channel.
         *  Access: Platform:ChatDeleteMessage IF user is not the message owner
         */
        [HttpDelete]
        [Route("{id}/message/{messageId}")]
        public HttpResponseMessage DeleteMessage(long id, long messageId)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount localAccount = instance.LocalAccount;

            if (localAccount != null)
            {
                PlatformContext context = instance.Context;

                ChatMessage message =
                    (from m in context.ChatMessage where m.Channel == id && m.Id == messageId select m)
                    .SingleOrDefault();

                // Check if message exists
                if (message != null)
                {
                    // Check for delete access
                    if (message.Author == localAccount.Id || instance.CheckAccess(ChatManager.FlagDeleteMessage))
                    {
                        // Remove message
                        context.ChatMessage.Remove(message);

                        // Save changes
                        try
                        {
                            context.SaveChanges();

                            instance.SetData(true, "bool");
                        }
                        catch (Exception e)
                        {
                            if (e is OptimisticConcurrencyException)
                            {
                                instance.SetError("SaveConcurrency");
                            }
                            else
                            {
                                instance.SetError("SaveError");
                            }
                        }
                    }
                    else
                    {
                        instance.SetAccessDenied("DeleteMessage");
                    }
                }
                else
                {
                    instance.SetError("InvalidMessage");
                }
            }
            else
            {
                instance.SetAccessDenied("InvalidAccount");
            }

            return instance.ToResponse();
        }

        /*
         *  GET api/chat/{channelId}/access
         *  ---
         *  Info: Gets a channel's access permissions.
         */
        [HttpGet]
        [Route("{id}/access")]
        public HttpResponseMessage GetChannelAccess(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);



            return instance.ToResponse();
        }

        /*
         *  PUT api/chat/{channelId}/access
         *  ---
         *  Info: Adds an access permission to a channel.
         *  Access: Platform:ChatAddAccess
         */
        [HttpPut]
        [Route("{id}/access")]
        public HttpResponseMessage AddChannelAccess(long id, [FromBody] ChatAccessDto access)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);



            return instance.ToResponse();
        }

        /*
         *  POST api/chat/{channelId}/access/{accessId}
         *  ---
         *  Info: Edits a channel's access permission.
         *  Access: Platform:ChatEditAccess
         */
        [HttpPost]
        [Route("{id}/access/{accessId}")]
        public HttpResponseMessage EditChannelAccess(long id, long accessId, [FromBody] ChatAccessDto accessEdit)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);



            return instance.ToResponse();
        }

        /*
         *  DELETE api/chat/{channelId}/access/{accessId}
         *  ---
         *  Info: Deletes a channel's access permission.
         *  Access: Platform:ChatDeleteAccess
         */
        [HttpDelete]
        [Route("{id}/access/{accessId}")]
        public HttpResponseMessage DeleteChannelAccess(long id, long accessId)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);



            return instance.ToResponse();
        }


        /*
         *  POST api/chat/{channelId}/mute/{userId}
         *  ---
         *  Info: Mutes a user in a channel.
         */
        [HttpPost]
        [Route("{id}/mute/{userId}")]
        public HttpResponseMessage MuteUser(long id, long userId)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);



            return instance.ToResponse();
        }
    }
}
