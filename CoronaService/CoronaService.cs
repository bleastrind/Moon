using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;
using System.IO.Log;
using System.Threading;
using ADOX;
using Corona.CoronaService;
using CoronaService;
using CoronaService.Properties;
using CoronaService.MoonDataSetTableAdapters;

namespace Corona.CoronaService
{
    public class CoronaService : ICoronaService, IDisposable
    {   
        public MoonDataSet MoonDataset { get; set; }
        public ResourcesTableAdapter ResrcTableAdapter { get; set; }
        public TagsTableAdapter TagTableAdapter { get; set; }
        private string DBPath;

        public CoronaService(string dbpath)
        {
            DBPath = dbpath;
            Init();
           
            MoonDataset = new MoonDataSet();
            ResrcTableAdapter = new ResourcesTableAdapter();
            TagTableAdapter = new TagsTableAdapter();
        }

        private void Init()
        {
            InitMoonDB(DBPath);
        }

        #region ICoronaService Members

        private void InitMoonDB(string moonDBPath)
        {
            DBPath = moonDBPath;
            if (!File.Exists(moonDBPath))
            {
                Catalog catalog = new Catalog();
                string connectstring = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBPath + ";Jet OLEDB:Engine Type=5;";
                try
                {
                    catalog.Create(connectstring);
                }
                catch (Exception e)
                {
                }
                
                string strConnection = "Provider=Microsoft.Jet.OleDb.4.0;";
                strConnection += "Data Source=" + DBPath + ";Persist Security Info=False;";
                OleDbConnection con = new OleDbConnection(strConnection);
                con.Open();
                string strSql1 = "CREATE TABLE Resources (ResourceId INTEGER IDENTITY(1, 1) PRIMARY KEY, ResourceName VARCHAR(255) NOT NULL, ResourceType VARCHAR(255), ResourcePath VARCHAR(255) NOT NULL)";
                OleDbCommand cmd1 = new OleDbCommand(strSql1, con);
                cmd1.ExecuteNonQuery();
                string strSql2 = "CREATE TABLE Tags (TagId INTEGER IDENTITY(1, 1) PRIMARY KEY, TagName VARCHAR(255) NOT NULL, ResourceId INTEGER NOT NULL, FOREIGN KEY (ResourceId) REFERENCES Resources(ResourceId))";
                OleDbCommand cmd2 = new OleDbCommand(strSql2, con);
                cmd2.ExecuteNonQuery();
                con.Close();
                Settings config = Settings.Default;
                config.MoonDBConnectionString = strConnection;
                config.Save();
                
            }
        }

