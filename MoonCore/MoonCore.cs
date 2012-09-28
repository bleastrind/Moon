using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corona.CoronaService;
using Corona;
using System.IO;

namespace Corona.MoonCore
{
    public class MoonCore
    {

        private ICoronaService coronaService =new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory+"MoonDB.accdb");
        private ResourceManager rescControl = new ResourceManager();
        /// <summary>
        /// 该属性表明MoonCore维持了所有系统已经建立的标签。在初始建立标签的时候，系统内部的标签仅仅有tagName属性。当执行了关联建立函数，即RelationCreated函数后
        /// 向后台数据库发出了建立标签的请求，同时建立了关联。
        /// </summary>       

        public void addView(IViewEventDispatcher dispatcher)
        {
            dispatcher.ResourceAdded += new ResourceAddedHandler(dispatcher_ResourceAdded);
            dispatcher.SearchRequested += new SearchRequestedHandler(dispatcher_SearchRequestAsked);
            dispatcher.TagCreated += new TagCreatedHandler(dispatcher_TagCreated);            
            dispatcher.RelationCreated += new RelationCreatedHandler(dispatcher_RelationCreated);
            dispatcher.TagModified += new TagModifiedHandler(dispatcher_TagModified);
            dispatcher.TagRemoved += new TagRemovedHandler(dispatcher_TagRemoved);
            dispatcher.ResourceOpened += new ResourceOpenedHander(dispatcher_ResourceOpened);

        }

        void dispatcher_ResourceOpened(Resource resc, ResourceOpenedResponseHandler callback)
        {
            //打开文件的文件夹。
            ResourceOpenedResponseEArgs e = new ResourceOpenedResponseEArgs();
            e.resc = resc;
            rescControl.OpenResource(e.resc);
            //其次回调
            if (callback != null)
                callback(e);
        }

        void dispatcher_TagModified(Tag target,string newTagName, TagModifiedResponseHandler callback)
        {
            TagModifiedResponseEArgs e = new TagModifiedResponseEArgs();            
            //首先调用数据局库。
            target.TagName = newTagName;
            coronaService.ModifyTag(target);
            //其次回调
            e.TagNewName = newTagName;

            if (callback != null)
                callback(e);
        }

        void dispatcher_TagRemoved(Tag target, TagRemovedResponseHandler callback)
        {
            TagRemovedResponseEArgs e=new TagRemovedResponseEArgs();            
            //首先调用数据库内部函数
            coronaService.RemoveTag(target.TagID);
            //其次回调
            e.TagID = target.TagID;
            if (callback != null)
                callback(e);
        }

        void dispatcher_SearchRequestAsked(List<IRequestObject> selectedObjects, SearchResponseHandler callback)
        {
            //该函数进行下列具体实现：首先进行Object中种类的识别和搜索。
            //进行识别完毕后，如果是资源占多数，则调用资源搜索的函数。
            //进行识别完毕后，如果是标签占多数，则调用标签搜索的函数。
            SearchResponseArgs e = new SearchResponseArgs();
            e.Result = new List<IRequestObject>();
            e.Result_Attribute = new List<bool>();
            int numberOfResources=0;
            int numberOfTags=0;
            List<Tag> selectedTags=new List<Tag>();
            List<Resource> selectedResources=new List<Resource>();
            foreach (IRequestObject reobj in selectedObjects)
            {

                if (reobj is Tag)
                {
                    numberOfTags++;
                    selectedTags.Add((Tag)reobj);
                }
                else
                {
                    numberOfResources++;
                    selectedResources.Add((Resource)reobj);
                }
            }
            //系统默认如果圈住的资源数目大于或等于标签数目，则系统进行资源的搜索。
            if (numberOfResources < numberOfTags)
            {
                List<Resource> res = coronaService.SearchResourcesFromTags(selectedTags);
                List<Resource> resourceToBeRemoved = new List<Resource>();
                foreach(Resource resc in res)
                {
                    //此处出现异常。需在遍历的时候，首先应该确定出应该删除的资源，并将其存储。
                    if (!File.Exists(resc.ResourcePath))
                    {
                        resourceToBeRemoved.Add(resc);
                    }                   
                }
                //确定完需要删除的Resource并将其一一删除。
                foreach (Resource resc in resourceToBeRemoved)
                {
                    rescControl.CheckResource(resc, coronaService);
                    res.Remove(resc);
                }
                foreach (Resource resc in res)
                {                    
                    e.Result.Add( rescControl.WrapResource(resc) ) ;
                    e.Result_Attribute.Add(true);
                }
                List<Tag> tags = coronaService.SearchTagsFromTags(selectedTags);
                foreach (Tag tag in tags)
                {
                    e.Result.Add(tag);
                    e.Result_Attribute.Add(false);
                }                
            }
            else
            {
                List<Tag> tags =coronaService.SearchTagsFromResources(selectedResources);
                foreach (Tag tag in tags)
                {
                    e.Result.Add(tag);
                    e.Result_Attribute.Add(true);
                }
                foreach (Resource resc in selectedResources)
                {
                    e.Result.Add( rescControl.WrapResource(resc) );
                    e.Result_Attribute.Add(false);
                }
            }
            if (callback != null)
                callback(e);
        }        

        void dispatcher_ResourceAdded(string rescPath, ResrcAddedResponseHandler callback)
        {
            ResrcAddedResponseEArgs e = new ResrcAddedResponseEArgs();
            //首先，进行系统内部搜索，看是否相应的资源已经存在。
            Resource resc = coronaService.GetResource(rescPath);            
            if (resc == null)
            {
                resc = rescControl.CreateResource(rescPath);
                coronaService.AddResource(resc);
                //其次，分析资源是否是系统可以识别的类型。如果是，以文件名作为标签名，自动添加标签。       

                rescControl.AnalyseResource(resc, coronaService);
            }

            e.resource = rescControl.WrapResource(resc);
            if (callback != null)
                callback(e);


        }

        void dispatcher_TagCreated(string tagName,TagCreatedReponseHandler callback)
        {
            TagCreatedResponseEArgs e = new TagCreatedResponseEArgs();
            e.tag = new Tag(tagName);
            if (callback != null) 
                callback(e);
        }
        /// <summary>
        /// 真正创建标签
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="resource"></param>
        /// <param name="relationCreatedCallback"></param>
        /// <param name="tagCreatedCallback"></param>
        void dispatcher_RelationCreated(Tag tag, Resource resource, RelationCreatedResponseHandler callback)
        {
            RelationCreatedResponseEArgs e = new RelationCreatedResponseEArgs();
            //首先将tag加入后台的数据库，并从后台的数据库中获得tagID参数。
            coronaService.AddTag(tag,resource);
            //其次完成回调部分。
            if ((tag.TagID != null) && (resource.ResourceID != null))
            {
                e.TagID = tag.TagID;
                e.ResrcID = resource.ResourceID;
                e.TagName = tag.TagName;
            }
            else
            {
            }
            if (callback != null) 
                callback(e);
        }
    }
}
