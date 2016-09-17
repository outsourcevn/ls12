using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebTMDT
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.RouteExistingFiles = true;

            routes.Add("ProductDetail", new SeoFriendlyRoute("{danhmuc}/{tensanpham}-{id}.html",
                new RouteValueDictionary(
                    new
                    {
                        controller = "Product",
                        action = "Detail",
                        id = UrlParameter.Optional,
                        tensanpham = UrlParameter.Optional,
                        danhmuc = UrlParameter.Optional,
                        
                    }),
                new MvcRouteHandler()));

            //routes.Add("gianhangUser", new SeoFriendlyRouteGianHang("gianhang/{username}-{TenCuaHang}",
            //   new RouteValueDictionary(
            //       new
            //       {
            //           controller = "Product",
            //           action = "GianHang",
            //           username = UrlParameter.Optional,
            //           TenCuaHang = UrlParameter.Optional
            //       }),
            //   new MvcRouteHandler()));

            // user/product/list
            routes.MapRoute(
                "ProductLists",                                        
                "user/product/list",                            
                new { controller = "Product", action = "ProductLists" }  
            );

            routes.MapRoute(
                "ProductEdit",
                "user/product/{id}/edit",
                new { controller = "Product", action = "Edit", id = UrlParameter.Optional }
                );

            // admin/products/list
            routes.MapRoute(
                "AdminProductsList",
                "admin/products/list",
                new { controller = "Admin", action = "ProductsList" }
            );

            routes.MapRoute(
                "AdminProductDelete",
                "admin/products/{id}/delete",
                new { controller = "Admin", action = "Delete", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "AdminProductEdit",
                "admin/products/{id}/edit",
                new { controller = "Admin", action = "Edit", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "gianhangUser",
                "gianhang/{username}/{TenCuaHang}",
                new { controller = "Product", action = "GianHang", username = UrlParameter.Optional, TenCuaHang = UrlParameter.Optional }
            );

           

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            

        }
    }

    

    public class SeoFriendlyRoute : Route
    {
        public SeoFriendlyRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var routeData = base.GetRouteData(httpContext);

            if (routeData != null)
            {
                if (routeData.Values.ContainsKey("id"))
                    routeData.Values["id"] = GetIdValue(routeData.Values["id"]);
            }

            return routeData;
        }

        private object GetIdValue(object id)
        {
            if (id != null)
            {
                string idValue = id.ToString();

                var regex = new Regex(@"^(?<id>\d+).*$");
                var match = regex.Match(idValue);

                if (match.Success)
                {
                    return match.Groups["id"].Value;
                }
            }

            return id;
        }
    }
}
