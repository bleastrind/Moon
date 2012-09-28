using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corona.CoronaService;
using Corona;
using Corona.MoonCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MoonCornTest
{


    /// <summary>
    ///这是 MoonCoreTest 的测试类，旨在
    ///包含所有 MoonCoreTest 单元测试
    ///</summary>
    [TestClass()]
    public class MoonCoreTest
    {


        private TestContext testContextInstance;
        protected List<Resource> allResource = new List<Resource>();
        protected List<Tag> allTag = new List<Tag>();
        protected Corona.MoonCore.MoonCore_Accessor target = new MoonCore_Accessor();
        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试属性
        // 
        //编写测试时，还可使用以下属性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///MoonCore的冒烟 的测试
        ///</summary>
        [TestMethod()]
        public void allMethodOfMoonCoreTest()
        {
            List<IRequestObject> testObjects = new List<IRequestObject>();
            List<IRequestObject> returnObjects = new List<IRequestObject>();
            Tag exe = new Tag("exe");
            Tag im = new Tag("IM");
            Tag ms = new Tag("MS");

            Resource msn = new Resource("MSN.exe", "C:/microsoft");
            Resource qq = new Resource("QQ.exe", "C:/QQ");
            List<Resource> testResource = new List<Resource>();
       
       //     Corona.MoonCore.MoonCore_Accessor target = new MoonCore_Accessor();

            target.dispatcher_TagCreated("exe", (e) => { exe = e.tag; });
            target.dispatcher_TagCreated("IM", (e) => { im = e.tag; });
            target.dispatcher_TagCreated("MS", (e) => { ms = e.tag; });

            target.dispatcher_ResourceAdded("G:\\QQ.txt", (e) => { qq = e.resource; });
            //target.dispatcher_ResourceAdded("C:\\microsoft.txt", (e) => { msn = e.resource; });
            testResource.Add(qq);
            System.Threading.Thread.Sleep(100);

            target.dispatcher_RelationCreated(exe, qq, (e) => { exe.TagID = e.TagID; });
            target.dispatcher_RelationCreated(im, qq, (e) => { im.TagID = e.TagID; });
            testObjects.Add(exe);
            testObjects.Add(im);

            //target.dispatcher_RelationCreated(im, msn, null);
            string resourceInfo = string.Empty; // TODO: 初始化为适当的值
            string rescPath = string.Empty; // TODO: 初始化为适当的值
            string expected = string.Empty; // TODO: 初始化为适当的值
            string actual = "";
            target.dispatcher_SearchRequestAsked(testObjects, (e) =>
            {
                returnObjects = e.Result;
            });
            //  target.dispatcher_ResourceAdded(resourceInfo, rescPath, null);
            foreach (IRequestObject ro in returnObjects)
            {
                Tag tag = ro as Tag;
                Resource res = ro as Resource;
                if (ro is Tag)
                    actual += tag.TagName + tag.TagID + tag.ResourceID;
                else if (ro is Resource)
                    actual += res.ResourceName + res.ResourcePath + res.ResourceID + res.TagIDs;
            }
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }


        ///// <summary>
        ///// MoonCore错误使用测试
        ///// </summary>
        //[TestMethod]
        //public void unusualUseTest()
        //{
        //    //     Corona.MoonCore.MoonCore_Accessor target = new MoonCore_Accessor();
        //    List<string> allFalseRes = new List<string>();
        //    string falseRes1 = "D:\\fetion";
        //    string falseRes2 = "D:\\fetion.exe.exe";
        //    string falseRes3 = "D:\\......";
        //    string falseRes4 = "D:\\ . ";
        //    string expected = "D:\\nonsense";
        //    string actual = "";
        //    allFalseRes.Add(falseRes1);
        //    allFalseRes.Add(falseRes2);
        //    allFalseRes.Add(falseRes3);
        //    allFalseRes.Add(falseRes4);
        //    for (int i = 0; i < 4; i++)
        //    {
        //        target.dispatcher_ResourceAdded(allFalseRes.ElementAt(i),
        //            (e) => { actual += e.resource.ResourceName + e.resource.ResourceID + e.resource.ResourcePath + e.resource.ResourceType; });

        //        actual += "\n";
        //    }
        //    Assert.AreEqual(expected, actual);
        //}

        ///// <summary>
        ///// MoonCore添加标签边界的测试
        ///// </summary>
        //[TestMethod]
        //public void tagBoundaryTest()
        //{
        //    //    Corona.MoonCore.MoonCore_Accessor target = new MoonCore_Accessor();
        //    //List<Tag> allTag= new List<Tag>();
        //    int i;
        //    string tagName;
        //    string info1 = "";
        //    string info2 = "";
        //    for (i = 0; i < 100; i++)
        //    {
        //        tagName = "tagName" + i;
        //        target.dispatcher_TagCreated(tagName, (e) => { allTag.Add(e.tag); });
        //    }
        //    foreach (Tag r in allTag)
        //    {
        //        info1 += r.TagName + "\n";
        //    }

        //    for (i = 0; i < 100; i++)
        //    {
        //        tagName = "tagName" + i;
        //        target.dispatcher_TagCreated(tagName, (e) => { allTag.Add(e.tag); });
        //    }
        //    foreach (Tag r in allTag)
        //    {
        //        info2 += r.TagName + "\n";
        //    }
        //    Assert.AreEqual(info1, info2);
        //}
        ///// <summary>
        ///// MoonCore添加资源边界的测试
        ///// </summary>
        //[TestMethod]
        //public void resBoundaryTest()
        //{
        //    //        Corona.MoonCore.MoonCore_Accessor target = new MoonCore_Accessor();
        //    //List<Resource> allResource = new List<Resource>();
        //    int i;
        //    string resourcePath;
        //    string info1 = "";
        //    string info2 = "";
        //    for (i = 0; i < 100; i++)
        //    {
        //        resourcePath = "C:\\" + i;
        //        target.dispatcher_ResourceAdded(resourcePath, (e) => { allResource.Add(e.resource); });
        //    }
        //    foreach (Resource r in allResource)
        //    {
        //        info1 += r.ResourceName + r.ResourceID + r.ResourcePath + "\n";
        //    }

        //    for (i = 0; i < 100; i++)
        //    {
        //        resourcePath = "C:/" + i;
        //        target.dispatcher_ResourceAdded(resourcePath, (e) => { allResource.Add(e.resource); });
        //    }
        //    foreach (Resource r in allResource)
        //    {
        //        info2 += r.ResourceName + r.ResourceID + r.ResourcePath + "\n";
        //    }
        //    Assert.AreEqual(info1, info2);
        //}

        ///// <summary>
        ///// MoonCore添加联系操作测试
        ///// </summary>
        //[TestMethod]
        //public void addRelationUse()
        //{
        //    string info1 = "";
        //    string info2 = "";

        //    for (int i = 0; i < 90; i++)
        //    {
        //        for (int j = 0; j < 90; j++)
        //        {
        //            target.dispatcher_RelationCreated(allTag.ElementAt(j), allResource.ElementAt(i),
        //                (e) =>
        //                {
        //                    info1 += allResource.ElementAt(i).ResourceName + e.ResrcID +
        //                        allTag.ElementAt(j).TagName + e.TagID;
        //                });
        //        }
        //    }


        //    for (int i = 0; i < 90; i++)
        //    {
        //        for (int j = 0; j < 90; j++)
        //        {
        //            target.dispatcher_RelationCreated(allTag.ElementAt(j), allResource.ElementAt(i),
        //                (e) =>
        //                {
        //                    info1 += allResource.ElementAt(i).ResourceName + e.ResrcID +
        //                        allTag.ElementAt(j).TagName + e.TagID;
        //                });
        //        }
        //    }
        //    Assert.AreEqual(info1, info2);
        //}

        ///// <summary>
        ///// MoonCore搜索操作测试
        ///// </summary>
        //[TestMethod]
        //public void searchTest()
        //{
        //    List<IRequestObject> returnObject=new List<IRequestObject>();
        //    List<IRequestObject> selectedTagObject = new List<IRequestObject>;
        //    foreach(Tag t in allTag)
        //    {
        //        selectedTagObject.Add(t);
        //    }
        //    List<IRequestObject> selectedResourceObject=new List<IRequestObject>();
        //    foreach(Resource r in allResource)
        //    {
        //        selectedResourceObject.Add(r);
        //    }
        //    string info1Expected="";
        //    string info1Actual="";
        //    string info2Expected="";
        //    string info2Actual="";
        //    target.dispatcher_SearchRequestAsked(selectedTagObject, (e) => {returnObject=e.Result ;});
        //    foreach(Resource r in returnObject )
        //    {
        //        info1Actual+=r.ResourceID+r.ResourcePath+r.TagIDs+"\n";
        //    }
        //    target.dispatcher_SearchRequestAsked(selectedResourceObject, (e) => {returnObject=e.Result ;});
        //    foreach(Tag r in returnObject )
        //    {
        //        info1Actual+=r.TagID+r.TagName+"\n";
        //    }
        //    Assert.AreEqual(info1Actual,info1Expected);
        //    Assert.AreEqual(info2Actual,info2Expected);
        //}
        ///// <summary>
        ///// MoonCore修改标签使用操作测试
        ///// </summary>
        //[TestMethod]
        //public void modifyTagTest()
        //{
        //    int i;
        //    string tagNewName;
        //    string info1 = "";
        //    string info2 = "";
        //    for (i = 0; i < 100; i++)
        //    {
        //        tagNewName = allTag.ElementAt(i).TagName + "NEW" + i;
        //        target.dispatcher_TagModified(allTag.ElementAt(i), tagNewName, (e) => { info1 += allTag.ElementAt(i).TagID + e.TagNewName; });
        //    }
            
        //    for (i = 0; i < 100; i++)
        //    {
        //        tagNewName = allTag.ElementAt(i).TagName + "NEW" + i;
        //        target.dispatcher_TagModified(allTag.ElementAt(i), tagNewName, (e) => { info2 += allTag.ElementAt(i).TagID + e.TagNewName; });
        //    }
        //    Assert.AreEqual(info1, info2);
        //}
    }
}
