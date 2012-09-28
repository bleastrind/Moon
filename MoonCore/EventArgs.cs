using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corona;
namespace Corona.MoonCore
{

    public class SearchResponseArgs : EventArgs
    {
        public List<IRequestObject> Result { get; set; }

        public List<bool> Result_Attribute { get; set; }
    }

    public class ResrcAddedResponseEArgs : EventArgs
    {
        public Resource resource { get; set; }
    }

    public class TagCreatedResponseEArgs : EventArgs
    {
        public Tag tag { get; set; }
    }

    public class RelationCreatedResponseEArgs : EventArgs
    {
        public string TagID { get; set; }
        public string ResrcID { get; set; }
        public string TagName { get; set; }
    }

    public class TagModifiedResponseEArgs : EventArgs
    {
        public string TagNewName { get; set; }
    }

    public class TagRemovedResponseEArgs : EventArgs
    {
        public string TagID { get; set; }
    }

    public class ResourceOpenedResponseEArgs : EventArgs
    {
        public Resource resc { get; set; }
    }
}
