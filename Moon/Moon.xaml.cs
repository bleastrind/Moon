using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Corona.MoonCore;
using Corona;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using Microsoft.Windows.Controls.Ribbon;
namespace Moon
{  

    /// <summary>
    /// Moon.xaml 的交互逻辑
    /// </summary>   
    public partial class Window1 : RibbonWindow,IViewEventDispatcher
    {
        private int i = 0;
        private int k = 0;
        Boolean flagGoBack = false;
        TagItem TagAddAndSearch = null;
        ArrayList tagAll = new ArrayList();
        ArrayList resourceAll = new ArrayList();
        
        MoonCore moonControl = new MoonCore();       
        
        public event ResourceAddedHandler ResourceAdded;

        public event TagCreatedHandler TagCreated;

        public event RelationCreatedHandler RelationCreated;

        public event SearchRequestedHandler SearchRequested;

        public event TagModifiedHandler TagModified;

        public event TagRemovedHandler TagRemoved;

        public event ResourceOpenedHander ResourceOpened;


        TagCollection tags = ViewData.TagCollection;
        ResourceCollection resources = ViewData.ResourceCollection;
        IList selectedTags { get { return tagList.SelectedItems; } }
        IList selectedResources { get { return resourceList.SelectedItems; }  }
        TagItem selectedTag { get { return tagList.SelectedItem as TagItem; } }
        ResourceItem selectedResource { get { return resourceList.SelectedItem as ResourceItem; } set { resourceList.SelectedItem = value; } }

        

        public Window1()
        {
            this.Resources.MergedDictionaries
                .Add(Microsoft.Windows.Controls.Ribbon.PopularApplicationSkins.Office2007Black);

            InitializeComponent();
          
            tagList.DataContext = tags;
            resourceList.DataContext = resources;
            
            tagList.SelectedItem = selectedTag;
            resourceList.SelectedItem = selectedResource;

            resourceList.Drop += new DragEventHandler(resourceList_Drop);
        }

        void resourceList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (object obj in (System.Array)e.Data.GetData(DataFormats.FileDrop))
                {
                    if (ResourceAdded != null)
                    {
                        ResourceAdded(obj.ToString(), resourceAddedCallBack);
                    }
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
                if (ResourceAdded != null)
                    ResourceAdded(e.Data.GetData(DataFormats.Text).ToString(), resourceAddedCallBack);

            List<IRequestObject> resource = new List<IRequestObject>();
            foreach (ResourceItem item in ViewData.ResourceCollection)
                resource.Add(item.Resource);
            SearchRequested(resource, SearchResponseCallBack);
                
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            moonControl.addView(this);
        }       

        private void resourceAddedCallBack(ResrcAddedResponseEArgs e)
        {
            Boolean isExist=false;
            if (e.resource != null)
            {  
                foreach (ResourceItem resc in resources)
                {
                    if (resc.Resource.ResourceName==e.resource.ResourceName)
                    {
                        isExist = true;
                    }
                }
                if (isExist == false)
                {
                    ResourceItem tmpResc = new ResourceItem();
                    tmpResc.Resource = e.resource;
                    resources.Add(tmpResc);
                }
                else
                {
                    isExist = false;
                }
            }
            else
            {

            }
        }

        //资源删除按钮的点击事件
        private void resourceRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceRemoveExcuted();
         
        }

        private void ResourceRemoveExcuted()
        {
            if (selectedResource != null)
            {
                List<ResourceItem> toberemoved = new List<ResourceItem>();
                foreach (ResourceItem r in selectedResources)
                    toberemoved.Add(r);
                foreach (ResourceItem r in toberemoved)
                    resources.Remove(r);
            }
        }

