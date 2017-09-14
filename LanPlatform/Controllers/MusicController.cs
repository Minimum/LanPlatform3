using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LanPlatform.Models;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/music")]
    public class MusicController : ApiController
    {
        [HttpPut]
        [Route("song")]
        public HttpResponseMessage CreateSong()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("song/{id}")]
        public HttpResponseMessage GetSong(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("song/{id}")]
        public HttpResponseMessage EditSong(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("song/{id}")]
        public HttpResponseMessage DeleteSong(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }

        [HttpPut]
        [Route("playlist")]
        public HttpResponseMessage CreatePlaylist()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("playlist/{id}")]
        public HttpResponseMessage GetPlaylist(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("playlist/{id}")]
        public HttpResponseMessage EditPlaylist(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("playlist/{id}")]
        public HttpResponseMessage DeletePlaylist(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }

        [HttpPut]
        [Route("playlist/{id}/song")]
        public HttpResponseMessage PlaylistAddSong(long id)
        {
            // ^ [FromBody] PlaylistEntry

            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("playlist/{id}/song")]
        public HttpResponseMessage PlaylistGetSongs(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("playlist/{id}/song/{entryId}")]
        public HttpResponseMessage PlaylistEditSong(long id, long entryId)
        {
            // ^ [FromBody] PlaylistEntry

            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("playlist/{id}/song/{entryId}")]
        public HttpResponseMessage PlaylistDeleteSong(long id, long entryId)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            return instance.ToResponse();
        }
    }
}
