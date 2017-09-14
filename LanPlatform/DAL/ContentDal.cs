using System;
using System.IO;
using System.Web.Hosting;
using LanPlatform.Content;

namespace LanPlatform.DAL
{
    public static class ContentDal
    {
        public static FileStream GetDataStream(ContentItem item)
        {
            String path = HostingEnvironment.MapPath("/App_Data/Content" + GetDataPath(item));

            return new FileStream(path, FileMode.Open);
        }

        public static void SaveData(ContentItem item, byte[] data)
        {
            String dir = HostingEnvironment.MapPath("/App_Data/Content");
            String path = HostingEnvironment.MapPath("/App_Data/Content" + GetDataPath(item));

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (FileStream stream = new FileStream(path, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(data);
            }

            return;
        }

        private static String GetDataPath(ContentItem item)
        {
            String path = "";
            String id = item.Id.ToString();

            for (int x = 0; x < id.Length; x += 2)
            {
                if (x < id.Length - 1)
                {
                    path += "/" + id.Substring(x, 2);
                }
                else
                {
                    path += "/" + id.Substring(x, 1);
                }
            }

            path += ".content";

            return path;
        }
    }
}