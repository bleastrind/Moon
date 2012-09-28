using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Corona.CoronaService
{
    public interface ICoronaService
    {
        
        /// <summary>
        /// 由选中的资源集查询与之关联的标签集
        /// </summary>
        /// <param name="selectedResources">选中的资源集</param>
        /// <returns>一个Tag的List，为与选中资源关联的标签集</returns>
        List<Tag> SearchTagsFromResources(List<Resource> selectedResources);

        /// <summary>
        /// 由选中的标签集查询与之关联的资源集
        /// </summary>
        /// <param name="selectedTags">选中的标签集</param>
        /// <returns>一个Resource的List，为与选中标签关联的资源集</returns>
        List<Resource> SearchResourcesFromTags(List<Tag> selectedTags);

        /// <summary>
        /// 由选中的标签集查询与之关联的标签集
        /// </summary>
        /// <param name="selectedTags">选中的标签集</param>
        /// <returns>一个Tag的List，为与选中标签关联的标签集</returns>
        /// <remarks>通过标签相似度查询</remarks>
        List<Tag> SearchTagsFromTags(List<Tag> selectedTags);

        /// <summary>
        /// 创建一个标签并建立联系，接口实现会填充Tag和Resource的id
        /// </summary>
        /// <param name="tag">待创建的标签实例</param>
        /// <param name="resource">对应的资源</param>
        void AddTag(Tag tag, Resource resource);

        /// <summary>
        /// 修改一个标签
        /// </summary>
        /// <param name="tag">修改后的标签实例</param>
        void ModifyTag(Tag tag);

        /// <summary>
        /// 删除一个标签
        /// </summary>
        /// <param name="tagID">待删除的标签ID</param>
        void RemoveTag(string tagID);

        /// <summary>
        /// 添加一个资源，接口实现会填充Resource的id
        /// </summary>
        /// <param name="resrc">待添加的资源实例</param>
        void AddResource(Resource resrc);

        /// <summary>
        /// 删除一个资源
        /// </summary>
        /// <param name="resrcID">待删除的资源实例</param>
        /// <returns>删除的资源ID</returns>
        void RemoveResource(string resrcID);

        /// <summary>
        /// 通过资源路径查找资源实例
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns>资源实例</returns>
        Resource GetResource(string resourcePath);
    }
}
