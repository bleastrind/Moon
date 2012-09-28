using Microsoft.VisualStudio.TestTools.UnitTesting;
using Corona;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using Corona.CoronaService;


namespace CoronaServiceTest
{
    
    
    /// <summary>
    ///这是 CoronaServiceTest 的测试类，旨在
    ///包含所有 CoronaServiceTest 单元测试
    ///</summary>
    [TestClass()]
    public class CoronaServiceTest
    {


        private TestContext testContextInstance;

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

        private bool isnumberic(string str)
        {
            try
            {
                Convert.ToInt64(str);
                return (true);
            }
            catch { }
            return (false);

        }

        /// <summary>
        ///SearchTagsFromTags 的测试
        ///</summary>
        [TestMethod()]
        public void SearchTagsFromTagsTest()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory+"testdb1.accdb"); // TODO: 初始化为适当的值
            Resource res1 = new Resource("1.jpg",@"D:\pictures\1.jpg");
            Resource res2 = new Resource("2.jpg",@"D:\pictures\2.jpg");
            Resource res3 = new Resource("3.jpg",@"D:\pictures\3.jpg");
            target.AddResource(res1); target.AddResource(res2); target.AddResource(res3);
            Tag tag1 = new Tag("河南");
            Tag tag2 = new Tag("北京");
            Tag tag3 = new Tag("内蒙古");
            Tag tag4 = new Tag("人物");
            Tag tag5 = new Tag("风景");
            Tag tag6 = new Tag("风景");
            target.AddTag(tag1,res1);
            target.AddTag(tag4,res1);
            target.AddTag(tag2,res2);
            target.AddTag(tag5,res2);
            target.AddTag(tag3,res3);
            target.AddTag(tag6,res3);
            List<Tag> selectedTags =new List<Tag>(); // TODO: 初始化为适当的值
            selectedTags.Add(tag1);
            selectedTags.Add(tag5);
            selectedTags.Add(tag3);
            List<Tag> actual;
            actual = target.SearchTagsFromTags(selectedTags);
            Assert.AreEqual(actual[0].TagName, "河南");
            Assert.AreEqual(actual[1].TagName,"风景");
            Assert.AreEqual(actual[2].TagName, "内蒙古");
            Assert.AreEqual(actual[3].TagName,"人物");
            List<Tag> TList = new List<Tag>();
            List<Tag> actual1 = target.SearchTagsFromTags(TList);//搜索空表
            Assert.AreEqual(0,actual1.Count);
            TList = null;
            actual1 = target.SearchTagsFromTags(TList);//搜索null
            Assert.AreEqual(0,actual1.Count);
            TList=new List<Tag>();
            TList.Add(new Tag("1.exe"));//搜索创建的标签表，没有ＩＤ
            actual1 = target.SearchTagsFromTags(TList);
            Assert.AreNotEqual(0,actual1.Count);
            target.RemoveTag(tag1.TagID);
            TList = new List<Tag>();
            TList.Add(tag1);
            actual1 = target.SearchTagsFromTags(TList);//搜索刚刚删除的标签
            Assert.AreNotEqual(0,actual1.Count);

        }

        /// <summary>
        ///SearchTagsFromResources 的测试
        ///</summary>
        [TestMethod()]
        public void SearchTagsFromResourcesTest()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb2.accdb"); // TODO: 初始化为适当的值
            Resource res1 = new Resource("1.jpg", @"D:\pictures\1.jpg");
            Resource res2 = new Resource("2.jpg", @"D:\pictures\2.jpg");
            Resource res3 = new Resource("3.jpg", @"D:\pictures\3.jpg");
            target.AddResource(res1); target.AddResource(res2); target.AddResource(res3);
            Tag tag1 = new Tag("河南");
            Tag tag2 = new Tag("北京");
            Tag tag3 = new Tag("内蒙古");
            Tag tag4 = new Tag("人物");
            Tag tag5 = new Tag("风景");
            Tag tag6 = new Tag("风景");
            target.AddTag(tag1, res1);
            target.AddTag(tag4, res1);
            target.AddTag(tag2, res2);
            target.AddTag(tag5, res2);
            target.AddTag(tag3, res3);
            target.AddTag(tag6, res3);
            List<Resource> RList=new List<Resource>();
            RList.Add(res1);
            RList.Add(res2);
            RList.Add(res3);
            List<Tag> TagExpected=new List<Tag>();
            TagExpected.Add(tag1); TagExpected.Add(tag2);TagExpected.Add(tag3);
            TagExpected.Add(tag4);TagExpected.Add(tag5);TagExpected.Add(tag6);
            List<Tag> TagActual;
            TagActual = target.SearchTagsFromResources(RList);//正常情况的测试
            foreach (Tag t in TagActual)
            {
                byte flag=0;
                foreach (Tag ta in TagExpected)
                {
                    if (t.TagID == ta.TagID)
                    {
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    Assert.Fail();
                }

            }
            foreach (Tag t in TagExpected)
            {
                byte flag = 0;
                foreach (Tag ta in TagActual)
                {
                    if (ta.TagID == t.TagID)
                    {
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    Assert.Fail();
                }
            }
            List<Resource> RList1 = new List<Resource>();//测空的资源表
            Assert.AreEqual(0,target.SearchTagsFromResources(RList1).Count);
            List<Resource> RList2 = null;//测试Null资源表
            Assert.AreEqual(0,target.SearchTagsFromResources(RList2).Count);
            Resource tempRes = new Resource("tt.jpg",@"C:\pictures");
            List<Resource> RList3 = new List<Resource>();//测试刚刚创建的资源
            RList3.Add(tempRes);
            Assert.AreEqual(0,target.SearchTagsFromResources(RList3).Count);
            target.RemoveResource(res1.ResourceID);
            List<Resource> RList4 = new List<Resource>();//测试刚刚删除的资源
            RList4.Add(res1);
            Assert.AreEqual(0,target.SearchTagsFromResources(RList4).Count);


        }

        /// <summary>
        ///SearchResourcesFromTags 的测试
        ///</summary>
        [TestMethod()]
        public void SearchResourcesFromTagsTest()
        {

            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb3.accdb"); // TODO: 初始化为适当的值
            Resource res1 = new Resource("1.jpg", @"D:\pictures\1.jpg");
            Resource res2 = new Resource("2.jpg", @"D:\pictures\2.jpg");
            Resource res3 = new Resource("3.jpg", @"D:\pictures\3.jpg");
            target.AddResource(res1); target.AddResource(res2); target.AddResource(res3);
            Tag tag1 = new Tag("河南");
            Tag tag2 = new Tag("北京");
            Tag tag3 = new Tag("内蒙古");
            Tag tag4 = new Tag("人物");
            Tag tag5 = new Tag("风景");
            Tag tag6 = new Tag("风景");
            target.AddTag(tag1, res1);
            target.AddTag(tag4, res1);
            target.AddTag(tag2, res2);
            target.AddTag(tag5, res2);
            target.AddTag(tag3, res3);
            target.AddTag(tag6, res3);
            List<Tag> TList = new List<Tag>();
            TList.Add(tag1);
            //TList.Add(tag2);
            TList.Add(tag6);
            List<Resource> ResExpected = new List<Resource>();
            ResExpected.Add(res1); ResExpected.Add(res2); ResExpected.Add(res3);
            List<Resource> ResActual;
            ResActual = target.SearchResourcesFromTags(TList);//正常情况的测试
            foreach (Resource t in ResActual)
            {
                byte flag = 0;
                foreach (Resource ta in ResExpected)
                {
                    if (t.ResourceID == ta.ResourceID)
                    {
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    Assert.Fail();
                }

            }
            foreach (Resource t in ResExpected)
            {
                byte flag = 0;
                foreach (Resource ta in ResActual)
                {
                    if (ta.ResourceID == t.ResourceID)
                    {
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    Assert.Fail();
                }
            }
            List<Tag> TList1 = new List<Tag>();//测空的资源表
            Assert.AreEqual(0,target.SearchResourcesFromTags(TList1).Count);
            List<Tag> TList2 = null;//测试Null资源表
            Assert.AreEqual(0,target.SearchResourcesFromTags(TList2).Count);
            Tag tempTag = new Tag("Favorite");
            List<Tag> TList3 = new List<Tag>();//测试刚刚创建的资源
            TList3.Add(tempTag);
            Assert.AreEqual(0,target.SearchResourcesFromTags(TList3).Count);
            target.RemoveTag(tag1.TagID);
            List<Tag> TList4 = new List<Tag>();//测试刚刚删除的资源
            TList4.Add(tag1);
            Assert.AreEqual(0,target.SearchResourcesFromTags(TList4).Count);
        }

        /// <summary>
        ///RemoveTag 的测试
        ///</summary>
        [TestMethod()]
        public void RemoveTagTest()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb4.accdb"); // TODO: 初始化为适当的值
            Resource res1 = new Resource("1.jpg", @"D:\pictures\1.jpg");
            target.AddResource(res1); 
            Tag tag1 = new Tag("河南");
            Tag tag2 = new Tag("人物");
            Tag tag3 = new Tag("汴京府");
            target.AddTag(tag1, res1);
            target.AddTag(tag2, res1);
            target.AddTag(tag3, res1);
            target.RemoveTag(tag1.TagID);//Remove已经存在数据库中的标签
            List<Resource> RL = new List<Resource>();
            RL.Add(res1);
            List<Tag> TL = null;
            TL = target.SearchTagsFromResources(RL);
            foreach (Tag t in TL)
            {
                if (t.TagID == tag1.TagID)
                {
                    Assert.Fail();
                    break;
                }
            }
            target.RemoveTag(tag1.TagID);//Remove已经删除过的，有ＩＤ
            Assert.IsTrue(true);
            Tag tempTag = new Tag("Homework");
            target.RemoveTag(tempTag.TagID);//测试刚刚新建的，ID此时应该没有赋值
            target.RemoveTag(null);//删除空的
            Assert.IsTrue(true);
        }

        /// <summary>
        ///RemoveResource 的测试
        ///</summary>
        [TestMethod()]
        public void RemoveResourceTest()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb5.accdb");
            Resource res1 = new Resource("1.jpg", @"D:\pictures\1.jpg");
            target.AddResource(res1);
            Tag tag1 = new Tag("河南");
            
            Tag tag4 = new Tag("人物");
           
            target.AddTag(tag1, res1);
            target.AddTag(tag4, res1);
            
            target.RemoveResource(res1.ResourceID);//删除正常的资源
            List<Tag> TL = new List<Tag>();
            TL.Add(tag1); TL.Add(tag4);
            List<Resource> RL = null;
            RL = target.SearchResourcesFromTags(TL);
            Assert.AreEqual(0,RL.Count);
            target.RemoveResource(res1.ResourceID);//删除已经删除的，即应有ＩＤ
            Assert.IsTrue(true);
            Resource tempRes = new Resource("12.jpg",@"C:\program files\p");
            target.RemoveResource(tempRes.ResourceID);
            target.RemoveResource(null);//删除空
            Assert.IsTrue(true);
        }

        /// <summary>
        ///ModifyTag 的测试
        ///</summary>
        [TestMethod()]
        public void ModifyTagTest()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb6.accdb"); // TODO: 初始化为适当的值
            Tag tag = new Tag("小游戏"); // TODO: 初始化为适当的值
            Resource res = new Resource("PopCollection.exe",
                                        @"D:\program files\games\PopCollection.exe");
            target.AddResource(res);
            target.AddTag(tag,res);
            target.ModifyTag(tag);
            Tag tag1 = new Tag("Pop小游戏");
            tag1.TagID = tag.TagID;
            tag1.ResourceID = tag.ResourceID;
            target.ModifyTag(tag1);
            List<Resource> RList = new List<Resource>();
            RList.Add(res);
            List<Tag> TList;
            TList=target.SearchTagsFromResources(RList);
            foreach (Tag t in TList)
            {
                if (t.TagID == tag.TagID)
                {
                    Assert.AreEqual(tag1.TagName,t.TagName);
                    break;
                }
            }
            Tag tag2 = tag;
            tag2.TagName = null;//修改为空名称
            target.ModifyTag(tag2);
            Assert.IsTrue(true);
            Tag tag3 = new Tag();
            tag3.TagName = "Pop";
            tag3.TagID = tag.TagID;
            tag3.ResourceID = tag.ResourceID;
            target.AddTag(tag3,res);
            tag3.TagName="Pop小游戏";//修改为重名的同一个资源的标签
            target.ModifyTag(tag3);
            Assert.IsTrue(true);
            Tag tag4 = new Tag();
            //不知道怎么测——————————————————————————————————————————————————————————————
            tag4.TagName ="~~~~";//tag4是恶意注入的SQL语句
            tag4.TagID =  @"0 or true";
            target.ModifyTag(tag4);
            TList = target.SearchTagsFromResources(RList);
            foreach (Tag t in TList)
            {
                if (t.TagName.Equals("~~~~"))
                    Assert.Fail();
            }
            
            



        }

        /// <summary>
        ///GetResource 的测试
        ///</summary>
        [TestMethod()]
        public void GetResourceTest()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb7.accdb"); // TODO: 初始化为适当的值
            string resourcePath = @"C:\Users\XZC\pictures\3.jpg";// TODO: 初始化为适当的值
            Resource resource = new Resource("3.jpg",resourcePath); // TODO: 初始化为适当的值
            target.AddResource(resource);
            Assert.AreEqual(resource.ResourceID, target.GetResource(resourcePath).ResourceID);//查询正常的路径
            Assert.IsNull(target.GetResource(@"D:\ddd\e.jpg"));//查询不存在的路径
            Assert.IsNull(target.GetResource(null));//查询空的路径
            target.RemoveResource(resource.ResourceID);
            Assert.IsNull(target.GetResource(resourcePath));//查询刚刚删除的资源
        }

        /// <summary>
        ///AddTag 的测试
        ///</summary>
        [TestMethod()]
        public void AddTagTestNormal()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb8.accdb"); // TODO: 初始化为适当的值
            Tag tag = new Tag("旅游"); // TODO: 初始化为适当的值
            Resource resource = new Resource("1.jpg",@"C:\Users\name\pictures\1.jpg");
            target.AddResource(resource);// TODO: 初始化为适当的值
            target.AddTag(tag, resource);
            
            Assert.IsTrue(isnumberic(tag.TagID));//tag.TagID是数字
            Assert.AreEqual(resource.ResourceID,tag.ResourceID);//Resource的ID赋给tag
        }
        /// <summary>
        /// 测试非正常的添加标签:没有Name，没有资源
        /// </summary>
        [TestMethod()]
        public void AddTagTestUnnormal()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb9.accdb");//todo:初始化为适当的值
            Tag tag = new Tag("山西游");//todo:初始化为适当的值
            Resource resource = new Resource("aa.jpg",@"C:\Users\name\pictures\aa.jpg");
            //这里故意没有加入资源，测试异常情况
            Assert.IsNull(tag.ResourceID);
            Assert.IsNull(tag.TagID);
            target.AddTag(tag,resource);//添加没有资源的标签
            Tag tag1 = new Tag(null);//todo:初始化为适当的值
            target.AddResource(resource);
            target.AddTag(tag1,resource);//添加空标签
            Tag tag2 = new Tag(null);
            target.AddTag(tag2,resource);//添加没有名字的标签
            Assert.IsNull(tag2.TagID);
            Assert.IsNull(tag2.ResourceID);    
        }
        /// <summary>
        /// 测试添加刚测试的标签
        /// </summary>
        [TestMethod()]
        public void AddTagTestUnnormal1()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb10.accdb"); // TODO: 初始化为适当的值
            Tag tag = new Tag("旅游"); // TODO: 初始化为适当的值
            Resource resource = new Resource("1.jpg", @"C:\Users\name\pictures\1.jpg");
            target.AddResource(resource);// TODO: 初始化为适当的值
            target.AddTag(tag, resource);
            string OldID = tag.TagID;
            target.RemoveTag(tag.TagID); 
            target.AddResource(resource);
            target.AddTag(tag,resource);//添加刚刚删除的标签
            string NewID=tag.TagID;
            Assert.AreNotEqual(OldID,NewID);
        }
        /// <summary>
        ///AddResource 的Normal测试
        ///</summary>
        [TestMethod()]
        public void AddResourceTestNormal()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb11.accdb"); // TODO: 初始化为适当的值
            Resource resrc = new Resource("Hainan.jpg", @"C:\Users\name\Pictures\Hainan.jpg"); // TODO: 初始化为适当的值
            target.AddResource(resrc);
            Assert.IsTrue(isnumberic(resrc.ResourceID));
            Assert.AreEqual(resrc.ResourceID, target.GetResource(@"C:\Users\name\Pictures\Hainan.jpg").ResourceID);
        }
        ///<summary>
        ///AddResource 的异常测试
        ///</summary>
        [TestMethod()]
        public void AddResourceTestUnnormal()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb12.accdb");//todo:初始化为适当的值
            Resource resrc = null;
            target.AddResource(resrc);//添加空资源
            Assert.IsTrue(true);
            Resource resrc1 = new Resource("", @"C:\Users\name\Pictures\Hainan.jpg");
            target.AddResource(resrc1);//添加没有名字的资源
            Assert.IsNull(resrc1.ResourceID);
            Resource resrc2=new Resource("3.exe","");
            target.AddResource(resrc2);//添加没有路径的资源
            Assert.IsNull(resrc2.ResourceID);
        }
        /// <summary>
        /// 测试刚刚添加并已经删除的标签的添加；
        /// </summary>
        [TestMethod()]
        public void AddResourceTestUnnormal1()
        {
            Corona.CoronaService.CoronaService target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory + "testdb13.accdb"); // TODO: 初始化为适当的值
            Resource resrc = new Resource("Hainan.jpg", @"C:\Users\name\Pictures\Hainan.jpg"); // TODO: 初始化为适当的值
            target.AddResource(resrc);
            target.RemoveResource(resrc.ResourceID);
            string OldID = resrc.ResourceID;
            target.AddResource(resrc);//添加刚刚删除的标签
            Assert.IsNotNull(resrc.ResourceID);
            Assert.AreNotEqual(OldID,resrc.ResourceID);
        }
     }
      
}
