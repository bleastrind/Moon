using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corona.MoonCore
{

    #region delegate of SearchRequestAsked

    public delegate void SearchRequestedHandler(List<IRequestObject> selectedObjects, SearchResponseHandler callback);

    public delegate void SearchResponseHandler(SearchResponseArgs e);
    #endregion

    #region delegate of ResourceAdded
    public delegate void ResrcAddedResponseHandler(ResrcAddedResponseEArgs e);
    public delegate void ResourceAddedHandler(string rescPath, ResrcAddedResponseHandler callback);
    #endregion

    #region delegate of TagCreated
    public delegate void TagCreatedReponseHandler(TagCreatedResponseEArgs e);
    public delegate void TagCreatedHandler(string tagName,TagCreatedReponseHandler callback);
    #endregion

    #region delegate of RelationCreated
    public delegate void RelationCreatedResponseHandler(RelationCreatedResponseEArgs e);
    public delegate void RelationCreatedHandler(Tag tag, Resource resource, RelationCreatedResponseHandler callback);
    #endregion

    #region delegate of TagModified
    public delegate void TagModifiedResponseHandler(TagModifiedResponseEArgs e);
    public delegate void TagModifiedHandler(Tag target,string newTagName,TagModifiedResponseHandler callback);
    #endregion

    #region delegate of TagRemoved
    public delegate void TagRemovedResponseHandler(TagRemovedResponseEArgs e);
    public delegate void TagRemovedHandler(Tag target,TagRemovedResponseHandler callback);
    #endregion 
    
    #region delegate of ResourceOpened
    public delegate void ResourceOpenedResponseHandler(ResourceOpenedResponseEArgs callback);
    public delegate void ResourceOpenedHander(Resource resc, ResourceOpenedResponseHandler callback);
    #endregion

    public interface IViewEventDispatcher
    {
        /// <summary>
        /// 当用户添加资源时引发
        /// </summary>
        event ResourceAddedHandler ResourceAdded;

        /// <summary>
        /// 当用户创建标签时引发
        /// </summary>
        event TagCreatedHandler TagCreated;

        /// <summary>
        /// 当用户建立标签和资源关联时引发
        /// </summary>
        event RelationCreatedHandler RelationCreated;

        /// <summary>
        /// 当用户发起查询时引发
        /// </summary>
        event SearchRequestedHandler SearchRequested;
        
        /// <summary>
        /// 当用户修改标签时引发
        /// </summary>
        event TagModifiedHandler TagModified;

        /// <summary>
        /// 当用户删除标签时引发
        /// </summary>
        event TagRemovedHandler TagRemoved;

        /// <summary>
        /// 当用户打开文件时引发
        /// </summary>
        event ResourceOpenedHander ResourceOpened;
    }

}
