using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corona
{
    public class Resource:IRequestObject
    {

        public string ResourceID
        {
            get;
            set;
        }

        public string ResourceName
        {
            get;
            set;
        }

        public string ResourceType
        {
            get;
            set;
        }

        public string ResourcePath
        {
            get;
            set;
        }

        public List<string> TagNames
        {
            get;
            set;
        }

        public Resource()
        {
        }

        public Resource(string resourceInfo, string rescPath)
        {
            if (resourceInfo.Contains("."))
            {
                ResourceName = resourceInfo.Split('.')[0];
                ResourceType = resourceInfo.Split('.')[1];
            }
            else
            {
               
            }
            ResourcePath = rescPath;
            TagNames = new List<string>();
        }

        public Resource(string resourceName, string resourceType, string resourcePath)
        {
            ResourceName = resourceName;
            ResourceType = resourceType;
            ResourcePath = resourcePath;
            TagNames = new List<string>();
        }

        public Resource(string resourceId, string resourceName, string resourceType, string resourcePath)
        {
            ResourceID = resourceId;
            ResourceName = resourceName;
            ResourceType = resourceType;
            ResourcePath = resourcePath;
            TagNames = new List<string>();
        }
    }
}
