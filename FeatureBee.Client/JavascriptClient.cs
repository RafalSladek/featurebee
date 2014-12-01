using System.Linq;
using System.Web;

namespace FeatureBee
{
    public static class JavascriptClient
    {
        /// <summary>
        /// Why not Array.indexof? because it doesn't work on IE 8. 
        /// Why here and not in a seprated JS File? Because it still has just few lines of code.
        /// </summary>
        private const string snippet = "<script>" +
                                       "featureBeeClient = {{" +
                                           "enabledToggles : [{0}]," +
                                           "isEnabled : function(toggleName) {{" +
                                                "for(var i = 0; i < this.enabledToggles.length; i++){{" +
                                                    "if(this.enabledToggles[i] === toggleName){{" +
                                                        "return true;" +
                                                    "}}" +
                                                "}}" +
                                                "return false;" +
                                           "}}" +
                                       "}};" +
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
