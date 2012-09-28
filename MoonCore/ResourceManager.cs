using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Corona.CoronaService;

namespace Corona.MoonCore
{
    abstract class OpenableResource : Resource
    {
        public abstract void Open();

    }

    class FileResource : OpenableResource
    {
        public override void Open()
        {
            System.Diagnostics.Process.Start("explorer.exe", this.ResourcePath);             
        }
    }

    class UrlResource : OpenableResource
    {
        public override void Open()
        {
            System.Diagnostics.Process.Start(this.ResourcePath);
        }

    }

    public class ResourceManager
    {
        /// <summary>
        /// 由于资源的种类很多，包括文件、图片以及声音等等。现阶段我仅仅实现了一种文件类型，因此必须保证可扩展性强。  
        /// </summary>

        public ResourceManager()
        {
        }

        public void AnalyseResource(Resource resc, ICoronaService coronaService)
        {
            if (resc is FileResource)
            {
                Tag target = new Tag(resc.ResourceName);
                coronaService.AddTag(target, resc);
            }
            else//系统不能识别,需自行添加
            {

            }
        }

        public void  CheckResource(Resource resc,ICoronaService coronaService)
        {
            //不存在，则进行资源删除，否则保留。
            if (!File.Exists(resc.ResourcePath))
            {
                coronaService.RemoveResource(resc.ResourceID);
            }
            else
            {
                
            }
        }

        public void OpenResource(Resource r)
        {
            if (r is OpenableResource)
                (r as OpenableResource).Open();
        }

        public Resource WrapResource(Resource res)
        {
            Resource resource = CreateResource(res.ResourcePath);
            resource.TagNames = res.TagNames;
            resource.ResourceID = res.ResourceID;
            return resource;
        }

        public Resource CreateResource(string rescPath)
        {
            if(rescPath.StartsWith("http://"))
            {
                UrlResource urlresc = new UrlResource();
                urlresc.ResourceName = rescPath;
                urlresc.ResourceType = "URL";
                urlresc.ResourcePath = rescPath;
                return urlresc;
            }
            else if (File.Exists(rescPath))
            {               
                FileResource fileresc = new FileResource();
                fileresc.ResourceName = rescPath.Substring(rescPath.LastIndexOf('\\')+1);
                fileresc.ResourceType = "File";
                fileresc.ResourcePath = rescPath;
                return fileresc;
            }
            else
            {
                return new Resource(rescPath,"OtherType",rescPath);
            }
        }
       
    }
}