        //资源搜索按钮的点击事件
        private void resourceSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchByResourceExcuted();
        
        }

        private void SearchByResourceExcuted()
        {
            List<IRequestObject> tmpResources = new List<IRequestObject>();
            if (selectedResources.Count != 0)
            {
                foreach (ResourceItem resc in selectedResources)
                {
                    tmpResources.Add(resc.Resource);
                }
            }
            else
            {
                MessageBox.Show("请选择需要搜索的资源！");
                return;
            }
            if (SearchRequested != null)
            {
                SearchRequested(tmpResources, SearchResponseCallBack);
            }
            else
            {

            }
        }

        //标签建立按钮的点击事件
        private void AddTagExcuted()
        {
            foreach (TagItem tag in tags)
            {
                if (tagName.Text == null || tag.Tag.TagName == tagName.Text)
                    return;
            }
            if (TagCreated != null)
            {
                TagCreated(tagName.Text, tagCreatedCallBack);
            }
        }

        private void tagCreatedCallBack(TagCreatedResponseEArgs e)
        {
            if (e.tag != null)
            {
                TagItem tmpTag = TagItem.CreateTagItem(e.tag);
                tags.Add(tmpTag);
                TagAddAndSearch = tmpTag;
                foreach (ResourceItem resc in selectedResources)
                {
                    if (RelationCreated != null)
                    {
                        RelationCreated(e.tag,resc.Resource, RelationCreatedCallBack);
                    }
              
                }
            }
      
        }

        private void AddTag_Excuted(object sender, ExecutedRoutedEventArgs e)
        {
            AddTagExcuted();
        }

        //标签删除按钮的点击事件
        private void tagRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            TagRemoveExcuted();
           
        }

        private void TagRemoveExcuted()
        {
            List<TagItem> toberemoved = new List<TagItem>();
            foreach (TagItem tag in selectedTags)
                toberemoved.Add(tag);
            foreach (TagItem tag in toberemoved)
            {
                if (tag.Tag.TagID != null)
                    TagRemoved(tag.Tag, tagRemovedCallBack);

                tags.Remove(tag);
            }
        }
        
        private void tagRemovedCallBack(TagRemovedResponseEArgs e)
        {
            if (tags != null)
            {                
                foreach (TagItem tag in tags)
                {
                    if (tag.Tag.TagID.Equals(e.TagID))
                    {
                        tags.Remove(tag);
                        return;
                    }
                }
            }
        }

        //标签修改按钮的点击事件
        private void tagModifyButton_Click(object sender, RoutedEventArgs e)
        {
            ModifyTagExcuted();
        }

        private void ModifyTagExcuted()
        {
            if (selectedTag == null)
                return;
            if (selectedTag.Tag.TagID == null)
            {
                foreach (TagItem tag in tags)
                {
                    if (tag.Tag.TagName == tagName.Text)
                        return;
                }
                selectedTag.Tag.TagName = tagName.Text;
                Tag temp = new Tag(selectedTag.Tag.TagID, selectedTag.Tag.TagName, selectedTag.Tag.ResourceID);
                TagItem tagItem = TagItem.CreateTagItem(temp);
                tagItem.Tag = temp;
                tagItem.State = selectedTag.State;
                int index = tags.IndexOf(selectedTag);
                tagList.SelectedItem = null;
                tags[index] = tagItem;
                return;
            }
            TagModified(selectedTag.Tag, tagName.Text, tagModifiedCallBack);
        }

        private void tagModifiedCallBack(TagModifiedResponseEArgs e)
        {
            if (e.TagNewName != null)
            {
                selectedTag.Tag.TagName = e.TagNewName;               
            }

        }

        private void ModifyTag_Excuted(object sender, ExecutedRoutedEventArgs e)
        {
            ModifyTagExcuted();
        }

        //标签搜索按钮的点击事件
        private void tagSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchByTagExcuted();
        }

        private void SearchByTagExcuted()
        {
            List<IRequestObject> tmpTags = new List<IRequestObject>();
            if (selectedTags.Count != 0)
            {
                foreach (TagItem tmp in selectedTags)
                {
                    tmpTags.Add(tmp.Tag);
                }
            }
            else
            {
                MessageBox.Show("请选择需要搜索的标签！");
                return;
            }
            if (SearchRequested != null)
            {
                SearchRequested(tmpTags, SearchResponseCallBack);
            }
            else
            {

            }
        }

        private void SearchResponseCallBack(SearchResponseArgs e)
        {
            //更换逻辑，区分直接的和间接的。
            int tmp = 0;
            bool obj_attribute = false;
            if (e.Result != null)
            {
                tags.Clear();
                resources.Clear();
                foreach (IRequestObject obj in e.Result) 
                {
                    obj_attribute = e.Result_Attribute[tmp];
                    if (obj is Tag)
                    {
                        TagItem item = TagItem.CreateTagItem(obj as Tag);
                        
                        if (item.Tag.TagID == null)
                        {
                            item.State = TagItemState.FreeNew;
                            //说明为直接获得。
                        }
                        else
                        {
                            item.State = TagItemState.FreeImmediate;
                            //说明为间接获得。
                        }
                        tags.Add(item);
                    }
                    else if (obj is Resource)
                    {
                        ResourceItem item = new ResourceItem(obj as Resource);
                        
                        if (obj_attribute)
                        {
                            //说明为直接获得。
                        }
                        else
                        {
                            //说明为间接获得。
                        }
                        resources.Add(item);
                    }
                    else
                        throw new Exception("New IRequestObject imported!");
                    tmp++;
                }
            }
            else
            {

            }
  
        }

        //资源与标签联系建立按钮的点击事件
        private void relationCreateButton_Click(object sender, RoutedEventArgs e)
        {
            RelationCreateExcuted();
            
        }

        private void RelationCreateExcuted()
        {
            foreach (TagItem tag in selectedTags)
                foreach (ResourceItem resource in selectedResources)
                {
                    if (RelationCreated != null)
                    {
                        RelationCreated(tag.Tag, resource.Resource, RelationCreatedCallBack);
                    }
                }
        }

        private void RelationCreatedCallBack(RelationCreatedResponseEArgs e)
        {
            foreach (TagItem item in ViewData.TagCollection)
            {
                if (e.TagID == item.Tag.TagID)
                    item.State = TagItemState.RelatedImmediate;
            }

            foreach (ResourceItem item in ViewData.ResourceCollection)
            {
                if (e.ResrcID == item.Resource.ResourceID)
                    item.Resource.TagNames.Add(e.TagName);
            }
        }
        
        private void RelationCreated_Excuted(object sender, ExecutedRoutedEventArgs e)
        {
            RelationCreateExcuted();
        }

        //资源打开按钮的点击事件
        private void ResourceOpenedExecuted()
        {
            if (ResourceOpened != null)
            {
                if (selectedResource != null)
                    ResourceOpened(selectedResource.Resource, ResourceOpenedCallBack);
                else
                {
                    MessageBox.Show("请选择要打开的文件！");
                }
            }
        }

        private void ResourceOpenedCallBack(ResourceOpenedResponseEArgs e)
        {            
        }

        private void OpenResource_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResourceOpenedExecuted();

        }

        
        private void ResourceSelectExcuted()
        {
            if (selectedResource != null)
            {

                foreach (TagItem item in ViewData.TagCollection)
                {
                    if (selectedResource.Resource.TagNames == null)
                        item.State.IsRelated = false;
                    else
                        item.State.IsRelated = selectedResource.Resource.TagNames.Contains(item.Tag.TagName);
                }


            }
        }


        private void OnCloseApplication(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        

        private void tagAddedAndSearchedExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            AddTagExcuted();
            selectedTags.Add(TagAddAndSearch);
            SearchByTagExcuted();
        }

        #region
        //记录标签list结果
        private void RemeberTag()
        {
            ArrayList resourceSep = new ArrayList();
            ArrayList tagSep = new ArrayList();
            resourceSep.Clear();
            foreach (ResourceItem r in ViewData.ResourceCollection)
            {
                resourceSep.Add(r);
            }
            foreach (TagItem t in ViewData.TagCollection)
            {
                tagSep.Add(t);
            }

            if (flagGoBack == false)
            {
                i++;
                k = i;
                tagAll.Add(tagSep);
                resourceAll.Add(resourceSep);
            }
            else
            {
                tagAll.RemoveRange(k + 1, i - k - 1);
                resourceAll.RemoveRange(k + 1, i - k - 1);
                i = k + 1;
                k = i;
                goForward.IsEnabled = false;
                flagGoBack = false;
            }
            if (k > 0)
            {
                goBack.IsEnabled = true;
            }
            else
            {
            }
        }

        //按照标签搜索
        private void SearchByTag_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RemeberTag();
            SearchByTagExcuted();
        }

        //记录资源list结果
        private void RemeberResc()
        {
            ArrayList resourceSep = new ArrayList();
            ArrayList tagSep = new ArrayList();
            foreach (ResourceItem r in ViewData.ResourceCollection)
            {
                resourceSep.Add(r);
            }
            foreach (TagItem t in ViewData.TagCollection)
            {
                tagSep.Add(t);
            }


            if (flagGoBack == false)
            {
                i++;
                k = i;
                tagAll.Add(tagSep);
                resourceAll.Add(resourceSep);
            }
            else
            {
                tagAll.RemoveRange(k + 1, i - k - 1);
                resourceAll.RemoveRange(k + 1, i - k - 1);
                i = k + 1;
                k = i;
                goForward.IsEnabled = false;
                flagGoBack = false;
            }
            if (k > 0)
            {
                goBack.IsEnabled = true;
            }
            else
            {
            }

        }

        //按照资源搜索
        private void SearchByResource_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RemeberResc();
            SearchByResourceExcuted();
        }

        //goBack
        private void goBack_Excuted(object sender, ExecutedRoutedEventArgs e)
        {
            k--;
            ArrayList resourceSep = new ArrayList();
            ArrayList tagSep = new ArrayList();
            foreach (ResourceItem r in ViewData.ResourceCollection)
            {
                resourceSep.Add(r);
            }
            foreach (TagItem t in ViewData.TagCollection)
            {
                tagSep.Add(t);
            }

            if (flagGoBack == false)
            {
                tagAll.Add(tagSep);
                resourceAll.Add(resourceSep);
                i++;
            }

            tags.Clear();
            resources.Clear();
            ArrayList lt = tagAll[k] as ArrayList;
            if (lt != null)
            {
                foreach (TagItem t in lt)
                {
                    tags.Add(t);
                }
            }
            else
            {
            }

            ArrayList lr = resourceAll[k] as ArrayList;
            if (lr != null)
            {
                foreach (ResourceItem r in lr)
                {
                    resources.Add(r);
                }
            }
            else
            {
            }
            goForward.IsEnabled = true;
            flagGoBack = true;
            if (k <= 0)
            {
                goBack.IsEnabled = false;
            }
            else
            {
            }
        }

        //goForward
        private void goForward_Excuted(object sender, ExecutedRoutedEventArgs e)
        {
            k++;
            ArrayList resourceSep = new ArrayList();
            ArrayList tagSep = new ArrayList();

            tags.Clear();
            resources.Clear();
            ArrayList lt = tagAll[k] as ArrayList;
            if (lt != null)
            {
                foreach (TagItem t in lt)
                {
                    tags.Add(t);
                }
            }
            else
            {
            }

            ArrayList lr = resourceAll[k] as ArrayList;
            if (lr != null)
            {
                foreach (ResourceItem r in lr)
                {
                    resources.Add(r);
                }
            }
            else
            {
            }

            if (i <= k + 1)
            {
                goForward.IsEnabled = false;
                goBack.IsEnabled = true;
            }
            else
            {
            }
            if (k >= 0)
            {
                goBack.IsEnabled = true;
            }
            else
            {
            }
        }
        #endregion

        private void resourceList_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    ResourceRemoveExcuted();
                    break;
                case Key.Enter:
                    SearchByResourceExcuted();
                    break;
                default:
                    break;
            }
                
        }

        private void tagList_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    TagRemoveExcuted();
                    break;
                case Key.Enter:
                    SearchByTagExcuted();
                    break;
                default:
                    break;

            }   
            
        } 

        private void buttonOnPopmenu_Click(object sender, RoutedEventArgs e)
        {
            popMenu.IsOpen = false;
            if (TagCreated != null)
            {
                foreach (TagItem tag in tags)
                {
                    if (tagName.Text == null || tag.Tag.TagName == tagName.Text)
                        return;
                }                
                if (textOnPopmenu != null)
                {
                    TagCreated(textOnPopmenu.Text, tagCreatedCallBack);
                }
            }
        }

        private void resourceList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ResourceSelectExcuted();
        }

        private void resourceList_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            popMenu.IsOpen = true;
        }

        private void resourceList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RemeberResc();
            SearchByResourceExcuted();
        }

        private void tagList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RemeberTag();
            SearchByTagExcuted();
        }

        private void RibbonButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
