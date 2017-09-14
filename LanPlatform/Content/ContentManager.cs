using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using LanPlatform.Accounts;
using LanPlatform.DAL;
using LanPlatform.Models;

namespace LanPlatform.Content
{
    public class ContentManager
    {
        protected PlatformContext Context;

        protected AppInstance Instance;

        public ContentManager(AppInstance instance)
        {
            Context = instance.Context;

            Instance = instance;
        }

        public String ContentUrl
        {
            get { return ConfigurationManager.AppSettings["LPContentUrl"]; }

            set { ConfigurationManager.AppSettings["LPContentUrl"] = value; }
        }

        public bool CheckUserUpload(UserAccount user, String filename)
        {
            // Check if user is root
            if (user.Root)
            {
                return true;
            }
            else
            {
                String extension = ParseExtension(filename);


            }

            return false;
        }

        public String ParseExtension(String filename)
        {
            String extension = "";

            int index = filename.LastIndexOf(".", StringComparison.OrdinalIgnoreCase);

            // Check if the file has an extension
            if (index != -1 && index < filename.Length)
            {
                extension = filename.Substring(index + 1);
            }

            return extension;
        }

        public void AddItem(ContentItem item)
        {
            Context.Content.Add(item);

            return;
        }

        public ContentItem GetItemById(long id)
        {
            return Context.Content.SingleOrDefault(s => s.Id == id);
        }

        public bool CheckAccess(ContentItem item, UserAccount account)
        {
            bool access = item.Visible;
            AccountManager accounts = Instance.Accounts;

            // If item isn't public, check if the user is logged in
            if (!access && account != null)
            {
                // Get local account's role ids
                List<long> roleIds = accounts.GetRolesByAccount(account.Id).Select(s => s.Id).ToList();

                // Check if the user owns the item
                access = item.Owner == account.Id ||
                    // Else check if the user has assigned access
                    Context.ContentAccess.SingleOrDefault(contentAccess => contentAccess.Content == item.Id &&
                                                                       (
                                                                           // Check if user is directly assigned access
                                                                           (contentAccess.User == account.Id) && contentAccess.Type == ContentAccessType.User ||
                                                                           // Check if any of the user's groups have access
                                                                           roleIds.Contains(contentAccess.User) && contentAccess.Type == ContentAccessType.Role
                                                                       )
                    ) != null || 
                    // Else check if the user has admin access
                    accounts.CheckAccess(account, "ContentView");
            }

            return access;
        }

        public FileStream GetDataStream(ContentItem item)
        {
            return ContentDal.GetDataStream(item);
        }

        public void SaveData(ContentItem item, byte[] data)
        {
            ContentDal.SaveData(item, data);

            return;
        }

        public static String GetDataHash(byte[] data)
        {
            SHA256Managed crypto = new SHA256Managed();
            byte[] hashBytes = crypto.ComputeHash(data);
            String hashString = "";

            foreach (byte x in hashBytes)
            {
                hashString += String.Format("{0:x2}", x);
            }

            return hashString;
        }

        public static Dictionary<String, ContentType> MimeTypes = new Dictionary<string, ContentType>
        {
            {"application/octet-stream", ContentType.Binary},
            {"image/bmp", ContentType.ImageBmp},
            {"image/gif", ContentType.ImageGif},
            {"image/jpeg", ContentType.ImageJpg},
            {"image/png", ContentType.ImagePng},
            {"image/svg+xml", ContentType.ImageSvg}
        };

        public static ContentType GetContentType(String mime)
        {
            ContentType contentType = ContentType.None;

            KeyValuePair<String, ContentType> kv = MimeTypes.FirstOrDefault(s => s.Key.Equals(mime, StringComparison.OrdinalIgnoreCase));

            if (kv.Key != null)
            {
                contentType = kv.Value;
            }

            return contentType;
        }
    }
}