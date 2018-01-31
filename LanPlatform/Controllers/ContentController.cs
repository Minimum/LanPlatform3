using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using LanPlatform.Content;
using LanPlatform.DTO.Content;
using LanPlatform.Models;
using Newtonsoft.Json;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/content")]
    public class ContentController : ApiController
    {
        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> UploadContent()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            ContentManager contentManager = new ContentManager(instance);

            if (instance.LocalAccount != null)
            {
                if (Request.Content.IsMimeMultipartContent())
                {
                    MultipartMemoryStreamProvider provider = new MultipartMemoryStreamProvider();

                    await Request.Content.ReadAsMultipartAsync(provider);

                    ContentItem item = new ContentItem();

                    foreach (HttpContent file in provider.Contents)
                    {
                        byte[] data = await file.ReadAsByteArrayAsync();

                        item.Owner = instance.LocalAccount.Id;
                        item.Hash = ContentManager.GetDataHash(data);
                        item.Filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                        item.Size = data.LongLength;
                        item.Type = ContentManager.GetContentType(MimeMapping.GetMimeMapping(item.Filename));
                        item.TimeAdded = instance.Time;

                        contentManager.AddItem(item);
                        contentManager.SaveData(item, data);

                        break;
                    }

                    return Ok(JsonConvert.SerializeObject(item));
                }

                return Conflict();
            }

            return Unauthorized();
        }

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetContentInfo(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            ContentManager contentManager = new ContentManager(instance);

            if (id > 0)
            {
                ContentItem item = contentManager.GetItemById(id);

                if (item != null)
                {
                    if (contentManager.CheckAccess(item, instance.LocalAccount))
                    {
                        instance.SetData(new ContentItemDto(item));
                    }
                    else
                    {
                        instance.SetAccessDenied("ContentView");
                    }
                }
                else
                {
                    instance.SetError("ContentDoesNotExist");
                }
            }

            return instance.ToResponse();
        }

        // POST	/{id}

        // DELETE	/{id}

        [HttpGet]
        [Route("{id}/data")]
        public HttpResponseMessage GetContentData(long id)
        {
            HttpResponseMessage response = null;
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            ContentManager contentManager = new ContentManager(instance);

            if (id > 0)
            {
                ContentItem item = contentManager.GetItemById(id);

                if (item != null)
                {
                    if (contentManager.CheckAccess(item, instance.LocalAccount))
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK);

                        response.Content = new StreamContent(contentManager.GetDataStream(item));

                        response.Content.Headers.ContentType = new MediaTypeHeaderValue(item.DataMime);
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.Forbidden);
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }

            return response;
        }
    }
}