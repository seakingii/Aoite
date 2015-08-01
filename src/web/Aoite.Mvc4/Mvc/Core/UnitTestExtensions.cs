using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Xunit;

namespace System.Web.Mvc
{
    /// <summary>
    /// 表示单元测试的扩展。
    /// </summary>
    public static class UnitTestExtensions
    {
        public static void AssertObject(this object expected, object actual)
        {
            if(!object.Equals(expected, actual))
            {
                var cr = GA.Compare(expected, actual);
                if(cr != null) Assert.Fail(cr.ToString());
            }
        }

        public static T AssertContent<T>(this T result, string expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");
            Assert.Equal(expected, Assert.IsAssignableFrom<ContentResult>(result).Content);
            return result;
        }

        public static T AssertContentType<T>(this T result, string expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            string actual = null;

            if(result is ContentResult) actual = (result as ContentResult).ContentType;
            else if(result is JsonResult) actual = (result as JsonResult).ContentType;
            else if(result is FileResult) actual = (result as FileResult).ContentType;
            else Assert.Fail("{0} 不是一个 ContentResult、JsonResult 或 FileResult。".Fmt(result.GetType().Name));

            Assert.Equal(expected, actual);
            return result;
        }

        public static T AssertContentEncoding<T>(this T result, Encoding expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            Encoding actual = null;

            if(result is ContentResult) actual = (result as ContentResult).ContentEncoding;
            else if(result is JsonResult) actual = (result as JsonResult).ContentEncoding;
            else Assert.Fail("{0} 不是一个 ContentResult 或 JsonResult。".Fmt(result.GetType().Name));

            Assert.Equal(expected, actual);
            return result;
        }

        public static T AssertFileContent<T>(this T result, byte[] expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            Assert.Equal(expected, Assert.IsAssignableFrom<FileContentResult>(result).FileContents);
            return result;
        }
        /*
        public static T AssertFileStream<T>(this T result, Stream expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            Assert.Equal(expected, Assert.IsAssignableFrom<FileStreamResult>(result).FileStream);
            return result;
        }*/

        public static T AssertFileName<T>(this T result, string expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            Assert.Equal(expected, Assert.IsAssignableFrom<FilePathResult>(result).FileName);
            return result;
        }

        public static T Assert404<T>(this T result, string expected = null) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");
            var actual = Assert.IsAssignableFrom<HttpNotFoundResult>(result).StatusDescription;
            if(expected != null) Assert.Equal(expected, actual);
            return result;
        }

        public static T AssertData<T>(this T result, object expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");
            var actual = Assert.IsAssignableFrom<JsonResult>(result).Data;
            AssertObject(expected, actual);
            return result;
        }

        public static T AssertModel<T>(this T result, object expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            object actual = null;

            if(result is PartialViewResult) actual = (result as PartialViewResult).Model;
            else if(result is ViewResult) actual = (result as ViewResult).Model;
            else Assert.Fail("{0} 不是一个 PartialViewResult 或 ViewResult。".Fmt(result.GetType().Name));

            AssertObject(expected, actual);
            return result;
        }

        public static T AssertViewName<T>(this T result, string expected = "") where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            string actual = null;

            if(result is PartialViewResult) actual = (result as PartialViewResult).ViewName;
            else if(result is ViewResult) actual = (result as ViewResult).ViewName;
            else Assert.Fail("{0} 不是一个 PartialViewResult 或 ViewResult。".Fmt(result.GetType().Name));

            Assert.Equal(expected, actual);
            return result;
        }

        public static T AssertUrl<T>(this T result, string expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            Assert.Equal(expected, Assert.IsAssignableFrom<RedirectResult>(result).Url);
            return result;
        }

        public static T AssertPermanent<T>(this T result, bool expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            bool actual = false;

            if(result is RedirectResult) actual = (result as RedirectResult).Permanent;
            else if(result is RedirectToRouteResult) actual = (result as RedirectToRouteResult).Permanent;
            else Assert.Fail("{0} 不是一个 RedirectResult 或 RedirectToRouteResult。".Fmt(result.GetType().Name));

            Assert.Equal(expected, actual);
            return result;
        }

        public static T AssertRouteName<T>(this T result, string expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            Assert.Equal(expected, Assert.IsAssignableFrom<RedirectToRouteResult>(result).RouteName);
            return result;
        }

        public static T AssertRoute<T>(this T result, string expectedAction, string expectedController = null) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            var r = Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            Assert.Equal(expectedAction, r.RouteValues.TryGetValue("action"));
            if(expectedController != null) Assert.Equal(expectedController, r.RouteValues.TryGetValue("controller"));
            return result;
        }
        public static T AssertRouteValues<T>(this T result, object expected) where T : ActionResult
        {
            return AssertRouteValues<T>(result, new RouteValueDictionary(expected));

        }

        public static T AssertRouteValues<T>(this T result, RouteValueDictionary expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            var r = Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            var actual = new RouteValueDictionary(r.RouteValues);

            if(actual.Count != expected.Count)
            {
                actual.Remove("action");
                actual.Remove("controller");
                if(actual.Count != expected.Count) Assert.Fail("预期数量为 {0}，实际为 {1}".Fmt(expected.Count, actual.Count));
            }

            foreach(var item in expected)
            {
                if(!actual.ContainsKey(item.Key)) Assert.Fail("实际找不到键 {0}".Fmt(item.Key));
                item.AssertObject(actual[item.Key]);
            }
            return result;
        }

        public static T AssertMasterName<T>(this T result, string expected) where T : ActionResult
        {
            if(result == null) throw new ArgumentNullException("result");

            Assert.Equal(expected, Assert.IsAssignableFrom<ViewResult>(result).MasterName);
            return result;
        }

        public static T AssertModelState<T>(this T controller, string expectedKey, string expectedErrorMessage) where T : Controller
        {
            if(controller == null) throw new ArgumentNullException("result");
            if(!controller.ModelState.ContainsKey(expectedKey)) Assert.Fail("找不到键 {0}".Fmt(expectedKey));
            foreach(var item in controller.ModelState[expectedKey].Errors)
            {
                if(item.ErrorMessage == expectedErrorMessage) return controller;
            }
            Assert.Fail("找不到匹配的错误“{0}”".Fmt(expectedErrorMessage));
            return controller;
        }
    }
}
