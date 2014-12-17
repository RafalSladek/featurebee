using System.Linq;
using System.Web;

namespace FeatureBee
{
    public static class JavascriptClient
    {
        /// <summary>
        /// Why not Array.indexOf? because it doesn't work in IE 8. 
        /// Why here and not in a seprated JS File? Because it still has just few lines of code.
        /// </summary>
        private const string snippet = "<script>" +
                                       "featureBeeClient={{" +
                                           "enabledToggles:[{0}]," +
                                           "isEnabled:function(toggleName){{" +
                                                "var t=this.enabledToggles;" +
                                                "for(var i=0,l=t.length;i<l;i++){{" +
                                                    "if(t[i]===toggleName){{" +
                                                        "return !0;" +
                                                    "}}" +
                                                "}}" +
                                                "return !1;" +
                                           "}}" +
                                       "}}" +
                                       "</script>";

        public static HtmlString Render()
        {
            return new HtmlString(
                string.Format(snippet,
                              string.Join(",", Feature.AllFeatures()
                                                      .Where(x => x.Enabled)
                                                      .Select(x => "\"" + HttpUtility.JavaScriptStringEncode(x.Name) + "\""))
                )
            );
        }
    }
}
