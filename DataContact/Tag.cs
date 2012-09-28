using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Corona
{


    public class Tag:IRequestObject
    {

        public string TagID
        {
            get;
            set;
        }

        public string TagName
        {
            get;
            set;
        }

        public string ResourceID
        {
            get;
            set; 
        }

        public Tag()
        {
        }

        public Tag(string tagName)
        {
            TagName = tagName;            
        }

        public Tag(string tagId, string tagName, string resourceId)
        {
            TagID = tagId;
            TagName = tagName;
            ResourceID = resourceId;
        }
    }
}
