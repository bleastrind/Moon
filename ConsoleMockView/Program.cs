using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corona;
using Corona.MoonCore;

namespace ConsoleMockView
{
    public class Program : IViewEventDispatcher
    {

        Program()
        {
            new MoonCore().addView(this);

        }
        public static void Main(string[] args) { }

        #region IViewEventDispatcher 成员

        public event SearchRequestedHandler SearchRequested;
        public event ResourceAddedHandler ResourceAdded;
        public event TagCreatedHandler TagCreated;
        public event RelationCreatedHandler RelationCreated;

        #endregion

        //void run()
        //{
        //    //if (SearchRequestAsked != null)
        //    //  SearchRequestAsked(null, ShowTags);
        //    //if (ResourceAdded != null)
        //    string resrcInfo1 = System.Console.ReadLine();
        //    NotifyResourceAdded(resrcInfo1);
        //    //ResourceAdded(resrcInfo1, ShowResourceID);
        //    string resrcInfo2 = System.Console.ReadLine();
        //    //ResourceAdded(resrcInfo2, ShowResourceID);
        //    NotifyResourceAdded(resrcInfo2);
        //    //ResourceAdded("2.txt", ShowResourceID);
        //    string tagName1 = System.Console.ReadLine();
        //    //TagCreated(tagName1, ShowTagID);
        //    NotifyTagCreated(tagName1);
        //    string tagName2 = System.Console.ReadLine();
        //    //TagCreated(tagName2, ShowTagID);
        //    //TagCreated("txt", ShowTagID);
        //    NotifyTagCreated(tagName2);

        //    string tagID1 = System.Console.ReadLine();
        //    string resrcID1 = System.Console.ReadLine();
        //    RelationCreated(tagID1, resrcID1, ShowRelation);
        //    NotifyRelationCreated(tagID1, resrcID1);
        //    string tagID2 = System.Console.ReadLine();
        //    string resrcID2 = System.Console.ReadLine();
        //    RelationCreated(tagID2, resrcID2, ShowRelation);

        //    System.Console.WriteLine("Now we are going to check the search method.");

        //    System.Console.WriteLine("The Selected tags are :");
        //    List<Tag> tags = new List<Tag>();
        //    string selectedTagID = System.Console.ReadLine();
        //    Tag t = new Tag();
        //    t.TagID = selectedTagID;
        //    System.Console.WriteLine(t.TagID);
        //    tags.Add(t);

        //    System.Console.WriteLine("The Selected resources are :");
        //    List<Resource> resources = new List<Resource>();
        //    string selectedResourceID = System.Console.ReadLine();
        //    Resource resc = new Resource();
        //    resc.ResourceID = selectedResourceID;
        //    System.Console.WriteLine(resc.ResourceID);
        //    resources.Add(resc);

        //    SearchResource(tags, ShowResourceResults);
        //    SearchTag(resources, ShowTagResults);
        //    //System.Console.WriteLine("Stop");
        //}
        bool notrecieved;
        string recieve;

        System.Threading.Mutex mutex;
        public bool test1()
        {

            mutex.WaitOne();

            NotifyRelationCreated("", "");
            while (notrecieved)
            {
                System.Threading.Thread.Sleep(100);
            }

            mutex.ReleaseMutex();
            if (recieve == "")
                return true;
            else
                return false;
        }

        private void NotifyRelationCreated(string tagID1, string resrcID1)
        {
            if (RelationCreated != null)
                RelationCreated(tagID1, resrcID1, ShowRelation);
        }

        private void NotifyTagCreated(string tagName)
        {
            if (TagCreated != null)
                TagCreated(tagName, ShowTagID);
        }

        private void NotifyResourceAdded(string resrcInfo)
        {
            if (ResourceAdded != null)
                ;//ResourceAdded(resrcInfo, ShowResourceID);
        }

        /*void ShowTags(SearchResponseEventArgs e)
        {
            System.Console.WriteLine(e.Test);
        }*/

        void ShowResourceID(ResrcAddedResponseEArgs e)
        {
            recieve = "<Resource> " + e.ResrcID + " has been added.";
            notrecieved = false;
        }

        void ShowTagID(TagCreatedResponseEArgs e)
        {
            System.Console.WriteLine("<Tag> " + e.TagID + " has been created.");
        }

        void ShowRelation(RelationCreatedResponseEArgs e)
        {
            System.Console.WriteLine("The relation between" + "<Resource>" + e.ResrcID + "<Tag>" + e.TagID + "has been created");
        }



        #region IViewEventDispatcher 成员


        public event TagModifiedHandler TagModified;

        public event TagRemovedHandler TagRemoved;

        #endregion
    }
}
