using Corona.CoronaService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Corona;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;


namespace CoronaServiceTest
{
    
    
    /// <summary>
    ///这是 ICoronaServiceTest 的测试类，旨在
    ///包含所有 ICoronaServiceTest 单元测试
    ///</summary>
    [TestClass()]
    public class ICoronaServiceTest
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

        ICoronaService  target = new Corona.CoronaService.CoronaService(AppDomain.CurrentDomain.BaseDirectory+"testdb1.accdb");
        /// <summary>
        ///AddResource 的测试
        ///</summary>
        [TestMethod()]
        public void ICornaServiceTest()
        {
            /* 测试AddResource方法
             * */
            //add the first Resource，and the Id is copy to the Resource
            Resource resrc1 = new Resource("Internet Explorer.exe",
                @"c:\program files\Internet Explorer"); // TODO: 初始化为适当的值
            target.AddResource(resrc1);
            Assert.IsTrue(isnumberic(resrc1.ResourceID));
            //add the second Resource
            Resource resrc2 = new Resource("TTPlayer.exe",
                @"D:\program files\TTPlayer\TTPlayer.exe");//TODO:初始化尾适当的值
            target.AddResource(resrc2);
            Assert.IsTrue(isnumberic(resrc2.ResourceID));

            /* 测试AddTag
            */
            //add the first Tag to the first Resource
            Tag tag1 = new Tag("5.0版本"); // TODO: 初始化为适当的值
            target.AddTag(tag1, resrc1);

            Assert.IsTrue(isnumberic(tag1.TagID));
            //add the second Tag to the second Resource
            Tag tag2 = new Tag("5.5版本");
            target.AddTag(tag2, resrc2);
            Assert.IsTrue(isnumberic(tag2.TagID));
            //add the third Tag to the first Resource
            Tag tag3 = new Tag("浏览器");
            target.AddTag(tag3, resrc1);
            Assert.IsTrue(isnumberic(tag3.TagID));
            //add the forth Tag to the second Resource
            Tag tag4 = new Tag("音乐播放器");
            target.AddTag(tag4, resrc2);
            Assert.IsTrue(isnumberic(tag4.TagID));
            /*测试RemoveTag
             * */
            string tagToRemoveID = tag1.TagID; // TODO: 初始化为适当的值
            target.RemoveTag(tagToRemoveID);
            List<Resource> resList1 = new List<Resource>();
            resList1.Add(resrc1);
            byte flag = 1;
            foreach (Tag t in target.SearchTagsFromResources(resList1))
            {
                if (t.TagID == tag1.TagID)
                {
                    flag = 0;
                    break;
                }
            }
            Assert.AreEqual(1, flag);
            /*测试修改标签ModifyTag
                * */
            //修改标签Tag4(考虑修改名字而不是修改标签的ID)
            //怎么修改标签呢？要传递内容的参数，还是……？？
            //  Tag tempTag = tag4;
            // target.ModifyTag(tag4);
            // Assert.AreNotEqual(tempTag.TagName, tag4.TagName);
            /*测试删除资源RemoveResource
             * */
            //删除资源resrc1
            string resrcToRemoveID = resrc1.ResourceID; // TODO: 初始化为适当的值
            target.RemoveResource(resrcToRemoveID);
            List<Tag> tagList1 = new List<Tag>();
            tagList1.Add(tag1);
            tagList1.Add(tag3);
            int flag1 = 1;
            foreach (Resource rs in target.SearchResourcesFromTags(tagList1))
            {
                if (rs.ResourceID == resrc1.ResourceID)
                {
                    flag1 = 0;
                    break;
                }
            }
            Assert.AreEqual(1, flag1);
            //为了便于测试现在把第一个资源在重新添加回去！！,假设AddResource能够为参数的ID属性赋值！
            target.AddResource(resrc1);//这时resrc1的ID属性应该是发生了变化，只是我们不必知道而已
            //把Tag1,Tag3重新添加到第一个资源中资源
            target.AddTag(tag1, resrc1);
            target.AddTag(tag3, resrc1);

            /// <summary>
            ///SearchResourcesFromTags 的测试
            ///</summary>
            List<Tag> selectedTags = new List<Tag>(); // TODO: 初始化为适当的值
            selectedTags.Add(tag1);
            selectedTags.Add(tag2);
            selectedTags.Add(tag3);
            List<Resource> ResListExpected = new List<Resource>(); // TODO: 初始化为适当的值
            ResListExpected.Add(resrc1);
            ResListExpected.Add(resrc2);
            List<Resource> ResListActual;
            ResListActual = target.SearchResourcesFromTags(selectedTags);
            //不判断生成的List是否有重复的元素
            //判断实际生成的List中，每个元素都在预期的List中，否则断言失败；
            foreach (Resource r in ResListActual)
            {
                byte flag2 = 0;
                foreach (Resource t in ResListExpected)
                {
                    if (r.ResourceID == t.ResourceID)
                    {
                        flag2 = 1;
                        break;
                    }
                }
                if (flag2 == 0)
                {
                    Assert.Fail();
                }

            }
            //判断在预期的List中每个元素都属于生成的实际List，否则断言失败；
            foreach (Resource r in ResListExpected)
            {
                byte flag2 = 0;
                foreach (Resource t in ResListActual)
                {
                    if (r.ResourceID == t.ResourceID)
                    {
                        flag2 = 1;
                        break;
                    }
                }
                if (flag2 == 0)
                {
                    Assert.Fail();
                }
            }

            //if tagList is null:

            //      List<Tag> selectedTags1 = null;//TODO: 初始化为适当的值
            //      List<Resource> ResListExpected1 = null;
            //      List<Resource> ResListActual1 = target.SearchResourcesFromTags(selectedTags1);
            //      Assert.AreEqual(ResListExpected1, ResListActual1);
            //如果标签的内容一致，但是不是同一个标签，但是把它看成是一个标签，
            //这时，需要根据标签的内容来进行检索标签；
            Tag tagOfBoth0 = new Tag("创建时间：2009/11/19");
            Tag tagOfBoth1 = new Tag("创建时间：2009/11/19");
            target.AddTag(tagOfBoth0, resrc1);
            target.AddTag(tagOfBoth1, resrc2);
            List<Tag> tagList2 = new List<Tag>();
            tagList2.Add(tagOfBoth0);
            List<Resource> resListActual2;
            resListActual2 = target.SearchResourcesFromTags(tagList2);
            byte flag3 = 0;
            foreach (Resource rs in resListActual2)
            {
                if (rs.ResourceID == resrc1.ResourceID)
                {
                    flag3 = 1;
                    break;
                }
            }
            if (flag3 == 1)
            {
                foreach (Resource rs in resListActual2)
                {
                    if (rs.ResourceID == resrc2.ResourceID)
                    {
                        flag3 = 2;
                        break;
                    }
                }
            }
            Assert.AreEqual(2, flag3);
            /// <summary>
            ///SearchTagsFromResources 的测试
            ///</summary>
            List<Resource> selectedResources = new List<Resource>(); // TODO: 初始化为适当的值
            selectedResources.Add(resrc1);
            selectedResources.Add(resrc2);
            List<Tag> tagListExpected = new List<Tag>(); // TODO: 初始化为适当的值
            tagListExpected.Add(tag1);
            tagListExpected.Add(tag3);
            tagListExpected.Add(tag2);
            tagListExpected.Add(tag4);
            tagListExpected.Add(tagOfBoth0);
            tagListExpected.Add(tagOfBoth1);
            List<Tag> tagListActual;
            tagListActual = target.SearchTagsFromResources(selectedResources);
            foreach (Tag r in tagListActual)
            {
                byte flag2 = 0;
                foreach (Tag t in tagListExpected)
                {
                    if (r.TagID == t.TagID)
                    {
                        flag2 = 1;
                        break;
                    }
                }
                if (flag2 == 0)
                {
                    Assert.Fail();
                }

            }
            //判断在预期的List中每个元素都属于生成的实际List，否则断言失败；
            foreach (Tag r in tagListExpected)
            {
                byte flag2 = 0;
                foreach (Tag t in tagListActual)
                {
                    if (r.ResourceID == t.ResourceID)
                    {
                        flag2 = 1;
                        break;
                    }
                }
                if (flag2 == 0)
                {
                    Assert.Fail();
                }
            }
            //if selectedResources is null
            //      List<Resource> selectedResources1 = new List<Resource>();
            //      List<Tag> tagListExpected1 = null;
            //      List<Tag> tagListActual1;
            //      tagListActual1 = target.SearchTagsFromResources(selectedResources1);
            //      Assert.AreEqual(tagListExpected1, tagListActual1);
            /*SearchTagsFromTagsTest的测试
             * 
             * */
            /*List<Tag> selectedTags2 = new List<Tag>();
            selectedTags2.Add(tag1);
            selectedTags2.Add(tag2);
            List<Tag> tagListActual2;
            tagListActual2 = target.SearchTagsFromTags(selectedTags2);
            Assert.IsTrue(tagListActual2.Contains(tag3) && tagListActual2.Contains(tag4) &&
                tagListActual2.Contains(tagOfBoth0) && tagListActual2.Contains(tagOfBoth1) &&
                !tagListActual2.Contains(tag1) && !tagListActual2.Contains(tag2));
            //if taglist is null
            selectedTags2.Clear();
            tagListActual2 = target.SearchTagsFromTags(selectedTags2);
            Assert.IsNull(tagListActual2);*/
        }


        /*

                /// <summary>
                ///SearchTagsFromTags 的测试
                ///</summary>
                [TestMethod()]
                public void SearchTagsFromTagsTest()
                {
                    ICoronaService target = CreateICoronaService(); // TODO: 初始化为适当的值
                    List<Tag> selectedTags = null; // TODO: 初始化为适当的值
                    List<Tag> expected = null; // TODO: 初始化为适当的值
                    List<Tag> actual;
                    actual = target.SearchTagsFromTags(selectedTags);
                    Assert.AreEqual(expected, actual);
               //     Assert.Inconclusive("验证此测试方法的正确性。");
                }
                */


        //判断字符串是不是数字
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
    }
}





   
