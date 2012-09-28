using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Corona;
namespace Moon
{
    public class ResourceItem
    {

        public Resource Resource
        {
            get;
            set;
        }

        public ResourceItem(Resource resource)
        {
            Resource = resource;
        }
        public ResourceItem() { }

    }

    public class TagItemState:INotifyPropertyChanged
    {
        public enum TagState
        {
            New,
            Immediate,
            More
        }

        private TagState _state;
        public TagState State 
        { 
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
            get { return _state; }
        }

        private bool _isRelated;
        public bool IsRelated
        { 
            set 
            {
                _isRelated = value;
                OnPropertyChanged("IsRelated");
            }
            get { return _isRelated; } 
        }

        public static TagItemState FreeNew{ get{ return new TagItemState(TagState.New,false);}}
        public static TagItemState FreeImmediate{ get{ return new TagItemState(TagState.Immediate, false);}}
        public static TagItemState FreeMore{ get{ return new TagItemState(TagState.More, false);}}
        public static TagItemState RelatedNew { get{ return new TagItemState(TagState.New, true);}}
        public static TagItemState RelatedImmediate { get{ return new TagItemState(TagState.Immediate, true);}}
        public static TagItemState RelatedMore { get { return new TagItemState(TagState.More, true); } }

        private TagItemState(TagState state,bool related ) 
        { 
            State = state;
            IsRelated = related;
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        #endregion

    }

    public class TagItem:INotifyPropertyChanged
    {
        private Tag _tag;
        public Tag Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                OnPropertyChanged("Tag");
            }
        }

        private PropertyChangedEventHandler statechangehandler = null;
        private TagItemState _state;
        public TagItemState State 
        {
            get { return _state; }
            set
            {
                _state = value;
                value.PropertyChanged -= statechangehandler;
                statechangehandler = new PropertyChangedEventHandler(state_PropertyChanged);
                value.PropertyChanged += statechangehandler;
                state_PropertyChanged(this, null);
            }
        }


        public static TagItem CreateTagItem(Tag tag)
        {
            TagItem item = new TagItem(tag,TagItemState.FreeNew);
            
            return item;
        }

        private TagItem(Tag tag,TagItemState state)
        {
            Tag = tag;
            State = state;
            
        }

        void state_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("State");
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        #endregion
    }

    public static class ViewData
    {

        private static readonly TagCollection _tagCollection = new TagCollection();
        private static readonly ResourceCollection _resourceCollection = new ResourceCollection();
        public static TagCollection TagCollection { get { return _tagCollection; } }
        public static ResourceCollection ResourceCollection { get { return _resourceCollection; } }
    }

    public class TagCollection : ObservableCollection<TagItem>{ }
    public class ResourceCollection : ObservableCollection<ResourceItem> { }

}