        /// <summary>
        /// 由选中的资源集查询与之关联的标签集
        /// </summary>
        /// <param name="selectedResources">选中的资源集</param>
        /// <returns>一个Tag的List，为与选中资源关联的标签集</returns>
        /// <remarks>MoonCore建立标签和资源联系时需根据ResourceId进行对应</remarks>
        public List<Tag> SearchTagsFromResources(List<Resource> selectedResources)
        {
            List<Tag> tags = new List<Tag>();
            List<Tag> result = new List<Tag>();
            if (selectedResources != null && selectedResources.Count > 0)
            {
                foreach (Resource resrc in selectedResources)
                {
                    try
                    {
                        result = GetDataByRelation(Int32.Parse(resrc.ResourceID));
                        tags.AddRange(result);
                        resrc.TagNames = new List<string>();
                        foreach (Tag tag in result)
                        {
                            resrc.TagNames.Add(tag.TagName);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }

                return tags;
            }
            return tags;
        }

        /// <summary>
        /// 由选中的标签集查询与之关联的资源集
        /// </summary>
        /// <param name="selectedTags">选中的标签集</param>
        /// <returns>一个Resource的List，为与选中标签关联的资源集</returns>
        /// <remarks>MoonCore建立资源与标签联系时需根据资源的TagIds进行对应</remarks>
        public List<Resource> SearchResourcesFromTags(List<Tag> selectedTags)
        {
            List<Resource> resources = new List<Resource>();
            if (selectedTags != null && selectedTags.Count > 0)
            {
                List<Tag> allTags = new List<Tag>();
                List<string> resourceIds = new List<string>();

                foreach (Tag tag1 in selectedTags)
                {
                    List<Tag> tags = GetDataByTagName(tag1.TagName);
                    allTags.AddRange(tags);
                    foreach (Tag tag2 in tags)
                    {
                        if (!resourceIds.Contains(tag2.ResourceID))
                            resourceIds.Add(tag2.ResourceID);
                    }
                }

                foreach (string resourceId in resourceIds)
                {
                    Resource resource = GetDataByResourceId(Int32.Parse(resourceId));
                    List<Tag> relatedTags = GetDataByRelation(Int32.Parse(resourceId));
                    resource.TagNames = new List<string>();
                    foreach (Tag tag in relatedTags)
                    {
                        resource.TagNames.Add(tag.TagName);
                    }
                    resources.Add(resource);
                }
                return resources;
            }
            return resources;
        }

        /// <summary>
        /// 由选中的标签集查询与之关联的标签集
        /// </summary>
        /// <param name="selectedTags">选中的标签集</param>
        /// <returns>一个Tag的List，为与选中标签关联的标签集</returns>
        /// <remarks>通过标签相似度查询</remarks>
        public List<Tag> SearchTagsFromTags(List<Tag> selectedTags)
        {
            List<Tag> tags = new List<Tag>();
            if (selectedTags != null && selectedTags.Count > 0)
            {
                tags.AddRange(selectedTags);
                //由标签集获得与其中每个标签关联资源的资源集的并集
                List<Resource> resources = SearchResourcesFromTags(selectedTags);

                //由资源集并集获得与之关联的所有标签集allTags
                List<Tag> allTags = SearchTagsFromResources(resources);
                
                //删除allTags中与选中标签集标签名相同的标签
                foreach (Tag tag in selectedTags)
                {
                    for (int i = 0; i < allTags.Count; i++)
                    {
                        if (allTags[i].TagName.Equals(tag.TagName))
                        {
                            allTags.RemoveAt(i);
                            i--;
                        }
                    }
                }

                //建立标签名频率字典
                Dictionary<string, int> frequency = new Dictionary<string, int>();

                //计算频率字典中相同标签名的重复次数
                foreach (Tag tag in allTags)
                {
                    if (!frequency.ContainsKey(tag.TagName))
                    {
                        frequency.Add(tag.TagName, 1);
                    }
                    else
                    {
                        frequency[tag.TagName]++;
                    }
                }

                //对频率字典排序
                var result = from pair in frequency orderby pair.Value descending select pair;
                foreach (KeyValuePair<string, int> pair in result)
                {
                    tags.Add(new Tag(pair.Key));
                }

                return tags;
            }
            return tags;
        }

        /// <summary>
        /// 创建一个标签并建立联系
        /// </summary>
        /// <param name="tag">待创建的标签实例</param>
        /// <param name="resource">对应的资源</param>
        public void AddTag(Tag tag, Resource resource)
        {
            try
            {
                long id = GetDataByTagNameAndRelation(tag.TagName, Int32.Parse(resource.ResourceID));
                if (id == -1)
                    CreateTagQuery(tag.TagName, Int32.Parse(resource.ResourceID));
                tag.TagID = id.ToString();
                tag.ResourceID = resource.ResourceID;
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// 修改一个标签
        /// </summary>
        /// <param name="tag">修改后的标签实例</param>
        public void ModifyTag(Tag tag)
        {
            try
            {
                if (tag.TagName != null && !tag.TagName.Equals(""))
                {
                    UpdateTagQuery(Int32.Parse(tag.TagID), tag.TagName);
                }
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// 删除一个标签
        /// </summary>
        /// <param name="tagID">待删除的标签ID</param>
        public void RemoveTag(string tagID)
        {
            try
            {
                DeleteTagQuery(Int32.Parse(tagID));
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// 添加一个资源
        /// </summary>
        /// <param name="resrc">待添加的资源实例</param>
        public void AddResource(Resource resrc)
        {
            if (resrc != null && resrc.ResourceName != null && !resrc.ResourceName.Equals("") &&
                resrc.ResourceType != null && !resrc.ResourceType.Equals("") &&
                resrc.ResourcePath != null && !resrc.ResourcePath.Equals(""))
            {
                CreateResourceQuery(resrc.ResourceName, resrc.ResourceType, resrc.ResourcePath);
                MoonDataSet.ResourcesDataTable dt = ResrcTableAdapter.GetDataByResourcePath(resrc.ResourcePath);
                if (dt.Rows.Count > 0)
                {
                    MoonDataSet.ResourcesRow r = dt[0] as MoonDataSet.ResourcesRow;
                    resrc.ResourceID = r.ResourceId.ToString();
                }
            }
        }

        /// <summary>
        /// 删除一个资源
        /// </summary>
        /// <param name="resrcID">待删除的资源实例</param>
        /// <returns>删除的资源ID</returns>
        public void RemoveResource(string resrcID)
        {
            try
            {
                List<Tag> tags = GetDataByRelation(Int32.Parse(resrcID));
                foreach (Tag tag in tags)
                {
                    RemoveTag(tag.TagID);
                }
                DeleteResourceQuery(Int32.Parse(resrcID));
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// 通过资源路径查找资源实例
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns>资源实例</returns>
        public Resource GetResource(string resourcePath)
        {
            
            return GetDataByResourcePath(resourcePath);
        }

        #endregion

        //具体数据库操作 SqlCommands

        /// <summary>
        /// 由标签名和关联的资源ID创建新标签
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="resourceId"></param>
        private void CreateTagQuery(string tagName, int resourceId)
        {
            TagTableAdapter.Insert(tagName, resourceId);
            TagTableAdapter.Update(MoonDataset.Tags);
            MoonDataset.Tags.AcceptChanges();
        }

        /// <summary>
        /// 由标签名和关联的资源ID查找标签ID
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        private long GetDataByTagNameAndRelation(string tagName, int resourceId)
        {
            MoonDataSet.TagsDataTable dt = TagTableAdapter.GetDataByTagNameAndRelation(tagName, resourceId);
            DataRowCollection dr = dt.Rows;
            if (dr.Count > 0)
            {
                return (dr[0] as MoonDataSet.TagsRow).TagId;
            }
            return -1;
        }

        /// <summary>
        /// 由资源ID查找与之关联的所有标签的集合
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        private List<Tag> GetDataByRelation(int resourceId)
        {
            MoonDataSet.TagsDataTable dt = TagTableAdapter.GetDataByRelation(resourceId);
            DataRowCollection dr = dt.Rows;
            MoonDataSet.TagsRow r;
            List<Tag> tags = new List<Tag>();
            for (int i = 0; i < dr.Count; i++)
            {
                r = dr[i] as MoonDataSet.TagsRow;
                tags.Add(new Tag(r.TagId.ToString(), r.TagName, r.ResourceId.ToString()));
            }

            return tags;
        }

        /// <summary>
        /// 查找标签名相同的所有标签的集合
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        private List<Tag> GetDataByTagName(string tagName)
        {
            MoonDataSet.TagsDataTable dt = TagTableAdapter.GetDataByTagName("%" + tagName + "%");
            DataRowCollection dr = dt.Rows;
            MoonDataSet.TagsRow r;
            List<Tag> tags = new List<Tag>();
            for (int i = 0; i < dr.Count; i++)
            {
                r = dt[i] as MoonDataSet.TagsRow;
                tags.Add(new Tag(r.TagId.ToString(), r.TagName, r.ResourceId.ToString()));
            }

            return tags;
        }

        /// <summary>
        /// 更新一个标签(只能修改标签名)
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="tagName"></param>
        private void UpdateTagQuery(int tagId, string tagName)
        {
            TagTableAdapter.Update(tagName, tagId);
            TagTableAdapter.Update(MoonDataset.Tags);
            MoonDataset.AcceptChanges();
        }

        /// <summary>
        /// 删除一个标签(同时删除与相应资源的关联)
        /// </summary>
        /// <param name="tagId"></param>
        private void DeleteTagQuery(int tagId)
        {
            TagTableAdapter.Delete(tagId);
            TagTableAdapter.Update(MoonDataset.Tags);
            MoonDataset.AcceptChanges();
        }

        /// <summary>
        /// 创建一个资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourcePath"></param>
        private void CreateResourceQuery(string resourceName, string resourceType, string resourcePath)
        {
            if (GetDataByResourcePath(resourcePath) == null)
            {
                ResrcTableAdapter.Insert(resourceName, resourceType, resourcePath);
                ResrcTableAdapter.Update(MoonDataset.Resources);
                MoonDataset.Resources.AcceptChanges();
            }
        }

        /// <summary>
        /// 由资源ID获取资源实例
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        private Resource GetDataByResourceId(int resourceId)
        {
            MoonDataSet.ResourcesDataTable dt = ResrcTableAdapter.GetDataByResourceId(resourceId);
            if (dt.Rows.Count > 0)
            {
                MoonDataSet.ResourcesRow r = dt.Rows[0] as MoonDataSet.ResourcesRow;
                return (new Resource(r.ResourceId.ToString(), r.ResourceName, r.ResourceType, r.ResourcePath));
            }
            return null;
        }

        /// <summary>
        /// 删除一个资源
        /// </summary>
        /// <param name="resourceId"></param>
        private void DeleteResourceQuery(int resourceId)
        {
            ResrcTableAdapter.Delete(resourceId);
            ResrcTableAdapter.Update(MoonDataset.Resources);
            MoonDataset.Resources.AcceptChanges();
        }

        /// <summary>
        /// 由资源路径获取资源实例
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        private Resource GetDataByResourcePath(string resourcePath)
        {
            try
            {
                MoonDataSet.ResourcesDataTable dt = ResrcTableAdapter.GetDataByResourcePath(resourcePath);            
                if (dt.Rows.Count > 0)
                {
                    MoonDataSet.ResourcesRow r = dt.Rows[0] as MoonDataSet.ResourcesRow;
                    return (new Resource(r.ResourceId.ToString(), r.ResourceName, r.ResourceType, r.ResourcePath));
                }
            }
            catch (Exception e)
            {
            }
            return null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private bool m_disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    MoonDataset.Dispose();
                }
                m_disposed = true;
            }
        }

    }
}
